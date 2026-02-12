# APIs and classes

1. **IManagedMqttClient** : 
    Is a self implemented managed `IMqttClient` that handles reconnect, connect logics and other features.

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
        //  add your custom converter/serializer if needed
        options.JsonSerializerOptions.Converters.Add(new MyConverter())
    })
    ```

    Note : You can also configure the Mqtt client using a configuration file just like any `MORYX` module. 


3. **Serialization**

    The serialization of payload is done via `MqttMessageSerialization` helper class, serialization and deserialization methods requires `JsonSerializerOptions` that is configured during setup of the Mqtt client.

    - JsonSerializationOptions

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
    Example:

    ```csharp
    public class BlablaService : IMqttService
    {
        private IMyDependency _dependency;
        private IManagedMqttClient _client;
        public Blabla(IMyDependency dependency, IManagedMqttClient client)
        {
            _dependency = dependency;
            _client = client;
        }

        // </inherited>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            // your logic here
            // _client.OnConnectedAsync += ....
            // _client.OnMessageReceived += ...
            return Task.CompletedTask;
        }

        // </inherited>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            //your logic here
            return Task.CompletedTask;
        }
    }
    ```

    To add your new service to the service collection do the following:
    ```csharp
    builder.Services.AddMqttClient();
                    .AddMqttService<BlablaService>(); // you custom service
    ```

    **Note** : the connection to the broker (Connect, re-connect on connection lost etc...) is automatically handled by the `IManagedMqttClient`. 

4. **Endpoints**

    If you need something with topic template and constraint support, you can use the `IMqttEndpoint`. Here is an example:
    - In your `program.cs` add the following:

    ```csharp
    builder.Services.AddMqttClient() // must be present
                    .AddMqttEndpoints(); // <-- this add supports for the endpoints
    ```
    - Create a new C# class that implements the `IMqttEndpoint`:
    ```csharp
    public class Blabla : IMqttEndpoint 
    {
        private IMyDependency _dependency;
        public Blabla(IMyDependency dependency)
        {
            _dependency = dependency;
        }

        public void Map(IMqttRouteBuilder route)
        {
            //topic template is supported here
            route.MapGet("my-topic/{id:int}/method/{methodName}", context =>
            {
                var messageBuilder = new MqttApplicationMessageBuilder();
                messageBuilder.WithTopic("another-topic");
                messageBuilder.WithPayload(MqttMessageSerialization.GetJsonPayload("Hello World!!!!"));
                return messageBuilder.Build();
            });
        }
    }
    ```

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

5. **Available features in endpoints**

    Endpoints topics support constraint just like in ASP.net controller route.

6. **Route constraint**

    Find the list of available constraints [Here](https://learn.microsoft.com/en-us/aspnet/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2#route-constraints)

6. **Message Body and Parameters inside a Topic**

    When the endpoint receives a message from the broker, it might come with a Body/payload and a parameter inside the topic. 
    To access the payload inside your endpoint you can use one of the following methods:
    ```csharp
    FromBody<T>(string propertyName)
    ```
    ```csharp
    FromBody(string propertyName, Type type)
    ```
    ```csharp
    RequestBody<T>()
    ```
    ```csharp
    RequestBodyObject()
    ```

    Given the following message body in Json:
    ```json
    {
        "Name": "Bloomberg",
        "Temperature":"-2 degrees Celsius"
    }
    ```

    The following examples shows how to use the Message body methods to extract the relevant data:
    ```csharp
    route.MapGet("/my-topic/{id:int}", ctx => {
        string name = ctx.FromBody<string>("Name"); // "Bloomberg"
        City city = RequestBody<City>();        // "{Name, Temperature}"
    });
    ```

    The same thing is possible for the topic parameters. Example:
    ```csharp
    route.MapGet("/my-topic/{id:int}", ctx => {
        int id = ctx.FromParameterValues<int>("id"); // this return 2 for '/my-topic/2'
    });
    ```



