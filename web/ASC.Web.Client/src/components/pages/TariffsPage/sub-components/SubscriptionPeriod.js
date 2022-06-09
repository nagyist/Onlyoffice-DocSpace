import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { useTranslation, Trans } from "react-i18next";
import Text from "@appserver/components/text";
import { inject, observer } from "mobx-react";

const StyledBody = styled.div`
  display: grid;
  grid-template-columns: repeat(3, 117px);
  border: 1px solid #d0d5da;
  margin-bottom: 24px;

  .tariff-active_period {
    border: 1px solid #2da7db;
    background: #2da7db;
    p {
      color: white;
    }
  }
  div {
    padding: 16px;
    p {
      text-align: center;
    }
  }
  div:nth-child(2n) {
    border-left: 1px solid #d0d5da;
    border-right: 1px solid #d0d5da;
  }
`;

const month = "0",
  year = "1",
  years = "2";
const SubscriptionPeriod = ({ t, setTariffPeriod, tariffPeriod }) => {
  useEffect(() => {
    setTariffPeriod(year);
  }, []);

  const onChangePeriod = (period) => {
    period !== tariffPeriod && setTariffPeriod(period);
  };

  const activeMonthPeriod = tariffPeriod === month;
  const activeYearPeriod = tariffPeriod === year;
  const activeYearsPeriod = tariffPeriod === years;

  return (
    <StyledBody>
      <div
        onClick={() => onChangePeriod(month)}
        className={`${activeMonthPeriod ? "tariff-active_period" : ""}`}
      >
        <Text>{t("MonthPeriod")}</Text>
      </div>
      <div
        onClick={() => onChangePeriod(year)}
        className={`${activeYearPeriod ? "tariff-active_period" : ""}`}
      >
        <Text>{t("YearPeriod")}</Text>
      </div>
      <div
        onClick={() => onChangePeriod(years)}
        className={`${activeYearsPeriod ? "tariff-active_period" : ""}`}
      >
        <Text>{t("ThreeYearPeriod")}</Text>
      </div>
    </StyledBody>
  );
};

export default inject(({ auth, payments }) => {
  const { tariffsInfo, setTariffPeriod, tariffPeriod } = payments;

  return { tariffsInfo, setTariffPeriod, tariffPeriod };
})(observer(SubscriptionPeriod));
