import React, { useCallback, useRef, useEffect } from "react";
import ElementRow from "./ElementRow";
import { inject, observer } from "mobx-react";
import { FixedSizeList as List } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";
import CustomScrollbarsVirtualList from "@appserver/components/scrollbar/custom-scrollbars-virtual-list";
import InfiniteLoader from "react-window-infinite-loader";
import Loader from "@appserver/components/loader";
import Text from "@appserver/components/text";
import Loaders from "@appserver/common/components/Loaders";
import EmptyContainer from "../../EmptyContainer/EmptyContainer";
let countLoad;
const ElementsPage = ({
  hasNextPage,
  isNextPageLoading,
  loadNextPage,
  folders,
  id,
  getIcon,
  onClick,
  loadingText,
  page,
  t,
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
      return !hasNextPage || index < folders.length;
    },
    [folders, hasNextPage]
  );
  // If there are more items to be loaded then add an extra row to hold a loading indicator.
  const itemCount = hasNextPage ? folders.length + 1 : folders.length;

  const loadMoreItems = useCallback(() => {
    if (isNextPageLoading) return;
    countLoad++;
    loadNextPage && loadNextPage();
  }, [isNextPageLoading, folders]);

  const renderPageLoader = useCallback(
    (style) => {
      return (
        <div style={style}>
          <div key="loader" className="select-folder_list-loader">
            <Loader
              //theme={theme}
              type="oval"
              size="16px"
              className="panel-loader"
            />
            {/* theme={theme} */}
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

  const Item = useCallback(
    ({ index, style }) => {
      const isLoaded = isItemLoaded(index);

      if (!isLoaded) {
        if (countLoad >= 1) return renderPageLoader(style);
        return renderFirstLoader(style);
      }

      const item = folders[index];

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
    [folders, renderPageLoader, id]
  );

  return (
    <div className="select-folder_list-body">
      <AutoSizer>
        {({ height, width }) => (
          <InfiniteLoader
            //theme={theme}
            ref={filesListRef}
            isItemLoaded={isItemLoaded}
            itemCount={itemCount}
            loadMoreItems={loadMoreItems}
          >
            {({ onItemsRendered, ref }) => (
              <List
                height={height}
                width={width}
                itemSize={48}
                itemCount={itemCount}
                outerElementType={CustomScrollbarsVirtualList}
                onItemsRendered={onItemsRendered}
                ref={ref}
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
