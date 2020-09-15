using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Pluralsight.AspNetCore.Auth.Web.Models;
using Pluralsight.AspNetCore.Auth.Web.Services;

namespace Pluralsight.AspNetCore.Auth.Web.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [Route("signin")]
        [HttpGet]
        public IActionResult SignIn()
        {            
            return View(new SignInModel());
        }

        
        [HttpPost]
        [Route("signin")]
        [ValidateAntiForgeryToken] // asp.net core adds automatically validate anti forgery token
        public async Task<IActionResult> SignIn(SignInModel model, string returnUrl =null)
        {
            if(ModelState.IsValid)
            {
                if( await _userService.ValidateCredentials(model.Username, model.Password,out User user))
                {
                    await SignInUser(user.Username);
                    if(returnUrl!=null)
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("password", "Invalid Credentials");
                }
            }
            return View(model);
        }

        [HttpPost]
        [Route("signout")]

        // No need to return Task<IActionResult> because, the signoutcallback url takes care of this
        public async Task SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // instead of hardcoding B2C_1_sing_up, use the claim tfp to sign out
            var scheme = User.FindFirst("tfp").Value;
            await HttpContext.SignOutAsync(scheme);
            //moved this settings to authentication configuration Options.SignedOutRedirectUri
            //return RedirectToAction("Index", "Home");  
        }

        [Route("signup")]
        [HttpGet]
        public IActionResult SignUp()
        {
            return View(new SignUpModel());
        }

        [Route("signup")]
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            if(ModelState.IsValid)
            {
                if(await _userService.AddUser(model.Username, model.Password))
                {
                    await SignInUser(model.Username);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("Error", "Unable to add User. Username aleady used...");
            }
            return View(model);
        }


        private async Task SignInUser(string username)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Name, username)
            };

            var identity = new ClaimsIdentity(claims, 
                CookieAuthenticationDefaults.AuthenticationScheme, 
                "name", null);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);
        }
    }
}
