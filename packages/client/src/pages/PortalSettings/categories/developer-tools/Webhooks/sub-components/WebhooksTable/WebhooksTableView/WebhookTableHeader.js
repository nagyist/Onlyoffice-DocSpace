import React, { useRef } from "react";
import TableHeader from "@docspace/components/table-container/TableHeader";

import { useTranslation } from "react-i18next";

export const WebhookTableHeader = ({ sectionWidth, tableRef }) => {
  const { t } = useTranslation(["Webhooks", "Common"]);
  const columns = useRef([
    {
      key: "Name",
      title: t("Name", { ns: "Common" }),
      resizable: true,
      enable: true,
      default: true,
      active: true,
    },
    {
      key: "URL",
      title: t("URL", { ns: "Webhooks" }),
      enable: true,
      resizable: true,
    },
    {
      key: "State",
      title: t("State", { ns: "Webhooks" }),
      enable: true,
      resizable: true,
    },
  ]);
  return (
    <TableHeader
      columns={columns.current}
      containerRef={tableRef}
      sectionWidth={sectionWidth}
      showSettings={false}
      style={{ position: "absolute" }}
    />
  );
};
