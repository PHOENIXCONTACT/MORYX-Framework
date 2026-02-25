# ResourceSynchronization

  - Notify other MORYX instances about the changes that occurred to a resource.

  1. **Available Topic to listen to**

      - `moryx/resources/changed`
      - `moryx/resources/added`
      - `moryx/resources/removed`


  2. **Setup**

    Add your configuration file `Moryx.Mqtt.Config.json`, inside the default MORYX Config folder. The content is as follow:

    ```json
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
      "RootTopic": "moryx/device-2" // optional
    }
    ```

    ```csharp
    // Program.cs
    builder.Services.AddTransient<ResourceToJsonConvert>(); // Required
    builder.Services.AddMqttClient((provider, options) =>
    {
        // Json Serialization options
        var resourceJsonConverter = provider.GetRequiredService<ResourceToJsonConverter>();
        options.JsonSerializerOptions.Converters.Add(resourceJsonConverter);
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.DefaultTopicStrategy.NoLocal = true; // optional
        options.DefaultTopicStrategy.RetainAsPublished = true; // optional
    }).AddResourceSynchronization();
    ```

  3. **Response Payload (Schema)**:

    The response payload is the `json` serialized string of the resource it doesn't have a predefined schema.

  4. **Usage**

    For the resource synchronization to work you need to use the following attributes:

      - `ResourceSynchronizationAttribute` for classes
      - `SynchronizableMemberAttribute` for properties

    `ResourceSynchronizationAttribute` has 2 modes `Full` and `Selective`. `Full` will serialize all the public properties inside the resource. While `Selective` will serialize properties that have the `SynchronizableMemberAttribute`.

    ```csharp
    [ResourceSynchronizationAttribute("Synchronization.Unique.MyResource",Mode.Selective)]
    public class MyResource: Resource
    {
      [SynchronizableMemberAttribute]
      public int InstanceCount { get; set; }

      public string Path { get; set; }
    }
    ```

    To serialize all properties :

    ```cs
    [ResourceSynchronizationAttribute("Synchronization.Unique.MyResource",Mode.Full)]
    public class MyResource: Resource
    {
      public int InstanceCount { get; set; }

      public string Path { get; set; }
    }
    ```

  5. **Design decision**

    This introduces the abstractions for synchronizing the state of **digital twins** across different MORYX application instances.

    The primary use case is for scenarios where multiple MORYX applications share control of, or interact with, the same physical resources. For example, a specialized **tool** might be used in several different machines on a factory floor. Each machine is controlled by a separate MORYX instance, but the tool's digital twin (representing its wear, calibration, or current status) must remain consistent across all of them. This feature provides the infrastructure to make that possible.

  6. **Summary**

    - Purpose:
      Fundamentals to keep digital twin state synchronized across multiple MORYX instances when they refer to the same physical resource.
    - How it works:
      - Resources opt-in via [ResourceSynchronization] with a Mode that defines property selection (Full or Selective).
      - Identification for synchronization uses IIdentifiableObject.Identity when available; otherwise falls back to IResource.Name.
      - Property selection:
        - Full: all public properties (except those explicitly ignored).
        - Selective: only properties marked with [SynchronizableMember] (again, respecting exclusion attributes like [JsonIgnore]).
    - Example resources:
      - AssembleResource: Full synchronization of all public properties.
      - Machine: Selective synchronization (UsageCounter and Status only) and uses Identity as the sync key.


## Implementation Details

  1. **ResourceSynchronizationAttribute**

    The core of this feature is an attribute to mark an `IResource` class for synchronization. Its `Mode` property dictates which properties are included in the synchronization payload.

    [More info](../../../src/Moryx.AbstractionLayer/Resources/Attributes/ResourceSynchronizationAttribute.cs)

  2. **Identification Logic**

    The synchronization mechanism requires a unique key to identify resource instances. The key is resolved with the following priority:

    - **`IIdentifiableObject.Identity` (Override):** If the resource class implements `IIdentifiableObject`, its `Identity` property will be used as the unique key. This is the **preferred** method.
    - **`IResource.Name` (Default):** If the class does not implement `IIdentifiableObject`, the system will fall back to using the `IResource.Name` property.

  3. **Serialization and Property Selection**

  The `SynchronizationMode` dictates which properties are included. To support `Selective` mode, a new [SynchronizableMemberAttribute](../../../src/Moryx.AbstractionLayer/Resources/Attributes/SynchronizableMemberAttribute.cs) will be introduced.

  The serialization logic is as follows:

  - **`Full` Mode:** All public properties of the resource are serialized.
  - **`Selective` Mode:** Only public properties explicitly decorated with `[SynchronizableMember]` are serialized.
  - **Exclusion is Always Respected:** Regardless of the mode, properties marked with an exclusion attribute (e.g., `[JsonIgnore]` or a similar attribute depending on the final serialization engine) will **never** be included in the synchronization payload.

  4. **Usage Examples**

    - **Example 1: Full Synchronization**

    This is a digital twin of an [AssembleResource](../../../src/Moryx.Resources.Samples/AssembleResource.cs) will have all its properties synchronized.

    - **Example 2: Selective Synchronization**

    This digital twin of a [machine](../../../src/Moryx.Resources.Samples/Machine.cs.cs) uses selective synchronization to only share critical operational data, not descriptive metadata.
