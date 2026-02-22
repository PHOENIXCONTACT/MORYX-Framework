# Test Cases

## Document View

| No. |Description       | To be done every iteration | Automated | Comment   |
|---|----------------|----------------|-----------|-----------|
|[01](01-show-all-documents.md) | [Show all documents of a selected operation](01-show-all-documents.md) ||||

### Preconditions

In order to execute all of the tests, you need to be able to create an operation and have some products and some corresponding documents.

* Fully setup and running demo project
* At least the DemoImporter was executed to have all needed data for the operation creation
* Documents are available in the backups folder for the product which should be chosen during the operation creation

## Log Messages View

| No. |Description       | To be done every iteration |Automated | Comment   |
|----|---------------|-----------|-----------|-----------|
|[02](02-display-message-without-log-entries.md)| [Without any log entries, a proper message will be displayed](02-display-message-without-log-entries.md) ||[ ] | |
|[03](03-show-message-on-loading-error.md) | [When an error occurs while loading messages from the backend, a proper message will be displayed](03-show-message-on-loading-error.md)| |[ ] | |
|[04](04-creation-failed-message.md) | [Error messages shows up if the creation of an operation failed](04-creation-failed-message.md)  | |[ ] | |
|[05](05-loading-text-during-slow-connection.md)| [Having a slow connection will display a loading text](05-loading-text-during-slow-connection.md)| |[ ] | |

### Preconditions

In order to execute all of the tests, you need to have the orders module set up properly. **All tests assume the orders view to be active** (click on the orders button at the shell)

* Fully setup and running demo project
* At least the DemoImporter was executed to have all needed data for the operation creation.

You will further need

* At least one existing order (see below snippet for an example)
* The `Guid` of that specific order (you might get it from the browser: *Developer-Tools -> Network -> GET orders request*)

```json
POST {baseUrl}/api/moryx/orders

{
  "order": {
    "number": "123456789",
    "type": "1-23",
    "materialParameters": [
    ],
    "operations": [
    ]
  },
  "totalAmount": 10,
  "name": "My order",
  "number": "1234",
  "productIdentifier": "2858946",
  "productRevision": 13,
  "productName": "TT-ST-M-SFP-24AC",
  "recipePreselection": 2
}
```

## General tests

| No. |Description       | To be done every iteration |Automated | Comment   |
|----|---------------|-----------|-----------|-----------|
|[06](06-create-and-start-an-operation.md) | [Create and start an order](06-create-and-start-an-operation.md) | |[]||
|[07](07-clear-add-dialog.md) | [Add operation dialog should be clearable](07-clear-add-dialog.md) | |[]||
|[08](08-show-recipes-of-an-operation.md) | [Show recipes of an operation](08-show-recipes-of-an-operation.md) | |[]||
|[09](09-interrupt-and-report-an-operation.md) | [Interrupt and report an operation](09-interrupt-and-report-an-operation.md) | | [] | |
|[10](10-search-product-in-creation-dialog.md) | [TBD: Search a product in the creation dialog](10-search-product-in-creation-dialog.md) | | [] | |
|[11](11-unknown-materialnumber-in-creation-dialog.md) | [TBD: Add a unknown materialnumber in the creation dialog](11-unknown-materialnumber-in-creation-dialog.md) | | [] | |
|[12](12-show-setup-indicator.md) | [TBD: Show setup indicator at an operation](12-show-setup-indicator.md) | | [] | |
