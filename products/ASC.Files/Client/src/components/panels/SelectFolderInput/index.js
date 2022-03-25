import React from "react";
import { Provider as MobxProvider, inject, observer } from "mobx-react";

import PropTypes from "prop-types";

import stores from "../../../store/index";
import SelectFolderDialog from "../SelectFolderDialog/index";
import StyledComponent from "./StyledSelectFolderInput";
import { getFolderPath } from "@appserver/common/api/files";
import FileInput from "@appserver/components/file-input";

class SelectFolderInputBody extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      isLoading: false,
      baseFolderPath: "",
      newFolderPath: "",
      baseFolderId: null,
    };
  }
  componentDidMount() {
    const { folderPath, setFirstLoad } = this.props;

    if (folderPath?.length !== 0) {
      this.setState({
        newFolderPath: folderPath,
      });
    }

    setFirstLoad(false);
  }

  componentDidUpdate(prevProps) {
    const { isSuccessSave, isReset, id } = this.props;
    const { newFolderPath, baseFolderPath } = this.state;

    if (isSuccessSave && isSuccessSave !== prevProps.isSuccessSave) {
      newFolderPath &&
        this.setState({
          baseFolderPath: newFolderPath,
          baseFolderId: id,
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
    const convertFoldersArray = await this.setFolderPath(folderId);

    this.setState({
      newFolderPath: convertFoldersArray,
    });
  };

  onSetBaseFolderPath = async (folderId) => {
    const convertFoldersArray = await this.setFolderPath(folderId);

    this.setState({
      baseFolderPath: convertFoldersArray,
      baseFolderId: folderId,
    });
  };

  onSetLoadingInput = (loading) => {
    this.setState({
      isLoading: loading,
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
  render() {
    return (
      <MobxProvider {...stores}>
        <SelectFolderInputBodyWrapper {...this.props} />
      </MobxProvider>
    );
  }
}

export default SelectFolderInput;
