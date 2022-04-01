import styled from "styled-components";

const StyledHeader = styled.div`
  .dialog-header {
    display: flex;
  }
  .arrow-button {
    margin: auto 12px auto 0;
  }
`;

const StyledRootPage = styled.div`
  .root-loader {
    display: flex;
  }
`;
const StyledBody = styled.div`
  height: 100%;
  .select-folder_content-body {
    display: grid;
    height: calc(100% - 32px);
    grid-template-rows: max-content auto max-content;
    .select-dialog_header-child {
      ${(props) => props.headerChild && `padding-bottom: 16px;`}
    }
  }
  .select-folder_list-body {
    height: 100%;
  }

  .select-folder_loader {
    overflow: hidden;
    .list-loader-wrapper {
      padding: 0;
    }
  }
  .select-folder_list-loader {
    display: flex;
    div:first-child {
      margin-right: 8px;
    }
  }
  .select-dialog_footer {
    border-top: 1px solid #eceef1;
    padding-top: 16px;
    .select-dialog_buttons {
      display: flex;
      padding: 0 16px 0 16px;

      margin-left: -16px;
      margin-right: -16px;
      width: 100%;
      box-sizing: border-box;
      button:first-child {
        margin-right: 10px;
      }
    }
    .select-dialog_footer-child {
      ${(props) => props.footerChild && `padding-bottom: 16px;`}
    }
  }
  .empty-folder_container {
    grid-template-areas:
      "img img"
      "headerText headerText";

    .ec-image {
      margin: auto;
    }
    .ec-header {
      margin: auto;
    }
  }

  .nav-thumb-vertical {
    margin-left: 5px !important;
  }
`;

const StyledWrapper = styled.div`
  display: grid;
  grid-template-columns: 32px auto 16px;
  grid-gap: 8px;
  position: relative;
  height: 48px;
  width: calc(100% - 16px);
  .element-row_text {
    margin-top: auto;
    margin-bottom: 16px;
    overflow: hidden;
    p {
      text-overflow: ellipsis;
      overflow: hidden;
      white-space: nowrap;
    }
  }
  .element-row_icon,
  .element-row_checkbox {
    margin: auto 0;
  }
  .element-row_clicked-area {
    position: absolute;
    top: 0;
    left: 0;

    width: 100%;
    height: 100%;
    cursor: pointer;
  }
`;

export { StyledRootPage, StyledBody, StyledHeader, StyledWrapper };
