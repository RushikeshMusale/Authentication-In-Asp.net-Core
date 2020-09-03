using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;
using Pluralsight.AspNetCore.Auth.Web.Services;
using System.Collections.Generic;

namespace Pluralsight.AspNetCore.Auth.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });

            IDictionary<string, string> users = new Dictionary<string, string> { { "rushi", "password" } };

            services.AddSingleton<IUserService>(new DummyUserService(users));


            services.AddAuthentication(options => 
            {                
                options.DefaultChallengeScheme = FacebookDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })                
                .AddFacebook(options=> 
                {
                    options.AppId = "";
                    options.AppSecret = "";
                }) // this will only authenticate
                 .AddCookie(); // so that it can login



        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRewriter(new RewriteOptions().AddRedirectToHttps(301, 44343));

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
