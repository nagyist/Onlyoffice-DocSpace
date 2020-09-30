﻿using System;
using System.Linq;
using System.Net;
using System.Security;

using ASC.Api.Core;
using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.MessagingSystem;
using ASC.Security.Cryptography;
using ASC.Web.Api.Routing;
using ASC.Web.Core;
using ASC.Web.Core.Utility;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Web.Api.Controllers
{
    [DefaultRoute]
    [ApiController]
    public class PortalController : ControllerBase
    {

        public Tenant Tenant { get { return ApiContext.Tenant; } }

        private ApiContext ApiContext { get; }
        private UserManager UserManager { get; }
        private TenantManager TenantManager { get; }
        private PaymentManager PaymentManager { get; }
        private CommonLinkUtility CommonLinkUtility { get; }
        private UrlShortener UrlShortener { get; }
        private AuthContext AuthContext { get; }
        private WebItemSecurity WebItemSecurity { get; }
        private SecurityContext SecurityContext { get; }
        private SettingsManager SettingsManager { get; }
        private IConfiguration Configuration { get; set; }
        public ILog Log { get; }


        public PortalController(
            IOptionsMonitor<ILog> options,
            ApiContext apiContext,
            UserManager userManager,
            TenantManager tenantManager,
            PaymentManager paymentManager,
            CommonLinkUtility commonLinkUtility,
            UrlShortener urlShortener,
            AuthContext authContext,
            WebItemSecurity webItemSecurity,
            SecurityContext securityContext,
            SettingsManager settingsManager,
            IConfiguration configuration
            )
        {
            Log = options.CurrentValue;
            ApiContext = apiContext;
            UserManager = userManager;
            TenantManager = tenantManager;
            PaymentManager = paymentManager;
            CommonLinkUtility = commonLinkUtility;
            UrlShortener = urlShortener;
            AuthContext = authContext;
            WebItemSecurity = webItemSecurity;
            SecurityContext = securityContext;
            SettingsManager = settingsManager;
            Configuration = configuration;
        }

        [Read("")]
        public Tenant Get()
        {
            return Tenant;
        }

        [Read("users/{userID}")]
        public UserInfo GetUser(Guid userID)
        {
            return UserManager.GetUsers(userID);
        }

        [Read("users/invite/{employeeType}")]
        public string GeInviteLink(EmployeeType employeeType)
        {
            if (!WebItemSecurity.IsProductAdministrator(WebItemManager.PeopleProductID, AuthContext.CurrentAccount.ID))
            {
                throw new SecurityException("Method not available");
            }

            return CommonLinkUtility.GetConfirmationUrl(string.Empty, ConfirmType.LinkInvite, (int)employeeType)
                   + $"&emplType={employeeType:d}";
        }

        [Update("getshortenlink")]
        public string GetShortenLink(string link)
        {
            try
            {
                return UrlShortener.Instance.GetShortenLink(link);
            }
            catch (Exception ex)
            {
                Log.Error("getshortenlink", ex);
                return link;
            }
        }


        [Read("usedspace")]
        public double GetUsedSpace()
        {
            return Math.Round(
                TenantManager.FindTenantQuotaRows(new TenantQuotaRowQuery(Tenant.TenantId))
                           .Where(q => !string.IsNullOrEmpty(q.Tag) && new Guid(q.Tag) != Guid.Empty)
                           .Sum(q => q.Counter) / 1024f / 1024f / 1024f, 2);
        }


        [Read("userscount")]
        public long GetUsersCount()
        {
            return UserManager.GetUserNames(EmployeeStatus.Active).Count();
        }

        [Read("tariff")]
        public Tariff GetTariff()
        {
            return PaymentManager.GetTariff(Tenant.TenantId);
        }

        [Read("quota")]
        public TenantQuota GetQuota()
        {
            return TenantManager.GetTenantQuota(Tenant.TenantId);
        }

        [Read("quota/right")]
        public TenantQuota GetRightQuota()
        {
            var usedSpace = GetUsedSpace();
            var needUsersCount = GetUsersCount();

            return TenantManager.GetTenantQuotas().OrderBy(r => r.Price)
                              .FirstOrDefault(quota =>
                                              quota.ActiveUsers > needUsersCount
                                              && quota.MaxTotalSize > usedSpace
                                              && !quota.Year);
        }


        [Read("path")]
        public string GetFullAbsolutePath(string virtualPath)
        {
            return CommonLinkUtility.GetFullAbsolutePath(virtualPath);
        }

        [Read("thumb")]
        public FileResult GetThumb(string url)
        {
            if (!SecurityContext.IsAuthenticated || !(Configuration["bookmarking:thumbnail-url"] != null))
            {
                return null;
            }

            url = url.Replace("&amp;", "&");
            url = WebUtility.UrlEncode(url);

            using var wc = new WebClient();
            var bytes = wc.DownloadData(string.Format(Configuration["bookmarking:thumbnail-url"], url));
            var type = wc.ResponseHeaders["Content-Type"] ?? "image/png";
            return File(bytes, type);
        }

        [Create("present/mark")]
        public void MarkPresentAsReaded()
        {
            try
            {
                var settings = SettingsManager.LoadForCurrentUser<OpensourcePresentSettings>();
                settings.Readed = true;
                _ = SettingsManager.SaveForCurrentUser(settings);
            }
            catch (Exception ex)
            {
                Log.Error("MarkPresentAsReaded", ex);
            }
        }
    }

    public static class PortalControllerExtension
    {
        public static DIHelper AddPortalController(this DIHelper services)
        {
            return services
                .AddUrlShortener()
                .AddMessageServiceService()
                .AddStudioNotifyServiceService()
                .AddApiContextService()
                .AddUserManagerService()
                .AddAuthContextService()
                .AddAuthContextService()
                .AddTenantManagerService()
                .AddEmailValidationKeyProviderService()
                .AddPaymentManagerService()
                .AddCommonLinkUtilityService()
                .AddAuthContextService()
                .AddWebItemSecurity()
                .AddSecurityContextService();
        }
    }
}