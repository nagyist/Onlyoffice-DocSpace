import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { useTranslation, Trans } from "react-i18next";
import Text from "@appserver/components/text";
import { inject, observer } from "mobx-react";
import Button from "@appserver/components/button";

const StyledBody = styled.div`
  .total-tariff_user {
    display: flex;
    align-items: center;
    justify-content: center;
    background: #f3f4f4;
    padding-top: 9px;
    padding-bottom: 9px;
    margin-bottom: 16px;
    p:first-child {
      margin-right: 8px;
    }
  }
  .total-tariff_total-price {
    margin-bottom: 16px;
    border-bottom: 1px solid #eceef1;
  }
  .total-tariff_sale-price {
    margin-bottom: 16px;
    ${(props) => !props.priceBenefit && "opacity: 0"};
  }
  button {
    width: 100%;
  }
`;

const month = "0",
  year = "1",
  years = "2";

const TotalTariffContainer = ({ t, usersCount, tariffsInfo, tariffPeriod }) => {
  useEffect(() => {}, []);
  const { availableTariffs } = tariffsInfo;

  const activeMonthPeriod = tariffPeriod === month;
  const activeYearPeriod = tariffPeriod === year;
  const activeYearsPeriod = tariffPeriod === years;

  const price = activeMonthPeriod
    ? availableTariffs[1].monthPrice
    : activeYearPeriod
    ? availableTariffs[1].yearPrice
    : availableTariffs[1].yearsPrice;

  const totalPrice =
    price * usersCount * (activeYearsPeriod ? 36 : activeYearPeriod ? 12 : 1);

  const priceWithoutBenefit =
    availableTariffs[1].monthPrice *
    usersCount *
    (activeYearsPeriod ? 36 : activeYearPeriod ? 12 : 0);

  const priceBenefit = priceWithoutBenefit
    ? priceWithoutBenefit - totalPrice
    : 0;

  return (
    <StyledBody priceBenefit={priceBenefit}>
      <div className="total-tariff_user">
        <Text fontSize="16px" textAlign="center" fontWeight={600}>
          {activeMonthPeriod
            ? availableTariffs[1].monthPrice
            : activeYearPeriod
            ? availableTariffs[1].yearPrice
            : availableTariffs[1].yearsPrice}
        </Text>
        <Text
          fontSize="11px"
          textAlign="center"
          fontWeight={600}
          color={"#A3A9AE"}
        >
          {t("PerUserMonth")}
        </Text>
      </div>
      <Text
        fontSize="48px"
        isBold
        textAlign={"center"}
        className="total-tariff_total-price"
      >
        {totalPrice}
      </Text>

      <Text
        fontSize="14px"
        isBold
        textAlign={"center"}
        className="total-tariff_sale-price"
      >
        <Trans t={t} i18nKey={"SavedMoney"} ns="Payments">
          {{ price: priceBenefit }}
        </Trans>
      </Text>

      <Button label={t("Common:ContinueButton")} size={"medium"} primary />
    </StyledBody>
  );
};

export default inject(({ auth, payments }) => {
  const { tariffsInfo, tariffPeriod } = payments;

  return { tariffsInfo, tariffPeriod };
})(observer(TotalTariffContainer));
