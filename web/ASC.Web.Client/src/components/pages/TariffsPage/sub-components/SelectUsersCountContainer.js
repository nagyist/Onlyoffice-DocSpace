import React from "react";
import styled from "styled-components";
import { useTranslation } from "react-i18next";
import Text from "@appserver/components/text";
import Slider from "@appserver/components/slider";
import PlusIcon from "../../../../../public/images/plus.react.svg";
import MinusIcon from "../../../../../public/images/minus.react.svg";

const StyledBody = styled.div`
  max-width: 352px;

  .tariff-users {
    display: flex;
    align-items: baseline;
    margin: 0 auto;
    width: max-content;

    .tariff-score {
      cursor: pointer;
    }
    .tariff-users_count {
      margin-left: 20px;
      margin-right: 20px;
    }
  }
  .tariff-users_text {
    margin-bottom: 12px;
    text-align: center;
  }
`;

const SelectUsersCountContainer = ({
  maxUsersCount,
  step,
  usersCount,
  onMinusClick,
  onPlusClick,
  onSliderChange,
}) => {
  const { t } = useTranslation("Payments");

  return (
    <StyledBody>
      <Text noSelect fontWeight={600} className="tariff-users_text">
        {t("SelectNumber")}
      </Text>
      <div className="tariff-users">
        <MinusIcon onClick={onMinusClick} className="tariff-score" />
        <Text noSelect fontSize={"44px"} className="tariff-users_count">
          {usersCount}
        </Text>
        <PlusIcon onClick={onPlusClick} className="tariff-score" />
      </div>

      <Slider
        type="range"
        min={"0"}
        max={maxUsersCount.toString()}
        step={step}
        withPouring
        value={usersCount}
        onChange={onSliderChange}
        colorPouring={"#20D21F"}
      />
    </StyledBody>
  );
};

export default SelectUsersCountContainer;
