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
                .AddGoogle(options =>
                {
                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to http://localhost:5000/signin-google
                    options.ClientId = "714407290460-atfjn7hr1okqa2k2873tl8brr4k4jps9.apps.googleusercontent.com";
                    options.ClientSecret = "P4QV9JsWcMN5Wc3jebIW-Sn0";
                })
                .AddFacebook(options =>
                {
                    options.AppId = "620834381843552";
                    options.AppSecret = "56fff97d7ebfb6ffb13455d3124f6a57";
                })
                .AddTwitter(options => {
                    options.ConsumerKey = "Yreax1d7766ki2PCa02jodgdl";
                    options.ConsumerSecret = "uVJ5LisrMf6raC5zo6B2MfDojmNjCIHTBYRZOCpQquQfYmrDVb";
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
