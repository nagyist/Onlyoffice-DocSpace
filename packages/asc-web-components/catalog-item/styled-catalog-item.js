import styled, { css } from "styled-components";

import Base from "../themes/base";

import { tablet } from "../utils/device";
import { isMobile } from "react-device-detect";

import Text from "@appserver/components/text";

// width, height
const badgeWithoutText = css`
  position: absolute;

  top: ${(props) => props.theme.catalogItem.badgeWithoutText.position};
  right: ${(props) => props.theme.catalogItem.badgeWithoutText.position};

  border-radius: 1000px;

  background-color: ${(props) =>
    props.theme.catalogItem.badgeWithoutText.backgroundColor};

  width: ${(props) => props.theme.catalogItem.badgeWithoutText.size} !important;
  min-width: ${(props) =>
    props.theme.catalogItem.badgeWithoutText.size} !important;
  height: ${(props) =>
    props.theme.catalogItem.badgeWithoutText.size} !important;
  min-height: ${(props) =>
    props.theme.catalogItem.badgeWithoutText.size} !important;

  margin: 0 !important;
`;

const StyledCatalogItemBadgeWrapper = styled.div`
  z-index: 3;

  margin-left: ${(props) => props.theme.catalogItem.badgeWrapper.marginLeft};
  margin-right: ${(props) => props.theme.catalogItem.badgeWrapper.marginRight};

  @media ${tablet} {
    display: flex;
    align-items: center;
    justify-content: center;

    width: ${(props) => props.theme.catalogItem.badgeWrapper.tablet.width};
    min-width: ${(props) => props.theme.catalogItem.badgeWrapper.tablet.width};
    height: ${(props) => props.theme.catalogItem.badgeWrapper.tablet.height};
    margin-right: ${(props) =>
      props.theme.catalogItem.badgeWrapper.tablet.marginRight};
  }

  ${isMobile &&
  css`
    display: flex;
    align-items: center;
    justify-content: center;

    width: ${(props) => props.theme.catalogItem.badgeWrapper.tablet.width};
    min-width: ${(props) => props.theme.catalogItem.badgeWrapper.tablet.width};
    height: ${(props) => props.theme.catalogItem.badgeWrapper.tablet.height};
    margin-right: ${(props) =>
      props.theme.catalogItem.badgeWrapper.tablet.marginRight};
  `}

  ${(props) => !props.showText && badgeWithoutText}

  .catalog-item__badge {
    display: flex;
    align-items: center;
    justify-content: center;

    width: ${(props) => props.theme.catalogItem.badgeWrapper.size};
    min-width: ${(props) => props.theme.catalogItem.badgeWrapper.size};
    height: ${(props) => props.theme.catalogItem.badgeWrapper.size};
    min-height: ${(props) => props.theme.catalogItem.badgeWrapper.size};

    div {
      display: flex;
      align-items: center;
      justify-content: center;

      width: 6px;
      height: ${(props) => props.theme.catalogItem.badgeWrapper.size};

      p {
        display: flex;
        align-items: center;
        justify-content: center;

        line-height: 16px;
      }
    }
  }
`;

StyledCatalogItemBadgeWrapper.defaultProps = { theme: Base };

const StyledCatalogItemInitialText = styled(Text)`
  position: absolute;
  top: 2px;
  left: 0;
  text-align: center;
  width: ${(props) => props.theme.catalogItem.initialText.width};
  line-height: ${(props) => props.theme.catalogItem.initialText.lineHeight};
  max-height: ${(props) => props.theme.catalogItem.initialText.lineHeight};
  color: ${(props) => props.theme.catalogItem.initialText.color};
  font-size: ${(props) => props.theme.catalogItem.initialText.fontSize};
  font-weight: ${(props) => props.theme.catalogItem.initialText.fontWeight};
  pointer-events: none;

  @media ${tablet} {
    width: ${(props) => props.theme.catalogItem.initialText.tablet.width};
    line-height: ${(props) =>
      props.theme.catalogItem.initialText.tablet.lineHeight};
    font-size: ${(props) =>
      props.theme.catalogItem.initialText.tablet.fontSize};
  }

  ${isMobile &&
  css`
    width: ${(props) => props.theme.catalogItem.initialText.tablet.width};
    line-height: ${(props) =>
      props.theme.catalogItem.initialText.tablet.lineHeight};
    font-size: ${(props) =>
      props.theme.catalogItem.initialText.tablet.fontSize};
  `}
`;

StyledCatalogItemInitialText.defaultProps = { theme: Base };

const StyledCatalogItemText = styled(Text)`
  width: ${(props) => props.theme.catalogItem.text.width};

  margin-left: ${(props) => props.theme.catalogItem.text.marginLeft};

  line-height: ${(props) => props.theme.catalogItem.text.lineHeight};

  z-index: 1;

  display: flex;
  align-items: center;

  pointer-events: none;

  color: ${(props) => props.theme.catalogItem.text.color};
  font-size: ${(props) => props.theme.catalogItem.text.fontSize};
  font-weight: ${(props) => props.theme.catalogItem.text.fontWeight};
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;

  @media ${tablet} {
    margin-left: ${(props) => props.theme.catalogItem.text.tablet.marginLeft};
    line-height: ${(props) => props.theme.catalogItem.text.tablet.lineHeight};
    font-size: ${(props) => props.theme.catalogItem.text.tablet.fontSize};
    font-weight: ${(props) => props.theme.catalogItem.text.tablet.fontWeight};
  }

  ${isMobile &&
  css`
    margin-left: ${(props) => props.theme.catalogItem.text.tablet.marginLeft};
    line-height: ${(props) => props.theme.catalogItem.text.tablet.lineHeight};
    font-size: ${(props) => props.theme.catalogItem.text.tablet.fontSize};
    font-weight: ${(props) => props.theme.catalogItem.text.tablet.fontWeight};
  `}
`;

