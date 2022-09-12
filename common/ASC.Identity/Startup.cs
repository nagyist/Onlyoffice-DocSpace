// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

using System.Security.Cryptography;

using ASC.Api.Core;
using ASC.Common.Utils;
using ASC.Identity.Controllers;

using Microsoft.IdentityModel.Tokens;

namespace ASC.Identity;

public class Startup : BaseWorkerStartup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment) : base(configuration, hostEnvironment)
    {
        _configuration = configuration;
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        var certificatePath = _configuration["oidc:certificate:path"];
        var certificatePassword = _configuration["oidc:certificate:password"];
        var certificateAlg = _configuration["oidc:certificate:alg"];

        if (certificateAlg != SecurityAlgorithms.EcdsaSha512)
        {
            throw new Exception(String.Format("unsupported {0}  certificate algoritm", certificateAlg));
        }
        
        var ecdsa = ECDsa.Create();

        ecdsa.ImportFromEncryptedPem(File.ReadAllText(certificatePath), certificatePassword);

        var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
        var configurationExtension = new ConfigurationExtension(_configuration);

        var connectionString = configurationExtension.GetConnectionStrings("default");

        services.AddControllersWithViews();

        // Adds IdentityServer
        services.AddIdentityServer(x =>
        {
            x.IssuerUri = "null";
            x.Authentication.CookieLifetime = TimeSpan.FromHours(2);
        })
        .AddSigningCredential(new ECDsaSecurityKey(ecdsa), IdentityServerConstants.ECDsaSigningAlgorithm.ES512)
        .AddConfigurationStore(options =>
        {
            options.ApiResource.Name = "identity_api_resources";
            options.ApiScope.Name = "identity_api_scopes";
            options.ApiScopeClaim.Name = "identity_api_scope_claims";
            options.ApiScopeProperty.Name = "identity_api_scope_properties";
            options.ApiResourceProperty.Name = "identity_api_resource_properties";
            options.ApiResourceClaim.Name = "identity_resource_claims";
            options.ApiResourceScope.Name = "identity_resource_scopes";
            options.ApiResourceProperty.Name = "identity_resource_properties";
            options.ApiResourceSecret.Name = "identity_resource_secrets";
            options.Client.Name = "identity_clients";
            options.ClientClaim.Name = "identity_client_claims";
            options.ClientCorsOrigin.Name = "identity_client_cors_origin";
            options.ClientGrantType.Name = "identity_client_grant_types";
            options.ClientIdPRestriction.Name = "identity_client_idp_restrictions";
            options.ClientPostLogoutRedirectUri.Name = "identity_client_post_logout_redirect_uri";
            options.ClientProperty.Name = "identity_client_properties";
            options.ClientRedirectUri.Name = "identity_client_redirect_uri";
            options.ClientScopes.Name = "identity_client_scopes";
            options.ClientSecret.Name = "identity_client_secrets";
            options.IdentityResource.Name = "identity_identity_resources";
            options.IdentityResourceClaim.Name = "identity_identity_resource_claims";
            options.IdentityResourceProperty.Name = "identity_identity_resource_properties";

            options.ConfigureDbContext = builder => builder.UseMySql(connectionString.ConnectionString, ServerVersion.Parse("8.0.25"),
                        mySqlOptionsAction: sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(migrationsAssembly);

                            //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        });
        })
        .AddOperationalStore(options =>
        {
            options.DeviceFlowCodes.Name = "identity_device_flow_codes";
            options.PersistedGrants.Name = "identity_persisted_grants";

            options.ConfigureDbContext = builder => builder.UseMySql(connectionString.ConnectionString, ServerVersion.Parse("8.0.25"),
                mySqlOptionsAction: sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(migrationsAssembly);

                    //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
        });

        services.AddTransient<IProfileService, ProfileService>();

        DIHelper.TryAdd<ProfileService>();
        DIHelper.TryAdd<AccountController>();
        DIHelper.TryAdd<ConsentController>();


        //services.ConfigureApplicationCookie(options =>
        //{
        //    options.Cookie.Name = "identity";
        //    options.Events.OnValidatePrincipal = < method to execute on validating principal event>           
        //});
    }

    public override void Configure(IApplicationBuilder app)
    {
        base.Configure(app);

        app.UseStaticFiles();

        app.UseIdentityServer();

        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });
    }
}