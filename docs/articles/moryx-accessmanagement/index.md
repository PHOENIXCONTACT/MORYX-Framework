# Identity and Access Management

This repository contains the packages required to setup an identity server with the `Moryx.Identity.AccessManagement` and a consuming application with `Moryx.Identity`.

The MORYX Access Management repository contains the following projects
- The `Moryx.Identity` package contains everything you need if you want to connect your application to a MORYX Access Management server 
- The `Moryx.Identity.AccessManagement` package contains the Moryx IAM Server. It is based on the [ASP.NET Core Identity Server](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-10.0&tabs=visual-studio).

## Further Reading
Refer to the following articles for instruction on setting up your application with a working IAM integration
- [How to configure your application to connect to a running MORYX Access Management Server](./how-to-integrate-in-your-application.md)
- [How to setup your endpoint and controller to run with an IAM](./how-to-integrate-in-endpoints.md)
- [How to integrate login via EntraID / AzureAD](./how-to-integrate-login-via-entra-id.md)

## Demo Application
A demo application for the identity server and a consuming application is given in the [Identity Demo](https://github.com/PHOENIXCONTACT/MORYX-Identity-Demo) repository.
Here you will also find a guide on how to setup and run a MORYX Identity Server in general.
