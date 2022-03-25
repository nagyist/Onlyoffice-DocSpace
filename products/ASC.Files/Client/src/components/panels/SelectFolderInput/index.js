import React from "react";
import { Provider as MobxProvider, inject, observer } from "mobx-react";

import PropTypes from "prop-types";

import stores from "../../../store/index";
import SelectFolderDialog from "../SelectFolderDialog/index";
import StyledComponent from "./StyledSelectFolderInput";
import { getFolderPath } from "@appserver/common/api/files";
import FileInput from "@appserver/components/file-input";

let path = "";

class SelectFolderInputBody extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      isLoading: false,
      baseFolderPath: "",
      fullFolderPath: "",
    };
  }
  componentDidMount() {
    const { folderPath, setFirstLoad } = this.props;

    if (folderPath?.length !== 0) {
      this.setState({
        fullFolderPath: folderPath,
      });
    }

    setFirstLoad(false);
  }

  componentDidUpdate(prevProps) {
    const { isSuccessSave, isReset } = this.props;
    const { fullFolderPath, baseFolderPath } = this.state;

    if (isSuccessSave && isSuccessSave !== prevProps.isSuccessSave) {
      fullFolderPath &&
        this.setState({
          baseFolderPath: fullFolderPath,
        });
    }

    if (isReset && isReset !== prevProps.isReset) {
      this.setState({
        fullFolderPath: baseFolderPath,
      });
    }
  }

  onSetFullPath = (pathName) => {
    this.setState({
      fullFolderPath: pathName,
    });
  };

  onSetBaseFolderPath = (pathName) => {
    this.setState({
      baseFolderPath: pathName,
    });
  };

  onSetLoadingInput = (loading) => {
    this.setState({
      isLoading: loading,
    });
  };
  render() {
    const { isLoading, baseFolderPath, fullFolderPath } = this.state;
    const {
      onClickInput,
      hasError,
      t,
      placeholder,
      maxInputWidth,
      ...rest
    } = this.props;
    return (
      <StyledComponent maxWidth={maxInputWidth}>
        <FileInput
          className="select-folder_file-input"
          value={fullFolderPath || baseFolderPath}
          hasError={hasError}
          onClick={onClickInput}
          placeholder={placeholder}
          simplifiedFileInput
        />

        <SelectFolderDialog {...rest} />
      </StyledComponent>
    );
  }
}

SelectFolderInputBody.propTypes = {
  onClickInput: PropTypes.func.isRequired,
  hasError: PropTypes.bool,
  placeholder: PropTypes.string,
};

SelectFolderInputBody.defaultProps = {
  hasError: false,
  placeholder: "",
};

const SelectFolderInputBodyWrapper = inject(({ filesStore }) => {
  const { setFirstLoad } = filesStore;
  return {
    setFirstLoad,
  };
})(observer(SelectFolderInputBody));
class SelectFolderInput extends React.Component {
  static setFolderPath = async (folderId) => {
    const foldersArray = await getFolderPath(folderId);

    const convertFolderPath = (foldersArray) => {
      path = "";
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

  render() {
    return (
      <MobxProvider {...stores}>
        <SelectFolderInputBodyWrapper {...this.props} />
      </MobxProvider>
    );
  }
}

export default SelectFolderInput;
