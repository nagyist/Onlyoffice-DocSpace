import { request } from "../client";
import { combineUrl } from "../../utils";

export function getModulesList() {
  return request({
    method: "get",
    url: "/modules/info",
  }).then((modules) => {
    const publicPath = window.AppServer?.cdnUrl || "";
    const newModules = modules
      .filter((module) => typeof module === "object")
      .map((m) => {
        return {
          ...m,
          iconUrl: combineUrl(publicPath, m.link, m.iconUrl),
          imageUrl: combineUrl(publicPath, m.link, m.imageUrl),
        };
      });

    return newModules;
  });
}
