# The goal:

The goal is to allow any MORYX developer to create their own Mqtt To MORYX facade, that way you can use Mqtt to communicate to with any MORYX facade ex: `ProductManagement`,`ResourceManagement` etc.

To achieve that, we took inspiration from 3 repositories :

-  [Hosted Mqtt Server](https://github.com/dotnet/MQTTnet/wiki/Server)
-  [How to use Mqtt as a service in asp.net controller](https://github.com/rafiulgits/mqtt-client-dotnet-core)
-  [Mqtt AttributeRouting](https://www.nuget.org/packages/MQTTnet.AspNetCore.AttributeRouting)

_Checking those repositories will give you an idea of what the `ManagedMqtt` is trying to achieve._

1. **APIs and classes**:

   **IManagedMqttClient** 
    A managed `IMqttClient` 
   **IMqttService** :
    Preferred base class for all services based on Mqtt .
   **IMqttEndpoint** :
    Mqtt Endpoint interface. Every endpoint will implement this interface.

2. **Usage**:

    To use the `IManagedMqttClient` you need to configure it in the `Program.cs` like so:

    ```csharp
    builder.Services.AddMqttClient();
    ```
    Optionally you can also configure the Mqtt client connection details like so :
    ```csharp
    builder.Services.AddMqttClient((provider, options) =>
    {
        options.Connection = new MqttConnectionConfig
        {
        Port = 1883,
        Id = "EndpointDemo1",
        Host = "localhost",
        QoS = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce,
        RootTopic = "mqtt/moryx"
        };
        // optional: add your custom converter/serializer
        options.JsonSerializerOptions.Converters.Add(new MyConverter())
    })
    ```

    You can also configure the Mqtt client using a configuration file just like any `MORYX` module. 
    ```csharp
        builder.Services.AddMqttClient();
    ```

    The configuration file can be found inside the default MORYX Config folder. The content is as follow:

    ``` json
    {
    "Id": "my-id",
    "Host": "localhost",
    "Port": 1883,
    "Username": null,
    "Password": null,
    "Tls": false,
    "QoS": "ExactlyOnce", // check the MqttQualityOfServiceLevel enum for more info
    "ReconnectDelayMs": 30000,
    "ReconnectWithClientSession": false,
    "RootTopic":  "" 
    }
    ```

3. **Serialization** :

    The serialization of payload is done via `MqttMessageSerialization` helper class, serialization and deserialization methods requires `JsonSerializerOptions` that is configured during setup of the Mqtt client.

4. **JsonSerializationOptions** :

    ```csharp
    // Program.cs
    builder.Services.AddMqttClient(builder: (provider, options) => 
    {
        options.JsonSerializerOptions.Converters.Add(new MyCustomConverter()) // here you add your custom converter
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; // change how references are handled
        //etc..
    })
    //.. other code
    ```

    if you want to implement your custom service based on the   `IManagedMqttClient` the recommended way is as follow :
    - In your `program.cs` add the following:

    ```csharp
    builder.Services.AddMqttClient(); // this adds the IManagedMqttClient to the service collection
    ```
    - Create a new C# class and inherit from `IMqttService`. `Note: Dependency injection is supported `. Now your service has access to the `IManagedMqttClient`, 
    [ResourceEventService Example](../../src/Moryx.AspNetCore.Mqtt.Services/ResourceEventService.cs)

    To add your new service to the service collection do the following:
    ```csharp
    builder.Services.AddMqttClient();
                    .AddMqttService<BlablaService>(); // you custom service
    ```

    **Note** : the connection to the broker (Connect, re-connect on connection lost etc...) is automatically handled by the `IManagedMqttClient`. 

5. **Endpoints** :

    If you need something with topic template and constraint support, you can use the `IMqttEndpoint`. Here is an example:
    - In your `program.cs` add the following:

    ```csharp
    builder.Services.AddMqttClient() // must be present
                    .AddMqttEndpoints(); // <-- this add supports for the endpoints
    ```
    - Create a new C# class that implements the `IMqttEndpoint` [Example](../../src/Moryx.AspNetCore.Mqtt.Endpoints/ResourceRpcEndpoint.cs). 

    There is a minimal version of the Mqtt endpoint, that you can use directly in the `program.cs`:

    ```csharp
    builder.Services.AddMqttClient()
                    .AddMqttEndpoints(route =>
                    {
                        route.MapGet("my-topic/{id:int}/method/{methodName}", context =>
                        {
                            var messageBuilder = new MqttApplicationMessageBuilder();
                            messageBuilder.WithTopic("another-topic");
                            messageBuilder.WithPayload(MqttMessageSerialization.GetJsonPayload("Hello World!!!!"));
                            return messageBuilder.Build();
                        });
                        // optional : this behave like an eventStream
                        route.MapPost(stream =>
                        {
                            var messageBuilder = new MqttApplicationMessageBuilder();
                            messageBuilder.WithTopic("another-topic");
                            var timer = new Timer(async arg =>
                            {
                                var messageBuilder = new MqttApplicationMessageBuilder();
                                messageBuilder.WithPayload(MqttMessageSerialization.GetJsonPayload("Hello World!!!!"));
                                var message = messageBuilder.Build();
                                await stream.WriteAsync(message);
                            }, null, 0, 10 * 1000);
                        });
                    });
    ```
    This can be useful for quick prototyping and testing.

6. **Available features in endpoints** :

    Endpoints topics support constraint just like in ASP.net controller route.

7. **Route constraint** :

    Find the list of available constraints [Here](https://learn.microsoft.com/en-us/aspnet/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2#route-constraints)

8. **Message Body and Parameters inside a Topic** :

    When the endpoint receives a message from the broker, it might come with a Body(payload) and a parameter inside the topic. 
    To access the payload inside your endpoint you can use one of the following methods:
    ```csharp
    FromBody<T>(string propertyName)
    ```
    ```csharp
    FromBody(string propertyName, Type type)
    ```
    ```csharp
    //Return the entire body of the request cast to the given type T
    RequestBody<T>()
    ```
    ```csharp
    //Get the request body as an Object
    RequestBodyObject()
    ```



