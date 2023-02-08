import pkg from "../../../package.json";
import { translations } from "SRC_DIR/autoGeneratedTranslations";

export function getLanguage(lng) {
  try {
    let language = lng == "en-US" || lng == "en-GB" ? "en" : lng;

    const splitted = lng.split("-");

    if (splitted.length == 2 && splitted[0] == splitted[1].toLowerCase()) {
      language = splitted[0];
    }

    return language;
  } catch (error) {
    console.error(error);
  }

  return lng;
}

export function loadLanguagePath(homepage, fixedNS = null) {
  return (lng, ns) => {
    const language = getLanguage(lng instanceof Array ? lng[0] : lng);

    const lngCollection = translations.get(language);

    const path = lngCollection?.get(`${fixedNS || ns}`);

    if (!path) return `/doceditor/locales/${language}/${fixedNS || ns}.json`;

    const isCommonPath = path?.indexOf("Common") > -1;
    const isClientPath = !isCommonPath && path?.indexOf("Editor") === -1;

    if (ns.length > 0 && ns[0] === "Common" && isCommonPath) {
      return path.replace("/doceditor/", "/static/");
    }

    if (ns.length > 0 && ns[0] != "Editor" && isClientPath) {
      return path.replace("/doceditor/", "/");
    }

    return path;
  };
}

export const initI18n = (initialI18nStoreASC) => {
  if (!initialI18nStoreASC || window.i18n) return;

  window.i18n = {};
  window.i18n.inLoad = [];
  window.i18n.loaded = {};

  for (let lng in initialI18nStoreASC) {
    const collection = translations.get(lng);

    for (let ns in initialI18nStoreASC[lng]) {
      const path = collection?.get(ns);

      if (!path) {
        window.i18n.loaded[`/doceditor/locales/${lng}/${ns}.json`] = {
          namespaces: ns,
          data: initialI18nStoreASC[lng][ns],
        };
      } else {
        if (ns === "Common") {
          window.i18n.loaded[`${path?.replace("/doceditor/", "/static/")}`] = {
            namespaces: ns,
            data: initialI18nStoreASC[lng][ns],
          };
        } else if (ns != "Editor") {
          window.i18n.loaded[`${path?.replace("/doceditor/", "/")}`] = {
            namespaces: ns,
            data: initialI18nStoreASC[lng][ns],
          };
        } else {
          window.i18n.loaded[`${path}`] = {
            namespaces: ns,
            data: initialI18nStoreASC[lng][ns],
          };
        }
      }
    }
  }
};
