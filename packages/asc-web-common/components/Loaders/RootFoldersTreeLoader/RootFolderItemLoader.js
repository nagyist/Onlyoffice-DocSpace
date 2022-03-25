import React from "react";
import RectangleLoader from "../RectangleLoader";
import CircleLoader from "../CircleLoader";
import { StyledItem } from "./StyledFoldersTreeLoader";
const RootFolderItemLoader = ({
  title,
  borderRadius,
  backgroundColor,
  foregroundColor,
  backgroundOpacity,
  foregroundOpacity,
  speed,
  animate,
}) => {
  return (
    <StyledItem>
      <div className="root-folders-loader_circle">
        <CircleLoader
          title={title}
          x="16"
          y="16"
          width="32"
          height="32"
          radius="16"
          backgroundColor={backgroundColor}
          foregroundColor={foregroundColor}
          backgroundOpacity={backgroundOpacity}
          foregroundOpacity={foregroundOpacity}
          speed={speed}
          animate={animate}
        />
      </div>
      <div className="root-folders-loader_rectangle-content">
        <RectangleLoader
          title={title}
          width="100%"
          height="16"
          borderRadius={borderRadius}
          backgroundColor={backgroundColor}
          foregroundColor={foregroundColor}
          backgroundOpacity={backgroundOpacity}
          foregroundOpacity={foregroundOpacity}
          speed={speed}
          animate={animate}
        />
      </div>
    </StyledItem>
  );
};

export default RootFolderItemLoader;
