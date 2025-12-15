---
uid: NotificationPublisher
---
# Notification Publisher

The NotificationPublisher module mainly collects all `INotificationSource` module facades and provides storage and publishing of these notifications from the notification sources. While the basic API and also some plugin implementations are part of the AbstractionLayer, the NotificationPublisher itself is part of the ControlSystem. The idea is, that even components of the AbstractionLayer (mainly resources) are allowed to create notifications, while only the ControlSystem and applications on top of it are able to process them.

## Provided facades

- No facade will be provided

## Dependencies

There are no direct dependencies to this module.

## Referenced facades

The `NotificationPublisher` collects all facades implementing `INotificationSource`.

## Used DataModels

- [Moryx.Notifications.Model](xref:Moryx.Notifications.Model)

## Architecture

The `NotificationPublisher` supports any type of `INotification` to process. The `Notification` itself will be processed by internal configured `INotificationProcessor`s.

A `INotificationProcessor` is responsible to process a specific notification type to store it in the database. There is a default `INotificationProcessor` which can process all notification types but will store all additional properties in the extension data of a notification entity.

The `NotificationPublisher` itself is a singleton component within the module.

![Overview](images/NotificationsOverview.png)

**Start Behavior**
While starting the `NotificationPublisher`, all processors will be started as well. All notifications will be restored from the processor who was responsible to manage the notification type. After loading the notifications the injected module facades `INotificationSource` will be triggered to restore them if available. Also all events will be registered to the source.

**Notification Listener**
To provide extensibility the `NotificationPublisher` provides the API of `INotificationListener` which is mainly a listening component on events of the `NotificationPublisher`. Events are `Published` or `Acknowledged`. Also the internal WCF service handles these events to publish information to the clients.

## Module Configuration

The Notification Publisher module can be configured using the config editor. There is not much configuration necessary. The `HostConfig` have to be changed if it differs from default. It depends on the application if the `NotificationPublisher` have listeners for further processing (e.g. pushing to a Sub/Super-System).

## Enable Notification Handling in a ServerModule

### Getting Notifications

To notice when a notification was published in the system follow the default procedure to register modules in your ModuleController.
Define the property for the `NotificationPublisher` (this requires you to reference the Moryx.Notifications package via Nuget).
Then ask the container in the `Start()` method to provide you with the instance.
The NotificationPublisher then provides you with desired notification access.

````cs
...
[RequiredModuleApi(IsStartDependency = true, IsOptional = false)]
public INotificationPublisher NotificationPublisher { get; set; }

protected override Task OnInitializeAsync(CancellationToken cancellationToken)
{
    // Register required facade
    Container.SetInstance(NotificationPublisher);
    ...
}
...

````

### Publishing Notifications

To enable a ServerModule for notification handling the following steps are necessary.

**Register notification components**
It is necessary to register some components to the local container of the ServerModule. Just call the method which is shown in the following example:

````cs
...
protected override Task OnInitializeAsync(CancellationToken cancellationToken)
{
  ...
  Container.RegisterNotifications();
  ...
}
...

````

**Provide notification Facade**
It is necessary to provide a `INotificationSource` facade to publish notifications. There is a facade class called `NotificationSourceFacade` which should be used by ServerModules to publish notifications. This is a reusable class to encapsulate the communication to the `NotificationPublisher`. Just implement the interface `IFacadeContainer<INotificationSource>` and implement the facade like in the following example:

````cs
public class ModuleController : ServerModuleFacadeControllerBase<ModuleConfig>, IFacadeContainer<INotificationSource>
{
  ...
  private readonly NotificationSourceFacade _notificationSourceFacade = new NotificationSourceFacade(ModuleName);
  INotificationSource IFacadeContainer<INotificationSource>.Facade => _notificationSourceFacade;
}

````

The ServerModule is now able to publish notifications to the `NotificationPublisher`.

### Publish a Notification

After enabling the ServerModule to handle notifications, each plugin can publish a notification. It is necessary to inject and use the `INotificationAdapter`.
The `INotificationAdapter` gets the notifications and handles the publishing over the facade to the `NotificationPublisher`.
It is also responsible to hold the notifications if the `NotificationPublisher` is not available and synchronize after the `NotificationPublisher` is available again.
Inside of a plugin it is just necessary to implement the interface `INotificationSender` and register to the `INotificationAdapter`.
Then the plugin can publish notification as often as necessary.
The following code snippet shows an example for how a notification publishing plugin could look like.

````cs
public class MyPlugin : IModulePlugin, INotificationSender
{
  public INotificationAdapter NotificationAdapter { get; set; }

  // Define a unique identifier for later statistic evaluation
  string INotificationSender.Identifier => "SenderIdentifier";

  public void AMethod()
  {
    // Create notification
    var notification = new Notification("A Title", "A message which should be shown", Severity.Info, true);

    // Publish notification
    NotificationAdapter.Publish(this, notification);
  }

  void INotificationSender.Acknowledge(INotification notification)
  {
    // This is a request to acknowledge a notification. You can acknowledge it if you want.
    // You can also call the following method whenever you want.
    NotificationAdapter.Acknowledge(this, notification);
  }
}

