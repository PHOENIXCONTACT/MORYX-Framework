# Resources

## UI

The resources UI mainly will be used to configure the current manufacutiring system. 

### Concept

The resource ui mainly is structured as a tree navigation. On the left side, the user can see the complete resource tree. If the user click on an tree item, a resource type specific details view will be opened. 
If no type specific resource details view was found, a default implementation will be used. The UI is using the concept of [Type specific detail views](xref:concepts-TypeBasedMasterDetailUis).

### Sample: Serial Number Provider

The serial number provider handles serial numbers and mac adresses. All number types wil have their own value buffer. The details view is 
build to configure the value buffer for each number type. On top, general resouce information are visible (name, local-, global identifier).
Below the genral information, the value buffer settings are visible. Each number type can be configured itself. 

![](images\ResourceUI\resourceUi_snProvider.png)

### Add specialized detail views

The resources ui can be extended with specialized views for specialized resources. The view will be selected with the resource type. 

Create an view model which will implement the [ResourceDetailsViewModelBase](xref:Marvin.Resources.UI.Interaction.ResourceDetailsViewModelBase).
To register this new view model, add the [ResourceDetailsRegistrationAttribute](xref:Marvin.Resources.UI.ResourceDetailsRegistrationAttribute) on top of the class.

The [ResourceDetailsRegistrationAttribute](xref:Marvin.Resources.UI.ResourceDetailsRegistrationAttribute) need a parameter *typeName*. It will define the resource type for which the view model should be used for.

````cs
[ResourceDetailsRegistration("Machine")]
internal class MachineDetailsViewModel : ResourceDetailsViewModelBase
{

}
````

### Register your own model/controller {#registerOwnModel}

You only will register the view model for the resource details. The view model should not contain any connection to sub systems. So for this, you need a dedicated model (or controller) 
who will conntect to you specialized service.

The resource ui module will register all implementations of [IResourceInteractionController](xref:Marvin.Resources.UI.IResourceInteractionController) and will start them.
A base implementation is the [ResourceInteractionControllerBase](xref:Marvin.Resources.UI.ResourceInteractionControllerBase) which also implements the [HttpServiceConnectorBase](xref:Marvin.Tools.Wcf.HttpServiceConnectorBase)
to connect to some BasicHTTP web service.
To register the controller to the container, add the [ResourceInteractionRegistrationAttribute](xref:Marvin.Resources.UI.ResourceInteractionRegistrationAttribute) on top of the class which registers 
the implementation. The attribute have also a params parameter to add custom services which the components exports. You can add your interface for your controller. With this service registration, your
controller is available in your view model.

````cs
[ResourceInteractionRegistration(typeof(IYourCustomController))]
internal class ResourceController : ResourceInteractionControllerBase<ResourceInteractionClient, IResourceInteraction>, IYourCustomController
{
    protected override string MinServerVersion
    {
        get { return "1.0.0"; }
    }

    protected override string ClientVersion
    {
        get { return "1.0.0"; }
    }
}
````