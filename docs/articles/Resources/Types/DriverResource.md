---
uid: DriverResource
---
# DriverResource

Drivers refer to components that manage the communication with another device and provide access to that device within the `AbstractionLayer` over a simple API. 
As a special type of resource they follow the same life cycle and configuration. 
Their interface [IDriver](../../../../src/Moryx.AbstractionLayer/Drivers/IDriver.cs) is a direct descendant of [IResource](../../../../src/Moryx.AbstractionLayer/Resources/IResource.cs) and deﬁnes a State property and changed event. 
Other driver interfaces like those for a Programable Logic Controller (PLC) or an RFID reader are derived from IDriver and should ideally offer an easy, but generic API. 
For a resource developer it is supposed to make the interaction with the device easy without being too speciﬁc for a single device.

Also have a look on the tutorial [how to build a driver](/docs/tutorials/HowToBuildADriver.md).