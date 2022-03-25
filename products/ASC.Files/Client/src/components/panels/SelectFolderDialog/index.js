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
import SelectFolderInput from "../SelectFolderInput";

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

  .loader {
    overflow: hidden;
    .list-loader-wrapper {
      padding: 0;
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
class SelectFolderDialog extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      resultingFolderTree: [],
      isDataLoading: false,
      isInitialLoader: false,
      folderInfo: {
        pathParts: [],
        folders: [],
        files: [],
        id: "root",
        title: "",
      },
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

    let requestedTreeFolders, filteredTreeFolders, folderTitle;

    const treeFoldersLength = treeFolders.length;
    let timerId = setTimeout(() => {
      this.setState({ isInitialLoader: true });
    }, 1000);

    if (treeFoldersLength === 0) {
      requestedTreeFolders = await this.getRequestFolderTree();
      clearTimeout(timerId);
      timerId = null;

      console.log("requestedFolderTree", requestedTreeFolders);
    }

    const foldersTree =
      treeFoldersLength > 0 ? treeFolders : requestedTreeFolders;

    if (foldersType === "common" && !id) {
      onSetBaseFolderPath && onSetBaseFolderPath(foldersTree[0].id);
      onSelectFolder && onSelectFolder(null, foldersTree[0].id);
    }

    if (
      foldersType === "exceptSortedByTags" ||
      foldersType === "exceptPrivacyTrashFolders"
    ) {
      filteredTreeFolders = this.getExceptionsFolders(foldersTree);

      console.log("exceptionsFolders", filteredTreeFolders);
    }

    this.setState({
      resultingFolderTree: filteredTreeFolders || foldersTree,
      isInitialLoader: false,
    });
  }

  componentWillUnmount() {}

  componentDidUpdate(prevProps) {
    const { isReset } = this.props;
    if (isReset && isReset !== prevProps.isReset) {
      this.onResetInfo();
    }
  }

  onResetInfo = async () => {
    const { id, onSelectFolder, foldersType } = this.props;

    if (!id) {
      let requestedTreeFolders = await this.getRequestFolderTree();

      if (foldersType === "common") {
        onSelectFolder && onSelectFolder(requestedTreeFolders[0].id);
        return;
      }

      if (foldersType === "third-party") {
        onSelectFolder && onSelectFolder("");
        return;
      }
      return;
    }

    onSelectFolder && onSelectFolder(id);
  };

  deletedCurrentFolderIdFromPathParts = (pathParts) => {
    pathParts.splice(-1, 1);
  };
  getSelectedFolderInfo = async (id) => {
    try {
      const data = await getFolder(id);

      clearTimeout(this.timerId);
      this.timerId = null;

      console.log("data", data);
      const pathParts = [...data.pathParts];

      this.deletedCurrentFolderIdFromPathParts(pathParts);

      this.setState({
        isDataLoading: false,
        folderInfo: {
          folders: data.folders,
          files: [], //data.files,
          title: data.current.title,
          id: data.current.id,
          pathParts: ["root", ...pathParts],
        },
      });
    } catch (e) {
      toastr.error(e);
      this.setState({
        isDataLoading: false,
      });
    }
  };

  onRowClick = (id) => {
    console.log("on row click - id ", id);

    this.timerId = setTimeout(() => {
      this.setState({ isDataLoading: true });
    }, 1000);

    this.getSelectedFolderInfo(id);
  };
  onButtonClick = (e) => {
    const { folderInfo } = this.state;
    const { onClose, onSelectFolder, onSetNewFolderPath } = this.props;
    console.log("e", e);
    onSetNewFolderPath && onSetNewFolderPath(folderInfo.id);
    onSelectFolder && onSelectFolder(e, folderInfo.id);
    onClose();
  };
  onArrowClickAction = async () => {
    const { folderInfo } = this.state;
    const { pathParts } = folderInfo;
    const newPathParts = [...pathParts];
    const prevFolderId = newPathParts.pop();
    console.log("prevFolder id", prevFolderId, "newPathParts", newPathParts);
    const isRootFolder = newPathParts.length === 0;

    if (!isRootFolder) {
      try {
        const data = await getFolder(prevFolderId);

        const pathParts = [...data.pathParts];
        this.deletedCurrentFolderIdFromPathParts(pathParts);

        this.setState({
          folderInfo: {
            folders: data.folders,
            files: [], //data.files,
            title: data.current.title,
            id: data.current.id,
            pathParts: ["root", ...pathParts],
          },
        });
      } catch (e) {
        toastr.error(e);
      }
    } else {
      this.setState({
        folderInfo: {
          folders: [],
          files: [], //data.files,
          title: "",
          id: "root",
          pathParts: [],
        },
      });
    }
  };
  render() {
    const {
      isPanelVisible,
      onClose,
      t,
      footer: footerChild,
      header: headerChild,
      onSave,
    } = this.props;
    const {
      resultingFolderTree,
      folderInfo,
      isDataLoading,
      isInitialLoader,
    } = this.state;
    const { title, id } = folderInfo;

    const isRootPage = id === "root";

    console.log("isDataLoading", isDataLoading);
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
              t("SelectFolder")
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
                {isDataLoading ? (
                  <div className="loader" key="loader">
                    <Loaders.ListLoader
                      withoutFirstRectangle
                      withoutLastRectangle
                    />
                  </div>
                ) : (
                  <div>
                    <ElementsPage
                      folderInfo={folderInfo}
                      onClick={this.onRowClick}
                    />
                  </div>
                )}
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

export default inject(({ treeFoldersStore }) => {
  const { treeFolders } = treeFoldersStore;

  return {
    treeFolders,
  };
})(observer(withTranslation(["SelectFolder", "Common"])(SelectFolderDialog)));
