---
uid: ResourceMethods
---
# Resource methods

A common requirement when building applications on the current version of the `AL` was interacting with a resource instance over the UI. Back then the only tool available to developers were additional web-services and custom detail views. The new version still has this feature, but also offers another way to implement interaction with a resource with minimal effort.
Developers can simply decorate a method with `EditorVisible`, the same way they do with properties described in [Resource details](xref:ResourceDetails). This will automatically add a button to region C with the methods name or optional display name. This will open a dialog where users can enter values and inspect the methods return value. Like the details view this feature uses the platforms `Entry` format and editor to serialize, transmit and visualize the methods parameters. This automatically includes features like the dropdown-box for enum parameters. With this feature previous tasks like adding a Selftest button to the UI, which would cost at least a full day, now only require the time for implementing the functionality, without any additional infrastructure effort.
