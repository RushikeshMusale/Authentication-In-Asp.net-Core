using Microsoft.ApplicationInsights.DataContracts;
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

            services.AddSingleton<Services.OAuthServices.IUserService, Services.OAuthServices.DummyUserService>();
            services.AddAuthentication(options => 
            {                
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = "Temporary";
            })                
                .AddFacebook(options=> 
                {
                    options.AppId = "1";
                    options.AppSecret = "1";
                }) // this will only authenticate
                .AddTwitter(options=> 
                {
                    options.ConsumerKey = "1";
                    options.ConsumerSecret = "1";
                }) 
                .AddCookie( options=>
                {
                    options.LoginPath = "/authfbtwitter/signinfbtwitter";
                }). // so that it can login
                AddCookie("Temporary"); 
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
