import React, { useRef, useEffect, useState } from "react";
import { inject, observer } from "mobx-react";
import styled from "styled-components";

import { ContextMenu, ContextMenuButton } from "@docspace/components";

import ContextHelper from "../../helpers/ContextHelper";

const StyledItemContextOptions = styled.div`
  margin-left: auto;
`;

const ItemContextOptions = ({
  t,
  selection,
  getContextOptions,
  getContextOptionActions,
  getUserContextOptions,

  isUser = false,
  itemTitleRef,
}) => {
  if (!selection) return null;

  const [contextHelper, setContextHelper] = useState(null);

  const contextMenuRef = useRef();

  const onContextMenu = (e) => {
    e.button === 2;
    if (!contextMenuRef.current.menuRef.current) itemTitleRef.current.click(e);
    contextMenuRef.current.show(e);
  };

  useEffect(() => {
    contextMenuRef.current.hide();
  }, [selection]);

  useEffect(() => {
    const contextHelper = new ContextHelper({
      t,
      isUser,
      selection,
      getContextOptions,
      getContextOptionActions,
      getUserContextOptions,
    });

    setContextHelper(contextHelper);
  }, [
    t,
    isUser,
    selection,
    getContextOptions,
    getContextOptionActions,
    getUserContextOptions,
  ]);

  const options = contextHelper?.getItemContextOptions();

  const withId = (options) => {
    if (Array.isArray(options)) {
      return options.map((option) => {
        if (option.items) {
          option.items = withId(option.items);
        }
        return option.key
          ? { ...option, id: `info-option_${option.key}` }
          : option;
      });
    }
    return options;
  };

  const getData = () => {
    return withId(options);
  };

  return (
    <StyledItemContextOptions>
      <ContextMenu
        ref={contextMenuRef}
        getContextModel={getData}
        withBackdrop={false}
      />
      {options?.length > 0 && (
        <ContextMenuButton
          id="info-options"
          className="expandButton"
          title={"Show item actions"}
          onClick={onContextMenu}
          getData={getData}
          directionX="right"
          isNew={true}
        />
      )}
    </StyledItemContextOptions>
  );
};

export default inject(({ filesStore, peopleStore, contextOptionsStore }) => {
  const { getUserContextOptions } = peopleStore.contextOptionsStore;
  const {
    setBufferSelection,
    getFilesContextOptions: getContextOptions,
  } = filesStore;
  const {
    getFilesContextOptions: getContextOptionActions,
  } = contextOptionsStore;

  return {
    setBufferSelection,
    getContextOptions,
    getContextOptionActions,
    getUserContextOptions,
  };
})(observer(ItemContextOptions));
