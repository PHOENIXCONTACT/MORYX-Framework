# Factory 4.x to 6.x

For this major release the main motivation was switching from our own MORYX Runtime to ASP.NET Core with MORYX extensions.
It includes replacing EntityFramework 6 with EntityFramework Core as well as the replacement of the First Level DI-Container. 
We also removed our own implementation of an injectable logger and will use the very similar logger API from Microsoft from now on.
Regarding the configuration of our module, we are able to simplify things here as well. 
Lastly, we remove the support for .NET Framework with the step to MORYX ControlSystem 6
For more information, please refer to the respective paragraphs below.

## .NET Framework
MORYX ControlSystem no longer supports the legacy .NET Framework and is only available for .NET 6.0 and above. There is also no more WCF support or embedded kestrel hosting. Instead standard ASP.NET API-Controllers can be used that import MORYX components (kernel, modules or facade).

## Facade Interfaces
- `IWorkerSupportExtended` is replaced by `IWorkerSupport`
- `IOrderManagementExtended` is replaced by `IOrderManagement`

## Setup Information
Basically there was a solution in the ISetupProvider which will return a setup recipe for a given production recipe if there are some necessary setup steps.
The additional solution with the IJobManagement is removed. This includes the following points:
- _JobEvaluation_ not includes the _RequiredSetup_ property anymore
- The class _SetupStep_ is deleted

All the points are used just to inform the Orders UI that there is a setup necessary and to show an indicator at the operation. This can also be done by just asking the _ISetupProvider_ for a setup recipe. If the answer is not null then a setup is necessary and an indicator can be displayed. A setup can be outdated after a capabilitiy change, a new added resource or after removing a resource. All information can be gotten from the AbstractionLayer and can be handled in the UI.
