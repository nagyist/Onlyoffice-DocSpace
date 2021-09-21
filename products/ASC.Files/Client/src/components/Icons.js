import styled from "styled-components";
import combineUrl from "@appserver/common/utils/combineUrl";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";

import FavoriteIcon from "../../public/images/favorite.react.svg";
import FileActionsConvertEditDocIcon from "../../public/images/file.actions.convert.edit.doc.react.svg";
import FileActionsLockedIcon from "../../public/images/file.actions.locked.react.svg";
import { homepage } from "../../package.json";

const EncryptedFileIcon = styled.div`
  background: url(${(props) => props.imageUrl}) no-repeat 0 0 / 16px 16px
    transparent;
  height: 16px;
  position: absolute;
  width: 16px;
  margin-top: 14px;
  margin-left: ${(props) => (props.isEdit ? "40px" : "12px")};
`;

EncryptedFileIcon.defaultProps = {
  imageUrl: combineUrl(
    window.AppServer?.cdnUrl,
    homepage,
    "images/security.svg"
  ),
};

export { EncryptedFileIcon };

export const StyledFavoriteIcon = styled(FavoriteIcon)`
  ${commonIconsStyles}
`;

export const StyledFileActionsConvertEditDocIcon = styled(
  FileActionsConvertEditDocIcon
)`
  ${commonIconsStyles}
  path {
    fill: #3b72a7;
  }
`;

export const StyledFileActionsLockedIcon = styled(FileActionsLockedIcon)`
  ${commonIconsStyles}
  path {
    fill: #3b72a7;
  }
`;
