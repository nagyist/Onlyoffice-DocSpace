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

using ASC.FederatedLogin;
using ASC.Geolocation;

using Microsoft.Extensions.Options;

using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Studio.Core.Tariffs;


[Scope]
public class SaasTariffsManager
{
    private readonly ILogger<SaasTariffsManager> _log;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;
    private readonly PaymentManager _paymentManager;
    private readonly WebItemSecurity _webItemSecurity;
    private readonly SettingsManager _settingsManager;
    private readonly TenantWhiteLabelSettingsHelper _tenantWhiteLabelSettingsHelper;
    private readonly TenantStatisticsProvider _tenantStatisticsProvider;
    private readonly GeolocationHelper _geolocationHelper;
    private readonly IOptionsSnapshot<AccountLinker> _accountLinker;
    private readonly Constants _constants;

    private readonly IDictionary<string, Dictionary<string, decimal>> _priceInfo;
    private readonly IEnumerable<TenantQuota> _quotaList;

    private readonly Tenant _currentTenant;
    private readonly Tariff _currentTariff;
    private readonly TenantQuota _currentQuota;
    private readonly RegionInfo _currentRegion;

    private bool CurrentTariffNonProfit { get => _currentQuota.NonProfit; }
    private bool CurrentTariffStartup { get => _currentQuota.Free; }
    private bool CurrentTariffBusiness { get => !_currentQuota.Free; }
    private bool CurrentTariffTrial { get => _currentQuota.Trial; }
    private bool CurrentTariffExpired { get => _currentTariff.DueDate.Date != DateTime.MaxValue.Date && _currentTariff.State >= TariffState.NotPaid; }


    public SaasTariffsManager(
        ILogger<SaasTariffsManager> logger,
        IHttpContextAccessor httpContextAccessor,
        UserManager userManager,
        TenantManager tenantManager,
        PaymentManager paymentManager,
        WebItemSecurity webItemSecurity,
        SettingsManager settingsManager,
        TenantWhiteLabelSettingsHelper tenantWhiteLabelSettingsHelper,
        TenantStatisticsProvider tenantStatisticsProvider,
        GeolocationHelper geolocationHelper,
        IOptionsSnapshot<AccountLinker> accountLinker,
        Constants constants)
    {
        _log = logger;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _paymentManager = paymentManager;
        _webItemSecurity = webItemSecurity;
        _settingsManager = settingsManager;
        _tenantWhiteLabelSettingsHelper = tenantWhiteLabelSettingsHelper;
        _tenantStatisticsProvider = tenantStatisticsProvider;
        _geolocationHelper = geolocationHelper;
        _accountLinker = accountLinker;
        _constants = constants;

        _priceInfo = tenantManager.GetProductPriceInfo(false);
        _quotaList = tenantManager.GetTenantQuotas(false);

        _currentTenant = _tenantManager.GetCurrentTenant(_httpContextAccessor.HttpContext);
        _currentTariff = _paymentManager.GetTariff(_currentTenant.Id);
        _currentQuota = _tenantManager.GetTenantQuota(_currentTenant.Id);

        _currentRegion = GetCurrentRegionInfo();
    }


