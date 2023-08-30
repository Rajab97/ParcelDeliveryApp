using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;

namespace UserManagementService.Identity.Statics
{
    public static class IdentityConfig
    {
        public static IEnumerable<IdentityResource> IdentityResources = new List<IdentityResource>() {
            new IdentityResources.OpenId(),
            new IdentityResources.Email(),
            new IdentityResources.Phone(),

            new IdentityResource()
            {
                Name = "user-profile",
                Description = "Get detailed profile information about user",
                Emphasize = true,
                DisplayName = "User profile",
                UserClaims = new[]{ CustomClaims.FirstName,
                                    CustomClaims.LastName,
                                    JwtClaimTypes.Address,
                                    JwtClaimTypes.BirthDate,
                                    JwtClaimTypes.Gender,
                }
            },
            new IdentityResource()
            {
                Name = "role",
                DisplayName = "Given role to user"
            }
        };
        public static IEnumerable<ApiScope> ApiScopes = new List<ApiScope>()
        {
               new ApiScope()
               {
                   Name= MyApiScopes.DeliveryManagement.CheckOrderCancelation,
                   Description = "This scope is for checking cancelation status from DeliveryManagementService"
               },
            new ApiScope()
               {
                   Name= MyApiScopes.UserManagement.GetCouriers,
                   Description = "This scope is for get couriers data from UserManagementService"
               }
        };
        public static IEnumerable<Client> Clients = new List<Client>()
        {
            #region Server to Server Communication
            new Client()
            {
                ClientId = "order-managemnet",
                ClientSecrets = new List<Secret>() { new Secret("secret".Sha256())},
                AllowedScopes = new[]{ MyApiScopes.DeliveryManagement.CheckOrderCancelation , MyApiScopes.UserManagement.GetCouriers },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AccessTokenLifetime = TimeSpan.FromMinutes(1).Seconds
                
            },
            #endregion
            #region Intercative applicaiton communication
            new Client()
            {
                ClientId = "interactive",
                AllowedGrantTypes = GrantTypes.Code,
                AllowOfflineAccess = true,
                    ClientSecrets = { new Secret("secret".Sha256()) },

                RedirectUris =           { "http://localhost:21402/signin-oidc" },
                PostLogoutRedirectUris = { "http://localhost:21402/" },
                FrontChannelLogoutUri =    "http://localhost:21402/signout-oidc",
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityScopes.Role
                },
            }
            #endregion
        };

        public static IEnumerable<ApiResource> ApiResources = new List<ApiResource>()
        {
            new ApiResource() {Name = "delivery" , DisplayName = "DeliveryManagementService", Scopes = { MyApiScopes.DeliveryManagement.CheckOrderCancelation } },
            new ApiResource() {Name = "user" , DisplayName = "UserManagementService", Scopes = { MyApiScopes.UserManagement.GetCouriers } }
        };
    }
}
