# Launcher

## Sorting

To sort your modules in the launcher, you can use the `SortIndex` property within the `appsettings.json`. This property allows you to define the order of modules by assigning them an index value. Modules with lower index values will appear before those with higher values.

````json
{
  "Shell": {
    "SortIndex": {
      "moduleA-route": 10,
      "moduleB-route": 20,
      "moduleC-route": 15
    }
  }
}
````

## External Modules

To define external modules in the launcher configuration, you can use the `ExternalModules` property within the `appsettings.json`. This allows you to specify
modules that should be loaded from external sources rather than being bundled with your application.

External modules are integrated with the `<embed>` tag; the external web-page must support being embedded in an iframe.

````json
{
  "Shell": {
    "SortIndex": {
      "example": 10
    },
    "ExternalModules": [
      {
        "Route": "example",
        "Title": "Example",
        "Url": "http://www.example.com",
        "Icon": "globe"
      }
    ]
  }
}
````
