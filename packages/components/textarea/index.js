import React, { useRef } from "react";
import PropTypes from "prop-types";
import {
  StyledTextarea,
  StyledScrollbar,
  StyledBorder,
} from "./styled-textarea";

// eslint-disable-next-line react/prop-types, no-unused-vars

const Textarea = ({
  className,
  id,
  isDisabled,
  isReadOnly,
  hasError,
  heightScale,
  maxLength,
  name,
  onChange,
  placeholder,
  style,
  tabIndex,
  value,
  fontSize,
  heightTextArea,
  color,
  theme,
  autoFocus,
  areaSelect,
  disableScroll,
}) => {
  const areaRef = useRef(null);

  React.useEffect(() => {
    if (areaSelect && areaRef.current) {
      areaRef.current.select();
    }
  }, [areaSelect]);

  const onChangeArea = (e) => {
    onChange && onChange(e);
  };

  const textarea = (
    <StyledTextarea
      id={id}
      placeholder={placeholder}
      onChange={onChangeArea}
      maxLength={maxLength}
      name={name}
      tabIndex={tabIndex}
      isDisabled={isDisabled}
      disabled={isDisabled}
      readOnly={isReadOnly}
      value={value}
      fontSize={fontSize}
      color={color}
      autoFocus={autoFocus}
      ref={areaRef}
    />
  );

  return disableScroll ? (
    <StyledBorder
      className={className}
      style={style}
      isDisabled={isDisabled}
      hasError={hasError}
    >
      {textarea}
    </StyledBorder>
  ) : (
    <StyledScrollbar
      className={className}
      style={style}
      stype="preMediumBlack"
      isDisabled={isDisabled}
      hasError={hasError}
      heightScale={heightScale}
      heighttextarea={heightTextArea}
    >
      {textarea}
    </StyledScrollbar>
  );
};

Textarea.propTypes = {
  /** Class name */
  className: PropTypes.string,
  /** Used as HTML `id` property  */
  id: PropTypes.string,
  /** Indicates that the field cannot be used */
  isDisabled: PropTypes.bool,
  /** Indicates that the field is displaying read-only content */
  isReadOnly: PropTypes.bool,
  /** Indicates the input field has an error  */
  hasError: PropTypes.bool,
  /** Indicates the input field has scale */
  heightScale: PropTypes.bool,
  /** Max Length of value */
  maxLength: PropTypes.number,
  /** Used as HTML `name` property  */
  name: PropTypes.string,
  /** Allow you to handle changing events of component */
  onChange: PropTypes.func,
  /** Placeholder for Textarea  */
  placeholder: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Used as HTML `tabindex` property */
  tabIndex: PropTypes.number,
  /** Value for Textarea */
  value: PropTypes.string,
  /** Value for font-size */
  fontSize: PropTypes.number,
  /** Value for height text-area */
  heightTextArea: PropTypes.number,
  /** Specifies the text color */
  color: PropTypes.string,
  /** Allow you to setting auto focus in component*/
  autoFocus: PropTypes.bool,
  /** Allow you to control select parameter in component*/
  areaSelect: PropTypes.bool,
  /** Allow you disable scrollbar wrapper in component*/
  disableScroll: PropTypes.bool,
};

Textarea.defaultProps = {
  className: "",
  isDisabled: false,
  isReadOnly: false,
  hasError: false,
  heightScale: false,
  placeholder: "",
  tabIndex: -1,
  value: "",
  fontSize: 13,
  isAutoFocussed: false,
  areaSelect: false,
  disableScroll: false,
};

export default Textarea;
