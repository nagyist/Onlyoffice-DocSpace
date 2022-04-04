import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { getFolder } from "@appserver/common/api/files";
import PropTypes from "prop-types";
import toastr from "studio/toastr";
import { FilterType, FolderType } from "@appserver/common/constants";
import SelectionPanel from "../SelectionPanel/SelectionPanelBody";
class SelectFileDialog extends React.PureComponent {
  constructor(props) {
    super(props);
    const { filter, t, id } = props;
    this.rootTitle = t("SelectFile");
    this.newFilter = filter.clone();

    this.state = {
      resultingFolderTree: [],
      isDataLoading: false,
      isInitialLoader: false,
      isNextPageLoading: false,
      page: 0,
      hasNextPage: true,

      folderId: id ? id : "root",
      title: this.rootTitle,
      files: [],
      selectedFileInfo: [],
    };
  }

  getFilterParameters = () => {
    const {
      isImageOnly,
      isDocumentsOnly,
      isArchiveOnly,
      isPresentationOnly,
      isTablesOnly,
      isMediaOnly,
      searchParam = "",
      ByExtension,
    } = this.props;

    if (isImageOnly) {
      return { filterType: FilterType.ImagesOnly, filterValue: searchParam };
    }
    if (isDocumentsOnly) {
      return { filterType: FilterType.DocumentsOnly, filterValue: searchParam };
    }
    if (isArchiveOnly) {
      return { filterType: FilterType.ArchiveOnly, filterValue: searchParam };
    }
    if (isPresentationOnly) {
      return {
        filterType: FilterType.PresentationsOnly,
        filterValue: searchParam,
      };
    }
    if (isTablesOnly) {
      return {
        filterType: FilterType.SpreadsheetsOnly,
        filterValue: searchParam,
      };
    }
    if (isMediaOnly) {
      return { filterType: FilterType.MediaOnly, filterValue: searchParam };
    }

    if (ByExtension) {
      return { filterType: FilterType.ByExtension, filterValue: searchParam };
    }

    return { filterType: FilterType.FilesOnly, filterValue: "" };
  };

  setFilter = () => {
    const { withSubfolders = true } = this.props;

    const filterParams = this.getFilterParameters();

    this.newFilter.filterType = filterParams.filterType;
    this.newFilter.search = filterParams.filterValue;
    this.newFilter.withSubfolders = withSubfolders;
  };

  async componentDidMount() {
    const { treeFolders, foldersType, id, foldersList } = this.props;

    this.setFilter();

    let timerId = setTimeout(() => {
      this.setState({ isInitialLoader: true });
    }, 1000);

    let resultingFolderTree, resultingId;

    try {
      [
        resultingFolderTree,
        resultingId,
      ] = await SelectionPanel.getBasicFolderInfo(
        treeFolders,
        foldersType,
        id,
        null,
        null,
        foldersList
      );
    } catch (e) {
      toastr.error(e);

      clearTimeout(timerId);
      timerId = null;
      this.setState({ isInitialLoader: false });

      return;
    }

    clearTimeout(timerId);
    timerId = null;

    this.setState({
      resultingFolderTree: resultingFolderTree,
      isInitialLoader: false,
    });
  }

  deletedCurrentFolderIdFromPathParts = (pathParts) => {
    pathParts.splice(-1, 1);
  };

  onButtonClick = (e) => {
    const { selectedFileInfo, folderId } = this.state;
    const { onClose, onSelectFile, onSetFileNameAndLocation } = this.props;
    onSetFileNameAndLocation &&
      onSetFileNameAndLocation(selectedFileInfo.title, folderId);
    onSelectFile && onSelectFile(selectedFileInfo);

    onClose && onClose();
  };

  onArrowClickAction = async () => {
    const { pathParts } = this.state;

    const newPathParts = [...pathParts];
    const prevFolderId = newPathParts.pop();
    const isRootFolder = newPathParts.length === 0;

    if (!isRootFolder) {
      try {
        this.setState({
          folderId: prevFolderId,
          files: [],
          hasNextPage: true,
          page: 0,
          isDataLoading: true,
        });
      } catch (e) {
        toastr.error(e);
      }
    } else {
      this.setState({
        isDataLoading: false,
        files: [],
        title: this.rootTitle,
        folderId: "root",
        pathParts: [],
      });
    }
  };

