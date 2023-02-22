﻿// (c) Copyright Ascensio System SIA 2010-2022
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

using JsonConverter = System.Text.Json.Serialization.JsonConverter;

namespace ASC.Api.Core;

public abstract class BaseStartup
{
    private const string CustomCorsPolicyName = "Basic";
    private const string BasicAuthScheme = "Basic";
    private const string MultiAuthSchemes = "MultiAuthSchemes";

    protected readonly IConfiguration _configuration;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly string _corsOrigin;

    protected virtual JsonConverter[] Converters { get; }
    protected virtual bool AddControllersAsServices { get; }
    protected virtual bool ConfirmAddScheme { get; }
    protected virtual bool AddAndUseSession { get; }
    protected DIHelper DIHelper { get; }
    protected bool LoadProducts { get; set; } = true;
    protected bool LoadConsumers { get; } = true;

    public BaseStartup(IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        _configuration = configuration;
        _hostEnvironment = hostEnvironment;

        _corsOrigin = _configuration["core:cors"];

        DIHelper = new DIHelper();

        if (bool.TryParse(_configuration["core:products"], out var loadProducts))
        {
            LoadProducts = loadProducts;
        }
    }

    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddCustomHealthCheck(_configuration);
        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        services.AddHttpClient();

        services.AddScoped<EFLoggerFactory>();
        services.AddBaseDbContextPool<AccountLinkContext>();
        services.AddBaseDbContextPool<CoreDbContext>();
        services.AddBaseDbContextPool<TenantDbContext>();
        services.AddBaseDbContextPool<UserDbContext>();
        services.AddBaseDbContextPool<TelegramDbContext>();
        services.AddBaseDbContextPool<FirebaseDbContext>();
        services.AddBaseDbContextPool<CustomDbContext>();
        services.AddBaseDbContextPool<WebstudioDbContext>();
        services.AddBaseDbContextPool<InstanceRegistrationContext>();
        services.AddBaseDbContextPool<IntegrationEventLogContext>();
        services.AddBaseDbContextPool<FeedDbContext>();
        services.AddBaseDbContextPool<MessagesContext>();
        services.AddBaseDbContextPool<WebhooksDbContext>();

        if (AddAndUseSession)
        {
            services.AddSession();
        }

        DIHelper.Configure(services);

        Action<JsonOptions> jsonOptions = options =>
            {
                options.JsonSerializerOptions.WriteIndented = false;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add(new ApiDateTimeConverter());

                if (Converters != null)
                {
                    foreach (var c in Converters)
                    {
                        options.JsonSerializerOptions.Converters.Add(c);
                    }
                }
            };

        services.AddControllers()
            .AddXmlSerializerFormatters()
            .AddJsonOptions(jsonOptions);

        services.AddSingleton(jsonOptions);

        DIHelper.AddControllers();
        DIHelper.TryAdd<CultureMiddleware>();
        DIHelper.TryAdd<IpSecurityFilter>();
        DIHelper.TryAdd<PaymentFilter>();
        DIHelper.TryAdd<ProductSecurityFilter>();
        DIHelper.TryAdd<TenantStatusFilter>();
        DIHelper.TryAdd<ConfirmAuthHandler>();
        DIHelper.TryAdd<BasicAuthHandler>();
        DIHelper.TryAdd<CookieAuthHandler>();
        DIHelper.TryAdd<WebhooksGlobalFilterAttribute>();


