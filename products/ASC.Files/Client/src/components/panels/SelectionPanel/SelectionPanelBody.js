import React from "react";
import ModalDialog from "@appserver/components/modal-dialog";
import RootPage from "./RootPage";
import ElementsPage from "./ElementsPage";
import IconButton from "@appserver/components/icon-button";
import Button from "@appserver/components/button";
import Loaders from "@appserver/common/components/Loaders";
import {
  StyledRootPage,
  StyledBody,
  StyledHeader,
} from "./StyledSelectionPanel";
import {
  getCommonFoldersTree,
  getFoldersTree,
  getThirdPartyFoldersTree,
} from "@appserver/common/api/files";
import toastr from "studio/toastr";
import {
  exceptSortedByTagsFolders,
  exceptPrivacyTrashFolders,
} from "../SelectionPanel/ExceptionFoldersConstants";
const SelectionPanelBody = ({
  isPanelVisible,
  isDataLoading,
  hasNextPage,
  resultingFolderTree,
  footerChild,
  headerChild,
  loadNextPage,
  loadingText,
  onButtonClick,
  onClose,
  onArrowClickAction,
  onSelectFile,
  onFolderClick,
  items,
  folderId,
  isNextPageLoading,
  isInitialLoader,
  page,
  title,
  t,
  selectedFileInfo,
  buttonText,
}) => {
  const isRootPage = folderId === "root";
  console.log("isRootPage", isRootPage);
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
                onClick={onArrowClickAction}
              />
              {title}
            </div>
          ) : (
            title
          )}
        </StyledHeader>
      </ModalDialog.Header>

      <ModalDialog.Body>
        {isRootPage ? (
          <StyledRootPage>
            {isInitialLoader ? (
              <div className="root-loader" key="loader">
                <Loaders.RootFoldersTree />
              </div>
            ) : (
              <RootPage data={resultingFolderTree} onClick={onFolderClick} />
            )}
          </StyledRootPage>
        ) : (
          <StyledBody footerChild={!!footerChild} headerChild={!!headerChild}>
            <div className="select-folder_content-body">
              <div className="select-dialog_header-child">{headerChild}</div>
              <div>
                <ElementsPage
                  selectedFileInfo={selectedFileInfo}
                  hasNextPage={hasNextPage}
                  isNextPageLoading={isNextPageLoading}
                  id={folderId}
                  items={items}
                  loadNextPage={loadNextPage}
                  onFolderClick={onFolderClick}
                  onSelectFile={onSelectFile}
                  loadingText={loadingText}
                  page={page}
                  t={t}
                />
              </div>
              <div className="select-dialog_footer">
                <div className="select-dialog_footer-child">{footerChild}</div>
                <div className="select-dialog_buttons">
                  <Button
                    //theme={theme}
                    primary
                    size="small"
                    label={buttonText}
                    onClick={onButtonClick}
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
};

class SelectionPanel extends React.Component {
  static getBasicFolderInfo = async (
    treeFolders,
    foldersType,
    id,
    onSetBaseFolderPath,
    onSelectFolder,
    foldersList
  ) => {
    const getRequestFolderTree = () => {
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

    const filterFoldersTree = (folders, arrayOfExceptions) => {
      let newArray = [];

      for (let i = 0; i < folders.length; i++) {
        if (!arrayOfExceptions.includes(folders[i].rootFolderType)) {
          newArray.push(folders[i]);
        }
      }

      return newArray;
    };
    const getExceptionsFolders = (treeFolders) => {
      switch (foldersType) {
        case "exceptSortedByTags":
          return filterFoldersTree(treeFolders, exceptSortedByTagsFolders);
        case "exceptPrivacyTrashFolders":
          return filterFoldersTree(treeFolders, exceptPrivacyTrashFolders);
      }
    };

    let requestedTreeFolders, filteredTreeFolders, passedId;

    const treeFoldersLength = treeFolders.length;

    if (treeFoldersLength === 0) {
      try {
        requestedTreeFolders = foldersList
          ? foldersList
          : await getRequestFolderTree();
      } catch (e) {
        toastr.error(e);
        return;
      }
    }

    const foldersTree =
      treeFoldersLength > 0 ? treeFolders : requestedTreeFolders;

    if (id || foldersType === "common") {
      passedId = id ? id : foldersTree[0].id;

      onSetBaseFolderPath && onSetBaseFolderPath(passedId);
      onSelectFolder && onSelectFolder(passedId);
    }

    if (
      foldersType === "exceptSortedByTags" ||
      foldersType === "exceptPrivacyTrashFolders"
    ) {
      filteredTreeFolders = getExceptionsFolders(foldersTree);
    }

    return [filteredTreeFolders || requestedTreeFolders, passedId];
  };
  render() {
    return <SelectionPanelBody {...this.props} />;
  }
}

export default SelectionPanel;
