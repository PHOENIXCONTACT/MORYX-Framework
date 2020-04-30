---
uid: HowToCreateAResource
---
# How to create a resource

This tutorial shows how [Resource](xref:Marvin.AbstractionLayer.Resources.Resource) should be implemented. Look [here](xref:ResourceConcept) if you are not firm with `Resource`. This tutorial describes how a basic `Resource` is created. Other specializations are [Public resources](xref:Marvin.AbstractionLayer.Resources.PublicResource), [Driver resources](xref:DriverResource) or [Interaction resources](xref:InteractionResource).

## Basic resource files

A resource has this basic solution structure which can be extended for your needs:

````fs
-Marvin.Resource.ExampleResource
|-IExampleResource.cs
|-ExampleResource.cs
````

The interface `IExampleResource` is the main interface for the driver and important for registration within the AbstractionLayer. The implementation of this interface is done with the `ExampleResource` class.

### The interface

This interface is simply derived from [IResource](xref:Marvin.AbstractionLayer.Resources.IResource). No further definitions are needed.

````cs
using Marvin.AbstractionLayer.Resources;

namespace Marvin.Resources.Samples.DriverTutorial
{
    public interface IExampleResource : IResource
    {
    }
}
````

### The implementation

Now implement `IExampleResource`:

````cs
using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Resources;
using Marvin.Serialization;

namespace Marvin.Resources.Samples.DriverTutorial
{
    [ResourceRegistration]
    [DisplayName("Example Resource"), Description("An example resource")]
    public class ExampleResource : Resource, IExampleResource
    {
        [DataMember, EditorBrowsable]
        public string AStringValue { get; set; }

        [DataMember, EditorBrowsable]
        public int AnIntValue { get; set; }

        public string ANonEditorBrowsableMember { get; set; }

        [EditorBrowsable, DisplayName("Square"), Description("Just multiplies given value with itself")]
        public int Square(int value)
        {
            return value * value;
        }
    }
}
````

The implementation of the `ExampleResource` derives from the [Resource](xref:Marvin.AbstractionLayer.Resources.Resource) base class. It also implements the `IResource` interface. The next important thing is the [ResourceRegistration attribute](xref:Marvin.AbstractionLayer.Resources.ResourceRegistrationAttribute). The AbstractionLayer can now identify this class as a resource. Additional attributes like `DisplayName` and `Description` are used within the Resource UI.

## How to use the Resource in a custom module

If you want to use the new resource from a custom module, you need to request the resource from the [ResourceManagement](xref:Marvin.AbstractionLayer.Resources.IResourceManagement). Inject the `ResourceManagement` into the `ModuleController` and pass the object to the inner container of your custom module.

````cs
    [ServerModule(ModuleName)]
    public class ModuleController : ServerModuleBase<ModuleConfig>
    {
        //Let the component be injected from the external container
        [RequiredModuleApi(IsOptional = false, IsStartDependency = true)]
        public IResourceManagement ResourceManagement { get; set; }

        public override string Name
        {
           ///
        }

        #region state transition
        protected override void OnInitialize()
        {
            //pass the component to the inner container
            Container.SetInstance(ResourceManagement);

            ResourceManagement.CapabilitiesChanged += ExampleResourceCapabilityChanged;
        }

        protected override void OnStart()
        {
            ///
        }

        protected override void OnStop()
        {
           ///
        }
        #endregion

    }
````

## CapabilityChanged event

If it is nesessary to react when a capability has changed, is it possible to attach to the [CapabilityChanged](xref:Marvin.AbstractionLayer.Resources.IResourceManagement.CapabilitiesChanged) event:

````cs
private void ExampleResourceCapabilityChanged(object sender, ICapabilities newCapabilities)
{
    var id = ((IResource)sender).Id;

    // Do something
}
````