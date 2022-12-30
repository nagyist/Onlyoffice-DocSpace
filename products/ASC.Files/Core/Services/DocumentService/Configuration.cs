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

namespace ASC.Web.Files.Services.DocumentService;

[EnumExtensions]
public enum EditorType
{
    Desktop,
    Mobile,
    Embedded,
    External,
}

public class ActionLinkConfig
{
    public ActionConfig Action { get; set; }

    public static string Serialize(ActionLinkConfig actionLinkConfig)
    {
        return JsonSerializer.Serialize(actionLinkConfig);
    }

    public class ActionConfig
    {
        public string Data { get; set; }
        public string Type { get; set; }
    }
}

public class Configuration<T>
{
    internal static readonly Dictionary<FileType, string> DocType = new Dictionary<FileType, string>
    {
        { FileType.Document, "word" },
        { FileType.Spreadsheet, "cell" },
        { FileType.Presentation, "slide" }
    };

    private FileType _fileTypeCache = FileType.Unknown;

    public DocumentConfig<T> Document { get; set; }

    public string DocumentType
    {
        get
        {
            DocType.TryGetValue(GetFileType, out var documentType);

            return documentType;
        }
    }

    public EditorConfiguration<T> EditorConfig { get; set; }

    public EditorType EditorType
    {
        set => Document.Info.Type = value;
        get => Document.Info.Type;
    }

    [JsonPropertyName("Error")]
    public string ErrorMessage { get; set; }

    public string Token { get; set; }

    public string Type
    {
        set => EditorType = (EditorType)Enum.Parse(typeof(EditorType), value, true);
        get => EditorType.ToString().ToLower();
    }

    internal FileType GetFileType
    {
        get
        {
            if (_fileTypeCache == FileType.Unknown)
            {
                _fileTypeCache = FileUtility.GetFileTypeByFileName(Document.Info.GetFile().Title);
            }

            return _fileTypeCache;
        }
    }

    public Configuration(
        File<T> file,
        IServiceProvider serviceProvider)
    {
        Document = serviceProvider.GetService<DocumentConfig<T>>();
        Document.Info.SetFile(file);
        EditorConfig = serviceProvider.GetService<EditorConfiguration<T>>();
        EditorConfig.SetConfiguration(this);
    }

    public static string Serialize(Configuration<T> configuration)
    {
        return JsonSerializer.Serialize(configuration);
    }
}

#region Nested Classes

[Transient]
public class DocumentConfig<T>
{
    private readonly DocumentServiceConnector _documentServiceConnector;
    private readonly PathProvider _pathProvider;
    private string _fileUri;
    private string _key = string.Empty;
    private string _title;
    public string FileType => Info.GetFile().ConvertedExtension.Trim('.');
    public InfoConfig<T> Info { get; set; }
    public bool IsLinkedForMe { get; set; }

    public string Key
    {
        set => _key = value;
        get => DocumentServiceConnector.GenerateRevisionId(_key);
    }

    public PermissionsConfig Permissions { get; set; }
    public string SharedLinkKey { get; set; }

    public string Title
    {
        set => _title = value;
        get => _title ?? Info.GetFile().Title;
    }

    public string Url
    {
        set => _fileUri = _documentServiceConnector.ReplaceCommunityAdress(value);
        get
        {
            if (!string.IsNullOrEmpty(_fileUri))
            {
                return _fileUri;
            }

            var last = Permissions.Edit || Permissions.Review || Permissions.Comment;
            _fileUri = _documentServiceConnector.ReplaceCommunityAdress(_pathProvider.GetFileStreamUrl(Info.GetFile(), SharedLinkKey, last));

            return _fileUri;
        }
    }

    public DocumentConfig(DocumentServiceConnector documentServiceConnector, PathProvider pathProvider, InfoConfig<T> infoConfig)
    {
        Info = infoConfig;
        Permissions = new PermissionsConfig();
        _documentServiceConnector = documentServiceConnector;
        _pathProvider = pathProvider;
    }
}

[Transient]
public class EditorConfiguration<T>
{
    private readonly AuthContext _authContext;
    private readonly BaseCommonLinkUtility _baseCommonLinkUtility;
    private readonly IDaoFactory _daoFactory;
    private readonly DocumentServiceTrackerHelper _documentServiceTrackerHelper;
    private readonly EntryManager _entryManager;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly FilesSettingsHelper _filesSettingsHelper;
    private readonly FileUtility _fileUtility;
    private readonly UserInfo _userInfo;
    private readonly UserManager _userManager;
    private Configuration<T> _configuration;

