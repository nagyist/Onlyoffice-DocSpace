import React from "react";
import { Provider as MobxProvider, inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import FileInput from "@appserver/components/file-input";
import stores from "../../../store/index";
import SelectFileDialog from "../SelectFileDialog";
import StyledComponent from "./StyledSelectFileInput";

class SelectFileInputBody extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      fileName: "",
    };
  }

  componentDidMount() {
    this.props.setFirstLoad(false);
  }

  onSetFileName = (fileName) => {
    this.setState({
      fileName: fileName,
    });
  };
  render() {
    const {
      onClickInput,
      hasError,
      t,
      placeholder,
      maxInputWidth,
      isPanelVisible,
      ...rest
    } = this.props;

    const { fileName } = this.state;

    return (
      <StyledComponent maxInputWidth={maxInputWidth}>
        <FileInput
          className="select-file_file-input"
          hasError={hasError}
          onClick={onClickInput}
          placeholder={placeholder}
          value={fileName}
          simplifiedFileInput
        />

        {isPanelVisible && (
          <SelectFileDialog
            {...rest}
            isPanelVisible={isPanelVisible}
            onSetFileName={this.onSetFileName}
          />
        )}
      </StyledComponent>
    );
  }
}

SelectFileInputBody.propTypes = {
  onClickInput: PropTypes.func.isRequired,
  hasError: PropTypes.bool,
  placeholder: PropTypes.string,
};

SelectFileInputBody.defaultProps = {
  hasError: false,
  placeholder: "",
};

const SelectFileInputBodyWrapper = inject(({ filesStore }) => {
  const { setFirstLoad } = filesStore;
  return {
    setFirstLoad,
  };
})(observer(SelectFileInputBody));

class SelectFileInput extends React.Component {
  render() {
    return (
      <MobxProvider {...stores}>
        <SelectFileInputBodyWrapper {...this.props} />
      </MobxProvider>
    );
  }
}

export default SelectFileInput;
