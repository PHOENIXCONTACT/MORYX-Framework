---
uid: ProductsUIOverview
---
# UI Products

The Product UI is responsible to provide all needed information about a product which are devided in its aspects. The Product UI can be extended and also has variable content.

The following posibilities are supported:

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

Additionally aspects show/hide themself if they are not relevant. For example: The `PropertiesAspectViewModel` is not *relevant* if the the product have no properties to configure.

For adding a custom aspect the [ProductAspectViewModelBase](Moryx.Products.UI.Interaction.Aspects.ProductAspectViewModelBase) can be used to implement an own view model. It provides the whole `IEditableObject` methods and also `Load` and `Save`. It is possible to load additional information from the server with the asynchronous `Load` method.

The following snipped is a sample implementation for the `WatchfaceProduct`:

````cs
[ProductAspectRegistration(nameof(WatchFaceAspectViewModel))]
public class WatchFaceAspectViewModel : ProductAspectViewModelBase
{
    public override string DisplayName => "Watchface";

    public override bool IsRelevant(ProductViewModel product)
    {
        return product.Model.Type == "WatchfaceProduct";
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();
    }
}

````

## Custom DetailView

### Functions of the base class

The [ProductDetailsViewModelBase](xref:Moryx.Products.UI.Interaction.ProductDetailsViewModelBase) is responsible to load the basic product data. Every known data will be loaded into the [ProductViewModel](xref:Moryx.Products.UI.ProductViewModel): `Recipes`, `Properties` and `Parts`. A deeper description can be found in the class definition.

### Add specialized detail views

The detail view can be replaced with specialized views for specialized product types. The view will be selected by the product type.

Create a view model and inherit the [ProductDetailsViewModelBase](xref:Moryx.Products.UI.Interaction.ProductDetailsViewModelBase) and implement the needed methods. To register this new view model, add the [ResourceDetailsRegistrationAttribute](xref:Moryx.Resources.UI.ResourceDetailsRegistrationAttribute) on top of the class.

The [ProductDetailsRegistration](xref:Moryx.Products.UI.ProductDetailsRegistrationAttribute) need a parameter *typeName*. It will define the product type for which the view model should be used for. Additionally add a user control which have the same name as the view model (without the model at the end) to fit the naming convension: TestChildProductView

````cs
[ProductDetailsRegistration("TestChildProduct")]
internal class TestChildProductViewModel : ProductDetailsViewModelBase
{

}
````