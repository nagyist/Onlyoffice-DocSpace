import React, { useCallback, useEffect, useRef } from "react";
import Loader from "@appserver/components/loader";
import Text from "@appserver/components/text";
import CustomScrollbarsVirtualList from "@appserver/components/scrollbar/custom-scrollbars-virtual-list";
import InfiniteLoader from "react-window-infinite-loader";
import AutoSizer from "react-virtualized-auto-sizer";
import { FixedSizeList as List } from "react-window";
import { inject, observer } from "mobx-react";
import ElementRow from "./ElementRow";
import EmptyContainer from "../../EmptyContainer/EmptyContainer";
import Loaders from "@appserver/common/components/Loaders";

let countLoad;
const ElementsPage = ({
  hasNextPage,
  isNextPageLoading,
  loadNextPage,
  items,
  id,
  getIcon,
  onFolderClick,
  onSelectFile,
  loadingText,
  page,
  t,
  selectedFileInfo,
}) => {
  const filesListRef = useRef(null);

  if (page === 0) {
    countLoad = 0;
  }
  useEffect(() => {
    if (filesListRef && filesListRef.current) {
      filesListRef.current.resetloadMoreItemsCache(true);
    }
  }, [id]);
  // Every row is loaded except for our loading indicator row.
  const isItemLoaded = useCallback(
    (index) => {
      return !hasNextPage || index < items.length;
    },
    [items, hasNextPage]
  );
  // If there are more items to be loaded then add an extra row to hold a loading indicator.
  const itemCount = hasNextPage ? items.length + 1 : items.length;

  const loadMoreItems = useCallback(() => {
    if (isNextPageLoading) return;
    countLoad++;
    loadNextPage && loadNextPage();
  }, [isNextPageLoading, items]);

  const renderPageLoader = useCallback(
    (style) => {
      return (
        <div style={style}>
          <div key="loader" className="select-folder_list-loader">
            <Loader
              // theme={theme}
              type="oval"
              size="16px"
              className="panel-loader"
            />
            <Text as="span">{loadingText}</Text>
          </div>
        </div>
      );
    },
    [loadingText]
  );
  const renderFirstLoader = (style) => {
    return (
      <div style={style}>
        <div className="select-folder_loader" key="loader">
          <Loaders.ListLoader withoutFirstRectangle withoutLastRectangle />
        </div>
      </div>
    );
  };
  const isFileChecked = useCallback(
    (file) => {
      const checked = selectedFileInfo
        ? file.id === selectedFileInfo.id
        : false;
      return checked;
    },
    [selectedFileInfo]
  );

  const Item = useCallback(
    ({ index, style }) => {
      let isChecked;
      const isLoaded = isItemLoaded(index);

      if (!isLoaded) {
        if (countLoad >= 1) return renderPageLoader(style);
        return renderFirstLoader(style);
      }

      const item = items[index];

      if (!!item.fileExst) {
        isChecked = isFileChecked(item);
      }

      return (
        <div style={style}>
          <ElementRow
            onFolderClick={onFolderClick}
            key={`${item.id}_${index}`}
            index={index}
            item={item}
            isChecked={isChecked}
            onSelectFile={onSelectFile}
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
    [items, selectedFileInfo, renderFirstLoader, renderPageLoader]
  );
  return (
    <div className="select-folder_list-body">
      <AutoSizer>
        {({ width, height }) => (
          <InfiniteLoader
            //theme={theme}
            ref={filesListRef}
            isItemLoaded={isItemLoaded}
            itemCount={itemCount}
            loadMoreItems={loadMoreItems}
          >
            {({ onItemsRendered, ref }) => (
              <List
                //theme={theme}
                height={height}
                itemCount={itemCount}
                itemSize={48}
                onItemsRendered={onItemsRendered}
                ref={ref}
                width={width + 8}
                outerElementType={CustomScrollbarsVirtualList}
              >
                {Item}
              </List>
            )}
          </InfiniteLoader>
        )}
      </AutoSizer>

      {!hasNextPage && itemCount === 0 && (
        <div className="select-file-dialog_empty-container">
          <EmptyContainer
            //theme={theme}
            headerText={t("Home:EmptyFolderHeader")}
            imageSrc="/static/images/empty_screen.png"
          />
        </div>
      )}
    </div>
  );
};

export default inject(({ settingsStore }) => {
  const { getIcon } = settingsStore;

  return {
    getIcon,
  };
})(observer(ElementsPage));
