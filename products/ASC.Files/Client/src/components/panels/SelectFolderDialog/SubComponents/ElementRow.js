import ItemIcon from "../../../ItemIcon";
import React from "react";
import Text from "@appserver/components/text";
import { StyledWrapper } from "../StyledSelectFolderDialog";
const ElementRow = (props) => {
  const { item, icon, onClick } = props;
  const { id, fileExst } = item;
  const element = <ItemIcon id={id} icon={icon} fileExst={fileExst} />;

  const onRowClick = () => {
    onClick && onClick(id);
  };
  return (
    <StyledWrapper>
      <div onClick={onRowClick} className="element-row_clicked-area"></div>
      <div className="element-row_icon">{element}</div>
      <div className="element-row_text">
        <Text fontSize="14px" fontWeight={600}>
          {item.title}
        </Text>
      </div>
    </StyledWrapper>
  );
};

export default ElementRow;
