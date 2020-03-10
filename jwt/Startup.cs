using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Identity.UI;
// using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using jwt.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
// using Microsoft.IdentityModel.Tokens;

namespace jwt
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
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddIdentityCore<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication("CookieAuth")
            .AddCookie("CookieAuth", options => {
                options.Cookie.Name = "Super.Scarlet.Cookie";
                options.LoginPath = "/Auth/Basic";
            })
            .AddJwtBearer(options => {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = "http://halo.world.com",
                    ValidIssuer = "http://halo.world.com",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySuperSecretKey"))
                };
            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoint => {
                endpoint.MapDefaultControllerRoute();
            });
        }
    }
}
