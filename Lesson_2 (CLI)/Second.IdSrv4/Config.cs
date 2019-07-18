// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
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
                new IdentityResource
                {
                    Name = "second.roles",
                    DisplayName = "The Roles for the Second IdSrv4",
                    UserClaims =
                    {
                        JwtClaimTypes.Role,
                    }
                }
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource("second.api1", "My API #1", 
                new List<string> {
                    JwtClaimTypes.Role
                }),

                new ApiResource("second.new.api1", "My New API #1", 
                new List<string> {
                    JwtClaimTypes.Role
                }),

                // adding api resource this way creates serror
                // https://github.com/IdentityServer/IdentityServer4/issues/980
                new ApiResource {
                    Name = "second.test.api",
                    DisplayName = "My Second Test API",
                    Scopes = new List<Scope>
                    {
                        new Scope(JwtClaimTypes.Role)
                    }
                }
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

                    AllowedScopes = { "openid", "profile", "second.api1", "second.roles" }
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
                    AllowedScopes = { "openid", "profile", "second.api1", "second.new.api1", "second.roles", "second.test.api" },
                },

                // MVC client using implicit
                new Client
                {
                    ClientId = "mvc.password",
                    ClientName = "MVC Resource Owner Password",
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    RedirectUris = { "http://localhost:8010/signin-oidc" },
                    FrontChannelLogoutUri = "http://localhost:8010/signout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:8010/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "second.api1", "second.roles" },
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