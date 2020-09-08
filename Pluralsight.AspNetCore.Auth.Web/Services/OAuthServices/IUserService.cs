using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pluralsight.AspNetCore.Auth.Web.Services.OAuthServices
{
    public interface IUserService
    {
        Task<User> GetUserById(string id);

        Task<User> AddUser(string id, string username, string password);
    }
}
