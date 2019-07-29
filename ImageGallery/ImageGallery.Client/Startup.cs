using ImageGallery.Client.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ImageGallery.Client
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
 
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // register an IHttpContextAccessor so we can access the current
            // HttpContext in services by injecting it
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // register an IImageGalleryHttpClient
            services.AddScoped<IImageGalleryHttpClient, ImageGalleryHttpClient>();

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;  // "Cookies"
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;            // "OpenIdConnect"
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddOpenIdConnect(options => {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = "http://localhost:8050";
                options.RequireHttpsMetadata = false;
                options.ResponseType = "id_token code";
                options.SaveTokens = true;
                options.ClientId = "image.gallery";
                options.ClientSecret = "secret";
                options.SignedOutRedirectUri = "http://localhost:8000";

                // options.GetClaimsFromUserInfoEndpoint = true;
                // options.Scope.Add("second.roles");
                // options.Scope.Add("second.new.api1");
                // options.Scope.Add("offline_access");

                // options.TokenValidationParameters = new TokenValidationParameters
                // {
                //     NameClaimType = JwtClaimTypes.Name,
                //     RoleClaimType = JwtClaimTypes.Role
                // };

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
