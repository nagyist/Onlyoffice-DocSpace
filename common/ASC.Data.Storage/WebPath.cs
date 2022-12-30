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

namespace ASC.Data.Storage;

[Singletone]
public class WebPathSettings
{
    private readonly IEnumerable<Appender> _appenders;

    public WebPathSettings(Configuration.Storage storage)
    {
        var section = storage;
        if (section != null)
        {
            _appenders = section.Appender;
        }
    }

    public string GetPath(HttpContext httpContext, ILoggerProvider options, string relativePath)
    {
        if (!string.IsNullOrEmpty(relativePath) && relativePath.IndexOf('~') == 0)
        {
            throw new ArgumentException($"bad path format {relativePath} remove '~'", nameof(relativePath));
        }

        var result = relativePath;
        var ext = Path.GetExtension(relativePath).ToLowerInvariant();

        if (_appenders.Any())
        {
            var avaliableAppenders = _appenders.Where(x => x.Extensions != null && x.Extensions.Split('|').Contains(ext) || string.IsNullOrEmpty(ext)).ToList();
            var avaliableAppendersCount = avaliableAppenders.Count;

            Appender appender;
            if (avaliableAppendersCount > 1)
            {
                appender = avaliableAppenders[relativePath.Length % avaliableAppendersCount];
            }
            else if (avaliableAppendersCount == 1)
            {
                appender = avaliableAppenders.First();
            }
            else
            {
                appender = _appenders.First();
            }

            if (appender.Append.IndexOf('~') == 0)
            {
                var query = string.Empty;
                //Rel path
                if (relativePath.IndexOfAny(new[] { '?', '=', '&' }) != -1)
                {
                    //Cut it
                    query = relativePath.Substring(relativePath.IndexOf('?'));
                    relativePath = relativePath.Substring(0, relativePath.IndexOf('?'));
                }
                //if (HostingEnvironment.IsHosted)
                //{
                //    result = VirtualPathUtility.ToAbsolute(string.Format("{0}/{1}{2}", appender.Append.TrimEnd('/'), relativePath.TrimStart('/'), query));
                //}
                //else
                //{
                result = $"{appender.Append.TrimEnd('/').TrimStart('~')}/{relativePath.TrimStart('/')}{query}";
                //}
            }
            else
            {
                //TODO HostingEnvironment.IsHosted
                if (SecureHelper.IsSecure(httpContext, options) && !string.IsNullOrEmpty(appender.AppendSecure))
                {
                    result = $"{appender.AppendSecure.TrimEnd('/')}/{relativePath.TrimStart('/')}";
                }
                else
                {
                    //Append directly
                    result = $"{appender.Append.TrimEnd('/')}/{relativePath.TrimStart('/')}";
                }
            }
        }

        return result;
    }
}

[Scope]
public class WebPath
{
    public IServiceProvider ServiceProvider { get; }
    public IHostEnvironment HostEnvironment { get; }
    
    private readonly WebPathSettings _webPathSettings;
    private readonly SettingsManager _settingsManager;
    private readonly StorageSettingsHelper _storageSettingsHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly ILoggerProvider _options;

    public WebPath(
        WebPathSettings webPathSettings,
        IServiceProvider serviceProvider,
        SettingsManager settingsManager,
        StorageSettingsHelper storageSettingsHelper,
        IHostEnvironment hostEnvironment,
        CoreBaseSettings coreBaseSettings,
        ILoggerProvider options)
    {
        _webPathSettings = webPathSettings;
        ServiceProvider = serviceProvider;
        _settingsManager = settingsManager;
        _storageSettingsHelper = storageSettingsHelper;
        HostEnvironment = hostEnvironment;
        _coreBaseSettings = coreBaseSettings;
        _options = options;
    }

    public WebPath(
        WebPathSettings webPathSettings,
        IServiceProvider serviceProvider,
        SettingsManager settingsManager,
        StorageSettingsHelper storageSettingsHelper,
        IHttpContextAccessor httpContextAccessor,
        IHostEnvironment hostEnvironment,
        CoreBaseSettings coreBaseSettings,
            ILoggerProvider options)
            : this(webPathSettings, serviceProvider, settingsManager, storageSettingsHelper, hostEnvironment, coreBaseSettings, options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<string> GetPathAsync(string relativePath)
    {
        if (!string.IsNullOrEmpty(relativePath) && relativePath.IndexOf('~') == 0)
        {
            throw new ArgumentException($"bad path format {relativePath} remove '~'", nameof(relativePath));
        }

        return InternalGetPathAsync(relativePath);
    }

    private async Task<string> InternalGetPathAsync(string relativePath)
    {
        if (_coreBaseSettings.Standalone && ServiceProvider.GetService<StaticUploader>().CanUpload()) //hack for skip resolve DistributedTaskQueueOptionsManager
        {
            try
            {
                var uri = await _storageSettingsHelper.DataStore(_settingsManager.Load<CdnStorageSettings>()).GetInternalUriAsync("", relativePath, TimeSpan.Zero, null);
                var result = uri.AbsoluteUri.ToLower();
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
            }
            catch (Exception)
            {

            }
        }

        return _webPathSettings.GetPath(_httpContextAccessor?.HttpContext, _options, relativePath);
    }
}
