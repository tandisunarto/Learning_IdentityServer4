// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scarlet.Auth.Data;
using Scarlet.Auth.Extensions;
using Scarlet.Auth.Models;

namespace Scarlet.Auth
{
  public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(
            IWebHostEnvironment environment,
            IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("AppConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));

            services.AddIdentityCore<ApplicationUser>(options => {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan =
                    TimeSpan.FromMinutes(Configuration["LoginLockedoutTimespan"] != null
                        ? Int32.Parse(Configuration["LoginLockedoutTimespan"])
                        : 5);
                options.Lockout.MaxFailedAccessAttempts =
                    Configuration["LoginFailedCount"] != null
                        ? Int32.Parse(Configuration["LoginFailedCount"])
                        : 3;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            })
            .AddRoles<IdentityRole>()
            .AddRoleValidator<RoleValidator<IdentityRole>>()
            .AddRoleManager<RoleManager<IdentityRole>>()
            .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // identityBuilder = new IdentityBuilder(identityBuilder.UserType, typeof(IdentityRole), identityBuilder.Services);
            // identityBuilder.AddRoleValidator<RoleValidator<IdentityRole>>();
            // identityBuilder.AddRoleManager<RoleManager<IdentityRole>>();
            // identityBuilder.AddSignInManager<SignInManager<ApplicationUser>>();
            // identityBuilder.AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            var builder = services.AddIdentityServer()
                .AddConfigurationStore(options => {
                    // this adds the config data from DB (clients, resources)
                    options.ConfigureDbContext = options =>
                        options.UseSqlite(Configuration.GetConnectionString("IS4Connection"),
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options => {
                        // this adds the operational data from DB (codes, tokens, consents)
                    options.ConfigureDbContext = options =>
                        options.UseSqlite(Configuration.GetConnectionString("IS4Connection"),
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddAspNetIdentity<ApplicationUser>()
                .AddIdentityServerAuthentication(Configuration);

            services.AddControllersWithViews();

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

            services.AddAuthentication()
                .AddMicrosoftAccount(options =>
                {
                    options.ClientId = Configuration["Authentication:Microsoft:Id"];
                    options.ClientSecret = Configuration["Authentication:Microsoft:Secret"];
                })
                .AddGoogle(options =>
                {
                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to http://localhost:5000/signin-google
                    options.ClientId = Configuration["Authentication:Google:Id"];
                    options.ClientSecret = Configuration["Authentication:Google:Secret"];
                })
                .AddFacebook(options =>
                {
                    options.AppId = Configuration["Authentication:Facebook:Id"];
                    options.AppSecret = Configuration["Authentication:Facebook:Secret"];
                })
                .AddTwitter(options => {
                    options.ConsumerKey = Configuration["Authentication:Twitter:Id"];
                    options.ConsumerSecret = Configuration["Authentication:Twitter:Secret"];
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
               endpoints.MapDefaultControllerRoute();
            });
        }

        // public void SetupSigningCredential(IIdentityServerBuilder builder)
        // {
        //     X509Store certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        //     certStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
        //     X509Certificate2Collection certCollection = certStore.Certificates.Find(
        //                             X509FindType.FindBySubjectName,
        //                             Configuration["SigningCertificate"] ?? "*.bis.icfi.com",
        //                             false);
        //     if (certCollection.Count > 0)
        //         builder.AddSigningCredential(certCollection[0]);
        // }
    }
}
