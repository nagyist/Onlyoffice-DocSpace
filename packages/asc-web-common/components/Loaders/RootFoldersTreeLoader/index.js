import React from "react";
import PropTypes from "prop-types";
import { StyledFoldersTreeLoader } from "./StyledFoldersTreeLoader";
import RootFolderItemLoader from "./RootFolderItemLoader";
const RootFoldersTreeLoader = ({ count, ...props }) => {
  const items = [];

  for (var i = 0; i < count; i++) {
    items.push(<RootFolderItemLoader key={`list_loader_${i}`} {...props} />);
  }
  return (
    <StyledFoldersTreeLoader className="list-loader-wrapper">
      {items}
    </StyledFoldersTreeLoader>
  );
};

RootFoldersTreeLoader.propTypes = {
  count: PropTypes.number,
};

RootFoldersTreeLoader.defaultProps = {
  count: 7,
};
export default RootFoldersTreeLoader;