    public ActionLinkConfig ActionLink { get; set; }

    public string CreateUrl
    {
        get
        {
            if (_configuration.Document.Info.Type != EditorType.Desktop)
            {
                return null;
            }

            if (!_authContext.IsAuthenticated || _userManager.IsUser(_authContext.CurrentAccount.ID))
            {
                return null;
            }

            return GetCreateUrl(_configuration.GetFileType);
        }
    }

    public CustomizationConfig<T> Customization { get; set; }

    public EncryptionKeysConfig EncryptionKeys { get; set; }
    
    public string Lang => _userInfo.GetCulture().Name;

    public string Mode => ModeWrite ? "edit" : "view";

    public bool ModeWrite { get; set; }

    public PluginsConfig Plugins { get; set; }

    public List<RecentConfig> Recent
    {
        get
        {
            if (!_authContext.IsAuthenticated || _userManager.IsUser(_authContext.CurrentAccount.ID))
            {
                return null;
            }

            if (!_filesSettingsHelper.RecentSection)
            {
                return null;
            }

            var filter = FilterType.FilesOnly;
            switch (_configuration.GetFileType)
            {
                case FileType.Document:
                    filter = FilterType.DocumentsOnly;
                    break;

                case FileType.OForm:
                    filter = FilterType.OFormOnly;
                    break;

                case FileType.OFormTemplate:
                    filter = FilterType.OFormTemplateOnly;
                    break;

                case FileType.Spreadsheet:
                    filter = FilterType.SpreadsheetsOnly;
                    break;

                case FileType.Presentation:
                    filter = FilterType.PresentationsOnly;
                    break;
            }

            var folderDao = _daoFactory.GetFolderDao<int>();
            var files = _entryManager.GetRecentAsync(filter, false, Guid.Empty, string.Empty, false).Result.Cast<File<int>>();

            var listRecent = from file in files
                             where !Equals(_configuration.Document.Info.GetFile().Id, file.Id)
                             select
                                 new RecentConfig
                                 {
                                     Folder = folderDao.GetFolderAsync(file.ParentId).Result.Title,
                                     Title = file.Title,
                                     Url = _baseCommonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.GetFileWebEditorUrl(file.Id))
                                 };

            return listRecent.ToList();
        }
    }
    
    public List<TemplatesConfig> Templates
    {
        set { }
        get
        {
            if (!_authContext.IsAuthenticated || _userManager.IsUser(_authContext.CurrentAccount.ID))
            {
                return null;
            }

            if (!_filesSettingsHelper.TemplatesSection)
            {
                return null;
            }

            var extension = _fileUtility.GetInternalExtension(_configuration.Document.Title).TrimStart('.');
            var filter = FilterType.FilesOnly;
            switch (_configuration.GetFileType)
            {
                case FileType.Document:
                    filter = FilterType.DocumentsOnly;
                    break;

                case FileType.OForm:
                    filter = FilterType.OFormOnly;
                    break;

                case FileType.OFormTemplate:
                    filter = FilterType.OFormTemplateOnly;
                    break;

                case FileType.Spreadsheet:
                    filter = FilterType.SpreadsheetsOnly;
                    break;

                case FileType.Presentation:
                    filter = FilterType.PresentationsOnly;
                    break;
            }

            var folderDao = _daoFactory.GetFolderDao<int>();
            var fileDao = _daoFactory.GetFileDao<int>();
            var files = _entryManager.GetTemplatesAsync(folderDao, fileDao, filter, false, Guid.Empty, string.Empty, false).ToListAsync().Result;
            var listTemplates = from file in files
                                select
                                    new TemplatesConfig
                                    {
                                        Image = _baseCommonLinkUtility.GetFullAbsolutePath("skins/default/images/filetype/thumb/" + extension + ".png"),
                                        Title = file.Title,
                                        Url = _baseCommonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.GetFileWebEditorUrl(file.Id))
                                    };
            return listTemplates.ToList();
        }
    }

    public UserConfig User { get; set; }

    public EditorConfiguration(
        UserManager userManager,
        AuthContext authContext,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        FilesLinkUtility filesLinkUtility,
        FileUtility fileUtility,
        BaseCommonLinkUtility baseCommonLinkUtility,
        PluginsConfig pluginsConfig,
        CustomizationConfig<T> customizationConfig,
        FilesSettingsHelper filesSettingsHelper,
        IDaoFactory daoFactory,
        EntryManager entryManager,
        DocumentServiceTrackerHelper documentServiceTrackerHelper)
    {
        _userManager = userManager;
        _authContext = authContext;
        _filesLinkUtility = filesLinkUtility;
        _fileUtility = fileUtility;
        _baseCommonLinkUtility = baseCommonLinkUtility;
        Customization = customizationConfig;
        _filesSettingsHelper = filesSettingsHelper;
        _daoFactory = daoFactory;
        _entryManager = entryManager;
        _documentServiceTrackerHelper = documentServiceTrackerHelper;
        Plugins = pluginsConfig;
        _userInfo = userManager.GetUsers(authContext.CurrentAccount.ID);

        if (!_userInfo.Id.Equals(ASC.Core.Configuration.Constants.Guest.ID))
        {
            User = new UserConfig
            {
                Id = _userInfo.Id.ToString(),
                Name = _userInfo.DisplayUserName(false, displayUserSettingsHelper),
            };
        }
    }

    internal void SetConfiguration(Configuration<T> configuration)
    {
        _configuration = configuration;
        Customization.SetConfiguration(_configuration);
    }

    private string GetCreateUrl(FileType fileType)
    {
        string title;
        switch (fileType)
        {
            case FileType.Document:
            case FileType.OForm:
            case FileType.OFormTemplate:
                title = FilesJSResource.TitleNewFileText;
                break;

            case FileType.Spreadsheet:
                title = FilesJSResource.TitleNewFileSpreadsheet;
                break;

            case FileType.Presentation:
                title = FilesJSResource.TitleNewFilePresentation;
                break;

            default:
                return null;
        }

        Configuration<T>.DocType.TryGetValue(fileType, out var documentType);

        return _baseCommonLinkUtility.GetFullAbsolutePath(_filesLinkUtility.FileHandlerPath)
               + "?" + FilesLinkUtility.Action + "=create"
               + "&doctype=" + documentType
               + "&" + FilesLinkUtility.FileTitle + "=" + HttpUtility.UrlEncode(title);
    }
}

