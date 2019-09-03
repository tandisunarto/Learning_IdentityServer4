using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace First.OAuth.Configurations
{
    public static class InMemoryConfigs
    {
        internal static IEnumerable<Client> Clients
        {
            get
            {
                return new List<Client>
                {
                    new Client
                    {
                        ClientId = "FirstWeb",
                        ClientName = "First Web Client Application",
                        AllowedGrantTypes = GrantTypes.Implicit,
                        AllowAccessTokensViaBrowser = true,
                        RedirectUris = {
                            "http://localhost:7001/signin-oidc"
                        },
                        PostLogoutRedirectUris = {
                            "http://localhost:7001/signout-callback-oidc"
                        },
                        AllowedScopes = {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            "health_data"
                            // IdentityServerConstants.StandardScopes.Email,
                            // "role",
                            // "customAPI.read"
                        },
                    },
                    new Client
                    {
                        ClientId = "FirstAPI",
                        ClientName = "First API Client Application",
                        ClientSecrets = new List<Secret> {
                            new Secret("FirstAPISecret".Sha256())},
                        AllowedGrantTypes = GrantTypes. ResourceOwnerPasswordAndClientCredentials,
                    //     AllowedScopes = new List<string> {
                    //         // because scope "role" is an identity resource, when calling /connect/token must use grant_type = "password" and provide username and password
                    //         // if scope only includes "customAPI.read" and "customAPI.write", when calling /connect/token use grant_type = "client_credentials" and no need
                    //         // to provide username and password
                    //         "role",
                    //         "customAPI.read",
                    //         "customAPI.write"
                    //     }
                    },
                };
            }
        }

        internal static IEnumerable<ApiResource> APIResources
        {
            get
            {
                return new List<ApiResource> {
                    new ApiResource
                    {
                        Name = "customAPI",
                        DisplayName = "Custom API",
                        Description = "Custom API Access",
                        // UserClaims = new List<string> { "role" },
                        // ApiSecrets = new List<Secret> { new Secret("ScopeSecret".Sha256())},
                        // Scopes = new List<Scope>
                        // {
                        //     new Scope("customAPI.read"),
                        //     new Scope("customAPI.write")
                        //     {
                        //         UserClaims = new List<string>
                        //         {
                        //             "employee_write"
                        //         }
                        //     }
                        // }
                    }
                };
            }
        }

        internal static IEnumerable<IdentityResource> IdentityResources
        {
            get
            {
                return new List<IdentityResource>
                {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile(),
                    // new IdentityResources.Email(),
                    // new IdentityResource
                    // {
                    //     Name = "role",
                    //     DisplayName = "Your Role Info",
                    //     UserClaims = new List<string>
                    //     {
                    //         "admin",
                    //         "guest"
                    //     }
                    // },
                    new IdentityResource
                    {
                        Name = "health_data",
                        DisplayName = "User Personal Health Information",
                        UserClaims =
                        {
                            "smoke",
                            "alcohol",
                        }
                    }
                };
            }
        }
    }
}