    public SaasTariffsPageData GetPageData()
    {
        var tariffPlanHeader = GetTariffPlanHeader();
        var tariffPlanDaysLeftNum = GetTariffPlanDaysLeftNum();
        var tariffPlanDaysLeftStr = GetTariffPlanDaysLeftStr(tariffPlanDaysLeftNum);
        var statistics = GetStatistics();
        var currencies = GetCurrencies();
        var tariffs = GetTariffs();

        var minPricePerUserPerMonth = tariffs.Where(x => !x.Trial && !x.Free).Min(x => x.PricePerUserPerMonth);
        var tariffPlanMinPriceHeader = GetTariffPlanMinPriceHeader(GetPriceString(minPricePerUserPerMonth));

        var targetTariff = tariffs.FirstOrDefault(x => x.Id == _currentQuota.Tenant) ?? tariffs.First(x => !x.Trial && !x.Free); //current tariff not visible
        var payment = GetPayment(targetTariff, _currentQuota, _currentQuota.ActiveUsers);

        return new SaasTariffsPageData()
        {
            TariffPlanHeader = tariffPlanHeader,
            TariffPlanDaysLeftNum = tariffPlanDaysLeftNum,
            TariffPlanDaysLeftStr = tariffPlanDaysLeftStr,
            TariffPlanChooseHeader = Resource.SaasTariffsPageTariffPlanChooseHeader,
            TariffPlanRenewHeader = Resource.SaasTariffsPageTariffPlanRenewHeader,
            TariffPlanReactivateHeader = Resource.SaasTariffsPageTariffPlanReactivateHeader,
            TariffPlanMinPriceHeader = tariffPlanMinPriceHeader,
            Statistics = statistics,
            Currencies = currencies,
            Tariffs = tariffs,
            Payment = payment
        };
    }

    public SaasTariffsPagePaymentData GetPaymentData(int tariffId, int usersCount)
    {
        var tariffs = GetTariffs();
        var targetTariff = tariffs.FirstOrDefault(x => x.Id == tariffId);
        var targetQuota = _quotaList.FirstOrDefault(q => q.Tenant == tariffId);

        var payment = GetPayment(targetTariff, targetQuota, usersCount);

        return payment;
    }

    public bool CheckStartupEnabled(out string errorMessage)
    {
        errorMessage = string.Empty;

        var startupQuota = GetStartupQuota();

        if (startupQuota == null)
        {
            errorMessage = Resource.SaasTariffsPageErrorStartupTariffNotFound;
            return false;
        }

        if (!_currentQuota.Trial)
        {
            errorMessage = Resource.SaasTariffsPageErrorStartupTariffNotAvailable;
            return false;
        }

        var currentUsersCount = _tenantStatisticsProvider.GetUsersCount();
        if (currentUsersCount > startupQuota.ActiveUsers)
        {
            errorMessage = string.Format(Resource.SaasTariffsPageErrorStartupUsersCountExceeded, startupQuota.ActiveUsers);
            return false;
        }

        var currentVisitorsCount = _tenantStatisticsProvider.GetVisitorsCount();
        if (currentVisitorsCount > 0)
        {
            errorMessage = string.Format(Resource.SaasTariffsPageErrorStartupGuestsCountExceeded, 0);
            return false;
        }

        var admins = _webItemSecurity.GetProductAdministrators(Guid.Empty);

        if (admins.Any(admin => admin.Id != _currentTenant.OwnerId))
        {
            errorMessage = string.Format(Resource.SaasTariffsPageErrorStartupAdminsCountExceeded, 1);
            return false;
        }

        var currentUsedSize = _tenantStatisticsProvider.GetUsedSize();
        if (currentUsedSize > startupQuota.MaxTotalSize)
        {
            errorMessage = string.Format(Resource.SaasTariffsPageErrorStartupStorageSizeExceeded, FileSizeComment.FilesSizeToString(startupQuota.MaxTotalSize));
            return false;
        }

        //var authServiceList = new AuthorizationKeys().AuthServiceList.Where(x => x.CanSet);

        //foreach (var service in authServiceList)
        //{
        //    if (service.Props.Any(r => !string.IsNullOrEmpty(r.Value)))
        //    {
        //        errorMessage = Resource.SaasTariffsPageErrorStartupDisable3rdParty;
        //        return false;
        //    }
        //}

        var tenantWhiteLabelSettings = _settingsManager.Load<TenantWhiteLabelSettings>();

        if (!_tenantWhiteLabelSettingsHelper.IsDefault(tenantWhiteLabelSettings))
        {
            errorMessage = Resource.SaasTariffsPageErrorStartupDisableWhitelabel;
            return false;
        }

        if (!string.IsNullOrEmpty(_currentTenant.MappedDomain))
        {
            errorMessage = Resource.SaasTariffsPageErrorStartupDisableDNS;
            return false;
        }

        var accountLinker = _accountLinker.Get("webstudio");

        foreach (var user in _userManager.GetUsers(EmployeeStatus.All))
        {
            var linkedAccounts = accountLinker.GetLinkedProfiles(user.Id.ToString());

            if (linkedAccounts.Any())
            {
                errorMessage = Resource.SaasTariffsPageErrorStartupDisableSocialNetworks;
                return false;
            }
        }

        var ssoSettingsV2 = _settingsManager.Load<SsoSettingsV2>();

        if (ssoSettingsV2.EnableSso)
        {
            errorMessage = Resource.SaasTariffsPageErrorStartupDisableSso;
            return false;
        }

        //if (ActiveDirectory.Base.Settings.LdapSettings.Load().EnableLdapAuthentication)
        //{
        //    errorMessage = Resource.SaasTariffsPageErrorStartupDisableLdap;
        //    return false;
        //}

        //using (var service = new BackupServiceClient())
        //{
        //    var scheduleResponse = service.GetSchedule(currentTenant.Id);

        //    if (scheduleResponse != null)
        //    {
        //        errorMessage = Resource.SaasTariffsPageErrorStartupDisableBackup;
        //        return false;
        //    }
        //}

        return true;
    }

