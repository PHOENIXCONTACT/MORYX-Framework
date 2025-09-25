# MORYX 8.x to 10.x

## Replaced result of visual instructions with dedicated result object

In *Moryx.Factory* **6.3** and **8.1** we introduced the new result object and optional extended APIs. The result object solved issues caused by localization of the different results. With **Moryx 10** we remove all old APIs based on strings.

## Merged `IProcessControlReporting` into `IProcessControl`

The `IProcessControlReporting` interface has been merged into `IProcessControl`. All reporting-related methods and the `ReportAction` enum are now part of `IProcessControl`. Remove usages of `IProcessControlReporting` and update your code to use the unified `IProcessControl` interface.

