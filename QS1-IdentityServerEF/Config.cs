// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace QS1_IdentityServerEF
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources => Create();

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("APIIdentity", "Identity API Test"),
                new ApiScope("scope1"),
                new ApiScope("scope2"),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client()
                {
                    ClientId = "Client-1",
                    AllowedGrantTypes = { GrantType.ClientCredentials },
                    ClientSecrets =
                    {
                        new Secret("golem".Sha256())
                    },
                    AllowedScopes = { "APIIdentity" }
                },
                new Client
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "scope1" }
                },
                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { "https://localhost:44300/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "scope2" }
                },
                 new Client()
                 {
                     ClientId = "mvc",
                     ClientSecrets = { new Secret("mvc-secret".Sha256()) },
                     AllowedGrantTypes = GrantTypes.Code,
                     RedirectUris = { "https://localhost:44385/signin-oidc" },
                     PostLogoutRedirectUris = { "https://localhost:44385/signout-callback-oidc" },
                     AllowedScopes = new List<string>
                     {
                         IdentityServerConstants.StandardScopes.OpenId,
                         IdentityServerConstants.StandardScopes.Profile,
                         "ctr",
                         "APIIdentity",
                     },
                     AllowOfflineAccess = true,

                 },
                 new Client()
                 {
                     ClientId = "js",
                     ClientName = "Javascript Client",
                     AllowedGrantTypes = GrantTypes.Code,
                     RequireClientSecret = false,
                    RedirectUris = { "https://localhost:6002/callback.html" },
                    PostLogoutRedirectUris = { "https://localhost:6002/index.html" },
                    AllowedCorsOrigins = { "https://localhost:6002" },
                    AllowedScopes = new List<string>
                     {
                         IdentityServerConstants.StandardScopes.OpenId,
                         IdentityServerConstants.StandardScopes.Profile,
                         "ctr",
                         "APIIdentity",
                     },

                 }
            };

        private static IEnumerable<IdentityResource> Create()
        {
            IdentityResource[] resouces = new IdentityResource[] {
                new IdentityResources.Profile(),
                new IdentityResources.OpenId(),
                new IdentityResources.OpenId(),
            };

            resouces[0].UserClaims = new[] { JwtClaimTypes.Name, JwtClaimTypes.GivenName };
            resouces[1].UserClaims = new[] { JwtClaimTypes.Name, JwtClaimTypes.GivenName };
            resouces[1].Name = "ctr";
            return resouces;
        }
    }
}