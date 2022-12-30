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

using Document = DocuSign.eSign.Model.Document;

namespace ASC.Web.Files.Helpers;

[Scope]
public class DocuSignToken
{
    public const string AppAttr = "docusign";

    private readonly ILogger<DocuSignHelper> _logger;
    private readonly TokenHelper _tokenHelper;
    private readonly AuthContext _authContext;
    private readonly ConsumerFactory _consumerFactory;

    public DocuSignToken(
        TokenHelper tokenHelper,
        ILogger<DocuSignHelper> logger,
        AuthContext authContext,
        ConsumerFactory consumerFactory)
    {
        _tokenHelper = tokenHelper;
        _authContext = authContext;
        _consumerFactory = consumerFactory;
        _logger = logger;
    }

    public OAuth20Token GetToken()
    {
        return _tokenHelper.GetToken(AppAttr);
    }

    public void DeleteToken(Guid? userId = null)
    {
        _tokenHelper.DeleteToken(AppAttr, userId);
    }

    public void SaveToken(OAuth20Token token)
    {
        ArgumentNullException.ThrowIfNull(token);

        _tokenHelper.SaveToken(new Token(token, AppAttr));
    }

    internal string GetRefreshedToken(OAuth20Token token)
    {
        if (token.IsExpired)
        {
            try
            {
                _logger.InformationDocuSignRefreshToken(_authContext.CurrentAccount.ID);

                var refreshed = _consumerFactory.Get<DocuSignLoginProvider>().RefreshToken(token.RefreshToken);

                if (refreshed != null)
                {
                    token.AccessToken = refreshed.AccessToken;
                    token.RefreshToken = refreshed.RefreshToken;
                    token.ExpiresIn = refreshed.ExpiresIn;
                    token.Timestamp = DateTime.UtcNow;

                    SaveToken(token);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorDocuSignRefreshToken(_authContext.CurrentAccount.ID, ex);
            }
        }

        return token.AccessToken;
    }
}

[Scope]
public class DocuSignHelper
{
    private readonly ILogger<DocuSignHelper> _logger;

    public const string UserField = "userId";
    
    private readonly DocuSignToken _docuSignToken;
    private readonly FileSecurity _fileSecurity;
    private readonly IDaoFactory _daoFactory;
    private readonly BaseCommonLinkUtility _baseCommonLinkUtility;
    private readonly UserManager _userManager;
    private readonly AuthContext _authContext;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly FileMarker _fileMarker;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly FilesMessageService _filesMessageService;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly IServiceProvider _serviceProvider;
    private readonly ConsumerFactory _consumerFactory;
    private readonly RequestHelper _requestHelper;

    public DocuSignHelper(
        DocuSignToken docuSignToken,
        FileSecurity fileSecurity,
        IDaoFactory daoFactory,
        ILogger<DocuSignHelper> logger,
        BaseCommonLinkUtility baseCommonLinkUtility,
        UserManager userManager,
        AuthContext authContext,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        FileMarker fileMarker,
        GlobalFolderHelper globalFolderHelper,
        FilesMessageService filesMessageService,
        FilesLinkUtility filesLinkUtility,
        IServiceProvider serviceProvider,
        ConsumerFactory consumerFactory,
        RequestHelper requestHelper)
    {
        _docuSignToken = docuSignToken;
        _fileSecurity = fileSecurity;
        _daoFactory = daoFactory;
        _baseCommonLinkUtility = baseCommonLinkUtility;
        _userManager = userManager;
        _authContext = authContext;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _fileMarker = fileMarker;
        _globalFolderHelper = globalFolderHelper;
        _filesMessageService = filesMessageService;
        _filesLinkUtility = filesLinkUtility;
        _serviceProvider = serviceProvider;
        _consumerFactory = consumerFactory;
        _logger = logger;
        _requestHelper = requestHelper;
    }

    private DocuSignAccount GetDocuSignAccount(OAuth20Token token)
    {
        ArgumentNullException.ThrowIfNull(token);

        var userInfoString = _requestHelper.PerformRequest(_consumerFactory.Get<DocuSignLoginProvider>().DocuSignHost + "/oauth/userinfo",
                                                          headers: new Dictionary<string, string> { { "Authorization", "Bearer " + _docuSignToken.GetRefreshedToken(token) } });

        _logger.DebugDocuSingUserInfo(userInfoString);

        var userInfo = (DocuSignUserInfo)JsonConvert.DeserializeObject(userInfoString, typeof(DocuSignUserInfo));

        if (userInfo.Accounts == null || userInfo.Accounts.Count == 0)
        {
            throw new Exception("Account is null");
        }

        var account = userInfo.Accounts[0];

        return account;
    }

    private ApiClient GetApiClient(DocuSignAccount account, OAuth20Token token)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(token);

        var apiClient = new ApiClient(account.BaseUri + "/restapi");

        apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + _docuSignToken.GetRefreshedToken(token));

        return apiClient;
    }
    
    public async Task<File<T>> SaveDocumentAsync<T>(string envelopeId, string documentId, string documentName, T folderId)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(envelopeId);
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(documentId);

        var token = _docuSignToken.GetToken();
        var account = GetDocuSignAccount(token);
        var apiClient = GetApiClient(account, token);

        var fileDao = _daoFactory.GetFileDao<T>();
        var folderDao = _daoFactory.GetFolderDao<T>();
        if (string.IsNullOrEmpty(documentName))
        {
            documentName = "new.pdf";
        }

        Folder<T> folder;
        if (folderId == null
            || (folder = await folderDao.GetFolderAsync(folderId)) == null
            || folder.RootFolderType == FolderType.TRASH
            || !await _fileSecurity.CanCreateAsync(folder))
        {
            if (_globalFolderHelper.FolderMy != 0)
            {
                folderId = _globalFolderHelper.GetFolderMy<T>();
            }
            else
            {
                throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_Create);
            }
        }

        var file = _serviceProvider.GetService<File<T>>();
        file.ParentId = folderId;
        file.Comment = FilesCommonResource.CommentCreateByDocuSign;
        file.Title = FileUtility.ReplaceFileExtension(documentName, ".pdf");

        var envelopesApi = new EnvelopesApi(apiClient);
        _logger.InformationDocuSignWebhookGetStream(documentId);
        using (var stream = await envelopesApi.GetDocumentAsync(account.AccountId, envelopeId, documentId))
        {
            file.ContentLength = stream.Length;
            file = await fileDao.SaveFileAsync(file, stream);
        }

        _filesMessageService.Send(file, MessageInitiator.ThirdPartyProvider, MessageAction.DocumentSignComplete, "DocuSign", file.Title);

        await _fileMarker.MarkAsNewAsync(file);

        return file;
    }


    [DebuggerDisplay("{AccountId} {BaseUri}")]
    private class DocuSignAccount
    {
        [JsonPropertyName("account_id")]
        public string AccountId { get; set; }

        [JsonPropertyName("base_uri")]
        public string BaseUri { get; set; }
    }

    private class DocuSignUserInfo
    {
        public List<DocuSignAccount> Accounts { get; set; }
    }
}

[EnumExtensions]
public enum DocuSignStatus
{
    Draft,
    Sent,
    Delivered,
    Completed,
    Declined,
    Voided,
}
