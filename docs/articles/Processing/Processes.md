---
uid: Processes
---
# Processes

A [Process](xref:Marvin.AbstractionLayer.IProcess) will be created to produce one article. Therefore it consists of a series of Activities from a defined workplan. It is managed by the ProcessController.

## Processes in a Resource

It is possible to get the process from an activity inside of a resource. This is sometimes necessary to get more information from the recipe or the product. But if there are information which are necessary for every resource which can handle the activity then consider to provide the information with activity parameters.

## Processes in the ControlSystem Kernel

A process contains information about the depending job and the recipe which are necessary inside of the Kernel to handle the progress of jobs, to predict if the process will fail, to note if it was reworked or many more.

A Process will be marked as `Reworked` if the `ArticleModificationType` is `Loaded`. This is only possible if the identifier of the article is known at the start. So the article must have been processed before. The `ArticleModificationType` can also be `Loaded` if an entry point of the workplan will be used.