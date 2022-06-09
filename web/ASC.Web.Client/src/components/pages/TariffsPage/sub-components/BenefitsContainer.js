import React from "react";
import styled from "styled-components";
import { useTranslation, Trans } from "react-i18next";
import Text from "@appserver/components/text";
import { inject, observer } from "mobx-react";
import CheckMarkIcon from "../../../../../public/images/check.mark.react.svg";

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

const BenefitsContainer = ({ tariffsInfo }) => {
  const { t } = useTranslation("Payments");

  return (
    <StyledBody>
      <Text fontSize={"14"} fontWeight={"600"} className="tariff-benefits_text">
        {t("Benefits")}
      </Text>
      {availableTariffs[1].features.map((item, index) => {
        return (
          <div className="tariff-benefits" key={index}>
            <CheckMarkIcon />
            <Text>{item}</Text>
          </div>
        );
      })}
    </StyledBody>
  );
};

export default inject(({ auth, payments }) => {
  const { tariffsInfo } = payments;

  return { tariffsInfo };
})(observer(BenefitsContainer));