    public TenantQuota ContinueStartup()
    {
        if (!CheckStartupEnabled(out var message))
        {
            throw new NotSupportedException(message);
        }

        var startupQuota = GetStartupQuota();

        _paymentManager.SetTariff(_currentTenant.Id, new Tariff
        {
            QuotaId = startupQuota.Tenant,
            DueDate = DateTime.MaxValue
        });

        return startupQuota;
    }



    private RegionInfo GetCurrentRegionInfo()
    {
        var defaultRegion = GetDefaultRegionInfo();

        var countryCode = _httpContextAccessor.HttpContext.Request.Query["country"];

        var currentRegion = GetRegionInfo(countryCode) ?? FindRegionInfo();

        if (currentRegion != null && !currentRegion.Name.Equals(defaultRegion.Name))
        {
            if (_priceInfo.Values.Any(value => value.ContainsKey(currentRegion.ISOCurrencySymbol)))
            {
                return currentRegion;
            }
        }

        return defaultRegion;
    }

    private static RegionInfo GetDefaultRegionInfo()
    {
        return GetRegionInfo("US");
    }

    private static RegionInfo GetRegionInfo(string isoTwoLetterCountryCode)
    {
        RegionInfo regionInfo = null;

        if (!string.IsNullOrEmpty(isoTwoLetterCountryCode))
        {
            try
            {
                regionInfo = new RegionInfo(isoTwoLetterCountryCode);
            }
            catch
            {
            }
        }

        return regionInfo;
    }

    private RegionInfo FindRegionInfo()
    {
        RegionInfo regionInfo = null;

        var geoinfo = _geolocationHelper.GetIPGeolocationFromHttpContext(_httpContextAccessor.HttpContext);

        if (geoinfo != null)
        {
            regionInfo = GetRegionInfo(geoinfo.Key);
        }

        if (regionInfo == null)
        {
            var owner = _userManager.GetUsers(_currentTenant.OwnerId);
            var culture = string.IsNullOrEmpty(owner.CultureName) ? _currentTenant.GetCulture() : owner.GetCulture();
            regionInfo = new RegionInfo(culture.LCID);
        }

        return regionInfo;
    }


    private string GetTariffPlanHeader()
    {
        if (CurrentTariffNonProfit)
        {
            return Resource.SaasTariffsPageNonProfitTariffPlanHeader;
        }

        if (CurrentTariffStartup)
        {
            return Resource.SaasTariffsPageStartupTariffPlanHeader;
        }

        if (CurrentTariffBusiness)
        {

            return CurrentTariffTrial
                ? CurrentTariffExpired ? Resource.SaasTariffsPageTrialExpiredTariffPlanHeader : Resource.SaasTariffsPageTrialTariffPlanHeader
                : CurrentTariffExpired ? Resource.SaasTariffsPageBusinessExpiredTariffPlanHeader : Resource.SaasTariffsPageBusinessTariffPlanHeader;
        }

        return string.Empty; // throw new Exception("Unknown tariff plan");
    }

