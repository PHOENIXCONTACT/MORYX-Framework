## Migrate to Core v4

For this major release the main motivation was switching from our own MORYX Runtime to ASP.NET Core with MORYX extensions.

### DI Container replacement

As of version 4 we replaced the global Castle Windsor container with Microsofts `ServiceCollection`

### Logging