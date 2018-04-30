// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace Second.IdSrv4
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource("second.api1", "My API #1")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                // client credentials resource owner password
                new Client
                {
                    ClientId = "client.ro",
                    ClientName = "Resource Owner Password Client",

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { new Secret("BF715316-C9B1-41F2-BFFA-644A2E06CBCA".Sha256()) },

                    AllowedScopes = { "second.api1" }
                },

                // client credentials flow client
                new Client
                {
                    ClientId = "client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "second.api1" }
                },

                // MVC client using hybrid flow
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",

                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    RedirectUris = { "http://localhost:8010/signin-oidc" },
                    FrontChannelLogoutUri = "http://localhost:8010/signout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:8010/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "second.api1" }
                },

                // MVC client using implicit
                new Client
                {
                    ClientId = "mvc.implicit",
                    ClientName = "MVC Client Implicit",

                    AllowedGrantTypes = GrantTypes.Implicit,

                    RedirectUris = { "http://localhost:8010/signin-oidc" },
                    FrontChannelLogoutUri = "http://localhost:8010/signout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:8010/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "second.api1" },
                    AllowAccessTokensViaBrowser = true
                },

                // SPA client using implicit flow
                new Client
                {
                    ClientId = "spa",
                    ClientName = "SPA Client",
                    ClientUri = "http://identityserver.io",

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris =
                    {
                        "http://localhost:5002/index.html",
                        "http://localhost:5002/callback.html",
                        "http://localhost:5002/silent.html",
                        "http://localhost:5002/popup.html",
                    },

                    PostLogoutRedirectUris = { "http://localhost:8010/index.html" },
                    AllowedCorsOrigins = { "http://localhost:8010" },

                    AllowedScopes = { "openid", "profile", "second.api1" }
                }
            };
        }
    }
}