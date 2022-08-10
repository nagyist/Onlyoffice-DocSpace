import React, { useState, useEffect, useRef } from "react";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import Box from "@docspace/components/box";
import TextInput from "@docspace/components/text-input";
import Textarea from "@docspace/components/textarea";
import Text from "@docspace/components/text";
import Label from "@docspace/components/label";
import Checkbox from "@docspace/components/checkbox";
import Button from "@docspace/components/button";
import ComboBox from "@docspace/components/combobox";
import Heading from "@docspace/components/heading";
import toastr from "@docspace/components/toast/toastr";
import { tablet } from "@docspace/components/utils/device";
import { objectToGetParams, loadScript } from "@docspace/common/utils";
import { inject, observer } from "mobx-react";
import Loaders from "@docspace/common/components/Loaders";

const Container = styled(Box)`
  width: 100%;
  display: flex;
  gap: 48px;
`;

const Description = styled(Text)`
  line-height: 20px;
`;

const Category = styled(Text)`
  margin-top: ${(props) => (props.marginTop ? props.marginTop : "20px")};
  font-weight: ${(props) => (props.fontWeight ? props.fontWeight : "700")};
  font-size: ${(props) => (props.fontSize ? props.fontSize : "16px")};
  line-height: ${(props) => (props.lineHeight ? props.lineHeight : "22px")};
`;

const Frame = styled(Box)`
  margin-top: 16px;

  > div {
    border: 1px solid #d0d5da;
    border-radius: 6px;
    width: 480px;
    height: 280px;
  }
`;

const Controls = styled(Box)`
  width: 350px;
  display: flex;
  flex-direction: column;
  gap: 16px;

  @media ${tablet} {
    width: 100%;
  }

  .label {
    min-width: fit-content;
  }
`;

const ControlsGroup = styled(Box)`
  display: flex;
  flex-direction: column;
  gap: ${(props) => (props.gap ? props.gap : "6px")};
`;

const Preview = styled(Box)`
  width: 480px;
  flex-direction: row;
`;

