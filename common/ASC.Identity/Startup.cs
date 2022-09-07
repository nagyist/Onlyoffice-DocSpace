using ASC.Api.Core;
using ASC.Common;
using ASC.Common.Utils;
using ASC.Identity.Controllers;

namespace ASC.Identity;

public class Startup : BaseWorkerStartup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment):base(configuration, hostEnvironment)
    {
        _configuration = configuration;
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

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
        .AddSigningCredential(Certificate.Get())
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

        DIHelper.TryAdd<IProfileService, ProfileService>();
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


//services.AddBaseDbContextPool<PersistedGrantDbContext>();
//services.AddBaseDbContextPool<ConfigurationDbContext>();
//.AddInMemoryIdentityResources(Config.IdentityResources)
//.AddInMemoryApiScopes(Config.ApiScopes)
//.AddInMemoryApiResources(Config.ApiResources)
//.AddInMemoryClients(Config.Clients);