    private int GetTariffPlanDaysLeftNum()
    {
        return CurrentTariffBusiness && CurrentTariffTrial && !CurrentTariffExpired
            ? _currentTariff.DueDate.Date.Subtract(DateTime.UtcNow.Date).Days
            : -1;
    }

    private static string GetTariffPlanDaysLeftStr(int count)
    {
        return count < 0 ? string.Empty : string.Format(Resource.SaasTariffsPageTariffPlanDaysLeft, count);
    }

    private static string GetTariffPlanMinPriceHeader(string minPriceStr)
    {
        return string.Format(Resource.SaasTariffsPageTariffPlanMinPriceHeader, minPriceStr);
    }

    private IEnumerable<string> GetStatistics()
    {
        var currentUsersCount = _tenantStatisticsProvider.GetUsersCount();

        var userStat = string.Format(Resource.SaasTariffsPageStatisticsAddedUsers, currentUsersCount, CurrentTariffNonProfit ? string.Empty : "/" + _currentQuota.ActiveUsers);

        var currentUsedSize = _tenantStatisticsProvider.GetUsedSize();

        var usedStat = string.Format(Resource.SaasTariffsPageStatisticsUsedStorageSpace, FileSizeComment.FilesSizeToString(currentUsedSize), FileSizeComment.FilesSizeToString(_currentQuota.MaxTotalSize));

        return new List<string>() { userStat, usedStat };
    }

    private IEnumerable<SaasTariffsPageCurrencyData> GetCurrencies()
    {
        var result = new List<SaasTariffsPageCurrencyData>();

        var defaultRegion = GetDefaultRegionInfo();

        result.Add(ToCurrencyData(defaultRegion));

        if (!_currentRegion.Name.Equals(defaultRegion.Name))
        {
            result.Add(ToCurrencyData(_currentRegion));
        }

        return result;
    }

    private static SaasTariffsPageCurrencyData ToCurrencyData(RegionInfo regionInfo)
    {
        return new SaasTariffsPageCurrencyData()
        {
            ISOCountryCode = regionInfo.Name,
            ISOCurrencySymbol = regionInfo.CurrencySymbol,
            CurrencyNativeName = regionInfo.CurrencyNativeName
        };
    }

    private IEnumerable<SaasTariffsPageTariffData> GetTariffs()
    {
        var result = _quotaList.Select(x => ToTariffData(x)).ToList();

        var maxPricePerUserPerMonth = result.Where(x => !x.Trial && !x.Free).Max(x => x.PricePerUserPerMonth);

        foreach (var tariff in result)
        {
            tariff.SavePercent = (int)(100 - 100 * tariff.PricePerUserPerMonth / maxPricePerUserPerMonth);
            tariff.SavePerUserPerMonth = maxPricePerUserPerMonth - tariff.PricePerUserPerMonth;
        }

        return result;
    }

    private SaasTariffsPageTariffData ToTariffData(TenantQuota tenantQuota)
    {
        var duration = tenantQuota.Year3 ? 36 : tenantQuota.Year ? 12 : 1;

        var price = GetPrice(tenantQuota);
        var pricePerMonth = price / duration;

        var features = GetFeatures(tenantQuota);

        return new SaasTariffsPageTariffData()
        {
            Id = tenantQuota.Tenant,
            Title = tenantQuota.Name,

            NonProfit = tenantQuota.NonProfit,
            Free = tenantQuota.Free,
            Trial = tenantQuota.Trial,

            DurationInMonths = duration,

            PricePerUser = price,
            PricePerUserStr = GetPriceString(price),

            PricePerUserPerMonth = pricePerMonth,
            PricePerUserPerMonthStr = GetPriceString(pricePerMonth),
            PricePerUserPerMonthHeader = Resource.SaasTariffsPagePricePerUserPerMonthHeader,

            Features = features
        };
    }

