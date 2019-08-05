using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace Safari.IDP
{
    public static class Config
    {
        public static IEnumerable<ApiResource> ApiResources
        {
            get
            {
                return new List<ApiResource>
                {
                    new ApiResource
                    {
                        Name = "api",
                        DisplayName = "Member Image Gallery API",
                        Scopes =
                        {
                            new Scope {
                                Name = "member.imagegallery.api",
                                DisplayName = "Member Access to the API"
                            },
                            new Scope {
                                Name = "guest.imagegallery.api",
                                DisplayName = "Guest Access to the API"
                            }
                        }
                    },
                    new ApiResource("imagegallery.api", "Image Gallery API")
                };
            }
        }

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
                            "roles",
                            "imagegallery.api",
                            "member.imagegallery.api",
                            "guest.imagegallery.api"
                        },
                        // AllowAccessTokensViaBrowser = true
                    },
                    new Client
                    {
                        ClientId = "image.gallery.ownerpassword",
                        ClientName = "Image Gallery",
                        ClientSecrets = { new Secret("secret".Sha256()) },
                        AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
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
                            "roles",
                            "imagegallery.api"
                        },
                        // AllowAccessTokensViaBrowser = true
                    },
                };
            }
        }
    }
}