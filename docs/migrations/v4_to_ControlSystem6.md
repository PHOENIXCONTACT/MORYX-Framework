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


# ControlSystem 4.x to 6.x

For this major release the main motivation was switching from our own MORYX Runtime to ASP.NET Core with MORYX extensions.
It includes replacing EntityFramework 6 with EntityFramework Core as well as the replacement of the First Level DI-Container. 
We also removed our own implementation of an injectable logger and will use the very similar logger API from Microsoft from now on.
Regarding the configuration of our module, we are able to simplify things here as well. 
Lastly, we remove the support for .NET Framework with the step to MORYX 6.
For more information regarding the ControlSystem specific changes, please refer to the respective paragraphs below.


## Package changes

Added:


Removed:


Renamed:
- `Moryx.WebShell` --> `Moryx.Launcher`

## Namespaces

## Notifications
Since the interfaces `INotification` and `IManagedNotification` were removed in the Abstraction Layer, you have to use `Notification` directly. 
Interfaces derived from `INotification` were also changed to classes implementing `Notification`
- `IBdeNotification` --> `BdeNotification`
- `IMachinePartNotification` --> `MachinePartNotification`
- `IProcessRelatedNotification` --> `ProcessRelatedNotification` 
## Facade Interfaces
- `IWorkerSupportExtended` is replaced by `IWorkerSupport`
- `IOrderManagementExtended` is replaced by `IOrderManagement`

## EntityFramework becomes EntityFramework Core
With the update of MORYX Core we switched to the current version of EntityFramework and with that come a few changes to the database structure of our modules. The subsequent scripts can be executed in the PgAdmin SQL editor of the respective module databases. Afterwards you are equiped with EF Core compatible databases while keeping the existing data ofcourse.

### Removed `Create()` in some `IRepository<>` interfaces

* `IActivityEntityRepository`
* `IProcessEntityRepository`

In case you used any of these `Create()` functions, please update your code by
using ef core's `DbContext`. As an example for `IActivitiyEntityRepository` the
changes could look like this:

```csharp
    // before
    var activityEntity = uow.GetRepository<IActivityEntityRepository>().Create(activityData.Task.Id, activityData.Resource.Id);
    activityEntity.ProcessId = processData.Id;
    activityEntity.Id = IdShiftGenerator.Generate(processEntity.Id, ProcessTestsBase.NextId);

    // after
    var activityEntity = uow.DbContext.ActivityEntities.Add(new()
    {
        Id = IdShiftGenerator.Generate(processEntity.Id, ProcessTestsBase.NextId),
        TaskId = activityData.Task.Id,
        ResourceId = activityData.Resource.Id,
        ProcessId = processData.Id,
    }).Entity;
```

### MORYX Machine Connector
```sql
-- Rename tables, indices and keys to match pluralized names
ALTER TABLE public."MachineStateEntity" RENAME TO "MachineStateEntities";
ALTER TABLE public."MachineStateEntities" RENAME CONSTRAINT "PK_public.MachineStateEntity" TO "PK_MachineStateEntities";

-- Create the default migration history table 
CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);
ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;
INSERT INTO public."__EFMigrationsHistory"("MigrationId", "ProductVersion") 
VALUES ('20220825105530_InitialCreate', '3.1.0');
ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");
	
--Drop the old migration table
DROP TABLE public."__MigrationHistory"
```

### MORYX Notifications
```sql
-- Rename tables, indices and keys to match pluralized names
ALTER TABLE public."NotificationEntity" RENAME TO "NotificationEntities";
ALTER TABLE public."NotificationEntities" RENAME CONSTRAINT "PK_public.NotificationEntity" TO "PK_NotificationEntities";
ALTER TABLE public."NotificationEntities" RENAME CONSTRAINT "FK_public.NotificationEntity_public.NotificationTypeEntity_Type" TO "FK_NotificationEntities_NotificationTypeEntities_TypeId";
ALTER INDEX "NotificationEntity_IX_TypeId" RENAME TO "IX_NotificationEntities_TypeId";

ALTER TABLE public."NotificationTypeEntity" RENAME TO "NotificationTypeEntities";
ALTER TABLE public."NotificationTypeEntities" RENAME CONSTRAINT "PK_public.NotificationTypeEntity" TO "PK_NotificationTypeEntities";


-- Create the default migration history table 
CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);
ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;
INSERT INTO public."__EFMigrationsHistory"("MigrationId", "ProductVersion") 
VALUES ('20220825085020_InitialCreate', '3.1.0');
ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");
	
--Drop the old migration table
DROP TABLE public."__MigrationHistory"
```

