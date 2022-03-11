import React from "react";
import moment from "moment";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import { withTranslation, Trans } from "react-i18next";
import { inject, observer } from "mobx-react";
import ArrowRightIcon from "../../../../../../../public/images/arrow.right.react.svg";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import Box from "@appserver/components/box";
import styled from "styled-components";
import FloatingButton from "@appserver/common/components/FloatingButton";
import { enableAutoBackup, enableRestore } from "@appserver/common/api/portal";
import Loader from "@appserver/components/loader";
import { StyledBackup, StyledSettingsHeader } from "./StyledBackup";
import Headline from "@appserver/common/components/Headline";
import IconButton from "@appserver/components/icon-button";
import toastr from "@appserver/components/toast/toastr";
import AutoBackup from "./auto-backup";
import ManualBackup from "./manual-backup";
import RestoreBackup from "./restore-backup";

const StyledArrowRightIcon = styled(ArrowRightIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.color};
  }
`;

class BackupMobileView extends React.Component {
  constructor(props) {
    super(props);
    const { language } = props;

    this.lng = language.substring(0, language.indexOf("-"));
    moment.locale(this.lng);

    this.state = {
      enableRestore: false,
      enableAutoBackup: false,
      isLoading: true,
      autoBackup: false,
      manualBackup: false,
      restoreBackup: false,
    };
  }

  componentDidMount() {
    this.setBasicSettings();
  }
  componentWillUnmount() {
    const { clearProgressInterval } = this.props;

    clearProgressInterval();
  }
  setBasicSettings = async () => {
    const { t, getProgress } = this.props;

    try {
      const requests = [enableRestore(), enableAutoBackup()];

      getProgress(t);

      const [canRestore, canAutoBackup] = await Promise.all(requests);

      this.setState({
        isLoading: false,
        enableRestore: canRestore,
        enableAutoBackup: canAutoBackup,
      });
    } catch (error) {
      toastr.error(error);
      this.setState({
        isLoading: false,
      });
    }
  };

  onClickFloatingButton = () => {
    const { manualBackup } = this.state;

    !manualBackup &&
      this.setState({
        manualBackup: true,
        autoBackup: false,
        restoreBackup: false,
      });
  };

  onClickModule = (name) => {
    this.setState({
      [name]: true,
    });
  };

  onBackToParent = () => {
    this.setState({
      autoBackup: false,
      manualBackup: false,
      restoreBackup: false,
    });
  };
  render() {
    const {
      t,
      helpUrlCreatingBackup,
      downloadingProgress,
      history,
    } = this.props;
    const {
      isLoading,
      enableRestore,
      enableAutoBackup,
      autoBackup,
      manualBackup,
      restoreBackup,
    } = this.state;

    const autoBackupTitle = t("AutoBackup");
    const manualBackupTitle = t("ManualBackup");
    const restoreBackupTitle = t("RestoreBackup");

    const isSectionOpen = autoBackup || manualBackup || restoreBackup;
    const headerTitle = autoBackup
      ? autoBackupTitle
      : manualBackup
      ? manualBackupTitle
      : restoreBackup
      ? restoreBackupTitle
      : "";

    const renderSection = (section, sectionTitle, keyHelp) => (
      <div className="backup-section_wrapper">
        <div className="backup-section_heading">
          <Text
            className="backup-section_text"
            onClick={() => this.onClickModule(section)}
          >
            {sectionTitle}
          </Text>
          <StyledArrowRightIcon
            className="backup-section_arrow-button"
            size="small"
            color="#333333"
          />
        </div>

        <Trans
          t={t}
          i18nKey={keyHelp}
          ns="Settings"
          components={{ strong: <></> }}
        />

        <Box marginProp="10px 0 0 0">
          <Link
            color="#316DAA"
            target="_blank"
            isHovered={true}
            href={helpUrlCreatingBackup}
            fontWeight={600}
          >
            {t("Common:LearnMore")}
          </Link>
        </Box>
      </div>
    );
    const renderBackupSections = () => (
      <>
        {renderSection("manualBackup", manualBackupTitle, "ManualBackupHelp")}

        {enableAutoBackup &&
          renderSection("autoBackup", autoBackupTitle, "AutoBackupHelp")}

        {enableRestore &&
          renderSection(
            "restoreBackup",
            restoreBackupTitle,
            "RestoreBackupHelp"
          )}
      </>
    );

    console.log("render");

    return isLoading ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <StyledBackup>
        {isSectionOpen && (
          <StyledSettingsHeader>
            <IconButton
              iconName="/static/images/arrow.path.react.svg"
              size="17"
              color="#A3A9AE"
              hoverColor="#657077"
              isFill={true}
              onClick={this.onBackToParent}
              className="backup_arrow-button"
            />

            <Headline type="content" className="backup_header" truncate={true}>
              {headerTitle}
            </Headline>
          </StyledSettingsHeader>
        )}

        {autoBackup && <AutoBackup />}
        {manualBackup && <ManualBackup />}
        {restoreBackup && <RestoreBackup history={history} />}

        {!isSectionOpen && renderBackupSections()}

        {downloadingProgress > 0 && downloadingProgress !== 100 && (
          <FloatingButton
            className="layout-progress-bar"
            icon="file"
            alert={false}
            percent={downloadingProgress}
            onClick={this.onClickFloatingButton}
          />
        )}
      </StyledBackup>
    );
  }
}

export default inject(({ auth, backup }) => {
  const { language } = auth;
  const { helpUrlCreatingBackup } = auth.settingsStore;
  const { getProgress, downloadingProgress, clearProgressInterval } = backup;
  return {
    helpUrlCreatingBackup,
    language,
    getProgress,
    downloadingProgress,
    clearProgressInterval,
  };
})(observer(withTranslation(["Settings", "Common"])(BackupMobileView)));
