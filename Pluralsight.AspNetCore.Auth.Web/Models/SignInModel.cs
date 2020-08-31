using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pluralsight.AspNetCore.Auth.Web.Models
{
    public class SignInModel
    {
        [Required(ErrorMessage = "Have to supply username")]        
        public string Username { get; set; }

        [Required(ErrorMessage = "Have to supply password")]
        public string Password { get; set; }
    }
}
