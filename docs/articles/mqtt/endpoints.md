# [ResourceRpcEndpoint](https://github.com/PHOENIXCONTACT/MORYX-Framework/blob/dev/docs/articles/mqtt/resource-endpoint.md)

    Endpoint to remote call a procedure/method on a resource.

1. **Setup** :

    When run your project for the first time a configuration file will be generated inside the default MORYX Config folder. The content will be as follow:

    ```json
    {
    "Id": "my-id",
    "Host": "localhost",
    "Port": 1883,
    "Username": "",
    "Password": "",
    "Tls": false,
    "QoS": "ExactlyOnce", // check the MqttQualityOfServiceLevel enum for more info
    "ReconnectDelayMs": 30000,
    "ReconnectWithClientSession": false,
    "RootTopic": ""
    }
    ```

    ```csharp
    // Program.cs
    builder.Services.AddMqttClient((provider, options) =>
    {
        var configManager = provider.GetRequiredService<IConfigManager>();
        //Setup Mqtt client connection
        var config = configManager.GetConfiguration<MqttConnectionConfig>();
        // If configuration is generated, save it back to persist defaults
        if (config.ConfigState == ConfigState.Generated)
        {
            config.ConfigState = ConfigState.Valid;
            configManager.SaveConfiguration(config);
        }
        // Mqtt Client configuration
        options.Connection = config;
        }).AddMqttEndpoints(); // <-- this add supports for the endpoints
    ```

2. **Available Endpoint topics** :

    `[root topic of the endpoint client]/resources/{identifier}/invoke/{methodName}`, example: `moryx/device-2/resources/123654/invoke/ChangeCapabilities`

    if no root topic was configured the topic become `resources/{identifier}/invoke/{methodName}`, example: `resources/123654/invoke/ChangeCapabilities`

    If the method you are invoking has parameters , you should put the value of those parameters in your payload ex:

    ```csharp
    // MyResource.cs
    //code..
    public void MyMethod(int testResult, string name)
    {
        //code..
    }
    //code..
    ```

    Given the previous method your json `payload` should look like the following :

    ```js
    {
        "TestResult": 564,
        "Name": "DefaultName"
    }
    ```

3. **Response Topic** :

    In MQTT v5+ you can specify a response topic in your message. If a response topic is present the endpoint will sent the response of the invocation to that topic. If a response topic was not define the default response topic is as follow: `resources/{identifier}/invoked/{methodName}` example based on the previous topic ex: `resources/123654/invoked/ChangeCapabilities`

4. **Response Payload** :

    If the method you are calling has a return value :

    ```csharp
    // MyResource.cs
    //code..
    public int MyMethod(int testResult, string name)
    {
        //code..
        return 10;
    }
    //code..
    ```

    Given a method like above the endpoint will sent the result back to the client.
    The schema of the json `payload` will look like the following:

    ```json
    {
    "Value" = object,
    "ValueType" = EntryValueType //   EntryConvert.TransformType(methodToInvoke.ReturnType)
    }
    ```

    Based on this schema our example will look like the following:

    ```json
    {
    "Value" = 10,
    "ValueType" = 4 //  check EntryValueType for more infos
    }
    ```
