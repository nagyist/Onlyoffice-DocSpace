import React from "react";
import { Provider as MobxProvider, inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import stores from "../../../store/index";
import SelectFolderDialog from "../SelectFolderDialog/index";
import StyledComponent from "./StyledSelectFolderInput";
import { getFolderPath } from "@appserver/common/api/files";
import FileInput from "@appserver/components/file-input";
import toastr from "@appserver/components/toast/toastr";
import i18n from "./i18n";
import { I18nextProvider } from "react-i18next";
class SelectFolderInputBody extends React.PureComponent {
  constructor(props) {
    super(props);
    const { id, foldersType } = this.props;

    const isNeedLoader = !!id || foldersType === "common";

    this.state = {
      isLoading: isNeedLoader,
      baseFolderPath: "",
      newFolderPath: "",
    };
  }
  componentDidMount() {
    const { setFirstLoad } = this.props;

    setFirstLoad(false);
  }

  componentDidUpdate(prevProps) {
    const { isSuccessSave, isReset } = this.props;
    const { newFolderPath, baseFolderPath } = this.state;

    if (isSuccessSave && isSuccessSave !== prevProps.isSuccessSave) {
      newFolderPath &&
        this.setState({
          baseFolderPath: newFolderPath,
        });
    }

    if (isReset && isReset !== prevProps.isReset) {
      this.setState({
        newFolderPath: baseFolderPath,
      });
    }
  }
  setFolderPath = async (folderId) => {
    const foldersArray = await getFolderPath(folderId);

    const convertFolderPath = (foldersArray) => {
      let path = "";
      if (foldersArray.length > 1) {
        for (let item of foldersArray) {
          if (!path) {
            path = path + `${item.title}`;
          } else path = path + " " + "/" + " " + `${item.title}`;
        }
      } else {
        for (let item of foldersArray) {
          path = `${item.title}`;
        }
      }
      return path;
    };

    const convertFoldersArray = convertFolderPath(foldersArray);

    return convertFoldersArray;
  };
  onSetNewFolderPath = async (folderId) => {
    let timerId = setTimeout(() => {
      this.setState({ isLoading: true });
    }, 500);

    try {
      const convertFoldersArray = await this.setFolderPath(folderId);
      clearTimeout(timerId);
      timerId = null;

      this.setState({
        newFolderPath: convertFoldersArray,
        isLoading: false,
      });
    } catch (e) {
      toastr.error(e);
      clearTimeout(timerId);
      timerId = null;

      this.setState({
        isLoading: false,
      });
    }
  };

  onSetBaseFolderPath = async (folderId) => {
    try {
      const convertFoldersArray = await this.setFolderPath(folderId);

      this.setState({
        baseFolderPath: convertFoldersArray,
        isLoading: false,
      });
    } catch (e) {
      toastr.error(e);
      this.setState({
        isLoading: false,
      });
    }
  };

  onSetLoadingInput = (isLoading) => {
    this.setState({
      isLoading,
    });
  };
  render() {
    const { isLoading, baseFolderPath, newFolderPath } = this.state;
    const {
      onClickInput,
      hasError,
      t,
      placeholder,
      maxInputWidth,
      isDisabled,
      ...rest
    } = this.props;

    return (
      <StyledComponent maxWidth={maxInputWidth}>
        <FileInput
          className="select-folder_file-input"
          value={newFolderPath || baseFolderPath}
          hasError={hasError}
          onClick={onClickInput}
          placeholder={placeholder}
          isDisabled={isDisabled || isLoading}
          simplifiedFileInput
        />

        <SelectFolderDialog
          {...rest}
          onSetBaseFolderPath={this.onSetBaseFolderPath}
          onSetNewFolderPath={this.onSetNewFolderPath}
        />
      </StyledComponent>
    );
  }
}

SelectFolderInputBody.propTypes = {
  onClickInput: PropTypes.func.isRequired,
  hasError: PropTypes.bool,
  isDisabled: PropTypes.bool,
  placeholder: PropTypes.string,
};

SelectFolderInputBody.defaultProps = {
  hasError: false,
  isDisabled: false,
  placeholder: "",
};

const SelectFolderInputBodyWrapper = inject(({ filesStore }) => {
  const { setFirstLoad } = filesStore;
  return {
    setFirstLoad,
  };
})(observer(SelectFolderInputBody));
class SelectFolderInput extends React.Component {
  render() {
    return (
      <MobxProvider {...stores}>
        <I18nextProvider i18n={i18n}>
          <SelectFolderInputBodyWrapper {...this.props} />
        </I18nextProvider>
      </MobxProvider>
    );
  }
}

export default SelectFolderInput;
