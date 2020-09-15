using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
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
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddOpenIdConnect("B2C_1_sign_up", // same as policy name (user flows)
                options =>
                {
                    // This is located in the home-> azure Ad b2c -> user flows -> b2c_1_sign_up -> run user flow
                    options.MetadataAddress = "https://psadb2cdemorushi.b2clogin.com/psadb2cdemorushi.onmicrosoft.com/v2.0/.well-known/openid-configuration?p=B2C_1_sign_up";
                    options.ClientId = "56cec3f2-f27d-4b53-ae7d-263ff7b2a419"; // Application id
                    options.ResponseType = OpenIdConnectResponseType.IdToken; // this requires to check in the implicit 
                                                                              //grants inside the azure b2c

                    options.CallbackPath = "/signin/B2C_1_sign_up"; // this should match with reply url in azure ad app.
                    // rest of things will be taken care by openIdConnect middleware
                    options.SignedOutCallbackPath = "/signout/B2C_1_sign_up";
                    options.SignedOutRedirectUri = "/";
                    options.TokenValidationParameters.NameClaimType = "name"; // so that name is displayed in the home page
                })
                .AddCookie();

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
