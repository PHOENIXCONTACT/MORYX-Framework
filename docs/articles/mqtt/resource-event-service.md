# ResourceEventService
Sends messages to the broker when a public event or event exposed using the `ResourceAvailableAsAttribute` is raised.

  1. **Available Topic to listen to**

    - `[rootTopic]/resources/{identifier}/event/{eventName}`
    - If the root topic is empty or null the topic template is : `resources/{identifier}/event/{eventName}`

  2. **Setup**

  A configuration file will be generated inside the default MORYX Config folder. The content is as follow:

  ``` json
  {
    "Id": "my-id",
    "Host": "localhost",
    "Port": 1883,
    "Username": null,
    "Password": null,
    "Tls": false,
    "QoS": 1, // check the MqttQualityOfServiceLevel enum for more info
    "ReconnectDelayMs": 30000,
    "ReconnectWithClientSession": true,
    "RootTopic":  ""
  }
  ```
  You can modify it according to your needs.
  ```csharp
  // Program.cs
  builder.Services.AddMqttClient()
                  .AddMqttService<ResourceEventService>(); 
  ```

  3. **Response Payload (Schema)** :

  Type `ResourceEventPayload.cs`

  ```json
  {
      "Resource" : {
          "Identifier" : string,
          "Name" : string
      },
      "Event" = string,
      "EventData" = {} // any object/type
  }
  ```

  4. **Example** :

  Given the following Resource:

  ```csharp
  public interface IMyPublicEvents
  {
      event EventHandler MyEvent;
      event EventHandler<int> EventWithResult;
  }

  [ResourceAvailableAs(typeof(IMyPublicEvents)))]
  public class ExampleResource : Resource, IMyPublicEvents 
  {
      public string Name => "New Resource";
      public string Identifier => "123456";
      public event EventHandler MyEvent;
      public event EventHandler<int> EventWithResult;
      //code...
  }
  ```
  - When `MyEvent` is raised the `ResourceEventService` publishes a message to the topic `moryx/device-2/resources/123456/event/MyEvent`:
  ```json
  {
    "Resource" : { "Identifier": "123456", "Name": "New Resource" },
    "Event" : "MyEvent",
    "EventData": null
  }
  ```

  - When `EventWithResult` is raised the `ResourceEventService` publishes a message to the topic `moryx/device-2/resources/123456/event/EventWithResult` :
  ```json
  {
    "Resource" : { "Identifier": "123456", "Name": "New Resource" },
    "Event" : "EventWithResult",
    "EventData": 10 // example 
  }
  ```

