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
import Slider from "@appserver/components/slider";
import PlusIcon from "../../../../public/images/plus.react.svg";
import MinusIcon from "../../../../public/images/minus.react.svg";
import SelectUsersCountContainer from "./sub-components/SelectUsersCountContainer";
import SubscriptionTitleContainer from "./sub-components/SubscriptionTitleContainer";
import BenefitsContainer from "./sub-components/BenefitsContainer";
import PaymentContainer from "./sub-components/PaymentContainer";

const StyledBody = styled.div`
  ${(props) =>
    props.trialExpired &&
    css`
      margin-left: auto;
      margin-right: auto;
    `}

  max-width: 889px;
  margin-bottom: 28px;
  margin-top: ${(props) => (props.trialExpired ? "48px" : "28px")};

  display: grid;
  grid-template-areas: "subscription subscription" "benefits payment";
  grid-template-columns: minmax(100px, 490px) minmax(100px, 400px);
  grid-template-rows: auto 1fr;

  border-top-left-radius: 12px;
  border-top-right-radius: 12px;

  border: 1px solid #d0d5da;

  .current-tariff {
    margin: 0 auto;
    margin-top: 40px;
  }

  .tariff-subscription_container {
    grid-area: subscription;
    padding: 24px;
    border-bottom: 1px solid #d0d5da;
  }
  .tariff-benefits_container {
    grid-area: benefits;
    padding: 24px;
    border-right: 1px solid #d0d5da;
  }
  .tariff-payment_container {
    grid-area: payment;
    padding: 24px;
  }

  @media (max-height: 1042px) {
    // margin-top: 0px;
  }
`;

const BusinessTariffPage = ({}) => {
  const { t } = useTranslation("Payments");

  return (
    <StyledBody>
      <div className={"tariff-subscription_container"}>
        <SubscriptionTitleContainer />
      </div>
      <div className={"tariff-benefits_container"}>
        <BenefitsContainer />
      </div>
      <div className={"tariff-payment_container"}>
        <PaymentContainer t={t} />
      </div>
    </StyledBody>
  );
};

export default inject(({ auth }) => {
  const { quota } = auth;

  return { quota };
})(withRouter(observer(BusinessTariffPage)));