### Moryx Orders
```sql
-- Rename tables, indices and keys to match pluralized names
ALTER TABLE public."OperationAdviceEntity" RENAME TO "OperationAdviceEntities";
ALTER TABLE public."OperationAdviceEntities" RENAME CONSTRAINT "PK_public.OperationAdviceEntity" TO "PK_OperationAdviceEntities";
ALTER TABLE public."OperationAdviceEntities" RENAME CONSTRAINT "FK_public.OperationAdviceEntity_public.OperationEntity_Operatio" TO "FK_OperationAdviceEntities_OperationEntities_OperationId";
ALTER INDEX "OperationAdviceEntity_IX_OperationId" RENAME TO "IX_OperationAdviceEntities_OperationId";

ALTER TABLE public."OperationEntity" RENAME TO "OperationEntities";
ALTER TABLE public."OperationEntities" RENAME CONSTRAINT "PK_public.OperationEntity" TO "PK_OperationEntities";
ALTER TABLE public."OperationEntities" RENAME CONSTRAINT "FK_public.OperationEntity_public.OrderEntity_OrderId" TO "FK_OperationEntities_OrderEntities_OrderId";
ALTER INDEX "OperationEntity_IX_OrderId" RENAME TO "IX_OperationEntities_OrderId";

ALTER TABLE public."OperationJobReferenceEntity" RENAME TO "OperationJobReferenceEntities";
ALTER TABLE public."OperationJobReferenceEntities" RENAME CONSTRAINT "PK_public.OperationJobReferenceEntity" TO "PK_OperationJobReferenceEntities";
ALTER TABLE public."OperationJobReferenceEntities" RENAME CONSTRAINT "FK_public.OperationJobReferenceEntity_public.OperationEntity_Op" TO "FK_OperationJobReferenceEntities_OperationEntities_OperationId";
ALTER INDEX "OperationJobReferenceEntity_IX_OperationId" RENAME TO "IX_OperationJobReferenceEntities_OperationId";

ALTER TABLE public."OperationRecipeReferenceEntity" RENAME TO "OperationRecipeReferenceEntities";
ALTER TABLE public."OperationRecipeReferenceEntities" RENAME CONSTRAINT "PK_public.OperationRecipeReferenceEntity" TO "PK_OperationRecipeReferenceEntities";
ALTER TABLE public."OperationRecipeReferenceEntities" RENAME CONSTRAINT "FK_public.OperationRecipeReferenceEntity_public.OperationEntity" TO "FK_OperationRecipeReferenceEntities_OperationEntities_Operatio~";
ALTER INDEX "OperationRecipeReferenceEntity_IX_OperationId" RENAME TO "IX_OperationRecipeReferenceEntities_OperationId";

ALTER TABLE public."OperationReportEntity" RENAME TO "OperationReportEntities";
ALTER TABLE public."OperationReportEntities" RENAME CONSTRAINT "PK_public.OperationReportEntity" TO "PK_OperationReportEntities";
ALTER TABLE public."OperationReportEntities" RENAME CONSTRAINT "FK_public.OperationReportEntity_public.OperationEntity_Operatio" TO "FK_OperationReportEntities_OperationEntities_OperationId";
ALTER INDEX "OperationReportEntity_IX_OperationId" RENAME TO "IX_OperationReportEntities_OperationId";

ALTER TABLE public."OrderEntity" RENAME TO "OrderEntities";
ALTER TABLE public."OrderEntities" RENAME CONSTRAINT "PK_public.OrderEntity" TO "PK_OrderEntities";

ALTER TABLE public."ProductPartEntity" RENAME TO "ProductPartEntities";
ALTER TABLE public."ProductPartEntities" RENAME CONSTRAINT "PK_public.ProductPartEntity" TO "PK_ProductPartEntities";
ALTER TABLE public."ProductPartEntities" RENAME CONSTRAINT "FK_public.ProductPartEntity_public.OperationEntity_OperationId" TO "FK_ProductPartEntities_OperationEntities_OperationId";
ALTER INDEX "ProductPartEntity_IX_OperationId" RENAME TO "IX_ProductPartEntities_OperationId";

-- Create the default migration history table 
CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);
ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;
INSERT INTO public."__EFMigrationsHistory"("MigrationId", "ProductVersion") 
VALUES ('20220825105205_InitialCreate', '3.1.0');
ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");
	
--Drop the old migration table
DROP TABLE public."__MigrationHistory"
```

