using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Pluralsight.AspNetCore.Auth.Web.Controllers
{
    [Route("authfbtwitter")]
    public class AuthFbTwitter : Controller
    {
        // For signing out we can same method from Auth Controller

        [Route("signinfbtwitter")]
        public IActionResult SignIn()
        {
            return View();
        }

        [Route("signinfbtwitter/{provider}")]
        public IActionResult SignIn(string provider, string returnUrl=null)
        {

            return Challenge(new AuthenticationProperties { RedirectUri = returnUrl ?? "/" }, provider);
        }
    }
}
