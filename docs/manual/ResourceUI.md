# Resource UI

The Resource UI shows basically all informaiton about the available resources. In context of the Control System there are in most case resources which are representing parts of the production machine like some stations or the drivers to communicate with a PLC. Each resource can be displayed in detail and can be modified.

## Overview

All resources will be shown on left side in the tree. Each resource can group some child resources. All details will be displayed after selecting a resource. All informations are grouped in aspects and available with the `Tabs` under the general information area. All information about the selected resource can only be changed in the `Edit Mode` which can be entered with the button on the right bottom corner. The edit mode can be left with the cancel or save button which will restore the old data or save the new data.

![Resource UI overview](images\resourceUI.png)

### Delete a resource

To delete a resource just select one and there will be a delete button on the right top corner. The selected resource will be deleted if this button will be clicked and a warning message will be confirmed like in the following picture.

![Resource delete dialog](images\resourceUIDeleteDialog.png)

### Create a new resource

The user can add a new resource with the add button above the resource tree on the left side. A new dialog will be shown with all possible resource types which can be created under the selected resource. So the amount of possible resource can vary by the selected resource. Detailed information like the name and tpe will be displayed on the right side of the dialog. 

!["Create Dialog"](images\resourceCreateDialog.png)

After pressing the create button, the new resouce will be loaded. The user can enter the detailed information and can save with the save button on the bottom right.

### Resource references

Each resource can reference different other resource to work correctly. For example is a reference to an `instructor` necessary to show instructions during the production. All possible references can be shown with the `Reference` tab in the details area. There will be all references on the left and all possible resource which can be used for the selected reference on the right side. To set a reference just select a resource which should be used and click on the `Link Target` button. An indicator will be displayed to show that the resource was linked to the selected reference. This action is only possible in the `Edit Mode`. There are two different types of references. A single reference can only be linked to one resource. A click on the `Link Target` button will automatically unlink an existing link. A multi refernece can be linked to more than one resource. A click on the `Link Target` button will link the selected resrouce and keep the existing links. 

![Resource references](images\resourceUIAspectReferences.png)

### Resource methods

A resource can provide some methods like a selt test of an electrical test station. Methods are available in the `Methods` tab and can be selected and invoced. The result of the invocation will also be displayed.

![Resource methods](images\resourceUIAspectMethods.png)
