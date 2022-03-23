import ItemIcon from "../../ItemIcon";
import React from "react";
import styled, { css } from "styled-components";
import Text from "@appserver/components/text";
const StyledWrapper = styled.div`
  display: grid;
  grid-template-columns: 32px 1fr;
  grid-gap: 8px;
  margin-bottom: 20px;
  position: relative;
  .element-row_text {
    margin: auto 0;
  }
  .element-row_clicked-area {
    position: absolute;
    top: 0;
    left: 0;

    width: 100%;
    height: 100%;
    cursor: pointer;
  }
`;

const ElementRow = (props) => {
  const { item, icon, onClick } = props;
  const { id, fileExst } = item;
  const element = <ItemIcon id={id} icon={icon} fileExst={fileExst} />;
  console.log("element Row");
  const onRowClick = () => {
    onClick && onClick(id);
  };
  return (
    <StyledWrapper>
      <div onClick={onRowClick} class="element-row_clicked-area"></div>
      <div>{element}</div>
      <div className="element-row_text">
        <Text fontSize="14px" fontWeight={600}>
          {item.title}
        </Text>
      </div>
    </StyledWrapper>
  );
};

export default ElementRow;
