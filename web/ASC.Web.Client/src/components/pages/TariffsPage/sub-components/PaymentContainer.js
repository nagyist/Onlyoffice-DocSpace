import React, { useState } from "react";
import styled from "styled-components";
import { useTranslation, Trans } from "react-i18next";
import Text from "@appserver/components/text";
import { inject, observer } from "mobx-react";

import SelectUsersCountContainer from "./SelectUsersCountContainer";
import SubscriptionPeriod from "./SubscriptionPeriod";
import TotalTariffContainer from "./TotalTariffContainer";

const StyledBody = styled.div`
  .tariff-benefits_text {
    margin-bottom: 20px;
  }
  .tariff-benefits {
    display: flex;
    margin-bottom: 16px;
    align-items: baseline;

    p {
      margin-left: 8px;
    }
  }
`;

const step = "50",
  minUsersCount = 3,
  maxUsersCount = 1000;

const PaymentContainer = ({ t }) => {
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
      <SubscriptionPeriod t={t} />
      <SelectUsersCountContainer
        maxUsersCount={maxUsersCount}
        step={step}
        usersCount={usersCount}
        onMinusClick={onMinusClick}
        onPlusClick={onPlusClick}
        onSliderChange={onSliderChange}
      />
      <TotalTariffContainer t={t} usersCount={usersCount} />
    </StyledBody>
  );
};

export default inject(({ auth, payments }) => {
  const { tariffsInfo } = payments;

  return { tariffsInfo };
})(observer(PaymentContainer));
