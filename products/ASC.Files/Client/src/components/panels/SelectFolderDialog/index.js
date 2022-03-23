import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { getFolder, getFoldersTree } from "@appserver/common/api/files";
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

const StyledHeader = styled.div`
  .dialog-header {
    display: flex;
  }
  .arrow-button {
    margin: auto 12px auto 0;
  }
`;

class SelectFolderDialog extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      resultingFolderTree: [],
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
          return SelectFolderDialog.getCommonFolders();
        } catch (err) {
          console.error(err);
        }
        break;

      case "third-party":
        try {
          return SelectFolderDialog.getCommonThirdPartyList();
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
    const { treeFolders, foldersType } = this.props;

    let requestedTreeFolders, filteredTreeFolders;

    const treeFoldersLength = treeFolders.length;

    if (treeFoldersLength === 0) {
      requestedTreeFolders = await this.getRequestFolderTree();
      console.log("requestedFolderTree", requestedTreeFolders);
    }

    const foldersTree =
      treeFoldersLength > 0 ? treeFolders : requestedTreeFolders;

    if (
      foldersType === "exceptSortedByTags" ||
      foldersType === "exceptPrivacyTrashFolders"
    ) {
      filteredTreeFolders = this.getExceptionsFolders(foldersTree);

      console.log("exceptionsFolders", filteredTreeFolders);
    }

    this.setState({
      resultingFolderTree: filteredTreeFolders || foldersTree,
    });
  }

  componentWillUnmount() {}

  deletedCurrentFolderIdFromPathParts = (pathParts) => {
    pathParts.splice(-1, 1);
  };
  getSelectedFolderInfo = async (id) => {
    try {
      const data = await getFolder(id);
      console.log("data", data);
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
  };

  onClick = (id) => {
    console.log("on row click - id ", id);
    this.getSelectedFolderInfo(id);
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
    const { isPanelVisible, onClose } = this.props;
    const { resultingFolderTree, folderInfo } = this.state;
    const { title, id } = folderInfo;
    const isRootPage = id === "root";

    return (
      <ModalDialog
        visible={isPanelVisible}
        zIndex={310}
        onClose={onClose}
        displayType="aside"
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
              "Folder Selection"
            )}
          </StyledHeader>
        </ModalDialog.Header>

        <ModalDialog.Body>
          {isRootPage ? (
            <RootPage data={resultingFolderTree} onClick={this.onClick} />
          ) : (
            <ElementsPage folderInfo={folderInfo} onClick={this.onClick} />
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
})(observer(withTranslation(["Common", "Translations"])(SelectFolderDialog)));
