import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import {
  getCommonFoldersTree,
  getFolder,
  getFoldersTree,
  getThirdPartyFoldersTree,
} from "@appserver/common/api/files";

import ModalDialog from "@appserver/components/modal-dialog";
import toastr from "studio/toastr";
import {
  exceptSortedByTagsFolders,
  exceptPrivacyTrashFolders,
} from "./ExceptionFoldersConstants";
import RootPage from "./RootPage";
import ElementsPage from "./ElementsPage";
import IconButton from "@appserver/components/icon-button";
import styled from "styled-components";
import Button from "@appserver/components/button";
import Loaders from "@appserver/common/components/Loaders";

const StyledHeader = styled.div`
  .dialog-header {
    display: flex;
  }
  .arrow-button {
    margin: auto 12px auto 0;
  }
`;

const StyledRootPage = styled.div`
  .root-loader {
    display: flex;
  }
`;
const StyledBody = styled.div`
  height: 100%;
  .content-body {
    display: grid;
    height: calc(100% - 32px);
    grid-template-rows: max-content auto max-content;
    .select-dialog_header-child {
      ${(props) => props.headerChild && `padding-bottom: 16px;`}
    }
  }

  .select-folder_loader {
    overflow: hidden;
    .list-loader-wrapper {
      padding: 0;
    }
  }
  .select-folder_list-loader {
    display: flex;
    div:first-child {
      margin-right: 8px;
    }
  }
  .select-dialog_footer {
    border-top: 1px solid #eceef1;
    padding-top: 16px;
    .select-dialog_buttons {
      display: flex;
      padding: 0 16px 0 16px;

      margin-left: -16px;
      margin-right: -16px;
      width: 100%;
      box-sizing: border-box;
      button:first-child {
        margin-right: 10px;
      }
    }
    .select-dialog_footer-child {
      ${(props) => props.footerChild && `padding-bottom: 16px;`}
    }
  }
`;
class SelectFolderDialog extends React.PureComponent {
  constructor(props) {
    super(props);
    const { filter, t } = props;
    this.rootTitle = t("SelectFolder");
    this.newFilter = filter.clone();

    this.state = {
      resultingFolderTree: [],
      isDataLoading: false,
      isInitialLoader: false,
      isNextPageLoading: false,
      page: 0,
      hasNextPage: true,

      id: "root",
      title: this.rootTitle,
      folders: [],
    };
  }
  filterFoldersTree = (folders, arrayOfExceptions) => {
    let newArray = [];

    for (let i = 0; i < folders.length; i++) {
      if (!arrayOfExceptions.includes(folders[i].rootFolderType)) {
        newArray.push(folders[i]);
      }
    }

    return newArray;
  };

  getRequestFolderTree = () => {
    const { foldersType } = this.props;

    switch (foldersType) {
      case "exceptSortedByTags":
      case "exceptPrivacyTrashFolders":
        try {
          return getFoldersTree();
        } catch (err) {
          console.error(err);
        }
        break;
      case "common":
        try {
          return getCommonFoldersTree();
        } catch (err) {
          console.error(err);
        }
        break;

      case "third-party":
        try {
          return getThirdPartyFoldersTree();
        } catch (err) {
          console.error(err);
        }
        break;
    }
  };

  getExceptionsFolders = (treeFolders) => {
    const { foldersType } = this.props;

    switch (foldersType) {
      case "exceptSortedByTags":
        return this.filterFoldersTree(treeFolders, exceptSortedByTagsFolders);
      case "exceptPrivacyTrashFolders":
        return this.filterFoldersTree(treeFolders, exceptPrivacyTrashFolders);
    }
  };

  async componentDidMount() {
    const {
      treeFolders,
      foldersType,
      id,
      onSetBaseFolderPath,
      onSelectFolder,
    } = this.props;

    let requestedTreeFolders,
      filteredTreeFolders,
      requests = [];

    const treeFoldersLength = treeFolders.length;

    let timerId = setTimeout(() => {
      this.setState({ isInitialLoader: true });
    }, 1000);

    if (treeFoldersLength === 0) {
      requests.push(this.getRequestFolderTree());
      if (id) requests.push(getFolder(id));

      [requestedTreeFolders] = await Promise.all(requests);

      clearTimeout(timerId);
      timerId = null;
    }

    const foldersTree =
      treeFoldersLength > 0 ? treeFolders : requestedTreeFolders;

    if (foldersType === "common") {
      // if (onSetBaseFolderPath) {
      //   this.getSelectedFolderInfo(id ? id : foldersTree[0].id);
      // }
      const passedId = id ? id : foldersTree[0].id;

      onSetBaseFolderPath && onSetBaseFolderPath(passedId);
      onSelectFolder && onSelectFolder(passedId);
    }

    if (
      foldersType === "exceptSortedByTags" ||
      foldersType === "exceptPrivacyTrashFolders"
    ) {
      filteredTreeFolders = this.getExceptionsFolders(foldersTree);
    }

    this.setState({
      resultingFolderTree: filteredTreeFolders || foldersTree,
      isInitialLoader: false,
    });
  }