  onFolderClick = (id) => {
    this.setState({
      folderId: id,
      files: [],
      selectedFileInfo: [],
      page: 0,
      hasNextPage: true,
      isDataLoading: true,
    });
  };

  onSelectFile = (item) => {
    this.setState({
      selectedFileInfo: item,
    });
  };
  _loadNextPage = () => {
    const { folderId, files, page } = this.state;
    const { withoutProvider, foldersType } = this.props;
    let dataWithoutProvider;
    if (this._isLoadNextPage) return;

    this._isLoadNextPage = true;

    const pageCount = 30;
    this.newFilter.page = page;
    this.newFilter.pageCount = pageCount;

    this.setState({ isNextPageLoading: true }, async () => {
      try {
        const data = await getFolder(folderId, this.newFilter);
        if (
          data.current.rootFolderType === FolderType.COMMON &&
          withoutProvider
        ) {
          const filterFolder = data.folders.filter((value) => {
            if (!value.providerKey) return value;
          });
          dataWithoutProvider = [...filterFolder, ...data.files];
        }

        const finalData = withoutProvider
          ? dataWithoutProvider
          : [...data.folders, ...data.files];

        const newFilesList = [...files].concat(finalData);

        const hasNextPage = newFilesList.length < data.total - 1;
        let firstLoadInfo = {};
        if (page === 0) {
          const pathParts = [...data.pathParts];

          this.deletedCurrentFolderIdFromPathParts(pathParts);

          const additionalPage = foldersType === "third-party" ? [] : ["root"];

          firstLoadInfo = {
            pathParts: [...additionalPage, ...pathParts],
            title: data.current.title,
          };
        }

        this._isLoadNextPage = false;
        this.setState((state) => ({
          isDataLoading: false,
          hasNextPage: hasNextPage,
          isNextPageLoading: false,
          page: state.page + 1,
          files: newFilesList,
          ...firstLoadInfo,
        }));
      } catch (e) {
        toastr.error(e);
        this.setState({
          isDataLoading: false,
        });
      }
    });
  };

  render() {
    const {
      isPanelVisible,
      onClose,
      t,
      footer: footerChild,
      header: headerChild,
    } = this.props;
    const {
      resultingFolderTree,
      isDataLoading,
      isInitialLoader,
      isNextPageLoading,
      hasNextPage,
      files,
      folderId,
      title,
      page,
      selectedFileInfo,
    } = this.state;

    const loadingText = `${t("Common:LoadingProcessing")} ${t(
      "Common:LoadingDescription"
    )}`;
    return (
      <SelectionPanel
        loadingText={loadingText}
        resultingFolderTree={resultingFolderTree}
        isInitialLoader={isInitialLoader}
        isNextPageLoading={isNextPageLoading}
        hasNextPage={hasNextPage}
        items={files}
        folderId={folderId}
        title={title}
        page={page}
        footerChild={footerChild}
        headerChild={headerChild}
        onButtonClick={this.onButtonClick}
        onClose={onClose}
        isPanelVisible={isPanelVisible}
        onArrowClickAction={this.onArrowClickAction}
        onFolderClick={this.onFolderClick}
        onSelectFile={this.onSelectFile}
        t={t}
        isDataLoading={isDataLoading}
        loadNextPage={this._loadNextPage}
        selectedFileInfo={selectedFileInfo}
        buttonText={t("SaveHere")}
      />
    );
  }
}

SelectFileDialog.propTypes = {
  onClose: PropTypes.func.isRequired,
  isPanelVisible: PropTypes.bool.isRequired,
  onSelectFile: PropTypes.func.isRequired,
  foldersType: PropTypes.oneOf([
    "common",
    "third-party",
    "exceptSortedByTags",
    "exceptPrivacyTrashFolders",
  ]).isRequired,
  id: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  withoutProvider: PropTypes.bool,
};

SelectFileDialog.defaultProps = {
  isPanelVisible: false,
  foldersType: "common",
  id: "",
  withoutProvider: false,
};

export default inject(({ treeFoldersStore, filesStore }) => {
  const { treeFolders } = treeFoldersStore;
  const { filter } = filesStore;
  return {
    treeFolders,
    filter,
  };
})(observer(withTranslation(["SelectFile", "Common"])(SelectFileDialog)));
