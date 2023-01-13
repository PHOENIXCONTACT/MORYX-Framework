---
uid: ProductsUIOverview
---
# UI Products

The Product UI is responsible to provide all needed information about a product which are devided in its aspects. The Product UI can be extended and also has variable content.

The following possibilities are supported:

- Replace complete detail view
- Replace only product aspects
- De- and activtate aspects
- Replace recipe details

## Aspects

Aspects are presented as `Tabs` whithin the default product detail view. An aspect allows to create a view for specific information of a product like properties, parts or recipes. It is also possible to create aspects to show a combination of the mentioned information. The `EditMode` like a `BeginEdit`, `EndEdit` or `CancelEdit` will be handled by the product itself but an aspect get this call before the product to have the possibility to store some aspect specific information. Each aspect can be enabled by adding them to the [ModuleConfig](xref:Moryx.Products.UI.Interaction.ModuleConfig) like in the following example:

````json
{
  "AspectConfigurations": [
    {
      "PluginName": "PropertiesAspectViewModel"
    },
    {
      "PluginName": "PartsAspectViewModel"
    },
    {
      "PluginName": "RecipesAspectViewModel"
    },
    {
      "PluginName": "RelationsAspectViewModel"
    },
    {
      "PluginName": "WatchFaceAspectViewModel"
    }
  ],
  "ConfigState": "Valid"
}
````

Additionally aspects show/hide themself if they are not relevant. For example: The `PropertiesAspectViewModel` is not *relevant* if the product has no properties to configure.

For adding a custom aspect the [ProductAspectViewModelBase](Moryx.Products.UI.Interaction.Aspects.ProductAspectViewModelBase) can be used to implement an own view model. It provides the whole `IEditableObject` methods and also `Load` and `Save`. It is possible to load additional information from the server with the asynchronous `Load` method.

## Custom DetailView

### Functions of the base class

The [ProductDetailsViewModelBase](xref:Moryx.Products.UI.Interaction.ProductDetailsViewModelBase) is responsible to load the basic product data. Every known data will be loaded into the [ProductViewModel](xref:Moryx.Products.UI.ProductViewModel): `Recipes`, `Properties` and `Parts`. A deeper description can be found in the class definition.