  componentWillUnmount() {}

  componentDidUpdate(prevProps) {}

  deletedCurrentFolderIdFromPathParts = (pathParts) => {
    pathParts.splice(-1, 1);
  };

  onButtonClick = (e) => {
    const { folderInfo } = this.state;
    const { onClose, onSelectFolder, onSetNewFolderPath, id } = this.props;
    console.log("e", id, folderInfo.id);
    if (id) {
      if (+id !== +folderInfo.id) {
        onSetNewFolderPath && onSetNewFolderPath(folderInfo.id);
        onSelectFolder && onSelectFolder(folderInfo.id);
      }
    } else {
      onSetNewFolderPath && onSetNewFolderPath(folderInfo.id);
      onSelectFolder && onSelectFolder(folderInfo.id);
    }

    onClose();
  };
  onArrowClickAction = async () => {
    const { pathParts } = this.state;

    const newPathParts = [...pathParts];
    const prevFolderId = newPathParts.pop();
    const isRootFolder = newPathParts.length === 0;

    if (!isRootFolder) {
      try {
        this.setState({
          id: prevFolderId,
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
        id: "root",
        pathParts: [],
      });
    }
  };

  onRowClick = (id) => {
    console.log("on row click - id ", id);

    this.setState({
      id,
      folders: [],
      page: 0,
      hasNextPage: true,
      isDataLoading: true,
    });
  };

  _loadNextPage = () => {
    const { id, folders, page } = this.state;

    if (this._isLoadNextPage) return;

    this._isLoadNextPage = true;
    const pageCount = 15;
    this.newFilter.page = page;
    this.newFilter.pageCount = pageCount;

    console.log("loadNextPage", this.state.isNextPageLoading);

    this.setState({ isNextPageLoading: true }, async () => {
      const data = await getFolder(id, this.newFilter);
      const newFoldersList = [...folders].concat(data.folders);
      console.log("newFoldersList", newFoldersList, "folders", folders);
      const hasNextPage = data.folders.length === pageCount;

      let firstLoadInfo = {};
      if (page === 0) {
        const pathParts = [...data.pathParts];
        this.deletedCurrentFolderIdFromPathParts(pathParts);

        firstLoadInfo = {
          pathParts: ["root", ...pathParts],
          title: data.current.title,
        };
      }
      console.log("firstLoadInfo", firstLoadInfo, "id", id);
      this._isLoadNextPage = false;
      this.setState((state) => ({
        isDataLoading: false,
        hasNextPage: hasNextPage,
        isNextPageLoading: false,
        page: state.page + 1,
        folders: newFoldersList,
        ...firstLoadInfo,
      }));
    });
  };

  onSetDataLoading = () => {
    this.setState({});
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
      folders,
      id,
      title,
    } = this.state;

    const isRootPage = id === "root";

    console.log("this.state", this.state);
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
                  onClick={this.onRowClick}
                />
              )}
            </StyledRootPage>
          ) : (
            <StyledBody footerChild={!!footerChild} headerChild={!!headerChild}>
              <div className="content-body">
                <div className="select-dialog_header-child">{headerChild}</div>
                <div>
                  <ElementsPage
                    hasNextPage={hasNextPage}
                    isNextPageLoading={isNextPageLoading}
                    id={id}
                    folders={folders}
                    loadNextPage={this._loadNextPage}
                    onClick={this.onRowClick}
                    loadingText={loadingText}
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

export default inject(({ treeFoldersStore, filesStore }) => {
  const { treeFolders } = treeFoldersStore;
  const { filter } = filesStore;
  return {
    treeFolders,
    filter,
  };
})(observer(withTranslation(["SelectFolder", "Common"])(SelectFolderDialog)));
