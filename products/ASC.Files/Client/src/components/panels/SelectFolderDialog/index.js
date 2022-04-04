import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { getFolder } from "@appserver/common/api/files";
import PropTypes from "prop-types";
import toastr from "studio/toastr";
import { FolderType } from "@appserver/common/constants";
import SelectionPanel from "../SelectionPanel/SelectionPanelBody";

class SelectFolderDialog extends React.PureComponent {
  constructor(props) {
    super(props);
    const { filter, t, id } = props;
    this.rootTitle = t("SelectFolder");
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
      folders: [],
    };
  }

  async componentDidMount() {
    const {
      treeFolders,
      foldersType,
      id,
      onSetBaseFolderPath,
      onSelectFolder,
      foldersList,
      treeFromInput,
    } = this.props;

    let timerId = setTimeout(() => {
      this.setState({ isInitialLoader: true });
    }, 1000);

    let resultingFolderTree, resultingId;

    if (!treeFromInput) {
      try {
        [
          resultingFolderTree,
          resultingId,
        ] = await SelectionPanel.getBasicFolderInfo(
          treeFolders,
          foldersType,
          id,
          onSetBaseFolderPath,
          onSelectFolder,
          foldersList
        );
      } catch (e) {
        toastr.error(e);

        clearTimeout(timerId);
        timerId = null;
        this.setState({ isInitialLoader: false });

        return;
      }
    }

    clearTimeout(timerId);
    timerId = null;

    this.setState({
      resultingFolderTree: treeFromInput ? treeFromInput : resultingFolderTree,
      isInitialLoader: false,
      ...(foldersType === "common" && {
        folderId: treeFromInput ? id : resultingId,
      }),
    });
  }

  deletedCurrentFolderIdFromPathParts = (pathParts) => {
    pathParts.splice(-1, 1);
  };

  onButtonClick = (e) => {
    const { folderId } = this.state;
    const { onClose, onSelectFolder, onSetNewFolderPath, id } = this.props;

    if (id) {
      if (+id !== +folderId) {
        onSetNewFolderPath && onSetNewFolderPath(folderId);
        onSelectFolder && onSelectFolder(folderId);
      }
    } else {
      onSetNewFolderPath && onSetNewFolderPath(folderId);
      onSelectFolder && onSelectFolder(folderId);
    }

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
          folders: [],
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
        folders: [],
        title: this.rootTitle,
        folderId: "root",
        pathParts: [],
      });
    }
  };

  onRowClick = (id) => {
    this.setState({
      folderId: id,
      folders: [],
      page: 0,
      hasNextPage: true,
      isDataLoading: true,
    });
  };

  _loadNextPage = () => {
    const { folderId, folders, page } = this.state;
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
          dataWithoutProvider = data.folders.filter((value) => {
            if (!value.providerKey) return value;
          });
        }

        const finalData = withoutProvider ? dataWithoutProvider : data.folders;

        const newFoldersList = [...folders].concat(finalData);

        const hasNextPage = finalData.length === pageCount;

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
          folders: newFoldersList,
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
      isInitialLoader,
      isNextPageLoading,
      hasNextPage,
      folders,
      folderId,
      title,
      page,
      isDataLoading,
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
        items={folders}
        folderId={folderId}
        title={title}
        page={page}
        footerChild={footerChild}
        headerChild={headerChild}
        onButtonClick={this.onButtonClick}
        onClose={onClose}
        isPanelVisible={isPanelVisible}
        onArrowClickAction={this.onArrowClickAction}
        onFolderClick={this.onRowClick}
        t={t}
        isDataLoading={isDataLoading}
        loadNextPage={this._loadNextPage}
        buttonText={t("SaveHere")}
      />
    );
  }
}

SelectFolderDialog.propTypes = {
  onClose: PropTypes.func.isRequired,
  isPanelVisible: PropTypes.bool.isRequired,
  onSelectFolder: PropTypes.func.isRequired,
  foldersType: PropTypes.oneOf([
    "common",
    "third-party",
    "exceptSortedByTags",
    "exceptPrivacyTrashFolders",
  ]).isRequired,
  id: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  withoutProvider: PropTypes.bool,
};

SelectFolderDialog.defaultProps = {
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
})(observer(withTranslation(["SelectFolder", "Common"])(SelectFolderDialog)));