### MORYX Processes
```sql
-- Rename tables, indices and keys to match pluralized names
ALTER TABLE public."ActivityEntity" RENAME TO "ActivityEntities";
ALTER TABLE public."ActivityEntities" RENAME CONSTRAINT "PK_public.ActivityEntity" TO "PK_ActivityEntities";
ALTER TABLE public."ActivityEntities" RENAME CONSTRAINT "FK_public.ActivityEntity_public.JobEntity_JobId" TO "FK_ActivityEntities_JobEntities_JobId";
ALTER TABLE public."ActivityEntities" RENAME CONSTRAINT "FK_public.ActivityEntity_public.ProcessEntity_ProcessId" TO "FK_ActivityEntities_ProcessEntities_ProcessId";
ALTER TABLE public."ActivityEntities" RENAME CONSTRAINT "FK_public.ActivityEntity_public.TracingType_TracingTypeId" TO "FK_ActivityEntities_TracingTypes_TracingTypeId";
ALTER INDEX "ActivityEntity_IX_JobId" RENAME TO "IX_ActivityEntities_JobId";
ALTER INDEX "ActivityEntity_IX_ProcessId" RENAME TO "IX_ActivityEntities_ProcessId";
ALTER INDEX "ActivityEntity_IX_TracingTypeId" RENAME TO "IX_ActivityEntities_TracingTypeId";

ALTER TABLE public."JobEntity" RENAME TO "JobEntities";
ALTER TABLE public."JobEntities" RENAME CONSTRAINT "PK_public.JobEntity" TO "PK_JobEntities";
ALTER TABLE public."JobEntities" RENAME CONSTRAINT "FK_public.JobEntity_public.JobEntity_PreviousId" TO "FK_JobEntities_JobEntities_PreviousId";
ALTER INDEX "JobEntity_IX_PreviousId" RENAME TO "IX_JobEntities_PreviousId";

ALTER TABLE public."ProcessEntity" RENAME TO "ProcessEntities";
ALTER TABLE public."ProcessEntities" RENAME CONSTRAINT "PK_public.ProcessEntity" TO "PK_ProcessEntities";
ALTER TABLE public."ProcessEntities" RENAME CONSTRAINT "FK_public.ProcessEntity_public.JobEntity_JobId" TO "FK_ProcessEntities_JobEntities_JobId";
ALTER INDEX "ProcessEntity_IX_JobId" RENAME TO "IX_ProcessEntities_JobId";

ALTER TABLE public."TokenHolderEntity" RENAME TO "TokenHolderEntities";
ALTER TABLE public."TokenHolderEntities" RENAME CONSTRAINT "PK_public.TokenHolderEntity" TO "PK_TokenHolderEntities";
ALTER TABLE public."TokenHolderEntities" RENAME CONSTRAINT "FK_public.TokenHolderEntity_public.ProcessEntity_ProcessId" TO "FK_TokenHolderEntities_ProcessEntities_ProcessId";
ALTER INDEX "TokenHolderEntity_IX_ProcessId" RENAME TO "IX_TokenHolderEntities_ProcessId";

ALTER TABLE public."TracingType" RENAME TO "TracingTypes";
ALTER TABLE public."TracingTypes" RENAME CONSTRAINT "PK_public.TracingType" TO "PK_TracingTypes";

-- Create the default migration history table 
CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);
ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;
INSERT INTO public."__EFMigrationsHistory"("MigrationId", "ProductVersion") 
VALUES ('20220831123023_InitialCreate', '3.1.0');
ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");
	
--Drop the old migration table
DROP TABLE public."__MigrationHistory"
```

### MORYX Users
```sql
-- Rename tables, indices and keys to match pluralized names
ALTER TABLE public."UserEntity" RENAME TO "UserEntities";
ALTER TABLE public."UserEntities" RENAME CONSTRAINT "PK_public.UserEntity" TO "PK_UserEntities";

-- Create the default migration history table 
CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);
ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;
INSERT INTO public."__EFMigrationsHistory"("MigrationId", "ProductVersion") 
VALUES ('20220825105836_InitialCreate', '3.1.0');
ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");
	
--Drop the old migration table
DROP TABLE public."__MigrationHistory"
```



