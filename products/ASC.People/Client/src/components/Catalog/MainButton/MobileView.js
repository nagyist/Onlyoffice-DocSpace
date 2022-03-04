import React from "react";
import styled, { css } from "styled-components";
import { isMobileOnly } from "react-device-detect";

import { mobile } from "@appserver/components/utils/device";

import MainButtonMobile from "@appserver/components/main-button-mobile";

const StyledMainButtonMobile = styled(MainButtonMobile)`
  position: fixed;
  z-index: 1000;
  right: 24px;
  bottom: 24px;

  @media ${mobile} {
    right: 16px;
    bottom: 16px;
  }

  ${isMobileOnly &&
  css`
    right: 16px;
    bottom: 16px;
  `}
`;

const MobileView = ({ actionOptions, sectionWidth }) => {
  const [isOpenButton, setIsOpenButton] = React.useState(false);

  const openButtonToggler = () => {
    setIsOpenButton((prevState) => !prevState);
  };

  return (
    <StyledMainButtonMobile
      sectionWidth={sectionWidth}
      actionOptions={actionOptions}
      isOpenButton={isOpenButton}
      onClose={openButtonToggler}
      title="Upload"
      percent={0}
      withButton={false}
    />
  );
};

export default React.memo(MobileView);