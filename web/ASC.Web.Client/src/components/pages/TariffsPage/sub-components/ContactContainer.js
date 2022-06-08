import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { withRouter } from "react-router";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import Text from "@appserver/components/text";
import Link from "@appserver/components/link";

const StyledContactContainer = styled.div`
  display: flex;
  margin: 0 auto;
  width: max-content;
  p {
    margin-right: 8px;
  }
`;

const ContactContainer = ({ salesEmail, theme }) => {
  const { t } = useTranslation("Payments");

  return (
    <StyledContactContainer>
      <Text>{t("HaveQuestions")}</Text>

      <Link
        href={`mailto:${salesEmail}`}
        color={theme.studio.paymentsEnterprise.linkColor}
      >
        {t("ContactUs")}
      </Link>
    </StyledContactContainer>
  );
};

export default inject(({ payments, auth }) => {
  const { tariffsInfo } = payments;

  const { salesEmail } = tariffsInfo;
  return {
    salesEmail,
    theme: auth.settingsStore.theme,
  };
})(withRouter(observer(ContactContainer)));
