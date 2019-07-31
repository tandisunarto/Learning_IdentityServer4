using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace Safari.IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources
        {
            get
            {
                return new List<IdentityResource> {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile(),
                    new IdentityResources.Address(),
                    new IdentityResource {
                        Name = "roles",
                        DisplayName = "Your role(s)",
                        Description = "Role(s) assigned to this user",
                        UserClaims = {
                            "role"
                        }
                    },
                    new IdentityResource {
                        Name = "employment",
                        DisplayName = "User Employment Status",
                        Description = "Your user employment detail (status, start_date, title)",
                        UserClaims = {
                            "status",
                            "start_date",
                            "title"
                        }
                    }
                };
            }
        }

        public static IEnumerable<Client> Clients
        {
            get
            {
                return new List<Client> {
                    new Client
                    {
                        ClientId = "image.gallery",
                        ClientName = "Image Gallery",
                        ClientSecrets = { new Secret("secret".Sha256()) },
                        AllowedGrantTypes = GrantTypes.Hybrid,
                        RedirectUris = 
                        {
                            "http://localhost:8000/signin-oidc" 
                        },
                        // FrontChannelLogoutUri = "http://localhost:8000/signout-oidc",
                        PostLogoutRedirectUris = 
                        { 
                            "http://localhost:8000/signout-callback-oidc" 
                        },

                        // AllowOfflineAccess = true,
                        AllowedScopes = 
                        {  
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            IdentityServerConstants.StandardScopes.Address,
                            "employment",
                            "roles"
                        },
                        // AllowAccessTokensViaBrowser = true
                    },
                };
            }
        }
    }
}