import React from "react";
import styled from "styled-components";
import { InfoText } from "../styled-components";

import { Link } from "@docspace/components";

import { useTranslation } from "react-i18next";

const InfoWrapper = styled.div`
  margin-bottom: 27px;
`;

export const WebhookInfo = () => {
  const { t } = useTranslation(["Webhooks"]);

  return (
    <InfoWrapper>
      <InfoText>{t("WebhooksInfo", { ns: "Webhooks" })}</InfoText>
      <Link
        fontWeight={600}
        color="#316DAA"
        isHovered
        type="page"
        href="https://api.onlyoffice.com/portals/basic">
        {t("WebhooksGuide", { ns: "Webhooks" })}
      </Link>
    </InfoWrapper>
  );
};
