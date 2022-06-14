import React from "react";
import styled from "styled-components";
import { useTranslation, Trans } from "react-i18next";
import Text from "@appserver/components/text";
import { inject, observer } from "mobx-react";

const StyledBody = styled.div`
  p:first-child {
    margin-bottom: 12px;
  }
`;

const SubscriptionTitleContainer = ({ tariffsInfo, organizationName }) => {
  const { t } = useTranslation("Payments");
  const { trialExpired, isStartup, isTrial, availableTariffs } = tariffsInfo;

  return (
    <StyledBody>
      <Text noSelect isBold fontSize={"16"}>
        <Trans
          t={t}
          i18nKey={isStartup ? "ReActivate" : "RenewSubscription"}
          ns="Payments"
        >
          {{ organizationName }}
        </Trans>
      </Text>

      <Text noSelect fontWeight={600} fontSize={"14"}>
        <Trans t={t} i18nKey="StartPrice" ns="Payments">
          {{ price: availableTariffs[1].price }}
        </Trans>
      </Text>
    </StyledBody>
  );
};

export default inject(({ auth, payments }) => {
  const { tariffsInfo } = payments;
  const { organizationName } = auth.settingsStore;

  return { tariffsInfo, organizationName };
})(observer(SubscriptionTitleContainer));