### MORYX Resources
```sql
-- Rename tables, indices and keys to match pluralized names
ALTER TABLE public."ResourceEntity" RENAME TO "Resources";
ALTER TABLE public."Resources" RENAME CONSTRAINT "PK_public.ResourceEntity" TO "PK_Resources";
ALTER INDEX "ResourceEntity_IX_Name" RENAME TO "IX_Resources_Name";

ALTER TABLE public."ResourceRelation" RENAME TO "ResourceRelations";
ALTER TABLE public."ResourceRelations" RENAME CONSTRAINT "FK_public.ResourceRelation_public.ResourceEntity_SourceId" TO "FK_ResourceRelations_Resources_SourceId";
ALTER TABLE public."ResourceRelations" RENAME CONSTRAINT "FK_public.ResourceRelation_public.ResourceEntity_TargetId" TO "FK_ResourceRelations_Resources_TargetId";
ALTER TABLE public."ResourceRelations" RENAME CONSTRAINT "PK_public.ResourceRelation" TO "PK_ResourceRelations";
ALTER INDEX "ResourceRelation_IX_SourceId" RENAME TO "IX_ResourceRelations_SourceId";
ALTER INDEX "ResourceRelation_IX_TargetId" RENAME TO "IX_ResourceRelations_TargetId";

-- Create the default migration history table 
CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);
ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;
INSERT INTO public."__EFMigrationsHistory"("MigrationId", "ProductVersion") 
VALUES ('20211104134830_InitialCreate', '3.1.0');
ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");
	
--Drop the old migration table
DROP TABLE public."__MigrationHistory"
```

