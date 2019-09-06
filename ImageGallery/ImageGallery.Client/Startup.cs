using System.IdentityModel.Tokens.Jwt;
using IdentityModel;
using ImageGallery.Client.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ImageGallery.Client
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddAuthorization(options => {
                options.AddPolicy(
                    "CanOrderFrame",
                    policyBuilder => {
                        policyBuilder.RequireAuthenticatedUser();
                        policyBuilder.RequireClaim("country", "be");
                        policyBuilder.RequireClaim("subscription_level", "PayingUser");
                    }
                );
            });

            // register an IHttpContextAccessor so we can access the current
            // HttpContext in services by injecting it
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // register an IImageGalleryHttpClient
            services.AddScoped<IImageGalleryHttpClient, ImageGalleryHttpClient>();

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;  // "Cookies"
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;            // "OpenIdConnect"
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
                options.AccessDeniedPath = "/Authorization/AccessDenied";
            })
            .AddOpenIdConnect(options => {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = "http://localhost:8050";
                options.RequireHttpsMetadata = false;
                options.ResponseType = "code id_token";
                options.SaveTokens = true;
                options.ClientId = "image_gallery";
                options.ClientSecret = "secret";
                options.SignedOutRedirectUri = "http://localhost:8000";

                options.GetClaimsFromUserInfoEndpoint = true;
                options.ClaimActions.Remove("amr");         // include amr in the claims
                options.ClaimActions.DeleteClaim("sid");    // exclude sid from the claims
                options.ClaimActions.DeleteClaim("idp");    // excllude idp from the claims

                options.Scope.Add("address");
                options.Scope.Add("employment");
                options.Scope.Add("roles");
                options.Scope.Add("payinguser_imagegallery_api");
                options.Scope.Add("freeuser_imagegallery_api");
                options.Scope.Add("imagegallery_api_roles");
                options.Scope.Add("country");
                options.Scope.Add("subscription_level");
                options.Scope.Add("offline_access");

                options.ClaimActions.MapUniqueJsonKey("role", "role");
                options.ClaimActions.MapUniqueJsonKey("start_date", "start_date");
                options.ClaimActions.MapUniqueJsonKey("country", "country");
                options.ClaimActions.MapUniqueJsonKey("subscription_level", "subscription_level");
                // options.Scope.Add("second.roles");
                // options.Scope.Add("second.new.api1");
                // options.Scope.Add("offline_access");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.Name,
                    RoleClaimType = JwtClaimTypes.Role
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error");
            }

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Gallery}/{action=Index}/{id?}");
            });
        }
    }
}
