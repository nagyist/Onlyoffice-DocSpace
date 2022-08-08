import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";
import Section from "@docspace/common/components/Section";
import Loaders from "@docspace/common/components/Loaders";
import { frameCallbackData, frameCallCommand } from "@docspace/common/utils";
import SelectFileDialog from "SRC_DIR/components/panels/SelectFileDialog";
import SelectFolderDialog from "SRC_DIR/components/panels/SelectFolderDialog";

const IntegrationPage = ({ frameConfig, setFrameConfig }) => {
  useEffect(() => {
    window.addEventListener("message", handleMessage);

    if (window.parent && !frameConfig) {
      frameCallCommand("setConfig");
    }

    return () => window.removeEventListener("message", handleMessage);
  }, []);

  const handleMessage = async (e) => {
    const eventData = typeof e.data === "string" ? JSON.parse(e.data) : e.data;

    if (eventData.data) {
      const { data, methodName } = eventData.data;

      let res;

      switch (methodName) {
        case "setConfig":
          res = await setFrameConfig(data);
          break;
        default:
          res = "Wrong method";
      }

      frameCallbackData(res);
    }
  };

  const onSelectCallback = (e) => {
    const { onSelectCallback } = frameConfig.events;
    onSelectCallback && onSelectCallback(e);
  };
  const onCloseCallback = (e) => {
    const { onCloseCallback } = frameConfig.events;
    onCloseCallback && onCloseCallback(e);
  };

  let selector = null;

  switch (frameConfig?.mode) {
    case "fileSelector": {
      selector = (
        <SelectFileDialog
          isPanelVisible={true}
          onClose={onCloseCallback}
          onSelectFile={onSelectCallback}
          headerName={frameConfig.name}
          foldersType="exceptPrivacyTrashFolders"
        />
      );
      break;
    }
    case "folderSelector": {
      selector = (
        <SelectFolderDialog
          isPanelVisible={true}
          onClose={onCloseCallback}
          onSelectFolder={onSelectCallback}
          foldersType="exceptPrivacyTrashFolders"
          displayType="modal"
          id={frameConfig.folderId}
          withoutProvider={false}
          withoutImmediatelyClose={false}
          isDisableTree={false}
        />
      );
      break;
    }
  }

  return (
    <Section>
      <Section.SectionBody>
        {frameConfig ? selector : <Loaders.Rectangle />}
      </Section.SectionBody>
    </Section>
  );
};

export default inject(({ auth }) => {
  const { settingsStore } = auth;
  const { frameConfig, isFrame, setFrameConfig } = settingsStore;

  return {
    auth,
    frameConfig,
    isFrame,
    setFrameConfig,
  };
})(observer(withRouter(IntegrationPage)));