### MORYX Products
```sql
-- Rename tables, indices and keys to match pluralized names
ALTER TABLE public."ConnectorEntity" RENAME TO "WorkplanConnectorEntities";
ALTER TABLE public."WorkplanConnectorEntities" RENAME CONSTRAINT "PK_public.ConnectorEntity" TO "PK_WorkplanConnectorEntities";
ALTER TABLE public."WorkplanConnectorEntities" RENAME CONSTRAINT "FK_public.ConnectorEntity_public.WorkplanEntity_WorkplanId" TO "FK_WorkplanConnectorEntities_Workplans_WorkplanId";
ALTER INDEX "ConnectorEntity_IX_Name" RENAME TO "IX_WorkplanConnectorEntities_Name";

ALTER TABLE public."ConnectorReference" RENAME TO "WorkplanConnectorReferences";
ALTER TABLE public."WorkplanConnectorReferences" RENAME COLUMN "StepId" TO "WorkplanStepId";
ALTER TABLE public."WorkplanConnectorReferences" RENAME CONSTRAINT "PK_public.ConnectorReference" TO "PK_WorkplanConnectorReferences";
ALTER TABLE public."WorkplanConnectorReferences" RENAME CONSTRAINT "FK_public.ConnectorReference_public.ConnectorEntity_ConnectorId" TO "FK_WorkplanConnectorReferences_WorkplanConnectorEntities_Conne~";
ALTER TABLE public."WorkplanConnectorReferences" RENAME CONSTRAINT "FK_public.ConnectorReference_public.StepEntity_StepId" TO "FK_WorkplanConnectorReferences_WorkplanSteps_WorkplanStepId";
ALTER INDEX "ConnectorReference_IX_ConnectorId" RENAME TO "IX_WorkplanConnectorReferences_ConnectorId";
ALTER INDEX "ConnectorReference_IX_StepId" RENAME TO "IX_WorkplanConnectorReferences_WorkplanStepId";

ALTER TABLE public."OutputDescriptionEntity " RENAME TO "WorkplanOutputDescriptions";
ALTER TABLE public."WorkplanConnectorReferences" RENAME COLUMN "StepEntityId" TO "WorkplanStepId";
ALTER TABLE public."WorkplanConnectorReferences" RENAME CONSTRAINT "PK_public.OutputDescriptionEntity" TO "PK_WorkplanOutputDescriptions";
ALTER TABLE public."WorkplanConnectorReferences" RENAME CONSTRAINT "FK_public.OutputDescriptionEntity_public.StepEntity_StepEntityI" TO "FK_WorkplanOutputDescriptions_WorkplanSteps_WorkplanStepId";
ALTER INDEX "OutputDescriptionEntity_IX_StepEntityId" RENAME TO "IX_WorkplanOutputDescriptions_WorkplanStepId";

ALTER TABLE public."PartLink" RENAME TO "PartLinks";
ALTER TABLE public."PartLinks" RENAME CONSTRAINT "PK_public.PartLink" TO "PK_PartLinks";
ALTER TABLE public."PartLinks" RENAME CONSTRAINT "FK_public.PartLink_public.ProductTypeEntity_ParentId" TO "FK_PartLinks_ProductTypes_ParentId";
ALTER TABLE public."PartLinks" RENAME CONSTRAINT "FK_public.PartLink_public.ProductTypeEntity_ChildId" TO "FK_PartLinks_ProductTypes_ChildId";
ALTER INDEX "PartLink_IX_ChildId" RENAME TO "IX_PartLinks_ChildId";
ALTER INDEX "PartLink_IX_ParentId" RENAME TO "IX_PartLinks_ParentId";

ALTER TABLE public."ProductFileEntity" RENAME TO "ProductFiles";
ALTER TABLE public."ProductFiles" RENAME CONSTRAINT "PK_public.ProductFileEntity" TO "PK_ProductFiles";
ALTER TABLE public."ProductFiles" RENAME CONSTRAINT "FK_public.ProductFileEntity_public.ProductTypeEntity_Product_Id" TO "FK_ProductFiles_ProductTypes_ProductId";
ALTER INDEX "ProductFileEntity_IX_Product_Id" RENAME TO "IX_ProductFiles_ProductId";

ALTER TABLE public."ProductInstanceEntity" RENAME TO "ProductInstances";
ALTER TABLE public."ProductInstances" RENAME COLUMN "PartLinkId" TO "PartLinkEntityId";
ALTER TABLE public."ProductInstances" RENAME CONSTRAINT "PK_public.ProductInstanceEntity" TO "PK_ProductInstances";
ALTER TABLE public."ProductInstances" RENAME CONSTRAINT "FK_public.ProductInstanceEntity_public.PartLink_PartLinkId" TO "FK_ProductInstances_PartLinks_PartLinkEntityId";
ALTER TABLE public."ProductInstances" RENAME CONSTRAINT "FK_public.ProductInstanceEntity_public.ProductInstanceEntity_Pa" TO "FK_ProductInstances_ProductInstances_ParentId";
ALTER TABLE public."ProductInstances" RENAME CONSTRAINT "FK_public.ProductInstanceEntity_public.ProductTypeEntity_Produc" TO "FK_ProductInstances_ProductTypes_ProductId";
ALTER INDEX "PartLink_IX_ChildId" RENAME TO "IX_PartLinks_ChildId";
ALTER INDEX "PartLink_IX_ParentId" RENAME TO "IX_PartLinks_ParentId";

ALTER TABLE public."ProductProperties" RENAME TO "ProductTypeProperties";
ALTER TABLE public."ProductTypeProperties" RENAME CONSTRAINT "PK_public.ProductProperties" TO "PK_ProductTypeProperties";
ALTER TABLE public."ProductTypeProperties" RENAME CONSTRAINT "FK_public.ProductProperties_public.ProductTypeEntity_ProductId" TO "FK_ProductTypeProperties_ProductTypes_ProductId";
ALTER INDEX "ProductProperties_IX_ProductId" RENAME TO "IX_ProductTypeProperties_ProductId";

ALTER TABLE public."ProductRecipeEntity" RENAME TO "ProductRecipes";
ALTER TABLE public."ProductRecipes" RENAME CONSTRAINT "PK_public.ProductRecipeEntity" TO "PK_ProductRecipes";
ALTER TABLE public."ProductRecipes" RENAME CONSTRAINT "FK_public.ProductRecipeEntity_public.ProductTypeEntity_ProductI" TO "FK_ProductRecipes_ProductTypes_ProductId";
ALTER TABLE public."ProductRecipes" RENAME CONSTRAINT "FK_public.ProductRecipeEntity_public.WorkplanEntity_WorkplanId" TO "FK_ProductRecipes_Workplans_WorkplanId";
ALTER INDEX "ProductRecipeEntity_IX_ProductId" RENAME TO "IX_ProductRecipes_ProductId";
ALTER INDEX "ProductRecipeEntity_IX_WorkplanId" RENAME TO "IX_ProductRecipes_WorkplanId";

ALTER TABLE public."ProductTypeEntity" RENAME TO "ProductTypes";
ALTER TABLE public."ProductTypes" RENAME CONSTRAINT "PK_public.ProductTypeEntity" TO "PK_ProductTypes";
ALTER TABLE public."ProductTypes" RENAME CONSTRAINT "FK_public.ProductTypeEntity_public.ProductProperties_CurrentVer" TO "FK_ProductTypes_ProductTypeProperties_CurrentVersionId";
ALTER INDEX "ProductTypeEntity_IX_CurrentVersionId" RENAME TO "IX_ProductTypes_CurrentVersionId";
-- TODO: ALTER INDEX "ProductTypeEntity_Identifier" RENAME TO "";
ALTER INDEX "ProductTypeEntity_Identifier_Revision_Index" RENAME TO "IX_ProductTypes_Identifier_Revision";

ALTER TABLE public."StepEntity" RENAME TO "WorkplanSteps";
ALTER TABLE public."WorkplanSteps" RENAME CONSTRAINT "PK_public.StepEntity" TO "PK_WorkplanSteps";
ALTER TABLE public."WorkplanSteps" RENAME CONSTRAINT "FK_public.StepEntity_public.WorkplanEntity_SubWorkplanId" TO "FK_WorkplanSteps_Workplans_SubWorkplanId";
ALTER TABLE public."WorkplanSteps" RENAME CONSTRAINT "FK_public.StepEntity_public.WorkplanEntity_WorkplanId" TO "FK_WorkplanSteps_Workplans_WorkplanId";
ALTER INDEX "StepEntity_IX_WorkplanId" RENAME TO "IX_WorkplanSteps_WorkplanId";
ALTER INDEX "StepEntity_IX_SubWorkplanId" RENAME TO "IX_WorkplanSteps_SubWorkplanId";
ALTER TABLE public."WorkplanSteps" ADD COLUMN "PositionX" float8 NOT NULL;
ALTER TABLE public."WorkplanSteps" ADD COLUMN "PositionY" float8 NOT NULL;

ALTER TABLE public."WorkplanEntity" RENAME TO "Workplans";
ALTER TABLE public."Workplans" RENAME CONSTRAINT "PK_public.WorkplanEntity" TO "PK_Workplans";

ALTER TABLE public."WorkplanReference" RENAME TO "WorkplanReferences";
ALTER TABLE public."WorkplanReferences" RENAME CONSTRAINT "PK_public.WorkplanReference" TO "PK_WorkplanReferences";
ALTER TABLE public."WorkplanReferences" RENAME CONSTRAINT "FK_public.WorkplanReference_public.WorkplanEntity_SourceId" TO "FK_WorkplanReferences_Workplans_SourceId";
ALTER TABLE public."WorkplanReferences" RENAME CONSTRAINT "FK_public.WorkplanReference_public.WorkplanEntity_TargetId" TO "FK_WorkplanReferences_Workplans_TargetId";
ALTER INDEX "WorkplanReference_IX_SourceId" RENAME TO "IX_WorkplanReferences_SourceId";
ALTER INDEX "WorkplanReference_IX_TargetId" RENAME TO "IX_WorkplanReferences_TargetId";


-- Create the default migration history table 
CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);
ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;
INSERT INTO public."__EFMigrationsHistory"("MigrationId", "ProductVersion") 
VALUES ('20220906133824_InitialCreate', '8.0.12');
ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");
	
--Drop the old migration table
DROP TABLE public."__MigrationHistory"
```

