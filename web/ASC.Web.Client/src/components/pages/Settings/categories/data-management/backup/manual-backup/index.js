import React from "react";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";
import Button from "@appserver/components/button";
import { startBackup } from "@appserver/common/api/portal";
import toastr from "@appserver/components/toast/toastr";
import ThirdPartyModule from "./sub-components/ThirdPartyModule";
import DocumentsModule from "./sub-components/DocumentsModule";
import ThirdPartyStorageModule from "./sub-components/ThirdPartyStorageModule";
import RadioButton from "@appserver/components/radio-button";
import { StyledModules, StyledManualBackup } from "./../StyledBackup";
import SelectFolderDialog from "files/SelectFolderDialog";
import Loader from "@appserver/components/loader";
import { saveToSessionStorage, getFromSessionStorage } from "../../../../utils";
import { BackupTypes } from "@appserver/common/constants";
import { isMobileOnly } from "react-device-detect";
import { inject, observer } from "mobx-react";

let selectedStorageType = "";

class ManualBackup extends React.Component {
  constructor(props) {
    super(props);

    selectedStorageType = getFromSessionStorage("LocalCopyStorageType");

    const checkedDocuments = selectedStorageType
      ? selectedStorageType === "Documents"
      : false;
    const checkedTemporary = selectedStorageType
      ? selectedStorageType === "TemporaryStorage"
      : true;
    const checkedThirdPartyResource = selectedStorageType
      ? selectedStorageType === "ThirdPartyResource"
      : false;
    const checkedThirdPartyStorage = selectedStorageType
      ? selectedStorageType === "ThirdPartyStorage"
      : false;

    this.state = {
      selectedFolder: "",
      isPanelVisible: false,
      isInitialLoading: isMobileOnly ? true : false,
      isCheckedTemporaryStorage: checkedTemporary,
      isCheckedDocuments: checkedDocuments,
      isCheckedThirdParty: checkedThirdPartyResource,
      isCheckedThirdPartyStorage: checkedThirdPartyStorage,
    };

    this.switches = [
      "isCheckedTemporaryStorage",
      "isCheckedDocuments",
      "isCheckedThirdParty",
      "isCheckedThirdPartyStorage",
    ];
  }

  setBasicSettings = async () => {
    const { getProgress, setCommonThirdPartyList, t } = this.props;
    try {
      getProgress(t);

      const commonThirdPartyList = await SelectFolderDialog.getCommonThirdPartyList();
      commonThirdPartyList && setCommonThirdPartyList(commonThirdPartyList);
    } catch (error) {
      console.error(error);
      this.clearSessionStorage();
    }

    this.setState({
      isInitialLoading: false,
    });
  };

  componentDidMount() {
    isMobileOnly && this.setBasicSettings();
  }

  componentWillUnmount() {
    const { clearProgressInterval } = this.props;
    clearProgressInterval();
  }

  onMakeTemporaryBackup = async () => {
    const { getIntervalProgress, setDownloadingProgress, t } = this.props;
    const { TemporaryModuleType } = BackupTypes;

    saveToSessionStorage("LocalCopyStorageType", "TemporaryStorage");

    try {
      await startBackup(`${TemporaryModuleType}`, null);
      setDownloadingProgress(1);
      getIntervalProgress(t);
    } catch (e) {
      toastr.error(`${t("BackupCreatedError")}`);
      console.error(err);
    }
  };

  onClickDownloadBackup = () => {
    const { temporaryLink } = this.props;
    const url = window.location.origin;
    const downloadUrl = `${url}` + `${temporaryLink}`;
    window.open(downloadUrl, "_self");
  };

  onClickShowStorage = (e) => {
    let newStateObj = {};
    const name = e.target.name;
    newStateObj[name] = true;

    const newState = this.switches.filter((el) => el !== name);
    newState.forEach((name) => (newStateObj[name] = false));

    this.setState({
      ...newStateObj,
    });
  };

