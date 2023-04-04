﻿import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { inject, observer } from "mobx-react";
import moment from "moment";
import { useTranslation } from "react-i18next";

import Loaders from "@docspace/common/components/Loaders";
import { setDocumentTitle } from "@docspace/client/src/helpers/filesUtils";

import PaymentContainer from "./PaymentContainer";

const PaymentsPage = ({
  language,
  isLoadedTariffStatus,
  isLoadedCurrentQuota,
  isInitPaymentPage,
  init,
}) => {
  const { t, ready } = useTranslation(["Payments", "Common", "Settings"]);

  useEffect(() => {
    moment.locale(language);
  }, []);

  useEffect(() => {
    setDocumentTitle(t("Common:PaymentsTitle"));
  }, [ready]);

  useEffect(() => {
    if (!isLoadedTariffStatus || !isLoadedCurrentQuota || !ready) return;

    init(t);
  }, [isLoadedTariffStatus, isLoadedCurrentQuota, ready]);

  return !isInitPaymentPage || !ready ? (
    <Loaders.PaymentsLoader />
  ) : (
    <PaymentContainer t={t} />
  );
};

PaymentsPage.propTypes = {
  isLoaded: PropTypes.bool,
};

export default inject(({ auth, payments }) => {
  const { language, currentQuotaStore, currentTariffStatusStore } = auth;

  const { isLoaded: isLoadedCurrentQuota } = currentQuotaStore;
  const { isLoaded: isLoadedTariffStatus } = currentTariffStatusStore;
  const { isInitPaymentPage, init } = payments;

  return {
    init,
    isInitPaymentPage,
    language,
    isLoadedTariffStatus,
    isLoadedCurrentQuota,
  };
})(observer(PaymentsPage));
