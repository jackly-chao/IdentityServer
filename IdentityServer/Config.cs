using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;

namespace IdentityServer
{
    /// <summary>
    /// IdentityServer初始配置
    /// </summary>
    public class Config
    {
        #region 资源
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource(nameof(IdentityResource),"认证资源",new List<string>{ nameof(IdentityResource) }),
                new IdentityResource("capi",new List<string> { "C端API"} )
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("testApi", "TestApi")
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope("testApi", "TestApi"),
            };
        }
        #endregion

        #region 客户端
        public static IEnumerable<Client> GetClients()
        {
            //授权类型的区别可参考：https://www.cnblogs.com/pangjianxin/p/9279865.html
            return new List<Client>
            {
                new Client 
                {
                    ClientId = "testClient11",
                    ClientName = "TestClient11",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris={"https://localhost:6001/oauth2-redirect.html",},
                    PostLogoutRedirectUris={"https://localhost:6001",},
                    AllowedCorsOrigins ={"https://localhost:6001",},
                    AccessTokenLifetime=3600,
                    AllowedScopes = {
                        "testApi"
                    }
                },

                ////授权方式参考：https://github.com/IdentityServer/IdentityServer4/blob/main/samples/Quickstarts/1_ClientCredentials/src/IdentityServer/Config.cs
                //new Client
                //{
                //    ClientId = "client",
                //    ClientSecrets = 
                //    {
                //        new Secret("secret".Sha256()),
                //    },
                //    AllowedGrantTypes = GrantTypes.ClientCredentials,
                //    //允许访问的资源
                //    AllowedScopes =
                //    {
                //        nameof(ApiScope),
                //    },
                //},
                ////授权方式参考：https://github.com/IdentityServer/IdentityServer4/blob/main/samples/Quickstarts/2_InteractiveAspNetCore/src/IdentityServer/Config.cs
                //new Client
                //{
                //    ClientId = "mvc",
                //    ClientSecrets =
                //    {
                //        new Secret("secret".Sha256()),
                //    },
                //    AllowedGrantTypes = GrantTypes.Code,
                //    //允许访问的资源
                //    AllowedScopes =
                //    {
                //        IdentityServerConstants.StandardScopes.OpenId,
                //        IdentityServerConstants.StandardScopes.Profile,
                //        nameof(ApiScope),
                //    },
                //    //登录成功后的回调地址
                //    RedirectUris =
                //    {
                //        "https://localhost:5002/signin-oidc"
                //    },
                //    //退出登录后的回调地址
                //    PostLogoutRedirectUris =
                //    {
                //        "https://localhost:5002/signout-callback-oidc"
                //    },
                //},
                ////授权方式参考：https://github.com/IdentityServer/IdentityServer4/blob/main/samples/Quickstarts/4_JavaScriptClient/src/IdentityServer/Config.cs
                //new Client
                //{
                //    ClientId = "js",
                //    ClientName = "JavaScript Client",
                //    RequireClientSecret = false,
                //    AllowedGrantTypes = GrantTypes.Code,
                //    //允许访问的资源
                //    AllowedScopes = 
                //    {
                //        IdentityServerConstants.StandardScopes.OpenId,
                //        IdentityServerConstants.StandardScopes.Profile,
                //        nameof(ApiResource),
                //    },
                //    //登录成功后的回调地址
                //    RedirectUris =
                //    {
                //        "https://localhost:5003/callback.html"
                //    },
                //    //退出登录后的回调地址
                //    PostLogoutRedirectUris =
                //    {
                //        "https://localhost:5003/index.html"
                //    },
                //    //允许跨域的地址
                //    AllowedCorsOrigins =     
                //    { 
                //        "https://localhost:5003" 
                //    },
                //},
                //new Client {
                //    ClientId = "CAPI",
                //    ClientName = "C端API",
                //    AllowedGrantTypes = GrantTypes.Implicit,
                //    AllowAccessTokensViaBrowser = true,

                //    RedirectUris =
                //    {
                //        "https://localhost:6001/oauth2-redirect.html",
                //    },
                //    PostLogoutRedirectUris = { "https://localhost:6001", "http://localhost:2364" },
                //    AllowedCorsOrigins =     { "https://localhost:6001", "http://localhost:2364"  },

                //    AccessTokenLifetime=3600,

                //    AllowedScopes = {
                //        IdentityServerConstants.StandardScopes.OpenId,
                //        IdentityServerConstants.StandardScopes.Profile,
                //           nameof(ApiScope),
                //           nameof(ApiResource),
                //    }
                //},
            };
        }
        #endregion

        #region 用户
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "test",
                    Password = "test123"
                },
            };
        }
        #endregion
    }
}
