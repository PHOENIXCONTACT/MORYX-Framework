---
uid: MasterDetailUIs
---
# Type based MasterDetail UIs

Within the abstraction layer we use the pattern of type based master detail views very often. Currently the `Products UI` and Resource UI are using this concept.

![Basic structure](images/BasicStructure.png)

Type based detail views will select a details view from a type given from the master. 
As an example: The Resource UI will select the detail view regarding the given resource type name.

- Master Type A will use *ADetailsViewModel*
- Master Type B will use *BDetailsViewModel*
- Master Type C will use *DefaultDetailsViewModel*
- Master Type D will use *DefaultDetailsViewModel*
- null will use EmptyDetailsViewModel

For master type **C** and **D** no special details view model are existant so a default view model will be selected.

## General Concept

Several base classes were created while implementing this feature.

The [IDetailsFactory](xref:Moryx.AbstractionLayer.UI.IDetailsFactory`1) is a view model factory which creates the special view models defined by the type.

````cs
T Create();
T Create(string typeName);
void Destroy(IScreen screen);
````

The factory has to be derived in order to register it in the local container of the module. The factory will be implemented by the local container.

## DetailsComponentSelector

Because of the selection of a component by a type name the [DetailsComponentSelector](xref:Moryx.AbstractionLayer.UI.DetailsComponentSelector´2) has to be derived to define the type of the details base interface. The component selector will automatically initialize the created component with a implementation of [IInteractionController](xref:Moryx.AbstractionLayer.UI.IInteractionController) when the component is implementing the [IDetailsViewModel](xref:Moryx.AbstractionLayer.UI.IDetailsViewModel) interface.

The base implementation will return a default view model if the requested type is not found. The default type name is defined within a constant class in [DetailsConstants](xref:Moryx.AbstractionLayer.UI.DetailsConstants)

As an example the `ProductDetailsComponentSelector` will define the [IProductDetails](xref:Moryx.Products.UI.IProductDetails) as the interface for the detail components. Additionally the `IProductsController` will be used as controller for initializing the instance.

````cs
[Plugin(LifeCycle.Singleton)]
internal class ProductDetailsComponentSelector : DetailsComponentSelector<IProductDetails, IProductsController>
{
    public ProductDetailsComponentSelector(IContainer container, IProductsContoller controller) : base(container, controller)
    {
    }
}
````

The component selector must be used within the `PluginFactory` so that the container can add the correct implementation of the factory:

````cs
[PluginFactory(typeof(ProductDetailsComponentSelector))]
internal interface IProductDetailsFactory : IDetailsFactory<IProductDetails>
{

}
````

## DetailsRegistrationAttribute

For the registration of the types of view models, the base class for details view models should be used: [DetailsRegistrationAttribute](xref:Moryx.AbstractionLayer.UI.DetailsRegistrationAttribute). The constructor can be overloaded to add the specialized details type:

````cs
public class ProductDetailsRegistrationAttribute : DetailsRegistrationAttribute
{
    public ProductDetailsRegistrationAttribute(string typeName)
       : base(typeName, typeof(IProductDetails))
    {

    }
}
````