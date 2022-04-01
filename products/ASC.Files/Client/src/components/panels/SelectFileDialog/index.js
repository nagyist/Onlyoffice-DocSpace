import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import {
  getCommonFoldersTree,
  getFolder,
  getFoldersTree,
  getThirdPartyFoldersTree,
} from "@appserver/common/api/files";
import PropTypes from "prop-types";
import ModalDialog from "@appserver/components/modal-dialog";
import toastr from "studio/toastr";

import RootPage from "./SubComponents/RootPage";
import IconButton from "@appserver/components/icon-button";
import Button from "@appserver/components/button";
import Loaders from "@appserver/common/components/Loaders";
import { FolderType } from "@appserver/common/constants";
import {
  StyledRootPage,
  StyledBody,
  StyledHeader,
} from "../SelectFolderDialog/StyledSelectFolderDialog";
import FilesList from "./FilesListBody";
import SelectFolderDialog from "../SelectFolderDialog";
class SelectFileDialog extends React.PureComponent {
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
      files: [],
      selectedFileInfo: [],
    };
  }

  async componentDidMount() {
    const { treeFolders, foldersType, id, foldersList } = this.props;

    let timerId = setTimeout(() => {
      foldersType !== "common" && this.setState({ isInitialLoader: true });
    }, 1000);

    let resultingFolderTree, resultingId;

    try {
      [
        resultingFolderTree,
        resultingId,
      ] = await SelectFolderDialog.getBasicFolderInfo(
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
      ...(foldersType === "common" && {
        folderId: resultingId,
      }),
    });
  }

  deletedCurrentFolderIdFromPathParts = (pathParts) => {
    pathParts.splice(-1, 1);
  };

  onButtonClick = (e) => {
    const { selectedFileInfo } = this.state;
    const { onClose, onSelectFile, onSetFileName } = this.props;
    onSetFileName && onSetFileName(selectedFileInfo.title);
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
        console.log("DATa", data);
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
        console.log("newFilesList", newFilesList.length, data.total);
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

    const isRootPage = folderId === "root";

    const loadingText = `${t("Common:LoadingProcessing")} ${t(
      "Common:LoadingDescription"
    )}`;
    return (
      <ModalDialog
        visible={isPanelVisible}
        zIndex={310}
        onClose={onClose}
        displayType="aside"
        withoutBodyScroll
        contentHeight="100%"
        contentPaddingBottom="0px"
      >
        <ModalDialog.Header>
          <StyledHeader>
            {!isRootPage ? (
              <div className="dialog-header">
                <IconButton
                  iconName="/static/images/arrow.path.react.svg"
                  size="17"
                  isFill={true}
                  className="arrow-button"
                  onClick={this.onArrowClickAction}
                />
                {title}
              </div>
            ) : (
              title
            )}
          </StyledHeader>
        </ModalDialog.Header>

        <ModalDialog.Body>
          {isRootPage && !isDataLoading ? (
            <StyledRootPage>
              {isInitialLoader ? (
                <div className="root-loader" key="loader">
                  <Loaders.RootFoldersTree />
                </div>
              ) : (
                <RootPage
                  data={resultingFolderTree}
                  onClick={this.onFolderClick}
                />
              )}
            </StyledRootPage>
          ) : (
            <StyledBody footerChild={!!footerChild} headerChild={!!headerChild}>
              <div className="select-folder_content-body">
                <div className="select-dialog_header-child">{headerChild}</div>
                <div>
                  <FilesList
                    hasNextPage={hasNextPage}
                    isNextPageLoading={isNextPageLoading}
                    id={folderId}
                    files={files}
                    loadNextPage={this._loadNextPage}
                    onFolderClick={this.onFolderClick}
                    loadingText={loadingText}
                    page={page}
                    t={t}
                    selectedFileInfo={selectedFileInfo}
                    onSelectFile={this.onSelectFile}
                  />
                </div>
                <div className="select-dialog_footer">
                  <div className="select-dialog_footer-child">
                    {footerChild}
                  </div>
                  <div className="select-dialog_buttons">
                    <Button
                      //theme={theme}
                      primary
                      size="small"
                      label={t("SaveHere")}
                      onClick={this.onButtonClick}
                      isDisabled={isDataLoading}
                    />
                    <Button
                      size="small"
                      label={t("Common:CancelButton")}
                      onClick={onClose}
                      isDisabled={isDataLoading}
                    />
                  </div>
                </div>
              </div>
            </StyledBody>
          )}
        </ModalDialog.Body>
      </ModalDialog>
    );
  }
}

SelectFileDialog.propTypes = {
  onClose: PropTypes.func.isRequired,
  isPanelVisible: PropTypes.bool.isRequired,
  // onSelectFolder: PropTypes.func.isRequired,
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
})(observer(withTranslation(["SelectFolder", "Common"])(SelectFileDialog)));
