import React from "react";
import PropTypes from "prop-types";

import { StyledBadge, StyledInner, StyledText } from "./styled-badge";

import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

const Badge = (props) => {
  //console.log("Badge render");

  const onClick = (e) => {
    if (!props.onClick) return;

    e.preventDefault();
    props.onClick(e);
  };

  const {
    fontSize,
    color,
    fontWeight,
    backgroundColor,
    borderRadius,
    padding,
    maxWidth,
    height,
    type,
    compact,
    isHovered,
    border,
    label,
  } = props;

  return (
    <ColorTheme
      {...props}
      isHovered={isHovered}
      onClick={onClick}
      border={border}
      themeId={ThemeType.Badge}
    >
      <StyledInner
        backgroundColor={backgroundColor}
        borderRadius={borderRadius}
        padding={padding}
        compact={compact}
        type={type}
        maxWidth={maxWidth}
        height={height}
      >
        <StyledText
          textAlign="center"
          type={type}
          fontWeight={fontWeight}
          color={color}
          fontSize={fontSize}
        >
          {label}
        </StyledText>
      </StyledInner>
    </ColorTheme>
  );
};

Badge.propTypes = {
  /** Label */
  label: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  /** CSS background-color */
  backgroundColor: PropTypes.string,
  /** CSS color */
  color: PropTypes.string,
  /** CSS font-size */
  fontSize: PropTypes.string,
  /** CSS font-weight */
  fontWeight: PropTypes.number,
  /** CSS border-radius */
  borderRadius: PropTypes.string,
  /** CSS padding */
  padding: PropTypes.string,
  /** CSS max-width */
  maxWidth: PropTypes.string,
  /** CSS line-height */
  lineHeight: PropTypes.string,
  /** onClick event */
  onClick: PropTypes.func,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Sets hovered state and link effects */
  isHovered: PropTypes.bool,
  /** Sets hovered state and link effects */
  onHovered: PropTypes.bool,
  /** Disables hover styles */
  noHover: PropTypes.bool,
  /** Type Badge */
  type: PropTypes.oneOf(["high", null]),
  /** Compact badge */
  compact: PropTypes.bool,
};

Badge.defaultProps = {
  label: 0,
  fontSize: "11px",
  fontWeight: 800,
  borderRadius: "11px",
  padding: "0 5px",
  maxWidth: "50px",
  height: "16px",
  isHovered: false,
  noHover: false,
};

export default Badge;
