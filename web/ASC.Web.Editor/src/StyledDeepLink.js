import styled from "styled-components";
import { tablet } from "@appserver/components/utils/device";

export const StyledBody = styled.div`
  display: flex;
  flex-direction: column;
  padding: 32px 16px;
  z-index: 320;

  @media ${tablet} {
    margin: 0 auto;
    width: 480px;
  }

  @media (max-width: 414px) {
    margin: 0 auto;
    width: 344px;
  }

  .description {
    margin-bottom: 32px;
  }

  .button {
    margin-top: 32px;
    margin-bottom: 24px;
  }

  .link {
    text-align: center;
  }
`;

export const StyledFileTile = styled.div`
  display: flex;
  gap: 16px;
  padding: 16px;
  margin: 16px 0;
  background: #f3f4f4;
  border-radius: 3px;
  align-items: center;
`;

export const StyledHeader = styled.div`
  display: flex;
  height: 48px;
  background-color: #0f4071;
  align-items: center;

  img {
    padding-left: 32px;
  }
`;
