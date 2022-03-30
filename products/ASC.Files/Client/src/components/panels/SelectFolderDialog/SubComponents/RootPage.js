import React from "react";
import Items from "../../../Article/Body/Items";

const RootPage = (props) => {
  console.log("root page");
  return <Items showText dialogTree {...props} />;
};

export default RootPage;