    private IEnumerable<SaasTariffsPageFeatureData> GetFeatures(TenantQuota tenantQuota)
    {
        var res = new List<SaasTariffsPageFeatureData>();

        var features = tenantQuota.Features.Split(' ', ',', ';');

        foreach(var feature in features)
        {
            var title = Resource.ResourceManager.GetString("SaasTariffsPageFeature_" + feature);

            if (string.IsNullOrEmpty(title))
            {
                continue;
            }

            if (title.Contains(':'))
            {
                title = string.Format(title, title.Split(':')[1]);
            }

            res.Add(new SaasTariffsPageFeatureData()
            {
                Id = feature,
                Title = title,
                Image = string.Format("{0}.svg", feature)
            });
        }

        return res;
    }

    private decimal GetPrice(TenantQuota quota)
    {
        if (!string.IsNullOrEmpty(quota.AvangateId) && _priceInfo.ContainsKey(quota.AvangateId))
        {
            var prices = _priceInfo[quota.AvangateId];
            if (prices.ContainsKey(_currentRegion.ISOCurrencySymbol))
            {
                return prices[_currentRegion.ISOCurrencySymbol];
            }
        }
        return quota.Price;
    }

    private string GetPriceString(decimal price)
    {
        var inEuro = "EUR".Equals(_currentRegion.ISOCurrencySymbol);

        var priceString = inEuro && Math.Truncate(price) != price
                                ? price.ToString(CultureInfo.InvariantCulture)
                                : ((int)price).ToString(CultureInfo.InvariantCulture);

        return string.Format("{0}{1}", _currentRegion.CurrencySymbol, priceString);
    }

    private SaasTariffsPagePaymentData GetPayment(SaasTariffsPageTariffData tariffModel, TenantQuota targetQuota, int usersCount)
    {
        var price = tariffModel.PricePerUser * usersCount;
        var save = tariffModel.SavePerUserPerMonth * tariffModel.DurationInMonths * usersCount;
        var shoppingData = GetShoppingData(targetQuota, usersCount);

        return new SaasTariffsPagePaymentData()
        {
            TariffId = tariffModel.Id,
            UsersCount = usersCount,

            Price = price,
            PriceStr = GetPriceString(price),

            Save = save,
            SaveStr = GetPriceString(save),

            ShoppingData = shoppingData
        };
    }

    protected SaasTariffsPageShoppingData GetShoppingData(TenantQuota targetQuota, int quantity)
    {
        var getLink = true;
        var buttonText = Resource.SaasTariffsPageBuyButtonText;
        var infoText = string.Empty;

        var currentUsersCount = _tenantStatisticsProvider.GetUsersCount();
        var currentVisitorsCount = _tenantStatisticsProvider.GetVisitorsCount();
        var currentUsedSize = _tenantStatisticsProvider.GetUsedSize();

        var quotaActiveUsers = targetQuota.ActiveUsers == -1 ? quantity : targetQuota.ActiveUsers;
        var quotaMaxTotalSize = targetQuota.ActiveUsers == -1 ? targetQuota.MaxTotalSize * quantity : targetQuota.MaxTotalSize;

        if (quotaActiveUsers < currentUsersCount
            || quotaMaxTotalSize < currentUsedSize
            || (!_currentQuota.Free && (quotaActiveUsers * _constants.CoefficientOfVisitors) < currentVisitorsCount))
        {
            getLink = false;
        }
        else if (Equals(targetQuota.Tenant, _currentQuota.Tenant) && quantity == _currentTariff.Quantity)
        {
            buttonText = Resource.SaasTariffsPageProlongButtonText;
            if (!_currentTariff.Prolongable)
            {
                getLink = false;
            }
            else if (_currentTariff.Autorenewal)
            {
                getLink = false;
                buttonText = Resource.SaasTariffsPageRecurringButtonText;
                infoText = Resource.SaasTariffsPageAutorenewInfoText;
            }
        }
        else if (_currentTariff.Prolongable)
        {
            buttonText = Resource.SaasTariffsPageBuyButtonText;
            infoText = Resource.SaasTariffsPageProlongableInfoText;
        }
        else if (_currentTariff.State == TariffState.Paid && quotaActiveUsers < _currentQuota.ActiveUsers)
        {
            getLink = false;
            buttonText = Resource.SaasTariffsPageBuyButtonText;
            infoText = _currentQuota.Year3
                ? Resource.SaasTariffsPageDisable3YearInfoText
                : Resource.SaasTariffsPageDisableYearInfoText;
        }

        var link = getLink ? GetShoppingLink(targetQuota, quantity) : string.Empty;

        return new SaasTariffsPageShoppingData()
        {
            ButtonText = buttonText,
            InfoText = infoText,
            ShoppingLink = link
        };
    }