StyledCatalogItemText.defaultProps = { theme: Base };

const StyledCatalogItemImg = styled.div`
  position: relative;

  display: flex;
  align-items: center;
  justify-content: center;

  z-index: 1;

  pointer-events: none;

  height: ${(props) => props.theme.catalogItem.img.svg.height};

  .icon {
    display: flex;
    align-items: center;
    justify-content: center;
    ${(props) =>
      props.dialogTree &&
      `
        border: 8px solid #ECEEF1;
        background: #ECEEF1;
        border-radius: 50%;
    `}
  }

  svg {
    width: ${(props) => props.theme.catalogItem.img.svg.width};
    height: ${(props) => props.theme.catalogItem.img.svg.height};
    path {
      fill: ${(props) => props.theme.catalogItem.img.svg.fill} !important;
    }
  }

  @media ${tablet} {
    height: ${(props) => props.theme.catalogItem.img.svg.tablet.height};
    svg {
      width: ${(props) => props.theme.catalogItem.img.svg.tablet.width};
      height: ${(props) => props.theme.catalogItem.img.svg.tablet.height};
    }
  }

  ${isMobile &&
  css`
    height: ${(props) => props.theme.catalogItem.img.svg.tablet.height};
    svg {
      width: ${(props) => props.theme.catalogItem.img.svg.tablet.width};
      height: ${(props) => props.theme.catalogItem.img.svg.tablet.height};
    }
  `}
`;

StyledCatalogItemImg.defaultProps = { theme: Base };

const draggingSiblingCss = css`
  background: ${(props) => props.theme.dragAndDrop.background} !important;

  &:hover {
    background: ${(props) =>
      props.theme.dragAndDrop.acceptBackground} !important;
  }
`;

const StyledCatalogItemSibling = styled.div`
  position: absolute;
  top: 0;
  left: 0;

  width: 100%;
  height: 100%;

  min-height: ${(props) => props.theme.catalogItem.container.height};
  max-height: ${(props) => props.theme.catalogItem.container.height};

  background-color: ${(props) =>
    !props.dialogTree &&
    props.isActive &&
    props.theme.catalogItem.sibling.active.background};

  &:hover {
    background-color: ${(props) =>
      props.theme.catalogItem.sibling.hover.background};
  }

  @media ${tablet} {
    min-height: ${(props) => props.theme.catalogItem.container.tablet.height};
    max-height: ${(props) => props.theme.catalogItem.container.tablet.height};
  }

  ${isMobile &&
  css`
    min-height: ${(props) => props.theme.catalogItem.container.tablet.height};
    max-height: ${(props) => props.theme.catalogItem.container.tablet.height};
  `}

  ${(props) => props.isDragging && draggingSiblingCss}
`;

StyledCatalogItemSibling.defaultProps = { theme: Base };

const StyledCatalogItemContainer = styled.div`
  display: flex;
  justify-content: ${(props) => (props.showText ? "space-between" : "center")};
  align-items: center;

  min-width: ${(props) => props.theme.catalogItem.container.width};
  min-height: ${(props) => props.theme.catalogItem.container.height};
  max-height: ${(props) => props.theme.catalogItem.container.height};

  position: relative;
  box-sizing: border-box;

  padding: ${(props) =>
    !props.dialogTree &&
    props.showText &&
    props.theme.catalogItem.container.padding};

  margin-bottom: ${(props) =>
    props.isEndOfBlock && props.theme.catalogItem.container.marginBottom};

  ${(props) => props.dialogTree && `margin-bottom: 16px;`}

  cursor: pointer;

  @media ${tablet} {
    min-height: ${(props) => props.theme.catalogItem.container.tablet.height};
    max-height: ${(props) => props.theme.catalogItem.container.tablet.height};

    padding: ${(props) =>
      props.showText && props.theme.catalogItem.container.tablet.padding};
    margin-bottom: ${(props) =>
      props.isEndOfBlock &&
      props.theme.catalogItem.container.tablet.marginBottom};
  }

  ${isMobile &&
  css`
    min-height: ${(props) => props.theme.catalogItem.container.tablet.height};
    max-height: ${(props) => props.theme.catalogItem.container.tablet.height};

    padding: ${(props) =>
      props.showText && props.theme.catalogItem.container.tablet.padding};
    margin-bottom: ${(props) =>
      props.isEndOfBlock &&
      props.theme.catalogItem.container.tablet.marginBottom};
  `}
`;
StyledCatalogItemContainer.defaultProps = { theme: Base };

export {
  StyledCatalogItemContainer,
  StyledCatalogItemImg,
  StyledCatalogItemInitialText,
  StyledCatalogItemText,
  StyledCatalogItemSibling,
  StyledCatalogItemBadgeWrapper,
};