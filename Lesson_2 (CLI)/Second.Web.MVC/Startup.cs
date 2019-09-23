using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Second.Web.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthorization(configure => {
                configure.AddPolicy("Canadian",
                    policy => policy.RequireClaim(ClaimTypes.Country));
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;  // "Cookies"
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;            // "OpenIdConnect"
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
                // change access denied page route, default by convention is /Account/AccessDenied
                options.AccessDeniedPath = "/Authorization/AccessDenied";
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = "http://localhost:8000";
                options.RequireHttpsMetadata = false;
                options.ResponseType = "id_token code";
                options.SaveTokens = true;
                options.ClientId = "mvc";
                options.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
                options.SignedOutRedirectUri = "http://localhost:8010";

                options.GetClaimsFromUserInfoEndpoint = true;
                options.Scope.Add("second.roles");
                options.Scope.Add("second.new.api1");
                options.Scope.Add("offline_access");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.Name,
                    RoleClaimType = JwtClaimTypes.Role
                };

                options.Events.OnTokenValidated = context =>
                {
                    // still does not work
                    var identity = context.Principal.Identity as ClaimsIdentity;
                    var claims = identity.Claims;
                    var ci = new ClaimsIdentity(
                        context.Principal.Identity.AuthenticationType,
                        JwtClaimTypes.Name,
                        JwtClaimTypes.Role
                    );
                    claims = claims.Append(new Claim("role", "Superman"));
                    ci.AddClaims(claims);
                    context.HttpContext.User = new ClaimsPrincipal(ci);
                    return Task.CompletedTask;
                };
            });            

            services.AddIdentityServer(options => {
                options.Events.RaiseSuccessEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseErrorEvents = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