````

#### Marking a notification as not acknowledgeable

Some notifications should not be acknowledged by the user because the notification is acknowledged by the system automatically.
So you need to define if the notification can be acknowledged by the user.
Please note that all notifications are non acknowledgeable by default.

A non acknowledgeable notification makes the 'Acknowledge' button on the UI invisible.

#### Disable notifications for processing and publishing

Notifications can be disabled globally within the publisher by modify the type in database (set `IsDisabled = true`). Disabled notifications are processed but not saved nor published by the publisher.

Warning: The `NotificationAdapter` will not be informed about the disabled notification. From the adapters point of view, the notification is processed normally and must be acknowledged from the sender.

### Publish a custom Notification

To publish a custom notification just implement a new class and inherit from `Notification` and publish it. Thats it ;-)
Remember this is a new type and there is no custom UI for the custom notification. So the default UI will be used but the new notification will be published and stored in the database which is more important.

## Notification Bar

The `NotificationBar` is used to display notifications for the user on any GUI. The `NotificationBar` has evolved from Moryx-Classic so sometime it is called MessageBar.

One type of notifications to be displayed by the `NotificationBar` are status information of the connected PLCs. For each machine the group MES maintains an excel-worksheet with all notifications, used by the PLCs. To take advantage of these notifications, a ModelSetup must be executed initially.

### Configuration

To use the NotificationBar it has to be configured as a region in the default-shell in the config-file. Excerpt from the `Moryx.ClientFramework.Base.ShellRegionConfig.mcf`:

````json
{
  "RegionName": "TopRegion",
  "PluginName": "NotificationBar",
  "Visibility": "Visible"
}
````

### Behavior

All messages may be acknowledged by their sender, when the ControlSystem starts. All messages that still need to be shown, will then have to be repeated by the source after finishing the start or initialization-phase.

### Severity

There are four levels of notification severity:

| Severity | Description |
|----------|-------------|
| Info | Will be shown with a green background (was blue in Moryx-Classic) |
| Warning | Will be shown with a yellow background (was green in Moryx-Classic) |
| Error | Will be shown with a red background (was red in Moryx-Classic) |
| Fatal | Will be shown with a violet background (did not exist in Moryx-Classic) |

### Module Configuration

The config for the NotificationBar is stored in the publisher ui config-file: `Moryx.Notifications.UI.Publisher.ModuleConfig.mcf`

````json
{
  "PushToFullscreen": true,
  "PushToFullscreenSeverity": "Error",
  "RotationInterval": 3000,
  "RotationOrder": ["Error", "Info"],
  "ConfigState": "Valid"
}
````

**RotationInterval**: Time in milliseconds that each notification will be shown in the title bar.
**RotationOrder**: A List of Severity Enum Values. If this is an empty array, all notifications will be shown in the title bar.

### RotationOrder

The NotificationBar will show all available notifications at the top line. Every notification is shown for short period of time, then the next notification will be shown. This behavior can be configured (see below). Depending on the configuration-setting for "Rotation" the NotificationBar may rotate through all notifications or a subset of notifications. These subsets may be:

| Config-Setting | Description |
|-----------------------------|-----------|
| "RotationOrder": ["Error"] | Rotate through error notifications, if there are no error notifications rotate through all other notifications. |
| "RotationOrder": ["Error", "Warning"] | Rotate through error notifications, if there are no error notifications, rotate through the warning notifications. If there are no warning notifications rotate through all other notifications. |

### Content

Each notification has a title which will be shown in the title-bar as well as in list of all notifications. It also has a content-area, that will become visible when the title of the notification is clicked in the list of notifications. The content is customized by the source of the notification. e.g. for PlcMessages the content is provided by the "Moryx.Devices.PhoenixPlc.Notification.UI"-Project. The "Moryx.Devices.PhoenixPlc.Notification.UI"-View shows 3 Comments, an actor, a sensor and an acknowledgement button. Other sources may provide arbitrary content.

## Custom Notification UI

It is possible to define a custom notification so it is also possible to define a custom notification UI. The implemented solution is basically the same as for custom resource and product UI´s.

**Create a new project**
Create a new project for your custom notification UI which could be placed in a application project. Use a name like `Moryx.Notifications.UI.CustomUI`.

**Create new User Control and ViewModel**
Create a new User Control and add a ViewModel class. The ViewModel must inherit from `NotificationDetailsViewModelBase` and use the `NotificationDetailsRegistration` attribute to register the custom notification UI for a custom notification type like in the following example:

````cs
[NotificationDetailsRegistration("CustomNotification")]
internal class CustomNotificationDetailsViewModel : NotificationDetailsViewModelBase
{
  public CustomNotificationDetailsViewModel(NotificationViewModel notification) : base(notification)
}
````

The base class provides a view model for the notification to get the information. It also provides a command for the acknowledge request. The notification view model provides the additional properties in a separate property which was converted with entry convert. So it is possible define a converted to copy the additional properties in a class for an easier usage. The `CellStateReportNotificationDetailsViewModel` is a good example for the mentioned convert.
