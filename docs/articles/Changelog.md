Changelog
========

| Date | Description |
|------------|--------------------------------------------------------------------------------------------------------------------------|
| 2017-11-28 | Readded product revisions to ui |
| 2017-10-04 | Set visibility of property Activities to private. Added Methods for threadsafe access to the list of activities.|
| 2017-09-04 | Renamed current recipe to default recipe. Recipe will now be classified by the RecipeClassification |
| 2017-08-03 | Workplans are now stored relational instead of JSON. |
| 2017-06-29 | Removed NonCompilableParameters. Use ParametersBase instead! Added IArticleModificationActivity |
| 2017-05-04 | Renamed ArticleRequirement to ProcessRequirement |
| 2017-04-28 | Added create revision possibility in product UI |
| 2017-04-18 | Changed NeededCapabilities to RequiredCapabilities and ParametersObj to Parameters |
| 2017-04-10 | Changed Property Capabilities. No Overwrite anymore but can be set internaly. Event OnCapabilitiesChanged added. See "How to create a resource" for an example. | 
| 2017-03-20 | Added `IMounting` interface |
| 2017-02-23 | Removed `CreateLink`, use `nameof(Product.Part)` instead |
| 2016-12-22 | Moved MarkingDriver, ScannerDriver, WpcHandlingResource, AssemblyInstructionResource to ControlSystem |
| 2016-12-21 | Refactoring of ProductStorage from delegates to strategy interface. See documentation for details. |
| 2016-12-06 | Added Binding to VisualInstruction. Use {Product.Identity} or similar to include values from the process into the message. |
| 2016-12-01 | TaskAssignment added, DisabledSteps added, SuccessAttribute removed (use value == 0), MappingValue changed to long from object |
| 2016-11-25 | Added created updated deleted operations to resource item |
| 2016-11-23 | Added interfaces for recipes |
| 2016-11-21 | TruTopsMarkDriver: Added additional property check on startup of the driver. Laser shutter will be opened on preparation of the driver.  |
| 2016-10-17 | Merged ProductStorage and ArticleStorage. See documentation for implementation details. |
| 2016-10-17 | Introduced IDriverState to extend the driver states with additional information |
| 2016-10-15 | Added support for moving axes within the TruTopsMark driver for trumpf laser |
| 2016-10-05 | Added save recipe option to ProductManagement facade |
| 2016-10-05 | Updated product management service to version from 1.0.3 to 1.1.0. |
| 2016-10-04 | Extend the Trumpf laser Driver API to have the possibility to trigger a request to get the last error and warning from the Trumpf laser |
| 2016-10-04 | Added new product interaction module for a clean start of the products ui |
| 2016-09-06 | Added driver for WolfSoldering cells |
| 2016-08-05 | Added TruTopsMark LaserDriver for TruMark Laser |
| 2016-04-01 | Inputs of Connectors now also arrays. Steps can no longer have a flexible list of inputs,but must define a fixed number. |
| 2016-03-29 | Removed simple loader. Improved loading of resources |
| 2016-03-24 | Handlermap is now a black box reuse and all switch/case handle methods must be replaced with CreateHandlerMap |
| 2016-02-15 | Added Marvin.Drivers Bundle library and added all interfaces and classes from Marvin.Resources.Drivers |