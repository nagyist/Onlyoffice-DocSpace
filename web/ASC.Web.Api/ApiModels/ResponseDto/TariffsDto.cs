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

using static ASC.Web.Studio.Core.Tariffs.SaasTariffsManager;

namespace ASC.Web.Api.ApiModel.ResponseDto;


public class SaasTariffsPageDto : IMapFrom<SaasTariffsPageData>
{
    public string TariffPlanHeader { get; set; }

    public int TariffPlanDaysLeftNum { get; set; }
    public string TariffPlanDaysLeftStr { get; set; }

    public string TariffPlanChooseHeader { get; set; }
    public string TariffPlanRenewHeader { get; set; }
    public string TariffPlanReactivateHeader { get; set; }
    public string TariffPlanMinPriceHeader { get; set; }

    public IEnumerable<string> Statistics { get; set; }

    public IEnumerable<SaasTariffsPageCurrencyDto> Currencies { get; set; }

    public IEnumerable<SaasTariffsPageTariffDto> Tariffs { get; set; }

    public SaasTariffsPagePaymentDto Payment { get; set; }
}

public class SaasTariffsPageCurrencyDto : IMapFrom<SaasTariffsPageCurrencyData>
{
    public string ISOCountryCode { get; set; }
    public string ISOCurrencySymbol { get; set; }
    public string CurrencyNativeName { get; set; }
}

public class SaasTariffsPageTariffDto : IMapFrom<SaasTariffsPageTariffData>
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

    public IEnumerable<SaasTariffsPageFeatureDto> Features { get; set; }
}


public class SaasTariffsPageFeatureDto : IMapFrom<SaasTariffsPageFeatureData>
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }
}

public class SaasTariffsPagePaymentDto : IMapFrom<SaasTariffsPagePaymentData>
{
    public int TariffId { get; set; }
    public int UsersCount { get; set; }

    public decimal Price { get; set; }
    public string PriceStr { get; set; }

    public decimal Save { get; set; }
    public string SaveStr { get; set; }

    public SaasTariffsPageShoppingDto ShoppingData { get; set; }
}

public class SaasTariffsPageShoppingDto : IMapFrom<SaasTariffsPageShoppingData>
{
    public string ButtonText { get; set; }
    public string InfoText { get; set; }
    public string ShoppingLink { get; set; }
}
