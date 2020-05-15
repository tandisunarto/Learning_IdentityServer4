// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scarlet.Auth.Data;
using Scarlet.Auth.Models;
using Serilog;

namespace Scarlet.Auth
{
    public class SeedData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlite(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    context.Database.Migrate();

                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var tandi = userManager.FindByNameAsync("tandi").Result;
                    if (tandi == null)
                    {
                        tandi = new ApplicationUser
                        {
                            UserName = "tandi"
                        };
                        var result = userManager.CreateAsync(tandi, "Password!1").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userManager.AddClaimsAsync(tandi, new Claim[] {
                            new Claim(JwtClaimTypes.Name, "Tandi Sunarto"),
                            new Claim(JwtClaimTypes.GivenName, "Tandi"),
                            new Claim(JwtClaimTypes.FamilyName, "Sunarto"),
                            new Claim(JwtClaimTypes.Email, "tandi.sunart@hotmail.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://tandi.com"),
                            new Claim(JwtClaimTypes.Address, 
                                @"{ 'street_address': '15622 Cliff Swallow Way', 'locality': 'Rockville', 'postal_code': 20853, 'country': 'United States' }", 
                                IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                        }).Result;

                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Log.Debug("tandi created");
                    }
                    else
                    {
                        Log.Debug("tandi already exists");
                    }

                    // var alice = userMgr.FindByNameAsync("alice").Result;
                    // if (alice == null)
                    // {
                    //     alice = new ApplicationUser
                    //     {
                    //         UserName = "alice"
                    //     };
                    //     var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                    //     if (!result.Succeeded)
                    //     {
                    //         throw new Exception(result.Errors.First().Description);
                    //     }

                    //     result = userMgr.AddClaimsAsync(alice, new Claim[]{
                    //     new Claim(JwtClaimTypes.Name, "Alice Smith"),
                    //     new Claim(JwtClaimTypes.GivenName, "Alice"),
                    //     new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    //     new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                    //     new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    //     new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                    //     new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                    // }).Result;
                    //     if (!result.Succeeded)
                    //     {
                    //         throw new Exception(result.Errors.First().Description);
                    //     }
                    //     Log.Debug("alice created");
                    // }
                    // else
                    // {
                    //     Log.Debug("alice already exists");
                    // }

                    // var bob = userMgr.FindByNameAsync("bob").Result;
                    // if (bob == null)
                    // {
                    //     bob = new ApplicationUser
                    //     {
                    //         UserName = "bob"
                    //     };
                    //     var result = userMgr.CreateAsync(bob, "Pass123$").Result;
                    //     if (!result.Succeeded)
                    //     {
                    //         throw new Exception(result.Errors.First().Description);
                    //     }

                    //     result = userMgr.AddClaimsAsync(bob, new Claim[]{
                    //     new Claim(JwtClaimTypes.Name, "Bob Smith"),
                    //     new Claim(JwtClaimTypes.GivenName, "Bob"),
                    //     new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    //     new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                    //     new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    //     new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                    //     new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                    //     new Claim("location", "somewhere")
                    // }).Result;
                    //     if (!result.Succeeded)
                    //     {
                    //         throw new Exception(result.Errors.First().Description);
                    //     }
                    //     Log.Debug("bob created");
                    // }
                    // else
                    // {
                    //     Log.Debug("bob already exists");
                    // }
                }
            }
        }
    }
}
