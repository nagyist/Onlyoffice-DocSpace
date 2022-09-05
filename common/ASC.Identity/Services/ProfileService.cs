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

using ASC.Web.Core.Users;

namespace ASC.Identity.Services;

[Transient]
public class ProfileService : IProfileService
{
    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly UserPhotoManager _userPhotoManager;

    public ProfileService(UserManager userManager,
                          TenantManager tenantManager,
                          DisplayUserSettingsHelper displayUserSettingsHelper,
                          UserPhotoManager userPhotoManager)
    {
        _userManager = userManager;
        _tenantManager = tenantManager;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _userPhotoManager = userPhotoManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

        var subjectId = subject.Claims.Where(x => x.Type == "sub").FirstOrDefault()?.Value;
        var tenantId = subject.Claims.Where(x => x.Type == "tenantId").FirstOrDefault()?.Value;

        if (tenantId == null || subjectId == null) return;

        _tenantManager.SetCurrentTenant(tenantId);

        var user = _userManager.GetUsers(new Guid(subjectId));

        var issuedClaims = await GetClaimsFromUser(user);

        context.IssuedClaims = issuedClaims.ToList();
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        context.IsActive = false;

        var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

        var subjectId = subject.Claims.Where(x => x.Type == "sub").FirstOrDefault()?.Value;
        var tenantId = subject.Claims.Where(x => x.Type == "tenantId").FirstOrDefault()?.Value;

        if (tenantId == null) return Task.CompletedTask;
        if (subjectId == null) return Task.CompletedTask;

        _tenantManager.SetCurrentTenant(tenantId);

        var user = _userManager.GetUsers(new Guid(subjectId));

        if (user != null && user.Id != Constants.LostUser.Id)
        {
            context.IsActive = user.TerminatedDate == null;
        }

        return Task.CompletedTask;
    }

    private async Task<IEnumerable<Claim>> GetClaimsFromUser(UserInfo userInfo)
    {
        var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, userInfo.Id.ToString()),
                new Claim(JwtClaimTypes.PreferredUserName, userInfo.UserName),
                new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.UserName),
                new Claim("tenantId", Convert.ToString(userInfo.Tenant))
            };

        if (String.IsNullOrEmpty(userInfo.FirstName))
        {
            claims.Add(new Claim(JwtClaimTypes.GivenName, userInfo.FirstName));
        }

        if (String.IsNullOrEmpty(userInfo.LastName))
        {
            claims.Add(new Claim(JwtClaimTypes.FamilyName, userInfo.LastName));
        }

        claims.Add(new Claim(JwtClaimTypes.Name, userInfo.DisplayUserName(_displayUserSettingsHelper)));

        if (!string.IsNullOrEmpty(userInfo.Email))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, userInfo.Email));
        }

        if (userInfo.BirthDate != null)
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Birthdate, userInfo.BirthDate.ToString()));
        }

        var smallPhotoUrl = await userInfo.GetSmallPhotoURL(_userPhotoManager);

        if (!string.IsNullOrEmpty(smallPhotoUrl))
        {
            claims.Add(new Claim(JwtClaimTypes.Picture, smallPhotoUrl));
        }

        if (!string.IsNullOrEmpty(userInfo.MobilePhone))
        {
            claims.AddRange(new[]
            {
                new Claim(JwtClaimTypes.PhoneNumber, userInfo.MobilePhone),
                new Claim(JwtClaimTypes.PhoneNumberVerified, userInfo.MobilePhoneActivationStatus == MobilePhoneActivationStatus.Activated ? "true" : "false", ClaimValueTypes.Boolean)
            });
        }

        return claims;
    }
}