## Remove Antipatterns to our Platform Architecture
There were times, when we not true to our own design guidelines. Within this update we correct some of the antipatterns that crept into our code this way.

### The powerful VisualInstructor Resource desolves into the new MORYX Worker Support Module
We still have a VisualInstructor resource, but this one is closer to a digital twin of an interface to display supportive information to a worker. 
It does not host its own endpoint and has no control over the communication to and from the new Worker Support module.
This module takes care of the management of visual instructions and offers a facade with the same functionalities we previously had stuffed into the resource. Over an external REST endpoint and a SignalR Hub you can distribute instructions and events regarding them to any devices you like.

## Setup Information
Basically there was a solution in the ISetupProvider which will return a setup recipe for a given production recipe if there are some necessary setup steps.
The additional solution with the IJobManagement is removed. This includes the following points:
- _JobEvaluation_ not includes the _RequiredSetup_ property anymore
- The class _SetupStep_ is deleted
- _ISetupJobHandler_ will not have the two methods to get setup steps by a given recipe
- _OperationModel_ will not have a _SetupRequirement_ property
- _IOperationData_ will not have a _RequiredSetup_ property
- The _EffortCalculator_ will not taking care of the information about the setups steps

All the points are used just to inform the Orders UI that there is a setup necessary and to show an indicator at the operation. This can also be done by just asking the _ISetupProvider_ for a setup recipe. If the answer is not null then a setup is necessary and an indicator can be displayed. A setup can be outdated after a capabilitiy change, a new added resource or after removing a resource. All information can be gotten from the AbstractionLayer and can be handled in the UI.
