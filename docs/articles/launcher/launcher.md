# Launcher

## Configuration

The launcher generates a config file named `Moryx.Launcher.LauncherConfig.json` in your configuration directory. This file contains settings for the launcher, including module sort-indices and external modules.

### Sorting

To sort your modules in the launcher, you can use the `ModuleSortIndices` property. This property allows you to define the order of modules by assigning them an index value. Modules with lower index values will appear before those with higher values.

````json
{
  "ModuleSortIndices": [
    {
      "Route": "Orders",
      "SortIndex": 1
    },
    {
      "Route": "WorkerSupport",
      "SortIndex": 10
    },
    {
      "Route": "example",
      "SortIndex": 100
    }
  ]
}

````

### External Modules

To define external modules in the launcher configuration, you can use the `ExternalModules` property. This allows you to specify
modules that should be loaded from external sources rather than being bundled with your application.

External modules are integrated with the `<embed>` tag; the external web-page must support being embedded in an iframe.

````json
{
  "ExternalModules": [
    {
      "Title": "Example",
      "Url": "http://www.example.com",
      "Icon": "globe",
      "Description": "Example description of the external module.",
      "Route": "example"
    }
  ]
}

````

### Region

To define a region in the launcher configuration, you can use the `Regions` property. This allows you to specify
which region should be loaded and where it shoud be placed in the launcher.

The region must define its own size using `css`, if that's a requirement for you. Otherwise the launcher will set an automatic size based on
the available space.

````json
{
  "Regions": [
    {
      "Region": "Right",
      "Name": "MyRegionName"
    }
  ]
}

````
**Note**: This will work under the following conditions:
1. You have created an [ASP.net Partial View](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/partial?view=aspnetcore-10.0#declare-partial-views).
2. You used the `LauncherRegionAttribute` in your partial view.
3. The name you used in your `LauncherRegion()` is the same in the configuration.
4. You have placed the partial view inside a folder called `Pages`.
5. The project in which you defined your partial view support Razor Pages and references the Launcher package or project. 

