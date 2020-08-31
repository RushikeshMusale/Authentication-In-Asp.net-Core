using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
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
