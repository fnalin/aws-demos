using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace fansoftapi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // https://aws.amazon.com/pt/blogs/developer/introducing-the-asp-net-core-identity-provider-preview-for-amazon-cognito/
            services.AddCognitoIdentity(config => {
                config.Password = new Microsoft.AspNetCore.Identity.PasswordOptions{
                    RequireDigit = false,
                    RequiredLength = 6,
                    RequiredUniqueChars = 0,
                    RequireLowercase = false,
                    RequireUppercase = false,
                    RequireNonAlphanumeric = false
                };
            });

            var credentials = new BasicAWSCredentials("ACCESSKEY", "SECRETKEY");
            var provider =
                new AmazonCognitoIdentityProviderClient(credentials, RegionEndpoint.GetBySystemName("us-east-1"));
            services.AddSingleton<IAmazonCognitoIdentityProvider>(provider);

            var cognitoUserPool = new CognitoUserPool("POOLID", "CLIENTID", provider, "CLIENTSECRETID");
            services.AddSingleton<CognitoUserPool>(cognitoUserPool);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
