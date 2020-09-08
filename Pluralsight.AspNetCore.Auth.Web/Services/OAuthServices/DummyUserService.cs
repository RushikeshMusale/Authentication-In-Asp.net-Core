using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pluralsight.AspNetCore.Auth.Web.Services.OAuthServices
{
    public class DummyUserService : IUserService
    {
        private IDictionary<string, User> _users = new Dictionary<string, User>();
        public Task<User> AddUser(string id, string username, string password)
        {
            var user = User.Create(id, username, password);
            _users.Add( id, user);
            return Task.FromResult(user);
        }

        public Task<User> GetUserById(string id)
            {
            if(_users.ContainsKey(id))
            {
                return Task.FromResult(_users[id]);
            }

            return Task.FromResult<User>(null);
        }
    }
}
