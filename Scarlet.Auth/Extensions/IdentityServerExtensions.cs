using System;
using IdentityServer4;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Scarlet.Auth.Extensions
{
	public static class IdentityServerExtensions
	{
		public static IIdentityServerBuilder AddIdentityServerAuthentication(
			this IIdentityServerBuilder builder,
			IConfiguration Configuration)
		{
            // Add Default Identity Schemas
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddCookie(IdentityConstants.ApplicationScheme, o =>
            {
                o.Cookie.Name = "Login.RememberMe";
                o.SlidingExpiration = true;
                if (!String.IsNullOrEmpty(Configuration["LoginRememberMeExpire"]))
                    o.ExpireTimeSpan = TimeSpan.FromMinutes(Double.Parse(Configuration["LoginRememberMeExpire"].ToString()));
            })
            .AddCookie(IdentityConstants.ExternalScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.ExternalScheme;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            })
            .AddCookie(IdentityConstants.TwoFactorRememberMeScheme, o =>
            {
                o.Cookie.Name = "Twofactor.RememberMe";
                o.SlidingExpiration = true;
                if (!String.IsNullOrEmpty(Configuration["TwofactorRememberMeExpire"]))
                    o.ExpireTimeSpan = TimeSpan.FromMinutes(Double.Parse(Configuration["TwofactorRememberMeExpire"].ToString()));
            })
            .AddCookie(IdentityConstants.TwoFactorUserIdScheme, o =>
            {
                o.Cookie.Name = "Twofactor.Id";
            });

			return builder;
		}
	}
}