import styled from "styled-components";

import { desktop } from "@appserver/components/utils/device";

const StyledFoldersTreeLoader = styled.div``;
const StyledItem = styled.div`
  display: grid;
  grid-template-columns: 32px 167px;
  grid-template-rows: 1fr;
  grid-column-gap: 8px;
  .root-folders-loader_rectangle-content {
    margin-top: 16px;
  }
  .root-folders-loader_circle {
    margin-top: 8px;
  }
`;
export { StyledFoldersTreeLoader, StyledItem };
