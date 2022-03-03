﻿namespace ASC.Files.Api;

public class SecutiryController : ApiControllerBase
{
    private readonly FileStorageService<int> _fileStorageServiceInt;
    private readonly FileStorageService<string> _fileStorageServiceString;

    public SecutiryController(
        FilesControllerHelper<int> filesControllerHelperInt,
        FilesControllerHelper<string> filesControllerHelperString,
        FileStorageService<int> fileStorageServiceInt,
        FileStorageService<string> fileStorageServiceString) 
        : base(filesControllerHelperInt, filesControllerHelperString)
    {
        _fileStorageServiceString = fileStorageServiceString;
        _fileStorageServiceInt = fileStorageServiceInt;
    }

    [Create("owner")]
    public IAsyncEnumerable<FileEntryWrapper> ChangeOwnerFromBodyAsync([FromBody] ChangeOwnerModel model)
    {
        return ChangeOwnerAsync(model);
    }

    [Create("owner")]
    [Consumes("application/x-www-form-urlencoded")]
    public IAsyncEnumerable<FileEntryWrapper> ChangeOwnerFromFormAsync([FromForm] ChangeOwnerModel model)
    {
        return ChangeOwnerAsync(model);
    }

    /// <summary>
    ///   Returns the external link to the shared file with the ID specified in the request
    /// </summary>
    /// <summary>
    ///   File external link
    /// </summary>
    /// <param name="fileId">File ID</param>
    /// <param name="share">Access right</param>
    /// <category>Files</category>
    /// <returns>Shared file link</returns>
    [Update("{fileId}/sharedlinkAsync")]
    public async Task<object> GenerateSharedLinkFromBodyAsync(string fileId, [FromBody] GenerateSharedLinkModel model)
    {
        return await _filesControllerHelperString.GenerateSharedLinkAsync(fileId, model.Share);
    }

    [Update("{fileId:int}/sharedlinkAsync")]
    public async Task<object> GenerateSharedLinkFromBodyAsync(int fileId, [FromBody] GenerateSharedLinkModel model)
    {
        return await _filesControllerHelperInt.GenerateSharedLinkAsync(fileId, model.Share);
    }

