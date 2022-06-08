import React from "react";
import styled from "styled-components";
import PropTypes from "prop-types";
import { inject, observer } from "mobx-react";
import Text from "@appserver/components/text";
import CheckMarkIcon from "../../../../../public/images/check.mark.react.svg";
import Button from "@appserver/components/button";

const StyledBody = styled.div`
  margin-top: 40px;
  margin-left: auto;
  margin-right: auto;
  padding: 0 56px;
  .available-tariffs {
    display: grid;
    grid-template-columns: repeat(2, minmax(100px, 400px));
    grid-gap: 48px;
    margin-bottom: 40px;
  }
  .tariff-name {
    margin-bottom: 32px;
  }
  .tariff-feature {
    display: flex;
    align-items: baseline;
    margin-bottom: 16px;
    p {
      margin-left: 10px;
    }
    svg {
      min-width: 16px;
      max-width: 16px;
    }
  }
`;

const StyledTariffBody = styled.div`
  border: 1px solid #d0d5da;
  border-radius: 12px;

  margin-top: 76px;
  position: relative;

  .popular-tariff {
    height: 40px;
    background: #2da7db;
    border-top-left-radius: 8px;
    border-top-right-radius: 8px;
    margin-left: -1px;
    margin-top: -1px;
    width: calc(100% + 2px);
  }
  .tariff-body {
    padding: 32px;
  }
  .tariff-price {
    margin-bottom: 32px;
  }

  .tariff-button {
    position: absolute;
    bottom: 32px;
    width: calc(100% - 64px);
  }
  .tariff-features {
    padding-bottom: 76px;
  }
`;
const AdvantagesContainer = ({ t, tariffsInfo }) => {
  const { availableTariffs } = tariffsInfo;

  return (
    <StyledBody>
      <Text fontSize="16px" isBold textAlign="center">
        {t("SuitablePlan")}
      </Text>

      <div className="available-tariffs">
        <StyledTariffBody>
          <div className="tariff-body">
            <Text
              fontSize="23px"
              isBold
              textAlign="center"
              className="tariff-name"
            >
              {availableTariffs[0].name}
            </Text>
            <div className="tariff-price">
              <Text fontSize="48px" textAlign="center" fontWeight={600}>
                {availableTariffs[0].price}
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
            <div className="tariff-features">
              {availableTariffs[0].features.map((item, index) => {
                return (
                  <div className="tariff-feature" key={index}>
                    <CheckMarkIcon />
                    <Text fontWeight={600}>{item}</Text>
                  </div>
                );
              })}
            </div>
            <Button
              className={"tariff-button"}
              label={t("Common:ContinueButton")}
              size={"medium"}
            />
          </div>
        </StyledTariffBody>

        <StyledTariffBody
          style={{ marginTop: "40px", border: "2px solid #2DA7DB" }}
        >
          <div className="popular-tariff"></div>
          <div className="tariff-body">
            <Text
              fontSize="23px"
              isBold
              textAlign="center"
              className="tariff-name"
            >
              {availableTariffs[1].name}
            </Text>
            <div className="tariff-price">
              <Text fontSize="48px" textAlign="center" fontWeight={600}>
                {availableTariffs[1].price}
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
            <div className="tariff-features">
              {availableTariffs[1].features.map((item, index) => {
                return (
                  <div className="tariff-feature" key={index}>
                    <CheckMarkIcon />
                    <Text fontWeight={600}>{item}</Text>
                  </div>
                );
              })}
            </div>
            <Button
              className={"tariff-button"}
              label={t("Common:ContinueButton")}
              size={"medium"}
              primary
            />
          </div>
        </StyledTariffBody>
      </div>
    </StyledBody>
  );
};

AdvantagesContainer.propTypes = {
  isLoaded: PropTypes.bool,
};

export default inject(({ auth, payments }) => {
  const { setQuota, quota } = auth;
  const { organizationName } = auth.settingsStore;
  const { tariffsInfo } = payments;

  return { setQuota, quota, organizationName, tariffsInfo };
})(observer(AdvantagesContainer));
