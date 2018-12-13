---
uid: ProductsUIOverview
---
# UI Products

The products ui is structured as a [tree navigation](xref:ProductTree). On the left side, the user can see the complete product structure. If the user click on a tree item, a product type specific [details view](xref:ProductDetails). will be opened. If no type specific details view was found, a default implementation will be used.

![Products workspace](images\ProductsWorkspace.png)

## Functions of the base class

The [ProductDetailsViewModelBase](xref:Marvin.Products.UI.Interaction.ProductDetailsViewModelBase) is responsible to load the basic product data. Every known data will be put in protected properites: `Recipes[]`, `CurrentRecipe` and `ProductProperties` as an array of `Entry[]`. A deeper description can be found in the class definition.

## Add specialized detail views

The products ui can be extended with specialized views for specialized product types. The view will be selected with the product structure type.

Create a view model and inherit the [ProductDetailsViewModelBase](xref:Marvin.Products.UI.Interaction.ProductDetailsViewModelBase) and implement the needed methods. To register this new view model, add the [ResourceDetailsRegistrationAttribute](xref:Marvin.Resources.UI.ResourceDetailsRegistrationAttribute) on top of the class.

The [ProductDetailsRegistration](xref:Marvin.Products.UI.ProductDetailsRegistrationAttribute) need a parameter *typeName*. It will define the product type for which the view model should be used for. Additionally add a user control which have the same name as the view model (without the model at the end) to fit the naming convension: TestChildProductView

````cs
[ProductDetailsRegistration("TestChildProduct")]
internal class TestChildProductViewModel : ProductDetailsViewModelBase
{

}
````