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
    [Route("authfb")]
    public class AuthFb : Controller
    {
        // For signing out we can same method from Auth Controller

        [Route("signinfb")]
        public IActionResult SignIn()
        {
            return Challenge(new AuthenticationProperties 
            { RedirectUri = "/" } // so that once user credentials are validated, it will redirect to root directory of our app
            ); 
        }
    }
}
