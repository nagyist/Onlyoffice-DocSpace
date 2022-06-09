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

const StyledBody = styled.div`
  margin-left: auto;
  margin-right: auto;
  max-width: 960px;

  margin-top: 48px;

  .current-tariff {
    margin: 0 auto;
    margin-top: 40px;
  }

  @media (max-height: 1042px) {
    // margin-top: 0px;
  }
`;

const step = "50",
  minUsersCount = 3,
  maxUsersCount = 1000;

const BusinessTariffPage = ({}) => {
  const { t } = useTranslation("Payments");

  const [usersCount, setUsersCount] = useState(minUsersCount);

  const onSliderChange = (e) => {
    const count = parseFloat(e.target.value);
    count > minUsersCount ? setUsersCount(count) : setUsersCount(minUsersCount);
  };

  const onPlusClick = () => {
    usersCount < maxUsersCount && setUsersCount(usersCount + 1);
  };

  const onMinusClick = () => {
    usersCount > minUsersCount && setUsersCount(usersCount - 1);
  };

  return (
    <StyledBody>
      <SubscriptionTitleContainer />
      <BenefitsContainer />
      <SelectUsersCountContainer
        maxUsersCount={maxUsersCount}
        step={step}
        usersCount={usersCount}
        onMinusClick={onMinusClick}
        onPlusClick={onPlusClick}
        onSliderChange={onSliderChange}
      />

      <ContactContainer />
    </StyledBody>
  );
};

export default inject(({ auth }) => {
  const { quota } = auth;

  return { quota };
})(withRouter(observer(BusinessTariffPage)));
