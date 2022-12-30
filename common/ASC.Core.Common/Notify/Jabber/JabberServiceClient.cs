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

namespace ASC.Core.Notify.Jabber;

[Scope]
public class JabberServiceClient
{
    private static readonly TimeSpan _timeout = TimeSpan.FromMinutes(2);
    private static DateTime _lastErrorTime;
    private readonly UserManager _userManager;
    private readonly AuthContext _authContext;
    private readonly TenantManager _tenantManager;

    public JabberServiceClient(UserManager userManager, AuthContext authContext, TenantManager tenantManager)
    {
        _userManager = userManager;
        _authContext = authContext;
        _tenantManager = tenantManager;
    }

    private static bool IsServiceProbablyNotAvailable()
    {
        return _lastErrorTime != default && _lastErrorTime + _timeout > DateTime.Now;
    }

    public bool SendMessage(int tenantId, string from, string to, string text, string subject)
    {
        if (IsServiceProbablyNotAvailable())
        {
            return false;
        }

        using (var service = GetService())
        {
            try
            {
                service.SendMessage(tenantId, from, to, text, subject);
                return true;
            }
            catch (Exception error)
            {
                ProcessError(error);
            }
        }

        return false;
    }

    public string GetVersion()
    {
        using (var service = GetService())
        {
            try
            {
                return service.GetVersion();
            }
            catch (Exception error)
            {
                ProcessError(error);
            }
        }

        return null;
    }
    
    public byte GetState(string userName)
    {
        const byte defaultState = 0;

        try
        {
            if (IsServiceProbablyNotAvailable())
            {
                return defaultState;
            }

            using var service = GetService();

            return service.GetState(GetCurrentTenantId(), userName);
        }
        catch (Exception error)
        {
            ProcessError(error);
        }

        return defaultState;
    }
    
    public Dictionary<string, byte> GetAllStates()
    {
        Dictionary<string, byte> states = null;
        try
        {
            if (IsServiceProbablyNotAvailable())
            {
                throw new Exception();
            }

            using var service = GetService();
            states = service.GetAllStates(GetCurrentTenantId(), GetCurrentUserName());
        }
        catch (Exception error)
        {
            ProcessError(error);
        }

        return states;
    }
    
    public void Ping(byte state)
    {
        try
        {
            if (IsServiceProbablyNotAvailable())
            {
                throw new Exception();
            }

            using var service = GetService();
            service.Ping(_authContext.CurrentAccount.ID.ToString(), GetCurrentTenantId(), GetCurrentUserName(), state);
        }
        catch (Exception error)
        {
            ProcessError(error);
        }
    }

    private int GetCurrentTenantId()
    {
        return _tenantManager.GetCurrentTenant().Id;
    }

    private string GetCurrentUserName()
    {
        return _userManager.GetUsers(_authContext.CurrentAccount.ID).UserName;
    }

    private static void ProcessError(Exception error)
    {
        if (error is FaultException)
        {
            throw error;
        }
        if (error is CommunicationException || error is TimeoutException)
        {
            _lastErrorTime = DateTime.Now;
        }

        throw error;
    }

    private JabberServiceClientWcf GetService()
    {
        return new JabberServiceClientWcf();
    }
}
