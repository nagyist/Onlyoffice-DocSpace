import React, { useEffect } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { useTranslation } from "react-i18next";
import PropTypes from "prop-types";
import Section from "@appserver/common/components/Section";
import Loader from "@appserver/components/loader";
import { setDocumentTitle } from "../../../helpers/utils";
import { inject, observer } from "mobx-react";
import ExpiredTrialPage from "./ExpiredTrialPage";

const TariffsPageWrapper = ({
  setQuota,
  quota,
  setTariffsInfo,
  tariffsInfo,
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
      <></>
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