  onMakeCopy = async (
    selectedFolder,
    moduleName,
    moduleType,
    key,
    selectedId,
    storageValues,
    selectedStorageId,
    selectedStorage
  ) => {
    const { isCheckedDocuments, isCheckedThirdParty } = this.state;
    const {
      t,
      getIntervalProgress,
      setDownloadingProgress,
      clearSessionStorage,
      setTemporaryLink,
    } = this.props;

    const storageValue =
      isCheckedDocuments || isCheckedThirdParty ? selectedFolder : selectedId;

    const storageParams = [
      {
        key: `${key}`,
        value: storageValue,
      },
    ];

    saveToSessionStorage("LocalCopyStorageType", moduleName);

    if (isCheckedDocuments || isCheckedThirdParty) {
      saveToSessionStorage("LocalCopyFolder", `${selectedFolder}`);

      SelectFolderDialog.getFolderPath(selectedFolder).then((folderPath) => {
        saveToSessionStorage("LocalCopyPath", `${folderPath}`);
      });
    } else {
      saveToSessionStorage("LocalCopyStorage", `${selectedStorageId}`);
      saveToSessionStorage(
        "LocalCopyThirdPartyStorageType",
        `${selectedStorage}`
      );

      for (let i = 0; i < storageValues.length; i++) {
        storageParams.push(storageValues[i]);
      }
    }

    try {
      await startBackup(moduleType, storageParams);
      setDownloadingProgress(1);
      setTemporaryLink("");
      getIntervalProgress(t);
    } catch (err) {
      toastr.error(`${t("BackupCreatedError")}`);
      console.error(err);

      clearSessionStorage();
    }
  };
  render() {
    const {
      t,
      temporaryLink,
      downloadingProgress,
      commonThirdPartyList,
    } = this.props;
    const {
      isInitialLoading,
      isCheckedTemporaryStorage,
      isCheckedDocuments,
      isCheckedThirdParty,
      isCheckedThirdPartyStorage,
    } = this.state;

    const isMaxProgress = downloadingProgress === 100;

    const isDisabledThirdParty = !!commonThirdPartyList;

    const commonRadioButtonProps = {
      fontSize: "13px",
      fontWeight: "400",
      value: "value",
      className: "backup_radio-button",
      onClick: this.onClickShowStorage,
    };

    const commonModulesProps = {
      isMaxProgress,
      onMakeCopy: this.onMakeCopy,
    };

    return isInitialLoading ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <StyledManualBackup>
        <StyledModules>
          <RadioButton
            label={t("TemporaryStorage")}
            name={"isCheckedTemporaryStorage"}
            key={0}
            isChecked={isCheckedTemporaryStorage}
            isDisabled={!isMaxProgress}
            {...commonRadioButtonProps}
          />
          <Text className="backup-description">
            {t("TemporaryStorageDescription")}
          </Text>
          {isCheckedTemporaryStorage && (
            <div className="manual-backup_buttons">
              <Button
                label={t("Common:Duplicate")}
                onClick={this.onMakeTemporaryBackup}
                primary
                isDisabled={!isMaxProgress}
                size="medium"
              />
              {temporaryLink?.length > 0 && isMaxProgress && (
                <Button
                  label={t("DownloadCopy")}
                  onClick={this.onClickDownloadBackup}
                  isDisabled={false}
                  size="medium"
                  style={{ marginLeft: "8px" }}
                />
              )}
              {!isMaxProgress && (
                <Button
                  label={t("Common:CopyOperation") + "..."}
                  isDisabled={true}
                  size="medium"
                  style={{ marginLeft: "8px" }}
                />
              )}
            </div>
          )}
        </StyledModules>

        <StyledModules>
          <RadioButton
            label={t("DocumentsModule")}
            name={"isCheckedDocuments"}
            key={1}
            isChecked={isCheckedDocuments}
            isDisabled={!isMaxProgress}
            {...commonRadioButtonProps}
          />

          <Text className="backup-description module-documents">
            {t("DocumentsModuleDescription")}
          </Text>

          {isCheckedDocuments && (
            <DocumentsModule
              {...commonModulesProps}
              isCheckedDocuments={isCheckedDocuments}
            />
          )}
        </StyledModules>

        <StyledModules isDisabled={isDisabledThirdParty}>
          <RadioButton
            label={t("ThirdPartyResource")}
            name={"isCheckedThirdParty"}
            key={2}
            isChecked={isCheckedThirdParty}
            isDisabled={isDisabledThirdParty || !isMaxProgress}
            {...commonRadioButtonProps}
          />
          <Text className="backup-description">
            {t("ThirdPartyResourceDescription")}
          </Text>
          {isCheckedThirdParty && <ThirdPartyModule {...commonModulesProps} />}
        </StyledModules>

        <StyledModules>
          <RadioButton
            label={t("ThirdPartyStorage")}
            name={"isCheckedThirdPartyStorage"}
            key={3}
            isChecked={isCheckedThirdPartyStorage}
            isDisabled={!isMaxProgress}
            {...commonRadioButtonProps}
          />
          <Text className="backup-description">
            {t("ThirdPartyStorageDescription")}
          </Text>
          {isCheckedThirdPartyStorage && (
            <ThirdPartyStorageModule {...commonModulesProps} />
          )}
        </StyledModules>
      </StyledManualBackup>
    );
  }
}

export default inject(({ backup }) => {
  const {
    setDownloadingProgress,
    getProgress,
    setTemporaryLink,
    setCommonThirdPartyList,
    downloadingProgress,
    temporaryLink,
    clearProgressInterval,
    getIntervalProgress,
    clearSessionStorage,
    commonThirdPartyList,
  } = backup;

  return {
    setDownloadingProgress,
    getProgress,
    getIntervalProgress,
    setTemporaryLink,
    setCommonThirdPartyList,
    downloadingProgress,
    temporaryLink,
    clearProgressInterval,
    clearSessionStorage,
    commonThirdPartyList,
  };
})(withTranslation(["Settings", "Common"])(observer(ManualBackup)));