[Transient]
public class InfoConfig<T>
{
    private readonly BreadCrumbsManager _breadCrumbsManager;
    private readonly FileSharing _fileSharing;
    private readonly SecurityContext _securityContext;
    private readonly UserManager _userManager;
    private string _breadCrumbs;
    private bool? _favorite;
    private bool _favoriteIsSet;
    private File<T> _file;

    public bool? Favorite
    {
        get
        {
            if (_favoriteIsSet)
            {
                return _favorite;
            }

            if (!_securityContext.IsAuthenticated || _userManager.IsUser(_securityContext.CurrentAccount.ID))
            {
                return null;
            }

            if (_file.ParentId == null || _file.Encrypted)
            {
                return null;
            }

            return _file.IsFavorite;
        }
        set
        {
            _favoriteIsSet = true;
            _favorite = value;
        }
    }

    public string Folder
    {
        get
        {
            if (Type == EditorType.Embedded || Type == EditorType.External)
            {
                return null;
            }

            if (string.IsNullOrEmpty(_breadCrumbs))
            {
                const string crumbsSeporator = " \\ ";

                var breadCrumbsList = _breadCrumbsManager.GetBreadCrumbsAsync(_file.ParentId).Result;
                _breadCrumbs = string.Join(crumbsSeporator, breadCrumbsList.Select(folder => folder.Title).ToArray());
            }

            return _breadCrumbs;
        }
    }

    public string Owner => _file.CreateByString;

    public List<AceShortWrapper> SharingSettings
    {
        get
        {
            if (Type == EditorType.Embedded
                || Type == EditorType.External
                || !_fileSharing.CanSetAccessAsync(_file).Result)
            {
                return null;
            }

            try
            {
                return _fileSharing.GetSharedInfoShortFileAsync(_file.Id).Result;
            }
            catch
            {
                return null;
            }
        }
    }

    public EditorType Type { get; set; } = EditorType.Desktop;

    public string Uploaded => _file.CreateOnString;

