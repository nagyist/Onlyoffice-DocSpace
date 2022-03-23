import React from "react";
import { Provider as MobxProvider } from "mobx-react";

import stores from "../../../store/index";
import store from "studio/store";
import SelectFolderDialog from "./index";

const { auth: authStore } = store;

const SelectFolderModalWrapper = (props) => <SelectFolderDialog {...props} />;

class SelectFolderModal extends React.Component {
  componentDidMount() {
    authStore.init(true);
  }

  render() {
    return (
      <MobxProvider auth={authStore} {...stores}>
        <SelectFolderModalWrapper {...this.props} />
      </MobxProvider>
    );
  }
}

export default SelectFolderModal;