    [Update("{fileId}/sharedlinkAsync")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<object> GenerateSharedLinkFromFormAsync(string fileId, [FromForm] GenerateSharedLinkModel model)
    {
        return await _filesControllerHelperString.GenerateSharedLinkAsync(fileId, model.Share);
    }

    [Update("{fileId:int}/sharedlinkAsync")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<object> GenerateSharedLinkFromFormAsync(int fileId, [FromForm] GenerateSharedLinkModel model)
    {
        return await _filesControllerHelperInt.GenerateSharedLinkAsync(fileId, model.Share);
    }

    /// <summary>
    /// Returns the detailed information about shared file with the ID specified in the request
    /// </summary>
    /// <short>File sharing</short>
    /// <category>Sharing</category>
    /// <param name="fileId">File ID</param>
    /// <returns>Shared file information</returns>
    [Read("file/{fileId}/share")]
    public Task<IEnumerable<FileShareWrapper>> GetFileSecurityInfoAsync(string fileId)
    {
        return _filesControllerHelperString.GetFileSecurityInfoAsync(fileId);
    }

    [Read("file/{fileId:int}/share")]
    public Task<IEnumerable<FileShareWrapper>> GetFileSecurityInfoAsync(int fileId)
    {
        return _filesControllerHelperInt.GetFileSecurityInfoAsync(fileId);
    }

    /// <summary>
    /// Returns the detailed information about shared folder with the ID specified in the request
    /// </summary>
    /// <short>Folder sharing</short>
    /// <param name="folderId">Folder ID</param>
    /// <category>Sharing</category>
    /// <returns>Shared folder information</returns>
    [Read("folder/{folderId}/share")]
    public Task<IEnumerable<FileShareWrapper>> GetFolderSecurityInfoAsync(string folderId)
    {
        return _filesControllerHelperString.GetFolderSecurityInfoAsync(folderId);
    }

    [Read("folder/{folderId:int}/share")]
    public Task<IEnumerable<FileShareWrapper>> GetFolderSecurityInfoAsync(int folderId)
    {
        return _filesControllerHelperInt.GetFolderSecurityInfoAsync(folderId);
    }

    [Create("share")]
    public async Task<IEnumerable<FileShareWrapper>> GetSecurityInfoFromBodyAsync([FromBody] BaseBatchModel model)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(model.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(model.FileIds);

        var result = new List<FileShareWrapper>();
        result.AddRange(await _filesControllerHelperInt.GetSecurityInfoAsync(fileIntIds, folderIntIds));
        result.AddRange(await _filesControllerHelperString.GetSecurityInfoAsync(fileStringIds, folderStringIds));

        return result;
    }

    [Create("share")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IEnumerable<FileShareWrapper>> GetSecurityInfoFromFormAsync([FromForm][ModelBinder(BinderType = typeof(BaseBatchModelBinder))] BaseBatchModel model)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(model.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(model.FileIds);

        var result = new List<FileShareWrapper>();
        result.AddRange(await _filesControllerHelperInt.GetSecurityInfoAsync(fileIntIds, folderIntIds));
        result.AddRange(await _filesControllerHelperString.GetSecurityInfoAsync(fileStringIds, folderStringIds));

        return result;
    }

    /// <summary>
    ///   Removes sharing rights for the group with the ID specified in the request
    /// </summary>
    /// <param name="folderIds">Folders ID</param>
    /// <param name="fileIds">Files ID</param>
    /// <short>Remove group sharing rights</short>
    /// <category>Sharing</category>
    /// <returns>Shared file information</returns>
    [Delete("share")]
    public async Task<bool> RemoveSecurityInfoAsync(BaseBatchModel model)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(model.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(model.FileIds);

        await _filesControllerHelperInt.RemoveSecurityInfoAsync(fileIntIds, folderIntIds);
        await _filesControllerHelperString.RemoveSecurityInfoAsync(fileStringIds, folderStringIds);

        return true;
    }

    [Update("{fileId:int}/setacelink")]
    public Task<bool> SetAceLinkAsync(int fileId, [FromBody] GenerateSharedLinkModel model)
    {
        return _filesControllerHelperInt.SetAceLinkAsync(fileId, model.Share);
    }

    [Update("{fileId}/setacelink")]
    public Task<bool> SetAceLinkAsync(string fileId, [FromBody] GenerateSharedLinkModel model)
    {
        return _filesControllerHelperString.SetAceLinkAsync(fileId, model.Share);
    }

    /// <summary>
    /// Sets sharing settings for the file with the ID specified in the request
    /// </summary>
    /// <param name="fileId">File ID</param>
    /// <param name="share">Collection of sharing rights</param>
    /// <param name="notify">Should notify people</param>
    /// <param name="sharingMessage">Sharing message to send when notifying</param>
    /// <short>Share file</short>
    /// <category>Sharing</category>
    /// <remarks>
    /// Each of the FileShareParams must contain two parameters: 'ShareTo' - ID of the user with whom we want to share and 'Access' - access type which we want to grant to the user (Read, ReadWrite, etc) 
    /// </remarks>
    /// <returns>Shared file information</returns>
    [Update("file/{fileId}/share")]
    public Task<IEnumerable<FileShareWrapper>> SetFileSecurityInfoFromBodyAsync(string fileId, [FromBody] SecurityInfoModel model)
    {
        return _filesControllerHelperString.SetFileSecurityInfoAsync(fileId, model.Share, model.Notify, model.SharingMessage);
    }

    [Update("file/{fileId:int}/share")]
    public Task<IEnumerable<FileShareWrapper>> SetFileSecurityInfoFromBodyAsync(int fileId, [FromBody] SecurityInfoModel model)
    {
        return _filesControllerHelperInt.SetFileSecurityInfoAsync(fileId, model.Share, model.Notify, model.SharingMessage);
    }

    [Update("file/{fileId}/share")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IEnumerable<FileShareWrapper>> SetFileSecurityInfoFromFormAsync(string fileId, [FromForm] SecurityInfoModel model)
    {
        return _filesControllerHelperString.SetFileSecurityInfoAsync(fileId, model.Share, model.Notify, model.SharingMessage);
    }

    [Update("file/{fileId:int}/share")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IEnumerable<FileShareWrapper>> SetFileSecurityInfoFromFormAsync(int fileId, [FromForm] SecurityInfoModel model)
    {
        return _filesControllerHelperInt.SetFileSecurityInfoAsync(fileId, model.Share, model.Notify, model.SharingMessage);
    }

    /// <summary>
    /// Sets sharing settings for the folder with the ID specified in the request
    /// </summary>
    /// <short>Share folder</short>
    /// <param name="folderId">Folder ID</param>
    /// <param name="share">Collection of sharing rights</param>
    /// <param name="notify">Should notify people</param>
    /// <param name="sharingMessage">Sharing message to send when notifying</param>
    /// <remarks>
    /// Each of the FileShareParams must contain two parameters: 'ShareTo' - ID of the user with whom we want to share and 'Access' - access type which we want to grant to the user (Read, ReadWrite, etc) 
    /// </remarks>
    /// <category>Sharing</category>
    /// <returns>Shared folder information</returns>
    [Update("folder/{folderId}/share")]
    public Task<IEnumerable<FileShareWrapper>> SetFolderSecurityInfoFromBodyAsync(string folderId, [FromBody] SecurityInfoModel model)
    {
        return _filesControllerHelperString.SetFolderSecurityInfoAsync(folderId, model.Share, model.Notify, model.SharingMessage);
    }

    [Update("folder/{folderId:int}/share")]
    public Task<IEnumerable<FileShareWrapper>> SetFolderSecurityInfoFromBodyAsync(int folderId, [FromBody] SecurityInfoModel model)
    {
        return _filesControllerHelperInt.SetFolderSecurityInfoAsync(folderId, model.Share, model.Notify, model.SharingMessage);
    }

    [Update("folder/{folderId}/share")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IEnumerable<FileShareWrapper>> SetFolderSecurityInfoFromFormAsync(string folderId, [FromForm] SecurityInfoModel model)
    {
        return _filesControllerHelperString.SetFolderSecurityInfoAsync(folderId, model.Share, model.Notify, model.SharingMessage);
    }

    [Update("folder/{folderId:int}/share")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IEnumerable<FileShareWrapper>> SetFolderSecurityInfoFromFormAsync(int folderId, [FromForm] SecurityInfoModel model)
    {
        return _filesControllerHelperInt.SetFolderSecurityInfoAsync(folderId, model.Share, model.Notify, model.SharingMessage);
    }

    public async Task<IEnumerable<FileShareWrapper>> SetSecurityInfoAsync(SecurityInfoModel model)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(model.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(model.FileIds);

        var result = new List<FileShareWrapper>();
        result.AddRange(await _filesControllerHelperInt.SetSecurityInfoAsync(fileIntIds, folderIntIds, model.Share, model.Notify, model.SharingMessage));
        result.AddRange(await _filesControllerHelperString.SetSecurityInfoAsync(fileStringIds, folderStringIds, model.Share, model.Notify, model.SharingMessage));

        return result;
    }

    [Update("share")]
    public Task<IEnumerable<FileShareWrapper>> SetSecurityInfoFromBodyAsync([FromBody] SecurityInfoModel model)
    {
        return SetSecurityInfoAsync(model);
    }

    [Update("share")]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<IEnumerable<FileShareWrapper>> SetSecurityInfoFromFormAsync([FromForm] SecurityInfoModel model)
    {
        return SetSecurityInfoAsync(model);
    }

    private async IAsyncEnumerable<FileEntryWrapper> ChangeOwnerAsync(ChangeOwnerModel model)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(model.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(model.FileIds);

        var result = AsyncEnumerable.Empty<FileEntry>();
        result.Concat(_fileStorageServiceInt.ChangeOwnerAsync(folderIntIds, fileIntIds, model.UserId));
        result.Concat(_fileStorageServiceString.ChangeOwnerAsync(folderStringIds, fileStringIds, model.UserId));

        await foreach (var e in result)
        {
            yield return await _filesControllerHelperInt.GetFileEntryWrapperAsync(e);
        }
    }
}