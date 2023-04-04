import React, { useCallback, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router-dom";
import { useTranslation, Trans } from "react-i18next";
import Text from "@docspace/components/text";
import ArrowRightIcon from "PUBLIC_DIR/images/arrow.right.react.svg";
import { StyledArticlePaymentAlert } from "../styled-article";
import styled from "styled-components";
import { combineUrl } from "@docspace/common/utils";

import Loaders from "../../Loaders";

const StyledArrowRightIcon = styled(ArrowRightIcon)`
  margin: auto 0;
  path {
    fill: ${(props) => props.color};
  }
`;

const PROXY_BASE_URL = combineUrl(
  window.DocSpaceConfig?.proxy?.url,
  "/portal-settings"
);

const ArticlePaymentAlert = ({
  pricePerManager,
  isFreeTariff,
  theme,
  currencySymbol,
  setPortalPaymentQuotas,
  currentTariffPlanTitle,
  toggleArticleOpen,
  tariffPlanTitle,
  history,
}) => {
  const { t, ready } = useTranslation("Common");

  const getQuota = useCallback(async () => {
    if (isFreeTariff)
      try {
        await setPortalPaymentQuotas();
      } catch (e) {
        console.error(e);
      }
  }, []);

  useEffect(() => {
    getQuota();
  }, []);

  const onClick = () => {
    const paymentPageUrl = combineUrl(
      PROXY_BASE_URL,
      "/payments/portal-payments"
    );
    history.push(paymentPageUrl);
    toggleArticleOpen();
  };

  const isShowLoader = !ready;

  return isShowLoader ? (
    <Loaders.Rectangle width="210px" height="88px" />
  ) : (
    <StyledArticlePaymentAlert
      onClick={onClick}
      isFreeTariff={isFreeTariff}
      theme={theme}
      id="document_catalog-payment-alert"
    >
      <div>
        <Text className="article-payment_border">
          {isFreeTariff ? (
            <Trans t={t} i18nKey="FreeStartupPlan" ns="Common">
              {{ planName: currentTariffPlanTitle }}
            </Trans>
          ) : (
            t("Common:LatePayment")
          )}
        </Text>
        <Text fontWeight={600}>
          {isFreeTariff
            ? t("Common:ActivateBusinessPlan", { planName: tariffPlanTitle })
            : t("Common:GracePeriodActivated")}
        </Text>
        <Text noSelect fontSize={"12px"}>
          {isFreeTariff ? (
            <>
              {pricePerManager ? (
                <Trans t={t} i18nKey="PerUserMonth" ns="Common">
                  From {{ currencySymbol }}
                  {{ price: pricePerManager }} per admin/month
                </Trans>
              ) : (
                <></>
              )}
            </>
          ) : (
            t("Common:PayBeforeTheEndGracePeriod")
          )}
        </Text>
      </div>

      <StyledArrowRightIcon />
    </StyledArticlePaymentAlert>
  );
};

export default withRouter(
  inject(({ auth }) => {
    const { paymentQuotasStore, currentQuotaStore, settingsStore } = auth;
    const { currentTariffPlanTitle } = currentQuotaStore;
    const { theme } = auth;
    const { setPortalPaymentQuotas, planCost, tariffPlanTitle } =
      paymentQuotasStore;

    return {
      setPortalPaymentQuotas,
      pricePerManager: planCost.value,
      theme,
      currencySymbol: planCost.currencySymbol,
      currentTariffPlanTitle,
      tariffPlanTitle,
    };
  })(observer(ArticlePaymentAlert))
);
