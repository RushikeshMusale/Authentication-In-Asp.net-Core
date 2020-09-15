using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Pluralsight.AspNetCore.Auth.Web.Models;
using Pluralsight.AspNetCore.Auth.Web.Services.OAuthServices;

namespace Pluralsight.AspNetCore.Auth.Web.Controllers
{
    [Route("authfbtwitter")]
    public class AuthFbTwitter : Controller
    {
        private readonly IUserService _userService;

        public AuthFbTwitter(IUserService userService)
        {
            _userService = userService;
        }
        // For signing out we can same method from Auth Controller
        [Route("signinfbtwitter")]
        public async Task<IActionResult> SignIn()
        {
            var authResult = await HttpContext.AuthenticateAsync("Temporary");
            if(authResult.Succeeded)
            {
                return RedirectToAction("Profile");
            }
            return View();
        }

        [Route("signinfbtwitter/{provider}")]
        public IActionResult SignIn(string provider, string returnUrl=null)
        {
            var redirectUri = Url.Action("Profile");
            if(returnUrl!=null)
            {
                redirectUri += "?ReturnUrl=" + returnUrl;
            }
            // Tries to authentciate the user with the scheme provided in the overload, here it is the provider
            // if successfully authenticated, it redirects to 'RedirectUri;
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUri }, provider);
        }


        [Route("signinfbtwitter/profile")]
        public async Task<IActionResult> Profile(string returnUrl = null)
        {
            // Since default authentication scheme is cookies (not a Temporary), 
            // we will have to authenticate user using temporary scheme
            // After that user 
            var authResult = await HttpContext.AuthenticateAsync("Temporary");

            if(!authResult.Succeeded)
            {
                return RedirectToAction("SignIn");
            }

            var user = await _userService.GetUserById(authResult
                                    .Principal
                                    .FindFirst(ClaimTypes.NameIdentifier).Value);

            if(user !=null)
            {
                return await SignInUser(user, returnUrl);
            }

            var model = new ProfileModel()
            {
                DisplayName = authResult.Principal.Identity.Name
            };

            var emailClaim = authResult.Principal.FindFirst(ClaimTypes.Email);

            if(emailClaim != null)
            {
                model.Email = emailClaim.Value;
            }
            return View(model);
        }

        [Route("signinfbtwitter/profile")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileModel model, string returnUrl = null)
        {
            var authResult = await HttpContext.AuthenticateAsync("Temporary");
            if(!authResult.Succeeded)
            {
                return RedirectToAction("SignIn");
            }

            if(ModelState.IsValid)
            {
                var user = await _userService.AddUser(authResult.Principal.FindFirst(ClaimTypes.NameIdentifier).Value, 
                    model.DisplayName, model.Email);
                return await SignInUser(user, returnUrl);
            }
            return View(model);
        }

        private async Task<IActionResult> SignInUser(User user, string returnUrl)
        {
            // signout from Temporary session
            await HttpContext.SignOutAsync("Temporary");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Redirect(returnUrl == null ? "/" : returnUrl);
        }
    }
}