    private string GetShoppingLink(TenantQuota quota, int quantity)
    {
        var link = string.Empty;

        var currency = _currentRegion.ISOCurrencySymbol;
        var language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        var customerId = _currentTenant.OwnerId.ToString();

        if (quota != null)
        {
            try
            {
                var uri = _paymentManager.GetShoppingUri(quota.Tenant, true, null, currency, language, customerId, quantity.ToString());
                if (uri == null)
                {
                    _log.ErrorGetNullShoppingLink(_currentTenant.Id, quota.Tenant, currency, language, customerId, quantity);
                }
                else
                {
                    link = uri.ToString();
                }
            }
            catch (Exception ex)
            {
                _log.ErrorGetShoppingLink(_currentTenant.Id, quota.Tenant, currency, language, customerId, quantity, ex.Message);
            }
        }
        return link;
    }

    private TenantQuota GetStartupQuota()
    {
        return _tenantManager.GetTenantQuotas(true).FirstOrDefault(q => q.Free && !q.Open);
    }


    public class SaasTariffsPageData
    {
        public string TariffPlanHeader { get; set; }

        public int TariffPlanDaysLeftNum { get; set; }
        public string TariffPlanDaysLeftStr { get; set; }

        public string TariffPlanChooseHeader { get; set; }
        public string TariffPlanRenewHeader { get; set; }
        public string TariffPlanReactivateHeader { get; set; }
        public string TariffPlanMinPriceHeader { get; set; }

        public IEnumerable<string> Statistics { get; set; }

        public IEnumerable<SaasTariffsPageCurrencyData> Currencies { get; set; }

        public IEnumerable<SaasTariffsPageTariffData> Tariffs { get; set; }

        public SaasTariffsPagePaymentData Payment { get; set; }
    }

    public class SaasTariffsPageCurrencyData
    {
        public string ISOCountryCode { get; set; }
        public string ISOCurrencySymbol { get; set; }
        public string CurrencyNativeName { get; set; }
    }

    public class SaasTariffsPageTariffData
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public bool NonProfit { get; set; }
        public bool Free { get; set; }
        public bool Trial { get; set; }


        public int DurationInMonths { get; set; }

        public decimal PricePerUser { get; set; }
        public string PricePerUserStr { get; set; }

        public decimal PricePerUserPerMonth { get; set; }
        public string PricePerUserPerMonthStr { get; set; }
        public string PricePerUserPerMonthHeader { get; set; }

        public int SavePercent { get; set; }
        public decimal SavePerUserPerMonth { get; set; }

        public IEnumerable<SaasTariffsPageFeatureData> Features { get; set; }
    }


    public class SaasTariffsPageFeatureData
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
    }

    public class SaasTariffsPagePaymentData
    {
        public int TariffId { get; set; }
        public int UsersCount { get; set; }

        public decimal Price { get; set; }
        public string PriceStr { get; set; }

        public decimal Save { get; set; }
        public string SaveStr { get; set; }

        public SaasTariffsPageShoppingData ShoppingData { get; set; }
    }

    public class SaasTariffsPageShoppingData
    {
        public string ButtonText { get; set; }
        public string InfoText { get; set; }
        public string ShoppingLink { get; set; }
    }

}
