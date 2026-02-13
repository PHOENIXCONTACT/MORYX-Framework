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
