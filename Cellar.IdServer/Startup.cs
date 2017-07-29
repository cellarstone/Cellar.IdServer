using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using StackExchange.Redis;

using Microsoft.IdentityModel.Tokens;
using IdentityServer4;
using IdentityServer4.Validation;

using Cellar.IdServer.Quickstart.UI;

using Cellar.IdServer.Configuration;

using System.IO;



namespace Cellar.IdServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            _env = env;

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }


        public IConfigurationRoot Configuration { get; }
        private IHostingEnvironment _env { get; set; }


        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            services.AddOptions();
            services.AddSingleton<IConfigurationRoot>((provider) => Configuration);


            // Connect to Redis database.
            var redis = ConnectionMultiplexer.Connect("redis-master:6379");
            services.AddDataProtection()
                .PersistKeysToRedis(redis, "DataProtection-Keys");

            services.AddIdentityServer(options =>
                {
                    options.Authentication.FederatedSignOutPaths.Add("/signout-callback-aad");
                    options.Authentication.FederatedSignOutPaths.Add("/signout-callback-idsrv");
                    options.Authentication.FederatedSignOutPaths.Add("/signout-callback-adfs");

                    options.Events.RaiseSuccessEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseErrorEvents = true;
                })
                 .AddInMemoryClients(Clients.Get())
                 .AddInMemoryIdentityResources(Resources.GetIdentityResources())
                 .AddInMemoryApiResources(Resources.GetApiResources())
                .AddDeveloperSigningCredential()
                .AddExtensionGrantValidator<ExtensionGrantValidator>()
                .AddExtensionGrantValidator<NoSubjectExtensionGrantValidator>()
                .AddSecretParser<ClientAssertionSecretParser>()
                .AddSecretValidator<PrivateKeyJwtSecretValidator>()
                .AddRedirectUriValidator<StrictRedirectUriValidatorAppAuth>()
                .AddCellarUsers(Configuration);

            services.AddMvc();

            return services.BuildServiceProvider(validateScopes: true);
        }



        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory
                    .WithFilter(new FilterLoggerSettings
                    {
                        { "Microsoft", LogLevel.Warning },
                        { "System", LogLevel.Warning },
                        { "Cellar.IdServer", LogLevel.Information }
                    })
                    .AddConsole();


            app.UseDeveloperExceptionPage();

            app.UseIdentityServer();

            // app.UseGoogleAuthentication(new GoogleOptions
            // {
            //     AuthenticationScheme = "Google",
            //     SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
            //     ClientId = "708996912208-9m4dkjb5hscn7cjrn5u0r4tbgkbj1fko.apps.googleusercontent.com",
            //     ClientSecret = "wdfPY6t8H8cecgjlxud__4Gh"
            // });

            // app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            // {
            //     AuthenticationScheme = "demoidsrv",
            //     SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
            //     SignOutScheme = IdentityServerConstants.SignoutScheme,
            //     AutomaticChallenge = false,
            //     DisplayName = "IdentityServer",
            //     Authority = "https://demo.identityserver.io/",
            //     ClientId = "implicit",
            //     ResponseType = "id_token",
            //     Scope = { "openid profile" },
            //     SaveTokens = true,
            //     CallbackPath = new PathString("/signin-idsrv"),
            //     SignedOutCallbackPath = new PathString("/signout-callback-idsrv"),
            //     RemoteSignOutPath = new PathString("/signout-idsrv"),
            //     TokenValidationParameters = new TokenValidationParameters
            //     {
            //         NameClaimType = "name",
            //         RoleClaimType = "role"
            //     }
            // });

            // app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            // {
            //     AuthenticationScheme = "aad",
            //     SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
            //     SignOutScheme = IdentityServerConstants.SignoutScheme,
            //     AutomaticChallenge = false,
            //     DisplayName = "AAD",
            //     Authority = "https://login.windows.net/4ca9cb4c-5e5f-4be9-b700-c532992a3705",
            //     ClientId = "96e3c53e-01cb-4244-b658-a42164cb67a9",
            //     ResponseType = "id_token",
            //     Scope = { "openid profile" },
            //     CallbackPath = new PathString("/signin-aad"),
            //     SignedOutCallbackPath = new PathString("/signout-callback-aad"),
            //     RemoteSignOutPath = new PathString("/signout-aad"),
            //     TokenValidationParameters = new TokenValidationParameters
            //     {
            //         NameClaimType = "name",
            //         RoleClaimType = "role"
            //     }
            // });

            // app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            // {
            //     AuthenticationScheme = "adfs",
            //     SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
            //     SignOutScheme = IdentityServerConstants.SignoutScheme,
            //     AutomaticChallenge = false,
            //     DisplayName = "ADFS",
            //     Authority = "https://adfs.leastprivilege.vm/adfs",
            //     ClientId = "c0ea8d99-f1e7-43b0-a100-7dee3f2e5c3c",
            //     ResponseType = "id_token",
            //     Scope = { "openid profile" },
            //     CallbackPath = new PathString("/signin-adfs"),
            //     SignedOutCallbackPath = new PathString("/signout-callback-adfs"),
            //     RemoteSignOutPath = new PathString("/signout-adfs"),
            //     TokenValidationParameters = new TokenValidationParameters
            //     {
            //         NameClaimType = "name",
            //         RoleClaimType = "role"
            //     }
            // });

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
