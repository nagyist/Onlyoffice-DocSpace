import React from "react";
import Text from "@appserver/components/text";
import RadioButton from "@appserver/components/radio-button";
import ItemIcon from "../../ItemIcon";
import { StyledWrapper } from "./StyledSelectionPanel";
const ElementRow = (props) => {
  const { item, icon, onFolderClick, isChecked, onSelectFile, index } = props;
  const { id, fileExst } = item;
  const element = <ItemIcon id={id} icon={icon} fileExst={fileExst} />;

  const onFolderRowClick = () => {
    onFolderClick && onFolderClick(id);
  };

  const onFileClick = () => {
    onSelectFile && onSelectFile(item);
  };

  const isFile = !!item.fileExst;
  return (
    <StyledWrapper>
      {!isFile && (
        <div
          onClick={onFolderRowClick}
          className="element-row_clicked-area"
        ></div>
      )}
      <div className="element-row_icon">{element}</div>
      <div className="element-row_text">
        <Text fontSize="14px" fontWeight={600}>
          {item.title}
        </Text>
      </div>
      <div className="element-row_checkbox">
        {isFile && (
          <RadioButton
            //theme={theme}
            fontSize="13px"
            fontWeight="400"
            name={`${index}`}
            label=""
            isChecked={isChecked}
            onClick={onFileClick}
            value=""
            className="select-file-dialog_checked"
          />
        )}
      </div>
    </StyledWrapper>
  );
};

export default ElementRow;
