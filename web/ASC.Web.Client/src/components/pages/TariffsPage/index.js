import React, { useEffect } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { useTranslation, Trans } from "react-i18next";
import PropTypes from "prop-types";
import Section from "@appserver/common/components/Section";
import Loader from "@appserver/components/loader";
import { setDocumentTitle } from "../../../helpers/utils";
import { inject, observer } from "mobx-react";
import ExpiredTrialPage from "./ExpiredTrialPage";
import BusinessTariffPage from "./BusinessTariffPage";
import CurrentTariffContainer from "./sub-components/CurrentTariffContainer";
import Text from "@appserver/components/text";

const StyledBody = styled.div`
  p {
    margin-bottom: 16px;
  }
`;
const TariffsPageWrapper = ({
  setQuota,
  quota,
  setTariffsInfo,
  tariffsInfo,
  organizationName,
}) => {
  const { t, ready } = useTranslation("Payments");

  useEffect(() => {
    setDocumentTitle(t("TariffsPlans")); //TODO: need to specify
  }, [ready]);

  useEffect(() => {
    (async () => {
      //if (isEmpty(quota, true)) {
      try {
        await Promise.all([setQuota(), setTariffsInfo()]);
      } catch (error) {
        toastr.error(error);
      }
      //}
    })();
  }, []);

  console.log("tariffsInfo", tariffsInfo);
  return ready && tariffsInfo && tariffsInfo.availableTariffs && quota ? (
    tariffsInfo.trialExpired ? (
      <ExpiredTrialPage />
    ) : (
      <StyledBody>
        {!tariffsInfo.isStartup ? (
          <Text noSelect isBold fontSize={"16"}>
            <Trans
              t={t}
              i18nKey={tariffsInfo.isTrial ? "TrialTitle" : "BusinessTitle"}
              ns="Payments"
            >
              {{ organizationName }}
            </Trans>
          </Text>
        ) : (
          <Text noSelect isBold fontSize={"16"}>
            {t("StartupTitle")}
          </Text>
        )}
        <CurrentTariffContainer />
        <BusinessTariffPage />
      </StyledBody>
    )
  ) : (
    <Loader className="pageLoader" type="rombs" size="40px" />
  );
};

TariffsPageWrapper.propTypes = {
  isLoaded: PropTypes.bool,
};

const TariffsPage = (props) => {
  return (
    <Section>
      <Section.SectionBody>
        <TariffsPageWrapper {...props} />
      </Section.SectionBody>
    </Section>
  );
};
export default inject(({ auth, payments }) => {
  const { setQuota, quota } = auth;
  const { organizationName } = auth.settingsStore;
  const { setTariffsInfo, tariffsInfo } = payments;
  return { setQuota, quota, organizationName, setTariffsInfo, tariffsInfo };
})(withRouter(observer(TariffsPage)));
