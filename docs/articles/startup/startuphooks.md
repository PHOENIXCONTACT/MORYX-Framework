# Startup hooks

Startup hooks are small reusable tools that run before the launch of the main application.
The initial idea was to use them for manual testing to get the application into a defined state,
but it is also possible to ensure some external dependencies are met.

## Definition

Startup hooks implement the lightweight IStartupHook interface.
That just gives them a priority to make sure they run in the correct order and a RunAsync function to execute the functionality.

## Usage

After including the package and namespace "Moryx.Startup.Hooks" you can use the
extension methods AddStartupHooks() and RunHooksAsync() in your program.cs to register and then execute all StartupHooks.

## Predefined Hooks

* **DatabaseHook** Can delete and create selected or all databases
* **OrderHook** Can create orders with preselected recipes
* **ResourceHook** Can run ResourceInitializers
* **ProductHook** Can run ProductInitializers

## Configuration

**In contrast to most other functions in MORYX Hooks are configured using Microsoft.Extensions.Configuration.** This means the execution can be controlled using the `appsettings.*.json` for different environments, command line paramters or environment variables.

The predefined Hooks are all configured using a subsection under `Hooks`

An appsettings.devlopment.json that recreates everything for easy testing might look something like this:

```json
// appsettings.development.json
{
  // .. other config
  "Hooks": {
    "Databases": {
      "DeleteAllDbs": true,
      "CreateDbs": true
    },
    "Products": {
      "Importers": [
        {
          "Name": "AppSpecificProductImporter",
          "ConfigType": "AppSpecificImportParameters",
          "OnlyOnFreshDb": false,
          "Disabled": false,
          "Parameters": {
            "OverrideProducts": true,
            "OverrideRecipes": true,
            "OverrideWorkplans": true
          }
        }
      ]
    },
    "Resources": {
      "Initializers": [
        {
          "Name": "AppSpecificInitializer",
          "ConfigType": "AppSpecificInitializerConfig",
          "Disabled": false,
          "Parameters": {
            "Local": true
          }
        }
      ]
    },
    "Orders": {
      "Operations": [
        {
          "OrderNumber": "1234",
          "Number": "0010",
          "ProductIdentifier": "1014421",
          "ProductRevision": 3,
          "TotalAmount": 100,
          "UnderDelivery": 50,
          "OverDelivery": 150,
          "RecipePreselection": 5
        },
        {
          "OrderNumber": "5678",
          "Number": "0010",
          "ProductIdentifier": "1014421",
          "ProductRevision": 3,
          "TotalAmount": 100,
          "UnderDelivery": 50,
          "OverDelivery": 150,
          "RecipePreselection": 6
        }
      ]
    }
  }

  // .. other config
}

```

## Defining custom hooks

Custom hooks can easily be defined by implementing the `IStartupHook` interface.
Because hooks in Moryx are commonly waiting for Module Events, there are two base classes to help with that, `ModuleHook` and `ModuleStartHook`. They are designed to select a module by their facade interface and allow you to define logic for module state changes.
These base classes are used in the predefined hooks, besides the DatabaseHook so you can look there for examples.

