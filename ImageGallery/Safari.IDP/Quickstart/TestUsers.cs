﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer4.Quickstart.UI
{
    public class TestUsers
    {
        public static List<TestUser> Users = new List<TestUser>
        {
            new TestUser {
                SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7",
                Username = "frank",
                Password = "frank",
                Claims =
                {
                    new Claim(JwtClaimTypes.Name, "Frank Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Frank"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.Email, "FrankSmith@email.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                    new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                    new Claim("status", "Full Time"),
                    new Claim("start_date", "10/5/2016"),
                    new Claim("role", "FreeUser"),
                    new Claim("subscription_level", "FreeUser"),
                    new Claim("country", "nl")
                }
            },
            new TestUser  {
                SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
                Username = "claire",
                Password = "claire",
                Claims =
                {
                    new Claim(JwtClaimTypes.Name, "Claire Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Claire"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.Email, "ClaireSmith@email.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                    new Claim(JwtClaimTypes.Address, @"{ 'street_address': '1001 Rockville Pike', 'locality': 'Rockville', 'postal_code': 50830, 'country': 'United States' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                    new Claim("status", "Part Time"),
                    new Claim("start_date", "5/21/2006"),
                    new Claim("role", "PayingUser"),
                    new Claim("subscription_level", "PayingUser"),
                    new Claim("country", "be")
                }
            }
        };
    }
}