        if (!string.IsNullOrEmpty(_corsOrigin))
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: CustomCorsPolicyName,
                                  policy =>
                                  {
                                      policy.WithOrigins(_corsOrigin)
                                      .SetIsOriginAllowedToAllowWildcardSubdomains()
                                      .AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .AllowCredentials();
                                  });
            });
        }

        services.AddDistributedCache(_configuration);
        services.AddEventBus(_configuration);
        services.AddDistributedTaskQueue();
        services.AddCacheNotify(_configuration);

        services.RegisterFeature();

        DIHelper.TryAdd(typeof(IWebhookPublisher), typeof(WebhookPublisher));

        if (LoadProducts)
        {
            DIHelper.RegisterProducts(_configuration, _hostEnvironment.ContentRootPath);
        }

        services.AddOptions();

        services.AddMvcCore(config =>
        {
            config.Conventions.Add(new ControllerNameAttributeConvention());

            var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

            config.Filters.Add(new AuthorizeFilter(policy));
            config.Filters.Add(new TypeFilterAttribute(typeof(TenantStatusFilter)));
            config.Filters.Add(new TypeFilterAttribute(typeof(PaymentFilter)));
            config.Filters.Add(new TypeFilterAttribute(typeof(IpSecurityFilter)));
            config.Filters.Add(new TypeFilterAttribute(typeof(ProductSecurityFilter)));
            config.Filters.Add(new CustomResponseFilterAttribute());
            config.Filters.Add(new CustomExceptionFilterAttribute());
            config.Filters.Add(new TypeFilterAttribute(typeof(WebhooksGlobalFilterAttribute)));
            config.Filters.Add(new TypeFilterAttribute(typeof(FormatFilter)));


            config.OutputFormatters.RemoveType<XmlSerializerOutputFormatter>();
            config.OutputFormatters.Add(new XmlOutputFormatter());
        })
        .AddApiExplorer();

        var authBuilder = services.AddAuthentication(options =>
        {
            options.DefaultScheme = MultiAuthSchemes;
            options.DefaultChallengeScheme = MultiAuthSchemes;
        }).AddScheme<AuthenticationSchemeOptions, CookieAuthHandler>(CookieAuthenticationDefaults.AuthenticationScheme, a => { })
          .AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>(BasicAuthScheme, a => { })
          .AddScheme<AuthenticationSchemeOptions, ConfirmAuthHandler>("confirm", a => { })
          .AddJwtBearer("Bearer", options =>
            {
                options.Authority = _configuration["core:oidc:authority"];
                options.IncludeErrorDetails = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = ctx =>
                    {
                        using var scope = ctx.HttpContext.RequestServices.CreateScope();

                        var securityContext = scope.ServiceProvider.GetService<ASC.Core.SecurityContext>();

                        var claimUserId = ctx.Principal.FindFirstValue("userId");

                        if (string.IsNullOrEmpty(claimUserId))
                        {
                            throw new Exception("Claim 'UserId' is not present in claim list");
                        }

                        var userId = new Guid(claimUserId);

                        securityContext.AuthenticateMeWithoutCookie(userId, ctx.Principal.Claims.ToList());

                        return Task.CompletedTask;
                    }
                };
            })
          .AddPolicyScheme(MultiAuthSchemes, JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    var authorizationHeader = context.Request.Headers[HeaderNames.Authorization].FirstOrDefault();

                    if (string.IsNullOrEmpty(authorizationHeader))
                    {
                        return CookieAuthenticationDefaults.AuthenticationScheme;
                    }

                    if (authorizationHeader.StartsWith("Basic "))
                    {
                        return BasicAuthScheme;
                    }

                    if (authorizationHeader.StartsWith("Bearer "))
                    {
                        var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                        var jwtHandler = new JwtSecurityTokenHandler();

                        if (jwtHandler.CanReadToken(token))
                        {
                            var issuer = jwtHandler.ReadJwtToken(token).Issuer;
                            if (!string.IsNullOrEmpty(issuer) && issuer.Equals(_configuration["core:oidc:authority"]))
                            {
                                return JwtBearerDefaults.AuthenticationScheme;
                            }
                        }
                    }

                    return CookieAuthenticationDefaults.AuthenticationScheme;
                };
            });

        services.AddAutoMapper(GetAutoMapperProfileAssemblies());

        if (!_hostEnvironment.IsDevelopment())
        {
            services.AddStartupTask<WarmupServicesStartupTask>()
                    .TryAddSingleton(services);
        }

        services.AddSwaggerGen();
    }

    public static IEnumerable<Assembly> GetAutoMapperProfileAssemblies()
    {
        return AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name.StartsWith("ASC."));
    }

    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseRouting();

        if (!string.IsNullOrEmpty(_corsOrigin))
        {
            app.UseCors(CustomCorsPolicyName);
        }

        if (AddAndUseSession)
        {
            app.UseSession();
        }

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseCultureMiddleware();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapCustom();

            endpoints.MapSwagger();

            endpoints.MapHealthChecks("/health", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });
        });
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.Register(_configuration, LoadProducts, LoadConsumers);
    }
}

