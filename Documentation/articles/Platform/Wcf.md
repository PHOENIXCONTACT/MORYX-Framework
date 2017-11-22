---
uid: Wcf
---
Wcf Connections
===============

## Concept for data transmission between client and server #

**Glossary**
| Term | Description |
|------|-------------|
| ServiceModel | Also known as client side *Controller*. Implements the connection handling of the service reference |
| Controller | Also known as client side *ServiceModel*. Implements the connection handling of the service reference |

To avoid the problems which occured when we used direct ServiceReferences for the different modules of each client, the following concept was
developed. Below the disadvantages of the previous approach are initially listed. Then the new concept will be described, 
that enables a unified way of data exchange between client and server. Finally the advantages of this concept will be discussed. 

**Disadvantages of the past concept**
* Every client module have to implement the needed ServiceReferences by itself. This leads to code duplication.
* If a service changes the references must be updated at many different client modules.
* Every client module must implement its own DTO-ViewModels, which also leads to duplicated code.
* Different client modules may need the same data but have to load it by itself.
* Available facades leads to a high coupling between the client modules.
    
### Future concept for Platfrom 3

On server side there are different services implemeted, which are seperated by functional areas (e.g. JobService, ProductService). 
These services offer fine-grained methods that also offer filtered data. 

The service interfaces (service interface, data models) are implemented in a different assembly. The assembly should have the same name as the bundle with the extension which have to be defined. Example Products: 

* Server Bundle: Marvin.Products.dll
* UI Bundle: Marvin.Products.UI.dll
* Service Assembly: Marvin.Products.Foo.dll

The _Service Assembly_ encapsulate the access to the referenced service, on the other hand they offer DTO's for these services. 

Basic DTO-ViewModels are also in separate libraries, which are referenced by the client modules (UI Bundle). The following picture illustrates 
this on the example of an client-server-applicaton which works on product data. As seen in the figure, the _Service Assembly_ are offered in the form of dll's.
These dll's are used on client **and** on server side. This also allows the reuse of the DTO's.


* _Advantages of the future concept_
    * The Services must be referenced only one time by a shared library and can be used from multiple client modules without any duplicated code.
    * Basic DTO-ViewModels are also shared by seperate libraries and without duplicated code.
    * Every client module can implement its own model to combine different ServicesModels to get combined information
    * Combined information can also be encapsulated with a self defined DTO-ViewModel.
    * The _Service Assembly_ can be used on c# clients (wpf client, other server, asp.net server)
       
The biggest advantage: The client do not have to generate code for the service because all needed information are available in the _Service Assembly_.