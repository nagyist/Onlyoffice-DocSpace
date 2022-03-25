import React, { useCallback } from "react";
import ElementRow from "./ElementRow";
import { inject, observer } from "mobx-react";
import { FixedSizeList as List } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";
import CustomScrollbarsVirtualList from "@appserver/components/scrollbar/custom-scrollbars-virtual-list";

const ElementsPage = ({ folderInfo, getIcon, onClick }) => {
  const { folders, files } = folderInfo;

  const foldersAndFiles = [...folders, ...files];

  const Item = useCallback(
    ({ index, style }) => {
      const item = foldersAndFiles[index];
      return (
        <div style={style}>
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
        </div>
      );
    },
    [foldersAndFiles]
  );

  console.log("elemenets page", foldersAndFiles.length);

  return (
    <AutoSizer>
      {({ height, width }) => (
        <List
          height={height}
          width={width}
          itemSize={48}
          itemCount={foldersAndFiles.length}
          itemData={foldersAndFiles}
          outerElementType={CustomScrollbarsVirtualList}
        >
          {Item}
        </List>
      )}
    </AutoSizer>
  );
};

export default inject(({ settingsStore }) => {
  const { getIcon } = settingsStore;

  return {
    getIcon,
  };
})(observer(ElementsPage));
