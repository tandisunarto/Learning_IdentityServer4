using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace App.Auth.Extensions
{
    public static class ExternalProvidersExtension
    {
        public static AuthenticationBuilder SetupExternalProviders(this IServiceCollection services)
        {
            return services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to https://localhost:5001/signin-google
                    options.ClientId = "911811550081-c9c9i1ja3mrcu301qck56d3paen1c7dm.apps.googleusercontent.com";
                    options.ClientSecret = "ioK_NTskcJJfXy6VaveF5MiJ";
                })
                .AddFacebook(options =>
                {
                    options.AppId = "2600433903540231";
                    options.AppSecret = "e64a7bc262991f8c1e9d30b3e0b3b886";
                });
        }
    }
}