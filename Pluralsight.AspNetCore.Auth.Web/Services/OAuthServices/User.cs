using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pluralsight.AspNetCore.Auth.Web.Services.OAuthServices
{
    public class User
    {
        private User()
        {
        }

        public static User Create(string id, string username, string email)
        {
            return new User { Id = id, Username = username, Email = email };
        }

        public string Id { get; private set; }

        public string Username { get; private set; }

        public string Email { get; private set; }
    }
}
