---
uid: DriverResource
---
# DriverResource

Drivers refer to components that manage the communication with another device and provide access to that device within the AL over a simple API. While in the best drivers were separate type of plugin, they are now just specialized resources. They follow the same life cycle and configuration. Their interface IDriver is a direct descendant of IResource and deﬁnes a State property and changed event. Other driver interfaces like those for a PLC or RFID reader are derived from IDriver and should ideally offer an easy, but generic API. For a resource developer it is supposed to make the interaction with the device easy without being too speciﬁc for a single device.

Have also a look on the tutorial [how to build a driver](xref:HowToBuildADriver).