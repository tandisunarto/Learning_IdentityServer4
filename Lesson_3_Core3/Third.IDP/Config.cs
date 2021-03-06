﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Third.IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource {
                    Name = "security_info",
                    DisplayName = "Security Info",
                    UserClaims = {
                        "sec_level",
                        "sec_zone",
                        "sec_exp"
                    }
                }
            };

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("api1", "My API #1")
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // client credentials flow client
                new Client
                {
                    ClientId = "client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "api1" }
                },

                // MVC client using code flow + pkce
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    AllowPlainTextPkce = false,
                    ClientSecrets = { new Secret("mvc-secret".Sha256()) },

                    RedirectUris = { "http://localhost:8010/signin-oidc" },
                    FrontChannelLogoutUri = "http://localhost:8010/signout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:8010/signout-callback-oidc" },

                    // AlwaysIncludeUserClaimsInIdToken = true,
                    // ***** 
                    // with this option, we don't need "options.GetClaimsFromUserInfoEndpoint = true" in the client application
                    // we just need to request the scope "options.Scope.Add("security_info")" and all claims in the scope will 
                    // be returned with id_token

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "api1", "address", "security_info" }
                },

                // SPA client using code flow + pkce
                new Client
                {
                    ClientId = "spa",
                    ClientName = "SPA Client",
                    ClientUri = "http://identityserver.io",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris =
                    {
                        "http://localhost:5002/index.html",
                        "http://localhost:5002/callback.html",
                        "http://localhost:5002/silent.html",
                        "http://localhost:5002/popup.html",
                    },

                    PostLogoutRedirectUris = { "http://localhost:5002/index.html" },
                    AllowedCorsOrigins = { "http://localhost:5002" },

                    AllowedScopes = { "openid", "profile", "api1" }
                }
            };
    }
}