    public InfoConfig(BreadCrumbsManager breadCrumbsManager, FileSharing fileSharing, SecurityContext securityContext, UserManager userManager)
    {
        _breadCrumbsManager = breadCrumbsManager;
        _fileSharing = fileSharing;
        _securityContext = securityContext;
        _userManager = userManager;
    }

    public File<T> GetFile()
    {
        return _file;
    }

    public void SetFile(File<T> file)
    {
        _file = file;
    }
}

public class PermissionsConfig
{
    public bool ChangeHistory { get; set; }
    public bool Comment { get; set; } = true;
    public bool Download { get; set; } = true;
    public bool Edit { get; set; } = true;
    public bool FillForms { get; set; } = true;
    public bool ModifyFilter { get; set; } = true;
    public bool Print { get; set; } = true;
    public bool Rename { get; set; }
    public bool Review { get; set; } = true;
}

#endregion Nested Classes

[Transient]
public class CustomerConfig<T>
{
    private readonly BaseCommonLinkUtility _baseCommonLinkUtility;
    private readonly SettingsManager _settingsManager;
    private readonly TenantWhiteLabelSettingsHelper _tenantWhiteLabelSettingsHelper;
    private Configuration<T> _configuration;

    public string Address => _settingsManager.LoadForDefaultTenant<CompanyWhiteLabelSettings>().Address;

    public string Logo => _baseCommonLinkUtility.GetFullAbsolutePath(_tenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.LoginPage, false).Result);

    public string Mail => _settingsManager.LoadForDefaultTenant<CompanyWhiteLabelSettings>().Email;

    public string Name => _settingsManager.LoadForDefaultTenant<CompanyWhiteLabelSettings>().CompanyName;

    public string Www => _settingsManager.LoadForDefaultTenant<CompanyWhiteLabelSettings>().Site;

    public CustomerConfig(
        SettingsManager settingsManager,
        BaseCommonLinkUtility baseCommonLinkUtility,
        TenantWhiteLabelSettingsHelper tenantWhiteLabelSettingsHelper)
    {
        _settingsManager = settingsManager;
        _baseCommonLinkUtility = baseCommonLinkUtility;
        _tenantWhiteLabelSettingsHelper = tenantWhiteLabelSettingsHelper;
    }

    internal void SetConfiguration(Configuration<T> configuration)
    {
        _configuration = configuration;
    }
}

[Transient]
public class CustomizationConfig<T>
{
    [JsonIgnore]
    public string GobackUrl;

    private readonly AuthContext _authContext;

    private readonly CommonLinkUtility _commonLinkUtility;

    private readonly CoreBaseSettings _coreBaseSettings;

    private readonly IDaoFactory _daoFactory;

    private readonly FileSecurity _fileSecurity;

    private readonly FilesSettingsHelper _filesSettingsHelper;

    private readonly FileUtility _fileUtility;

    private readonly GlobalFolderHelper _globalFolderHelper;

    private readonly PathProvider _pathProvider;

    private readonly SettingsManager _settingsManager;

    private readonly ThirdPartySelector _thirdPartySelector;

    private Configuration<T> _configuration;

    public bool About => !_coreBaseSettings.Standalone && !_coreBaseSettings.CustomMode;

    public CustomerConfig<T> Customer { get; set; }
    
    public bool? Forcesave
    {
        get
        {
            return _fileUtility.CanForcesave
                   && !_configuration.Document.Info.GetFile().ProviderEntry
                   && _thirdPartySelector.GetAppByFileId(_configuration.Document.Info.GetFile().Id.ToString()) == null
                   && _filesSettingsHelper.Forcesave;
        }
    }
    
    public LogoConfig<T> Logo { get; set; }

    public bool MentionShare
    {
        get
        {
            return _authContext.IsAuthenticated
                   && !_configuration.Document.Info.GetFile().Encrypted
                   && FileSharing.CanSetAccessAsync(_configuration.Document.Info.GetFile()).Result;
        }
    }
    
    private FileSharing FileSharing { get; }

