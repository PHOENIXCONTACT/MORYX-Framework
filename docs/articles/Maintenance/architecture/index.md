# Overview

The `MaintenanceWeb` project is divided into two parts:

- `Marvin.Runtime.Maintenance.Web`(*) - the web server which serves the web UI
- `Marvin.Runtime.Maintenance.Web.UI` - the web UI

*(***):Note that the REST endpoints are not served by the `Marvin.Runtime.Maintenance.Web`. The endpoints are served by the `MarvinPlatform`*

## Marvin.Runtime.Maintenance.Web

This .NET C# project uses common `MarvinPlatform` technologies and is a lightweight web server that just serves two files:

- index.html
- bundle.js

Usually, you will find th web UI under [http://localhost/MaintenanceWeb](http://localhost/MaintenanceWeb). If you want another port or endpoint feel free to change modules config (`MaintenanceWebConfig`).

## Marvin.Runtime.Maintenance.Web.UI

This React project is the browser application to maintain and configure platform specific topics. In the JavaScript world there are many ways to create a JavaScript based client side UI. This plurality makes it difficult for rookies to decide which framework and libraries he should use. We did choose the following libraries to create the Maintenance Web UI:

- `Node.js` - JavaScript runtime
- `TypeScript` - JavaScript superset to add types to the Maintenance project
- `Webpack` - The bundler & more
- `React` - UI framework
- `Reactstrap` - React implementation of Bootstrap
- `Redux` - Manager for domain specific data

As mentioned before there are many ways to create an client side JavaScript application. [This article](xref:Decisions) will give you some answers about pros & cons and more.

## Further information

![Overview](/articles/architecture/images/Overview.png "Overview")

In [ContainerAndComponents](xref:ContainerAndComponents) you can find out how a `React` application is structured in general.

In [Business logic with Redux](xref:BusinessLogicWithRedux) you will get deeper information how business data is organized and accessed.