import React, { useState } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { useTranslation, Trans } from "react-i18next";
import PropTypes from "prop-types";
import { inject, observer } from "mobx-react";
import CurrentTariffContainer from "./sub-components/CurrentTariffContainer";
import AdvantagesContainer from "./sub-components/AdvantagesContainer";
import Text from "@appserver/components/text";
import ContactContainer from "./sub-components/ContactContainer";
import BusinessTariffPage from "./BusinessTariffPage";
import IconButton from "@appserver/components/icon-button";

const StyledBody = styled.div`
  margin-left: auto;
  margin-right: auto;
  max-width: 960px;

  margin-top: 48px;

  .current-tariff {
    margin: 0 auto;
    margin-top: 40px;
  }

  .tariff-title {
    display: flex;
    p {
      margin-left: 16px;
    }
  }
  @media (max-height: 1042px) {
    // margin-top: 0px;
  }
`;

const ExpiredTrialPage = ({ organizationName }) => {
  const { t } = useTranslation("Payments");

  const [businessDescription, setBusinessDescription] = useState(false);

  const onOpenBusinessDescription = () => {
    setBusinessDescription(true);
  };

  const hideBusinessDescription = () => {
    setBusinessDescription(false);
  };
  return (
    <StyledBody>
      {businessDescription ? (
        <>
          <div className="tariff-title">
            <IconButton
              iconName="/static/images/arrow.path.react.svg"
              size="17"
              color="#A3A9AE"
              hoverColor="#657077"
              onClick={hideBusinessDescription}
            />
            <Text fontSize={"18px"} isBold>
              {"Business tariff"}
            </Text>
          </div>
          <BusinessTariffPage></BusinessTariffPage>
        </>
      ) : (
        <>
          <Text fontSize="23px" isBold textAlign="center">
            <Trans t={t} i18nKey="HeaderExpiredTrialLicense" ns="Payments">
              {{ organizationName }}
            </Trans>
          </Text>
          <CurrentTariffContainer />
          <AdvantagesContainer
            t={t}
            onOpenBusinessDescription={onOpenBusinessDescription}
          />
        </>
      )}

      <ContactContainer />
    </StyledBody>
  );
};

export default inject(({ auth }) => {
  const { organizationName } = auth.settingsStore;

  return { organizationName };
})(withRouter(observer(ExpiredTrialPage)));
