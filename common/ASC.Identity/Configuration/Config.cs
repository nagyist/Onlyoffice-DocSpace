public class Config
{
    public static IEnumerable<ApiResource> GetApiResources()
    {
        return new List<ApiResource>
            {
                new ApiResource("crud", "crud API")
                {
                    Scopes = { "read", "write" }
                }
            };
    }

    public static IEnumerable<ApiScope> GetApiScopes()
    {
        return new List<ApiScope>
        {
            new ApiScope(name: "read", displayName: "Read you data"),
            new ApiScope(name: "write", displayName: "Write you data")
        };
    }

    public static IEnumerable<IdentityResource> GetIdentityResources()
    {
        return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Phone(),
                new IdentityResources.Email(),
                new IdentityResources.Address(),
                new IdentityResource("roles", "User role(s)", new List<string> { "role" })
            };
    }

    public static IEnumerable<Client> GetClients(Dictionary<string, string> clientsUrl)
    {
        return new List<Client>
            {

                new Client
                {
                    ClientId = "client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                        
                    // scopes that client has access to
                    AllowedScopes = { "invoice.pay", "manage", "customer.contact" },
                },
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    ClientSecrets = new List<Secret> { new Secret("secret".Sha256()) },
                    ClientUri = $"{clientsUrl["Mvc"]}",
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowAccessTokensViaBrowser = false,
                    RequireConsent = true,
                    AllowOfflineAccess = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    
                    // where to redirect to after login
                    RedirectUris = new List<string>
                    {
                        $"{clientsUrl["Mvc"]}/signin-oidc"
                    },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = new List<string>
                    {
                        $"{clientsUrl["Mvc"]}/signout-callback-oidc"
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        "roles",
                        "read",
                        "write"
                    },
                    AccessTokenLifetime = 60*60*2, // 2 hours
                    IdentityTokenLifetime= 60*60*2 // 2 hours
                },
                new Client
                {
                    ClientId = "js",
                    ClientName = "JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false,

                    RedirectUris =           { "https://localhost:5003/callback.html" },
                    PostLogoutRedirectUris = { "https://localhost:5003/index.html" },
                    AllowedCorsOrigins =     { "https://localhost:5003" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "read",
                        "write"
                    }
                }
            };
    }
}


//public static class Config
//{
//    public static IEnumerable<IdentityResource> IdentityResources =>
//        new IdentityResource[]
//        {
//                new IdentityResources.OpenId(),
//                new IdentityResources.Profile(),
//                new IdentityResources.Phone(),
//                new IdentityResources.Email(),
//                new IdentityResources.Address(),
//                new IdentityResource("roles", "User role(s)", new List<string> { "role" })
//        };

//    public static IEnumerable<ApiScope> ApiScopes =>
//        new ApiScope[]
//        {
//                 new ApiScope("api1", "My API"),
//                 new ApiScope(name: "read", displayName: "Read you data"),
//                 new ApiScope(name: "write", displayName: "Write you data"),
//                 new ApiScope(name: "delete", displayName: "Delete you data"),
//                 new ApiScope("api4", "My API"),

//                 // invoice API specific scopes
//                 new ApiScope(name: "invoice.read",   displayName: "Reads your invoices."),
//                 new ApiScope(name: "invoice.pay",    displayName: "Pays your invoices."),

//                 // customer API specific scopes
//                 new ApiScope(name: "customer.read",    displayName: "Reads you customers information."),
//                 new ApiScope(name: "customer.contact", displayName: "Allows contacting one of your customers."),

//                 // shared scope
//                 new ApiScope(name: "manage", displayName: "Provides administrative access to invoice and customer data.")
//        };

//    public static IEnumerable<ApiResource> ApiResources =>
//        new List<ApiResource>
//                {
//                        new ApiResource("crud", "crud API")
//                        {
//                            Scopes = { "read", "write", "delete" }
//                        },
//                         new ApiResource("invoice", "Invoice API")
//                        {
//                            Scopes = { "invoice.read", "invoice.pay", "manage" }
//                        },

//                        new ApiResource("customer", "Customer API")
//                        {
//                            Scopes = { "customer.read", "customer.contact", "manage" }
//                        }
//                };


//    public static IEnumerable<Client> Clients =>
//        new Client[]
//        {
//                new Client
//                        {
//                            ClientId = "client",

//                            // no interactive user, use the clientid/secret for authentication
//                            AllowedGrantTypes = GrantTypes.ClientCredentials,

//                            // secret for authentication
//                            ClientSecrets =
//                            {
//                                new Secret("secret".Sha256())
//                            },

//                            // scopes that client has access to
//                            AllowedScopes = { "invoice.pay", "manage", "customer.contact" },
//                        },
//                 // interactive ASP.NET Core MVC client
//                new Client
//                        {
//                            ClientId = "mvc",
//                            ClientSecrets = { new Secret("secret".Sha256()) },
//                            RequireConsent = false,
//                            RequirePkce = false,
//                            AllowedGrantTypes = GrantTypes.Code,


//                            // where to redirect to after login
//                            RedirectUris = { "https://localhost:5002/signin-oidc" },

//                            // where to redirect to after logout
//                            PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

//                            AllowedScopes = new List<string>
//                            {
//                                IdentityServerConstants.StandardScopes.OpenId,
//                                IdentityServerConstants.StandardScopes.Profile,
//                                IdentityServerConstants.StandardScopes.Phone,
//                                IdentityServerConstants.StandardScopes.Email,
//                                IdentityServerConstants.StandardScopes.Address,
//                                "roles",
//                                "read",
//                                "write"
//                            }
//                        },
//                new Client
//                        {
//                            ClientId = "js",
//                            ClientName = "JavaScript Client",
//                            AllowedGrantTypes = GrantTypes.Code,
//                            RequireClientSecret = false,

//                            RedirectUris =           { "https://localhost:5003/callback.html" },
//                            PostLogoutRedirectUris = { "https://localhost:5003/index.html" },
//                            AllowedCorsOrigins =     { "https://localhost:5003" },

//                            AllowedScopes =
//                            {
//                                IdentityServerConstants.StandardScopes.OpenId,
//                                IdentityServerConstants.StandardScopes.Profile,
//                                "read",
//                                "write"
//                            }
//                        }
//        };
//}