const PortalIntegration = (props) => {
  const { t, setDocumentTitle } = props;

  setDocumentTitle(`Portal integration`);

  const scriptUrl = `${window.location.origin}/static/scripts/api.js`;

  const dataSortBy = [
    { key: "DateAndTime", label: "Last modified date", default: true },
    { key: "AZ", label: "Title" },
    { key: "Type", label: "Type" },
    { key: "Size", label: "Size" },
    { key: "DateAndTimeCreation", label: "Creation date" },
    { key: "Author", label: "Author" },
  ];

  const dataFilterType = [
    { key: 0, label: "None", default: true },
    { key: 1, label: "Files" },
    { key: 2, label: "Folders" },
    { key: 3, label: "Documents" },
    { key: 4, label: "Presentations" },
    { key: 5, label: "Spreadsheets" },
    { key: 7, label: "Images" },
    { key: 10, label: "Archives" },
    { key: 12, label: "Media" },
    { key: 8, label: "By user" },
    { key: 11, label: "By extension" },
  ];

  const dataSortOrder = [
    { key: "descending", label: "Descending", default: true },
    { key: "ascending", label: "Ascending" },
  ];

  const dataDisplayType = [
    { key: "row", label: "Row", default: true },
    { key: "table", label: "Table" },
    { key: "tile", label: "Tile" },
  ];

  const [config, setConfig] = useState({
    width: "100%",
    height: "400px",
    frameId: "ds-frame",
    showHeader: false,
    showTitle: true,
    showMenu: false,
    showFilter: false,
    showAction: false,
  });

  const [sortBy, setSortBy] = useState(dataSortBy[0]);
  const [sortOrder, setSortOrder] = useState(dataSortOrder[0]);
  const [filterType, setFilterType] = useState(dataFilterType[0]);
  const [displayType, setDisplayType] = useState(dataDisplayType[0]);
  const [withSubfolders, setWithSubfolders] = useState(false);

  const params = objectToGetParams(config);

  const frameId = config.frameId || "ds-frame";

  const destroyFrame = () => {
    DocSpace.destroyFrame();
  };

  const loadFrame = () => {
    const script = document.getElementById("integration");

    if (script) {
      destroyFrame();
      script.remove();
    }

    const params = objectToGetParams(config);

    loadScript(`${scriptUrl}${params}`, "integration");
  };

  const onChangeWidth = (e) => {
    setConfig((config) => {
      return { ...config, width: e.target.value };
    });
  };

  const onChangeHeight = (e) => {
    setConfig((config) => {
      return { ...config, height: e.target.value };
    });
  };

  const onChangeFolderId = (e) => {
    setConfig((config) => {
      return { ...config, folder: e.target.value };
    });
  };

  const onChangeFrameId = (e) => {
    setConfig((config) => {
      return { ...config, frameId: e.target.value };
    });
  };

  const onChangeWithSubfolders = (e) => {
    setConfig((config) => {
      return { ...config, withSubfolders: !withSubfolders };
    });

    setWithSubfolders(!withSubfolders);
  };

  const onChangeSortBy = (item) => {
    setConfig((config) => {
      return { ...config, sortby: item.key };
    });

    setSortBy(item);
  };

  const onChangeSortOrder = (item) => {
    setConfig((config) => {
      return { ...config, sortorder: item.key };
    });

    setSortOrder(item);
  };

  const onChangeFilterType = (item) => {
    setConfig((config) => {
      return { ...config, filterType: item.key };
    });

    setFilterType(item);
  };

  const onChangeDisplayType = (item) => {
    setConfig((config) => {
      return { ...config, viewAs: item.key };
    });

    setDisplayType(item);
  };

  const onChangeShowHeader = (e) => {
    setConfig((config) => {
      return { ...config, showHeader: !config.showHeader };
    });
  };

  const onChangeShowTitle = () => {
    setConfig((config) => {
      return { ...config, showTitle: !config.showTitle };
    });
  };

  const onChangeShowMenu = (e) => {
    setConfig((config) => {
      return { ...config, showMenu: !config.showMenu };
    });
  };

  const onChangeShowFilter = (e) => {
    setConfig((config) => {
      return { ...config, showFilter: !config.showFilter };
    });
  };

  const onChangeShowAction = (e) => {
    setConfig((config) => {
      return { ...config, showAction: !config.showAction };
    });
  };

  const onChangeCount = (e) => {
    setConfig((config) => {
      return { ...config, count: e.target.value };
    });
  };

  const onChangePage = (e) => {
    setConfig((config) => {
      return { ...config, page: e.target.value };
    });
  };

  const onChangeSearch = (e) => {
    setConfig((config) => {
      return { ...config, search: e.target.value };
    });
  };

  const onChangeAuthor = (e) => {
    setConfig((config) => {
      return { ...config, authorType: e.target.value };
    });
  };

  const codeBlock = `<div id="${frameId}">Fallback text</div>\n<script src="${scriptUrl}${params}"></script>`;

  return (
    <Box>
      <Description color="#657077">
        Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod
        tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim
        veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea
        commodo consequat. Duis aute irure dolor in reprehenderit in voluptate
        velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint
        occaecat cupidatat non proident, sunt in culpa qui officia deserunt
        mollit anim id est laborum.
      </Description>
      <Container>
        <Controls>
          <Category>Setting up the integration window</Category>
          <ControlsGroup>
            <Label className="label" text="Frame id" />
            <TextInput
              scale={true}
              onChange={onChangeFrameId}
              placeholder="Enter frame id"
              value={config.frameId}
            />
          </ControlsGroup>
          <ControlsGroup>
            <Label className="label" text="Width" />
            <TextInput
              scale={true}
              onChange={onChangeWidth}
              placeholder="Enter width"
              value={config.width}
            />
          </ControlsGroup>
          <ControlsGroup>
            <Label className="label" text="Height" />
            <TextInput
              scale={true}
              onChange={onChangeHeight}
              placeholder="Enter height"
              value={config.height}
            />
          </ControlsGroup>
          <ControlsGroup gap="10px">
            <Label className="label" text="Interface elements" />
            <Checkbox
              label="Header (only mobile devices)"
              onChange={onChangeShowHeader}
              isChecked={config.showHeader}
            />
            <Checkbox
              label="Menu"
              onChange={onChangeShowMenu}
              isChecked={config.showMenu}
            />
            <Checkbox
              label="Title"
              onChange={onChangeShowTitle}
              isChecked={config.showTitle}
            />
            <Checkbox
              label="Action button"
              onChange={onChangeShowAction}
              isChecked={config.showAction}
            />
            <Checkbox
              label="Filter"
              onChange={onChangeShowFilter}
              isChecked={config.showFilter}
            />
          </ControlsGroup>
          <Category marginTop="10px">Setting the display of data</Category>
          <ControlsGroup>
            <Label className="label" text="Folder id" />
            <TextInput
              scale={true}
              onChange={onChangeFolderId}
              placeholder="Enter id"
              value={config.folder}
            />
          </ControlsGroup>
          <ControlsGroup>
            <Label className="label" text="Filter" />
            <ComboBox
              onSelect={onChangeFilterType}
              options={dataFilterType}
              scaled={true}
              scaledOptions={true}
              selectedOption={filterType}
              displaySelectedOption
            />
          </ControlsGroup>
          <ControlsGroup>
            <Label className="label" text="Search term" />
            <TextInput
              scale={true}
              onChange={onChangeSearch}
              placeholder="Search"
              value={config.search}
            />
            <Checkbox
              label="With subfolders"
              onChange={onChangeWithSubfolders}
              isChecked={withSubfolders}
            />
          </ControlsGroup>
          <ControlsGroup>
            <Label className="label" text="Author" />
            <TextInput
              scale={true}
              onChange={onChangeAuthor}
              placeholder="Enter name"
              value={config.authorType}
            />
          </ControlsGroup>
          <ControlsGroup>
            <Label className="label" text="Items count" />
            <TextInput
              scale={true}
              onChange={onChangeCount}
              placeholder="Enter count"
              value={config.count}
            />
          </ControlsGroup>
          <ControlsGroup>
            <Label className="label" text="Page" />
            <TextInput
              scale={true}
              onChange={onChangePage}
              placeholder="Enter number page"
              value={config.page}
            />
          </ControlsGroup>
          <ControlsGroup>
            <Label className="label" text="Sort by" />
            <ComboBox
              onSelect={onChangeSortBy}
              options={dataSortBy}
              scaled={true}
              scaledOptions={true}
              selectedOption={sortBy}
              displaySelectedOption
              directionY="top"
            />
          </ControlsGroup>
          <ControlsGroup>
            <Label className="label" text="Sort order" />
            <ComboBox
              onSelect={onChangeSortOrder}
              options={dataSortOrder}
              scaled={true}
              scaledOptions={true}
              selectedOption={sortOrder}
              displaySelectedOption
              directionY="top"
            />
          </ControlsGroup>
        </Controls>
        <Preview>
          <Category>Preview</Category>
          <Frame>
            <Box id={frameId} className="frameStyle">
              <Loaders.Rectangle height="100%" />
            </Box>
          </Frame>
          <Category fontWeight="600" fontSize="13px" lineHeight="20px">
            Paste this code block on page
          </Category>
          <Textarea value={codeBlock} isReadOnly disableScroll />
        </Preview>
      </Container>
    </Box>
  );
};

export default inject(({ setup, auth }) => {
  const { settingsStore, setDocumentTitle } = auth;
  const { theme } = settingsStore;

  return {
    theme,
    setDocumentTitle,
  };
})(withTranslation(["Settings", "Common"])(observer(PortalIntegration)));
