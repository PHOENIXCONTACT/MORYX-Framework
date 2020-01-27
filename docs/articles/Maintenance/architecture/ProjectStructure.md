---
uid: ProjectStructure
---
# Project structure

This article explains the general structure of the `Marvin.Runtime.Maintenance.Web.UI` project. You'll see that the structure is very common compared to other open source web projects.

## Root

The following tree shows all important files &amp; folders within root. Note that there are may be some more folder or files which are not listed below. Just ignore them.

````text
Root
|
└ .vscode
|   └ Contains Visual Studio Code settings for starting and debugging
|
└ dist
|   | index.html - Static HTML start page
|   └ bundle.js - Transpiled application
|
└ node_modules
|   └ Contains alle third libraries that are reference in package.json
|
└ src
|   └ Application sources
|
└ package.json - Package settings & scripts for Node.js
└ tsconfig.json - TypeScript settings
└ webpack.config.js - Global Webpack settings
└ webpack.dev.config.js - Development dependant Webpack settings
└ webpack.prod.config.js - Production dependant Webpack settings
````

## The src folder

Here in the src folder is all the stuff for the `Marvin.Runtime.Maintenance.Web.UI`. The structure leans towards the application tabs. Every subfolder can be seen as module which should be "live" alone in future development.

````text
src
└ common
|   └ Some common stuff and the main application view
└ dashboard
|   └ Dashboard module
└ databases
|   └ Databases module
└ log
|   └ Log module
└ modules
|   └ Modules module
└ types
|   └ Own predefind TypeScript types & constants
└ index.tsx - Bootstrapper
└ Version.ts - Version number placeholder. The content of this file will be set by the build server
````

### Module structure

Every module follows the same structure. Please note that the described module structure is optimal and all modules should have no or less dependency to other modules but in the real world there are some dependencies between their models and their reducers.

````text
module
|
└ api - All stuff needed for data exchange
|   └ requests - Type definitions for requests
|   └ responses - Type definitions for responses
|   └ ModuleRestClient.ts - Specialized REST client implementation
└ components - Specialized components for this modules
|
└ container - The layout views for this module
└ images - (Optional) Contains all images for this module
└ models - All data models for this module
└ redux - Reducer and action implementation for this module
└ scss - All styles for this module
````
