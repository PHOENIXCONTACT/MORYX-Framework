# Moryx.ControlSystem.Wpc.Endpoints

Provides REST-API to interact with the `ProcessHolderPosition` and `ProcessHolderGroup` within a MORYX application.

# Usage
Include the  `Moryx.ControlSystem.Wpc.Endpoints` package reference to your start project and that's it.

# Features
The Endpoints allows you to:

    - Get All ProcessHolder positions with their corresponding Group or Parent Resource.
    - Subscribe to all `ProcessHolderPosition` process update events.
    - Reset a `ProcessHolderPosition`
    - Reset a `ProcessHolderGroup` which automatically reset all `ProcessHolderPosition`s

# Additional feature related to the UI
 You can put the `EntryVisualizationAttribute` on your custom `ProcessHolderGroup` implementation class or your `Resource` that holds the `ProcessHolderPosition` to display a custom icon in the `Process Holder Group` UI.
 **Notes:** the icon name should be from `https://fonts.google.com/icons`

Example:

 ```cs
 [ResourceRegistration]
 [EntryVisualization(null,"user_icon")] <-- the icon to display
 public class MyResource : Cell {

    [DataMember]
    public IProcessHolderPosition SlotA { get; set; }
    [DataMember]
    public IProcessHolderPosition SlotB { get; set; }

    //More code....
 }
 ```  
