import Box from "@docspace/components/box";
import styled from "styled-components";
import Base from "../themes/base";

export const StyledBox = styled(Box)`
  position: relative;
  width: ${(props) => (props.scale ? "100%" : props.theme.inputPhone.width)};
  border: 1px solid ${(props) =>
    props.hasError
      ? props.theme.inputPhone.errorBorderColor
      : props.theme.inputPhone.inactiveBorderColor};
  border-radius: 3px;
  :focus-within {
    border-color: ${(props) =>
      props.hasError
        ? props.theme.inputPhone.errorBorderColor
        : props.theme.inputPhone.activeBorderColor};
  }

  .country-box {
    width: 57px;
    background: ${(props) => props.theme.inputPhone.backgroundColor};
    border-radius: 3px;
  }

  .input-phone {
    height: ${(props) => props.theme.inputPhone.height};
    padding-left: 20px;
    margin-left: -8px;
    border-left: 1px solid ${(props) =>
      props.theme.inputPhone.inactiveBorderColor};
    border-top-left-radius: 0;
    border-bottom-left-radius: 0;
    background: ${(props) => props.theme.inputPhone.backgroundColor};
  }

  .prefix {
    position: relative;
    top: 0;
    left: 12px;
    font-size: 14px;
    font-weight: 400;
  }

  .combo-button {
    width: 100%;
    height: 100%;
    border-right: 0;
    border-top-right-radius: 0;
    border-bottom-right-radius: 0;
    cursor: pointer;
    padding-left: 0;

    .invalid-flag {
      width: 26px;
      height: 20px;
      margin-left: 6px;
      margin-top: 9px;
    }

    .forceColor {
      width: 36px;
      height: 36px;
      svg {
        path:last-child {
          fill: none;
        }
      }
    }
  }

  .combo-buttons_arrow-icon {
    border-left: 4px solid transparent;
    border-right: 4px solid transparent;
    border-top: 4px solid #a3a9ae;
    cursor: pointer;
    margin: 0;
    position: absolute;
    top: 21px;
    right: 10px;
  }

  .drop-down {
    padding: 12px 16px;
    box-sizing: border-box;
    margin-top: 5px;
    outline: 1px solid ${(props) => props.theme.inputPhone.inactiveBorderColor};
    border-radius: 3px;
    box-shadow: none;
  }

  .search-country_input {
    .search-input-block {
      font-weight: 400;
      border-color: ${(props) => props.theme.inputPhone.inactiveBorderColor};
      :focus-within {
        border-color: ${(props) => props.theme.inputPhone.activeBorderColor};
      }
      ::placeholder {
        color: ${(props) => props.theme.inputPhone.placeholderColor}
      }
    }

  }

  .country-name {
    margin-left: 10px;
    color: ${(props) => props.theme.inputPhone.color};
  }

  .country-dialcode {
    margin-left: 5px;
    color: ${(props) => props.theme.inputPhone.dialCodeColor};
  }

  .country-item {
    display: flex;
    align-items: center;
    max-width: 100%;
    padding: 0;
    height: 36px;
    svg {
      width: 36px !important;
      height: 36px !important;
    }

    .drop-down-icon > div {
      height: 36px;
    }
  
    .drop-down-icon {
      width: 36px;
      height: 36px;
      margin-right: 0;
      svg {
        path:last-child {
          fill: none;
        }
      }
    }
  }

  .phone-input_empty-text {
    padding: 30px 0;
    word-break: break-all;
    color: ${(props) => props.theme.inputPhone.placeholderColor};
  }

  .nav-thumb-vertical {
    background: ${(props) => props.theme.inputPhone.placeholderColor};
    cursor: pointer;
    :hover {
      background: ${(props) => props.theme.inputPhone.scrollBackground};
    }
  }

  .phone-input_error-text {
    position: absolute;
    word-break: break-all;
    top: 48px;
  }
}
`;
StyledBox.defaultProps = { theme: Base };
