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


using ASC.Web.Studio.Core.Tariffs;

using static ASC.Web.Studio.Core.Tariffs.SaasTariffsManager;

namespace ASC.Web.Api.Controllers;

[Scope]
[DefaultRoute]
[ApiController]
public class TariffsController : ControllerBase
{
    private readonly PermissionContext _permissionContext;
    private readonly SaasTariffsManager _saasTariffManager;
    private readonly IMapper _mapper;

    public TariffsController(PermissionContext permissionContext, SaasTariffsManager saasTariffManager, IMapper mapper)
    {
        _permissionContext = permissionContext;
        _saasTariffManager = saasTariffManager;
        _mapper = mapper;
    }


    [HttpGet("saas/page")]
    public SaasTariffsPageDto GetPageData()
    {
        var pageData = _saasTariffManager.GetPageData();

        var pageDto = _mapper.Map<SaasTariffsPageData, SaasTariffsPageDto>(pageData);

        return pageDto;
    }

    [HttpGet("saas/payment/{tariffId}/{usersCount}")]
    public SaasTariffsPagePaymentDto GetPaymentData(int tariffId, int usersCount)
    {
        var paymentData = _saasTariffManager.GetPaymentData(tariffId, usersCount);

        var paymentDto = _mapper.Map<SaasTariffsPagePaymentData, SaasTariffsPagePaymentDto>(paymentData);

        return paymentDto;
    }

    [HttpGet("saas/checkstartup")]
    public bool CheckStartup()
    {
        return _saasTariffManager.CheckStartupEnabled(out _);
    }

    [HttpPut("saas/continuestartup")]
    public TenantQuota ContinueStartup()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        return _saasTariffManager.ContinueStartup();
    }
}

