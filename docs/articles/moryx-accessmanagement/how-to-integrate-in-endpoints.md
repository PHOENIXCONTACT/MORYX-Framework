# Setting up Authentication and Authorization in your Module
This task is a 2-step endevour.
We need to make sure the endpoint only allows requests from an authenticated user with the necessary authorization for a certain action and we probably want to hide buttons and elements in the UI they are not allowed to use anyway.
## Setting up the Endpoint
### Permissions
We start by defining all the different permissions for the actions of our endpoint.
As an example we list the `Permissions.cs` file from the Endpoint of the ResourceManagement module.
```csharp
public static class Permissions
{
    private const string _prefix = "Moryx.MyModule.";
    public const string CanRead = _prefix + "CanRead";
    public const string CanDelete = _prefix + "CanDelete";
    // ...
}
```
## Actions
With these permissions at hand we now secure the different actions at the Resource Endpoint using the `Authorize` attribute.
```csharp
// MyController.cs

[Authorize(Policy = Permissions.CanRead)]
public ActionResult MyControllerMethod()
{
    // ...
}
```
For more information on limiting access to endpoints take a look at the [Asp.Net documentation](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/simple?view=aspnetcore-10.0)

### Checking for permissions
We provide a method to check on the endpoint side by default, using the [`MoryxIdentityHandler`](../../../src/Moryx.Identity/MoryxIdentityHandler.cs), which can be [added to the Program.cs of your project](./how-to-integrate-in-your-application.md#include-moryx-identity-management-in-your-moryx-application).

You could also verify permisions in your user interfaces or in other client apps, for this make use of the API methods from the [Authentication Controller](../../../src/Moryx.Identity.AccessManagement/Controllers/API/AuthController.cs).