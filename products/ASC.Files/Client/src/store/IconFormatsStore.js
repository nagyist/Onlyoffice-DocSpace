import { makeAutoObservable } from "mobx";
import { presentInArray } from "../helpers/files-helpers";
import combineUrl from "@appserver/common/utils/combineUrl";
import { homepage } from "../../package.json";

class IconFormatsStore {
  archive = [
    ".ace",
    ".arc",
    ".arj",
    ".bh",
    ".cab",
    ".enc",
    ".gz",
    ".gz",
    ".ha",
    ".jar",
    ".lha",
    ".lzh",
    ".pak",
    ".pk3",
    ".rar",
    ".tar",
    ".tgz",
    ".uu",
    ".uue",
    ".xxe",
    ".z",
    ".zip",
    ".zoo",
  ];
  image = [
    ".ai",
    ".bmp",
    ".cmx",
    ".cod",
    ".gif",
    ".ico",
    ".ief",
    ".jpe",
    ".jpeg",
    ".jpg",
    ".pbm",
    ".png",
    ".pnm",
    ".ppm",
    ".psd",
    ".rgb",
    ".tif",
    ".tiff",
    ".webp",
    ".xbm",
    ".xpm",
    ".xwd",
  ];
  sound = [
    ".aac",
    ".ac3",
    ".aiff",
    ".amr",
    ".ape",
    ".cda",
    ".flac",
    ".m4a",
    ".mid",
    ".mka",
    ".mp3",
    ".mpc",
    ".oga",
    ".ogg",
    ".pcm",
    ".ra",
    ".raw",
    ".wav",
    ".wma",
  ];
  video = [
    ".3gp",
    ".asf",
    ".avi",
    ".f4v",
    ".fla",
    ".flv",
    ".m2ts",
    ".m4v",
    ".mkv",
    ".mov",
    ".mp4",
    ".mpeg",
    ".mpg",
    ".mts",
    ".ogv",
    ".svi",
    ".vob",
    ".webm",
    ".wmv",
  ];
  html = [".htm", ".mht", ".html"];
  ebook = [".fb2", ".ibk", ".prc", ".epub"];
  document = [
    ".doc",
    ".docx",
    ".docm",
    ".dot",
    ".dotx",
    ".dotm",
    ".odt",
    ".fodt",
    ".ott",
    ".rtf",
    ".txt",
    ".html",
    ".htm",
    ".mht",
    ".pdf",
    ".djvu",
    ".fb2",
    ".epub",
    ".xps",
    ".doct",
    ".docy",
    ".gdoc",
  ];
  presentation = [
    ".pps",
    ".ppsx",
    ".ppsm",
    ".ppt",
    ".pptx",
    ".pptm",
    ".pot",
    ".potx",
    ".potm",
    ".odp",
    ".fodp",
    ".otp",
    ".pptt",
    ".ppty",
    ".gslides",
  ];
  spreadsheet = [
    ".xls",
    ".xlsx",
    ".xlsm",
    ".xlt",
    ".xltx",
    ".xltm",
    ".ods",
    ".fods",
    ".ots",
    ".csv",
    ".xlst",
    ".xlsy",
    ".xlsb",
    ".gsheet",
  ];

  constructor() {
    makeAutoObservable(this);
    //makeObservable(this, {});
  }

  isArchive = (extension) => presentInArray(this.archive, extension);

  isImage = (extension) => presentInArray(this.image, extension);

  isSound = (extension) => presentInArray(this.sound, extension);

  isHtml = (extension) => presentInArray(this.html, extension);

  isEbook = (extension) => presentInArray(this.ebook, extension);

  isDocument = (extension) => presentInArray(this.document, extension);

  isPresentation = (extension) => presentInArray(this.presentation, extension);

  isSpreadsheet = (extension) => presentInArray(this.spreadsheet, extension);

  getIcon = (
    size = 24,
    fileExst = null,
    providerKey = null,
    contentLength = null
  ) => {
    if (fileExst || contentLength) {
      const isArchiveItem = this.isArchive(fileExst);
      const isImageItem = this.isImage(fileExst);
      const isSoundItem = this.isSound(fileExst);
      const isHtmlItem = this.isHtml(fileExst);

      const icon = this.getFileIcon(
        fileExst,
        size,
        isArchiveItem,
        isImageItem,
        isSoundItem,
        isHtmlItem
      );
      return icon;
    } else {
      return this.getFolderIcon(providerKey, size);
    }
  };

