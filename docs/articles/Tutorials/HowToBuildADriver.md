---
uid: HowToBuildADriver
---
# How to build a driver

A driver is the interface between the `ControlSystem` and physical parts of the production environment. This tutorial shows how a driver should be implemented.

## Basic driver files

A driver has this basic solution structure which can be extended for your needs:

````fs
-Marvin.Driver.ExampleDriver
|-IExampleDriver.cs
|-ExampleDriver.cs
````

The interface `IExampleDriver` is the main interface for the driver and important for registration within the AbstractionLayer. The implementation of this interface is done with the `ExampleDriver` class.

### The interface

This interface is simply derived from [IDriver](xref:Marvin.AbstractionLayer.Drivers.IDriver). No further definitions are needed.

````cs
using Marvin.AbstractionLayer.Drivers;

namespace Marvin.Resources.Samples.DriverTutorial
{
    public interface IExampleDriver : IDriver
    {
    }
}
````

### The implementation

Now implement `IDriver`:

````cs
using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Drivers;
using Marvin.AbstractionLayer.Resources;
using Marvin.Serialization;

namespace Marvin.Resources.Samples.DriverTutorial
{
    [ResourceRegistration]
    [DisplayName("Example Driver"), Description("An example driver")]
    public class ExampleDriver : Driver, IExampleDriver
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

The implementation of the `ExampleDriver` derives from the [Driver](xref:Marvin.AbstractionLayer.Drivers.Driver) base class. It also implements the `IDriver` interface. The next important thing is the [ResourceRegistration attribute](xref:Marvin.AbstractionLayer.Resources.ResourceRegistrationAttribute). The AbstractionLayer can now identify this driver as a resource. Additional attributes like `DisplayName` and `Description` are used within the Resource UI.

![ResourceUI](images\ExampleDriverResourceUI.png)

The two properties `AStringValue` and `AnIntValue` are shown in the ResourceUI and can be edited by the user. The member `ANonEditorBrowsableMember` is invisible for the user and is only used inside the AbstractionLayer.
The `Square` function is also visible in the Resource UI. And: It is callable from there.

## Additional things that are good to know

The `ExampleDriver` is just a simple implementation for a driver. As like every [Resource](xref:Marvin.AbstractionLayer.Resources.Resource) you can `Initialize`, `Start`, `Stop` a driver. Also `State machine` support is built in:

````cs
using Marvin.AbstractionLayer.Drivers;
using Marvin.AbstractionLayer.Resources;

namespace Marvin.Resources.Samples.DriverTutorial
{
    [ResourceRegistration]
    [DisplayName("StateExample Driver"), Description("An example driver that uses the state machine")]
    public class StateExampleDriver : Driver, IExampleDriver
    {
        ...

        /// <seealso cref="IDriver"/> 
        public override void Initialize()
        {
            base.Initialize();

            StateMachine.Initialize<ExampleStateBase>(this);
        }

        ...
    }
}
````

## When to use a driver

If you want to communicate with the outside world like a database , a `PhoenixPlc` device, scanner or bar code reader you should implement it as a driver.