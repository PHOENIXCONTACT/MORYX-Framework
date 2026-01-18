let node = document.head.getElementsByTagName("meta")?.namedItem("moryx-pathbase");
let path_base = node?.content ?? "";


export const environment = {
  production: true,
  assets: path_base + "/_content/Moryx.Workplans.Web/",
  rootUrl: path_base,
};