    public CustomizationConfig(
        CoreBaseSettings coreBaseSettings,
        SettingsManager settingsManager,
        FileUtility fileUtility,
        FilesSettingsHelper filesSettingsHelper,
        AuthContext authContext,
        FileSecurity fileSecurity,
        IDaoFactory daoFactory,
        GlobalFolderHelper globalFolderHelper,
        PathProvider pathProvider,
        CustomerConfig<T> customerConfig,
        LogoConfig<T> logoConfig,
        FileSharing fileSharing,
        CommonLinkUtility commonLinkUtility,
        ThirdPartySelector thirdPartySelector)
    {
        _coreBaseSettings = coreBaseSettings;
        _settingsManager = settingsManager;
        _fileUtility = fileUtility;
        _filesSettingsHelper = filesSettingsHelper;
        _authContext = authContext;
        _fileSecurity = fileSecurity;
        _daoFactory = daoFactory;
        _globalFolderHelper = globalFolderHelper;
        _pathProvider = pathProvider;
        Customer = customerConfig;
        Logo = logoConfig;
        FileSharing = fileSharing;
        _thirdPartySelector = thirdPartySelector;
        _commonLinkUtility = commonLinkUtility;
    }

    internal void SetConfiguration(Configuration<T> configuration)
    {
        _configuration = configuration;

        if (_coreBaseSettings.Standalone)
        {
            Customer.SetConfiguration(_configuration);
        }
        else
        {
            Customer = null;
        }

        Logo.SetConfiguration(_configuration);
    }
}

public class EncryptionKeysConfig
{
    public string PrivateKeyEnc { get; set; }
    public string PublicKey { get; set; }
}

[Transient]
public class LogoConfig<T>
{
    private readonly CommonLinkUtility _commonLinkUtility;

    private readonly FileUtility _fileUtility;

    private readonly TenantLogoHelper _tenantLogoHelper;

    private Configuration<T> _configuration;

    public string Image
    {
        get
        {
            var fillingForm = _fileUtility.CanWebRestrictedEditing(_configuration.Document.Title);

            return _configuration.EditorType == EditorType.Embedded
                || fillingForm
                    ? _commonLinkUtility.GetFullAbsolutePath(_tenantLogoHelper.GetLogo(WhiteLabelLogoTypeEnum.DocsEditorEmbed).Result)
                    : _commonLinkUtility.GetFullAbsolutePath(_tenantLogoHelper.GetLogo(WhiteLabelLogoTypeEnum.DocsEditor).Result);
        }
    }
    
    public string Url
    {
        set { }
        get => _commonLinkUtility.GetFullAbsolutePath(_commonLinkUtility.GetDefault());
    }

    public LogoConfig(
        CommonLinkUtility commonLinkUtility,
        TenantLogoHelper tenantLogoHelper,
        FileUtility fileUtility)
    {
        _commonLinkUtility = commonLinkUtility;
        _tenantLogoHelper = tenantLogoHelper;
        _fileUtility = fileUtility;
    }

    internal void SetConfiguration(Configuration<T> configuration)
    {
        _configuration = configuration;
    }
}

[Transient]
public class PluginsConfig
{
    private readonly BaseCommonLinkUtility _baseCommonLinkUtility;

    private readonly ConsumerFactory _consumerFactory;

    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly TenantManager _tenantManager;
    
    public PluginsConfig(
        ConsumerFactory consumerFactory,
        BaseCommonLinkUtility baseCommonLinkUtility,
        CoreBaseSettings coreBaseSettings,
        TenantManager tenantManager)
    {
        _consumerFactory = consumerFactory;
        _baseCommonLinkUtility = baseCommonLinkUtility;
        _coreBaseSettings = coreBaseSettings;
        _tenantManager = tenantManager;
    }
}

public class RecentConfig
{
    public string Folder { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
}

public class TemplatesConfig
{
    public string Image { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
}

public class UserConfig
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public static class ConfigurationExtention
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<DocumentConfig<string>>();
        services.TryAdd<DocumentConfig<int>>();

        services.TryAdd<InfoConfig<string>>();
        services.TryAdd<InfoConfig<int>>();

        services.TryAdd<EditorConfiguration<string>>();
        services.TryAdd<EditorConfiguration<int>>();

        services.TryAdd<PluginsConfig>();

        services.TryAdd<CustomizationConfig<string>>();
        services.TryAdd<CustomizationConfig<int>>();

        services.TryAdd<CustomerConfig<string>>();
        services.TryAdd<CustomerConfig<int>>();

        services.TryAdd<LogoConfig<string>>();
        services.TryAdd<LogoConfig<int>>();
    }
}