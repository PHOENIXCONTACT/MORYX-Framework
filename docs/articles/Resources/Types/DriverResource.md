---
uid: DriverResource
---
# DriverResource

Drivers refer to components that manage the communication with another device and provide access to that device within the `AbstractionLayer` over a simple API. 
As a special type of resource they follow the same life cycle and configuration. 
Their interface [IDriver](xref:Moryx.AbstractionLayer.Drivers.IDriver) is a direct descendant of [IResource](xref:Moryx.AbstractionLayer.Resources.IResource) and deﬁnes a State property and changed event. 
Other driver interfaces like those for a Programable Logic Controller (PLC) or an RFID reader are derived from IDriver and should ideally offer an easy, but generic API. 
For a resource developer it is supposed to make the interaction with the device easy without being too speciﬁc for a single device.

Have also a look on the tutorial [how to build a driver](../../Tutorials/HowToBuildADriver.md).