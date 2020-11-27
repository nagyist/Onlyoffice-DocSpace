import React from "react";
import PropTypes from "prop-types";
import { Box, utils } from "asc-web-components";
import styled, { css } from "styled-components";
import RecoverAccess from "./recover-access-container";
import { isSafari } from "react-device-detect";
import { connect } from "react-redux";
import { useTranslation } from "react-i18next";
import { LayoutContextConsumer } from "../../Layout/context";

const { tablet } = utils.device;

const backgroundColor = "#0F4071";

const Header = styled.header`
  align-items: center;
  background-color: ${backgroundColor};
  display: flex;
  width: calc(100vw - 64px);
  height: 56px;
  justify-content: center;
  padding: 0 32px;

  @media ${tablet} {
    transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
    -moz-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
    -ms-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
    -webkit-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
    -o-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);

    position: inherit;
    top: ${(props) => (props.valueTop ? "0" : "-56px")};
  }

  .header-items-wrapper {
    width: 960px;

    @media (max-width: 768px) {
      width: 475px;
    }
    @media (max-width: 375px) {
      width: 311px;
    }
  }

  .header-logo-wrapper {
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  }

  .header-logo-min_icon {
    display: none;
    cursor: pointer;
    width: 24px;
    height: 24px;
  }

  .header-logo-icon {
    width: 146px;
    height: 24px;
    position: relative;
    padding: 4px 20px 0 6px;
    cursor: pointer;
  }
`;

const HeaderUnAuth = ({
  enableAdmMess,
  wizardToken,
  isAuthenticated,
  isLoaded,
}) => {
  const { t } = useTranslation();

  return (
    <LayoutContextConsumer>
      {(value) => (
        <Header isLoaded={isLoaded} valueTop={value.isVisible}>
          <Box
            displayProp="flex"
            justifyContent="space-between"
            alignItems="center"
            className="header-items-wrapper"
          >
            {!isAuthenticated && isLoaded ? (
              <div>
                <a className="header-logo-wrapper" href="/">
                  <img
                    className="header-logo-min_icon"
                    src="images/nav.logo.react.svg"
                  />
                  <img
                    className="header-logo-icon"
                    src="images/nav.logo.opened.react.svg"
                  />
                </a>
              </div>
            ) : (
              <></>
            )}

            <div>
              {enableAdmMess && !wizardToken && <RecoverAccess t={t} />}
            </div>
          </Box>
        </Header>
      )}
    </LayoutContextConsumer>
  );
};

HeaderUnAuth.displayName = "Header";

HeaderUnAuth.propTypes = {
  enableAdmMess: PropTypes.bool,
  wizardToken: PropTypes.string,
  isAuthenticated: PropTypes.bool,
  isLoaded: PropTypes.bool,
};

const mapStateToProps = (state) => {
  const { isAuthenticated, isLoaded, settings } = state.auth;
  const { enableAdmMess, wizardToken } = settings;

  return {
    enableAdmMess,
    wizardToken,
    isAuthenticated,
    isLoaded,
  };
};

export default connect(mapStateToProps)(HeaderUnAuth);
