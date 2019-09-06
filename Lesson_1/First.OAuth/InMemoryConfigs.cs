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
                            "health_data",
                            "roles",
                            "custom-api"
                        },
                    }
                };
            }
        }

        internal static IEnumerable<ApiResource> APIResources
        {
            get
            {
                return new List<ApiResource> {
                    new ApiResource(
                        "custom-api",
                        "Custom API",
                        new List<string> { 
                            JwtClaimTypes.Role
                        }
                    )
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
                    new IdentityResource
                    {
                        Name = "roles",
                        DisplayName = "Your Role Info",
                        UserClaims = 
                        {
                            JwtClaimTypes.Role
                        }
                    },
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
