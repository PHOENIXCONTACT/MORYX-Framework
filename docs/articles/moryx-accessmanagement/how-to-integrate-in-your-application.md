# Include Moryx Identity Management in your Moryx Application
To include the MORYX Access Management in your application add the `Moryx.Identity` package to your application project.
The configuration of the authentication in the `Program.cs` file follows the standard ASP approach, find details to the authentication middleware [here](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-10.0).
We start by binding the config to the `MoryxIdentityOption`.
```csharp
...
// ...
var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json", optional: false).Build();
services.Configure<MoryxIdentityOptions>(config.GetSection(MoryxIdentityOptions.MoryxIdentity));
// ...
```

The subsequent snippet shows a minimal configuration example from the `appsettings.json` for setting up the IAM connection
```json
{
    "MoryxIdentity": {
      "BaseAddress": "https://localhost:5001"
    },
    ...
}
```
**Note: The application and the IAM Server need to be hosted under the same second level domain for the cookie in which the token is shared to work. For more information read up [here](https://developer.mozilla.org/en-US/docs/Web/Security/Same-origin_policy#cross-origin_data_storage_access)**

Now we setup the services. MORYX Identity uses [policy-based authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-6.0). 
For the modular structure of our applications it is necessary, however, to dynamically determine the policies required for the authorization of all endpoints included in the application.
Therefore, we provide a [custom implementation of the IAuthorizationPolicyProvider](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/iauthorizationpolicyprovider?view=aspnetcore-10.0) interface.
This implementation needs to be provided within the service collection now, after adding the authorization middleware to add all required policies.
Optionally, you can also use our implementation of the [IAuthorizationMiddlewareResultHandler](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authorization.iauthorizationmiddlewareresulthandler?view=aspnetcore-10.0) interface to get a more precise response for specific authorization failures, such as the missing permissions of an user for 403 errors.
Beforehand, we add the authentication middleware and set the `MoryxIdentity` authentication scheme to be the default
```c#
// ...
services.AddAuthentication(MoryxIdentityDefaults.AuthenticationScheme).AddMoryxIdentity();
services.AddAuthorization();
services.AddSingleton<IAuthorizationPolicyProvider, MoryxAuthorizationPolicyProvider>();
services.AddSingleton<IAuthorizationMiddlewareResultHandler, MoryxIdentityResultHandler>();
// ...
```

To complete the setup of the authorization and authentication middleware, the application builder must be instructed to use them.
```c#
// ...
app.MapControllers()
// ...
```

That's it. You can now start the IAM Server and your application. 


## Running an Application without IAM
**We heavily discourage using this option in production environments** as it allows everyone in your network to do whatever they want through the available endpoints of your application.

To use your application without a running IAM, e.g. in a development environment, you need to create a `AuthorizationPolicyProvider`, which returns new policies for each request and enable the `AnonymousAttribute` in the `Configure` function. 
This `ExamplePolicyProvider` provides you with the ability to use any policy protected endpoint in your application
```csharp
internal class ExamplePolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
{
    public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName) => await base.GetPolicyAsync(policyName) ??
        new AuthorizationPolicyBuilder().RequireClaim("Permission", policyName).Build();
}
```
Adding this policy provider to the services and enabling the usage of the endpoint anonymously is done in the following way
```csharp
// ...
services.AddSingleton<IAuthorizationPolicyProvider, ExamplePolicyProvider>(); // instead of
// services.AddSingleton<IAuthorizationPolicyProvider, MoryxAuthorizationPolicyProvider>();

/// ...

app.MapControllers().WithMetadata(new AllowAnonymousAttribute()); // instead of
// app.MapControllers()
/// ...
```

Besides these specific snippets you don't need any of the code you would normally need when you want to connect to an actual IAM Server.