  getFolderIcon = (providerKey, size = 32) => {
    const publicPath = window.AppServer?.cdnUrl || "";
    const folderPath = `images/icons/${size}`;
    let filePath = "folder.svg";

    switch (providerKey) {
      case "Box":
      case "BoxNet":
        filePath = "/folder/box.svg";
      case "DropBox":
      case "DropboxV2":
        filePath = "folder/dropbox.svg";
      case "Google":
      case "GoogleDrive":
        filePath = "folder/google.svg";
      case "OneDrive":
        filePath = "folder/onedrive.svg";
      case "SharePoint":
        filePath = "folder/sharepoint.svg";
      case "Yandex":
        filePath = "folder/yandex.svg";
      case "kDrive":
        filePath = "folder/kdrive.svg";
      case "WebDav":
        filePath = "folder/webdav.svg";
      default:
        filePath = "folder.svg";
    }

    return combineUrl(publicPath, homepage, folderPath, filePath);
  };

  icons = [
    ".avi",
    ".csv",
    ".djvu",
    ".doc",
    ".docx",
    ".dotx",
    ".dvd",
    ".epub",
    ".fb2",
    ".flv",
    ".fodt",
    ".iaf",
    ".ics",
    ".m2ts",
    ".mht",
    ".mkv",
    ".mov",
    ".mp4",
    ".mpg",
    ".odp",
    ".ods",
    ".odt",
    ".otp",
    ".ots",
    ".ott",
    ".pdf",
    ".pot",
    ".pps",
    ".ppsx",
    ".ppt",
    ".pptm",
    ".pptx",
    ".rtf",
    ".svg",
    ".txt",
    ".webm",
    ".xls",
    ".xlsm",
    ".xlsx",
    ".xps",
    ".xml",
  ];

  getFilePath = (ext) => {
    if (ext === ".pb2") return "fb2.svg";

    if (this.icons.indexOf(ext) !== -1) return `${ext.substring(1)}.svg`;

    return "file.svg";
  };

  getArchiveIcon = (publicPath, folderPath) => {
    return combineUrl(publicPath, folderPath, "file_archive.svg");
  };

  getImageIcon = (publicPath, folderPath) => {
    return combineUrl(publicPath, folderPath, "image.svg");
  };

  getSoundIcon = (publicPath, folderPath) => {
    return combineUrl(publicPath, folderPath, "sound.svg");
  };

  getHtmlIcon = (publicPath, folderPath) => {
    return combineUrl(publicPath, folderPath, "html.svg");
  };

  getFileIcon = (
    extension,
    size = 32,
    archive = false,
    image = false,
    sound = false,
    html = false
  ) => {
    const publicPath = window.AppServer?.cdnUrl || "";
    const folderPath = `/static/images/icons/${size}`;

    if (archive) return this.getArchiveIcon(publicPath, folderPath);

    if (image) return this.getImageIcon(publicPath, folderPath);

    if (sound) return this.getSoundIcon(publicPath, folderPath);

    if (html) return this.getHtmlIcon(publicPath, folderPath);

    const filePath = this.getFilePath(extension);

    return combineUrl(publicPath, folderPath, filePath);
  };

  getIconSrc = (ext, size = 24) => {
    const publicPath = window.AppServer?.cdnUrl || "";
    const folderPath = `/static/images/icons/${size}`;

    if (presentInArray(this.archive, ext, true))
      return this.getArchiveIcon(publicPath, folderPath);

    if (presentInArray(this.image, ext, true))
      return this.getImageIcon(publicPath, folderPath);

    if (presentInArray(this.sound, ext, true))
      return this.getSoundIcon(publicPath, folderPath);

    if (presentInArray(this.html, ext, true))
      return this.getHtmlIcon(publicPath, folderPath);

    const extension = ext.toLowerCase();

    return this.getFileIcon(extension, size);
  };
}

export default new IconFormatsStore();
