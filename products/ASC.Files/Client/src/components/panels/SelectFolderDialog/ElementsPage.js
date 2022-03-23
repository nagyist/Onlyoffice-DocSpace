import React from "react";

import Items from "../../Article/Body/Items";
import ElementRow from "./ElementRow";
import { inject, observer, Provider as MobxProvider } from "mobx-react";
class ElementsPage extends React.Component {
  constructor(props) {
    super(props);
    this.state = {};
  }

  componentDidMount() {}

  componentWillUnmount() {}

 
  render() {
    const { folderInfo, getIcon, onClick } = this.props;
    const { folders, files } = folderInfo;

    const foldersAndFiles = [...folders, ...files];
    console.log("elemenets page", foldersAndFiles);
    return (
      <>
        {foldersAndFiles.map((item, index) => (
          <ElementRow
            onClick={onClick}
            key={`${item.id}_${index}`}
            item={item}
            icon={getIcon(
              32,
              item.fileExst,
              item.providerKey,
              item.contentLength
            )}
          />
        ))}
      </>
    );
  }
}

export default inject(({ settingsStore }) => {
  const { getIcon } = settingsStore;

  return {
    getIcon,
  };
})(observer(ElementsPage));
