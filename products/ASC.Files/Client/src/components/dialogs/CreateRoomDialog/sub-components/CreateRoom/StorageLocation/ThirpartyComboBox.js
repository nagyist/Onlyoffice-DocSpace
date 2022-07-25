import React, { useState, useRef } from "react";
import styled from "styled-components";
import { ReactSVG } from "react-svg";

import { thirparties } from "../../../data";

import Text from "@appserver/components/text";
import Button from "@appserver/components/button";
import DropDownItem from "@appserver/components/drop-down-item";
import {
  StyledDropDown,
  StyledDropDownWrapper,
} from "../../common/StyledDropdown";

const StyledStorageLocation = styled.div`
  display: flex;
  flex-direction: column;

  .set_room_params-thirdparty {
    display: flex;
    flex-direction: row;
    gap: 8px;
    &-combobox {
      width: 100%;
      display: flex;
      flex-direction: row;
      justify-content: space-between;
      padding: 5px 7px;
      background: #ffffff;
      border-radius: 3px;
      max-height: 32px;

      border: ${(props) => `1px solid ${props.isOpen ? "#2DA7DB" : "#d0d5da"}`};
      &:hover {
        border: ${(props) =>
          `1px solid ${props.isOpen ? "#2DA7DB" : "#a3a9ae"}`};
      }

      &-text {
        font-weight: 400;
        font-size: 13px;
        line-height: 20px;
        color: ${(props) => (props.isGrayLabel ? "#a3a9ae" : "#333333")};
      }

      &-expander {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 6.35px;
        svg {
          transform: ${(props) =>
            props.isOpen ? "rotate(180deg)" : "rotate(0)"};
          width: 6.35px;
          height: auto;
          path {
            fill: #a3a9ae;
          }
        }
      }
    }

    &-checkbox {
      margin-top: 8px;
      .checkbox {
        margin-right: 8px;
      }
      .checkbox-text {
        font-weight: 400;
        font-size: 13px;
        line-height: 20px;
      }
    }
  }
`;

const ThirpartyComboBox = ({
  t,
  roomParams,
  setRoomParams,
  setIsScrollLocked,
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const toggleIsOpen = () => {
    if (isOpen) setIsScrollLocked(false);
    else setIsScrollLocked(true);
    setIsOpen(!isOpen);
  };

  const [isGrayLabel, setIsGrayLabel] = useState(true);

  const setStorageLocaiton = (thirparty) => {
    setIsGrayLabel(false);
    setRoomParams({ ...roomParams, storageLocation: thirparty });
    toggleIsOpen();
    setIsScrollLocked(false);
  };

  const dropdownRef = useRef(null);
  return (
    <StyledStorageLocation isGrayLabel={isGrayLabel} isOpen={isOpen}>
      <div className="set_room_params-thirdparty">
        <div
          className="set_room_params-thirdparty-combobox"
          onClick={toggleIsOpen}
        >
          <Text className="set_room_params-thirdparty-combobox-text" noSelect>
            {roomParams.storageLocation?.title || "Select"}
          </Text>
          <ReactSVG
            className="set_room_params-thirdparty-combobox-expander"
            src={"/static/images/expander-down.react.svg"}
          />
        </div>

        <Button
          className="set_room_params-thirdparty-connect"
          size="small"
          label={t("Common:Connect")}
        />
      </div>

      <StyledDropDownWrapper
        className="dropdown-content-wrapper"
        ref={dropdownRef}
      >
        <StyledDropDown
          className="dropdown-content"
          open={isOpen}
          forwardedRef={dropdownRef}
          clickOutsideAction={toggleIsOpen}
          maxHeight={158}
        >
          {thirparties.map((thirdparty) => (
            <DropDownItem
              className="dropdown-item"
              label={thirdparty.title}
              key={thirdparty.id}
              height={32}
              heightTablet={32}
              onClick={() => setStorageLocaiton(thirdparty)}
            />
          ))}
        </StyledDropDown>
      </StyledDropDownWrapper>
    </StyledStorageLocation>
  );
};

export default ThirpartyComboBox;
