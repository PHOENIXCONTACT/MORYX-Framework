# MORYX Architecture Design Guidelines

Within the MORYX-Platform you will encounter several different concepts for implementing different functionalities.
In the following text, you will get a short summary of what concepts there are with a comprehensive visualization how they are orchestrated.
To jump directly to a more detailed explanation and a set of Best Practices for the respective concept select it in the list below.

The different terms for softare constructs you will (probably) come across when working within the MORYX-Platform:

- [TODO:Modules](https://github.com/PHOENIXCONTACT/MORYX-Home/tree/main/development/architecture/components.md#modulelifecycle-componentwork) (with their [Components](https://github.com/PHOENIXCONTACT/MORYX-Home/tree/main/development/architecture/components.md) and *TODO:Plugins*)
- *TODO:Adapters*
- [Endpoints](https://github.com/PHOENIXCONTACT/MORYX-Home/tree/main/development/architecture/endpoints.md)
- *TODO:Web-UIs*

- [Resources](https://github.com/PHOENIXCONTACT/MORYX-Framework/blob/dev/docs/articles/Resources/Overview.md) (with their [Drivers](https://github.com/PHOENIXCONTACT/MORYX-Framework/blob/dev/docs/articles/Resources/Types/DriverResource.md))
- [Products](https://github.com/PHOENIXCONTACT/MORYX-Framework/blob/dev/docs/articles/Products/Concept.md)

The figure provides an overview of the orchestration of the different concepts while the subsequent statements give a first intuition on how their purposes are separated.

![Moryx-Platform](/docs/images/MORYX-Platform-Overview.png)

*Modules* enclose a reusable piece of functionality accessible through their Facades.
They are the most complex and most powerful entities in the platform.

*Adapters* are closely related to Modules and their code structure is fairly similar.
They differ from a Module in their purpose.
The functionality of an Adapter is solely targeted upon the interaction with a specific IT-System (e.g. SAP, TeamCenter etc.).
It actively engages in an interaction with the IT system.

*Endpoints* consume a Module's Facade and make it externally (i.e. to other processes) accessible based on a certain technology (e.g. REST-Endpoint, MQTT-Endpoint, etc.).
Separating Endpoints from Adapters, an Endpoint is passive in the sense that it is provided and can be used freely.
Adapters on the other hand might use similar technology as an Endpoint, but their purpose is to interact with an IT-System which influences the design process significantly as well.  

*Web-UIs* are Web pages, which are -in most cases- directly related to a Module.
They consume an Endpoint of the Module to dynamically load data and provide functionality.
A Web-UI might of course consume multiple Endpoints to improve the user experience.

*Resources* and *Products* are digital twins of the produced and producing physical things in an application.
The physical counterpart makes a resource clearly separable from a module, even though it might not be obvious every time, which of the two you need to choose at first glance.
