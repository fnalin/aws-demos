using System.Text;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using fansoftapi.Models.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace fansoftapi
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

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

            var credentials = new BasicAWSCredentials("ACCESS_KEY", "SECRET_KEY");
            var provider =
                new AmazonCognitoIdentityProviderClient(credentials, RegionEndpoint.GetBySystemName("us-east-1"));
            services.AddSingleton<IAmazonCognitoIdentityProvider>(provider);

            var cognitoUserPool = new CognitoUserPool("POOL_ID", "CLIENT_ID", provider, "CLIENT_SECRET_KEY");
            
            services.AddSingleton<CognitoUserPool>(cognitoUserPool);

            var securitySettingsConfig = _config.GetSection("SecuritySettings");
            services.Configure<SecuritySettings>(securitySettingsConfig);
            var securitySettings = securitySettingsConfig.Get<SecuritySettings>();
            var key = Encoding.UTF8.GetBytes(securitySettings.Secret);

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = 
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options => {
                    options.RequireHttpsMetadata = false;
                    // salva o Token
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters{
                        // Validar Emissor
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = securitySettings.ValidoEm,
                        ValidIssuer = securitySettings.Emissor
                        /*
                        ,
                        ValidAudiences = new string []{},
                        ValidIssuers = new string [] {}
                        */
                    };
                });
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
