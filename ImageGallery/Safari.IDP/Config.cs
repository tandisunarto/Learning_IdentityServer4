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
                        Name = "imagegallery_api",
                        DisplayName = "User Image Gallery API",
                        Scopes =
                        {
                            new Scope {
                                Name = "payinguser_imagegallery_api",
                                DisplayName = "Paying User Access to the API",
                            },
                            new Scope {
                                Name = "freeuser_imagegallery_api",
                                DisplayName = "Free User Access to the API",
                            }
                        }
                    },
                    new ApiResource(
                        "imagegallery_api_roles", 
                        "Image Gallery API with Roles",
                        new List<string>() { 
                            "role" 
                        })
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
                    },
                    new IdentityResource {
                        Name = "country",
                        DisplayName = "The country you are living in",
                        UserClaims = {
                            "country"
                        }
                    },
                    new IdentityResource {
                        Name = "subscription_level",
                        DisplayName = "Your Subscription Level",
                        UserClaims = {
                            "subscription_level"
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
                        ClientId = "image_gallery",
                        ClientName = "Image Gallery",
                        ClientSecrets = { new Secret("secret".Sha256()) },
                        AllowedGrantTypes = GrantTypes.Hybrid,
                        AccessTokenLifetime = 120,
                        AllowOfflineAccess = true,
                        UpdateAccessTokenClaimsOnRefresh = true,
                        // IdentityTokenLifetime =
                        // AuthorizationCodeLifetime
                        // AbsoluteRefreshTokenLifetime 
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
                            // "imagegallery_api",
                            "payinguser_imagegallery_api",
                            "freeuser_imagegallery_api",
                            "imagegallery_api_roles",
                            "country",
                            "subscription_level"
                        },
                        // AllowAccessTokensViaBrowser = true
                    },
                    new Client
                    {
                        ClientId = "image_gallery_ownerpassword",
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
                            "imagegallery_api"
                        },
                        // AllowAccessTokensViaBrowser = true
                    },
                };
            }
        }
    }
}