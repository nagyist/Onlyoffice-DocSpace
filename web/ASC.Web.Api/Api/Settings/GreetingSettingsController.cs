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

namespace ASC.Web.Api.Controllers.Settings;

public class GreetingSettingsController : BaseSettingsController
{
    private Tenant Tenant { get { return _apiContext.Tenant; } }

    private readonly MessageService _messageService;
    private readonly TenantManager _tenantManager;
    private readonly PermissionContext _permissionContext;
    private readonly TenantInfoSettingsHelper _tenantInfoSettingsHelper;

    public GreetingSettingsController(
        TenantInfoSettingsHelper tenantInfoSettingsHelper,
        MessageService messageService,
        ApiContext apiContext,
        TenantManager tenantManager,
        PermissionContext permissionContext,
        WebItemManager webItemManager,
        IMemoryCache memoryCache) : base(apiContext, memoryCache, webItemManager)
    {
        _tenantInfoSettingsHelper = tenantInfoSettingsHelper;
        _messageService = messageService;
        _tenantManager = tenantManager;
        _permissionContext = permissionContext;
    }

    [Read("greetingsettings")]
    public ContentResult GetGreetingSettings()
    {
        return new ContentResult { Content = Tenant.Name };
    }

    [Create("greetingsettings")]
    public ContentResult SaveGreetingSettingsFromBody([FromBody] GreetingSettingsRequestsDto inDto)
    {
        return SaveGreetingSettings(inDto);
    }

    [Create("greetingsettings")]
    [Consumes("application/x-www-form-urlencoded")]
    public ContentResult SaveGreetingSettingsFromForm([FromForm] GreetingSettingsRequestsDto inDto)
    {
        return SaveGreetingSettings(inDto);
    }

    private ContentResult SaveGreetingSettings(GreetingSettingsRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        Tenant.Name = inDto.Title;
        _tenantManager.SaveTenant(Tenant);

        _messageService.Send(MessageAction.GreetingSettingsUpdated);

        return new ContentResult { Content = Resource.SuccessfullySaveGreetingSettingsMessage };
    }

    [Create("greetingsettings/restore")]
    public ContentResult RestoreGreetingSettings()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        _tenantInfoSettingsHelper.RestoreDefaultTenantName();

        return new ContentResult
        {
            Content = Tenant.Name
        };
    }
}