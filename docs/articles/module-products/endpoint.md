# Products Endpoint

The Products Endpoint provides a REST API for managing product types, product instances, and recipes.

## Facade

This endpoint is based on the [`IProductManagement`](/src/Moryx.AbstractionLayer/Products/IProductManagement.cs) facade for product types, instances, and recipes.

## Controllers

- **ProductManagementController**: Manages product types, instances, importers, and recipes

## Permissions

Permissions are defined in [`ProductPermissions`](/src/Moryx.AbstractionLayer.Products.Endpoints/ProductPermissions.cs).

The `Moryx.Products.CanViewTypes` permission controls access to the UI page.

| Permission String | Description |
|-------------------|-------------|
| `Moryx.Products.CanViewTypes` | Permission for all actions related to viewing one or multiple product types |
| `Moryx.Products.CanEditType` | Permission for all actions related to editing one or multiple product types |
| `Moryx.Products.CanDeleteType` | Permission for all actions related to deleting one or multiple product types |
| `Moryx.Products.CanDuplicateType` | Permission for all actions related to duplicating one or multiple product types |
| `Moryx.Products.CanImport` | Permission for all actions related to see and execute a product importer |
| `Moryx.Products.CanViewInstances` | Permission for all actions related to viewing one or multiple product instances |
| `Moryx.Products.CanCreateInstances` | Permission for all actions related to creating one or multiple product instances |
| `Moryx.Products.CanCreateAndEditRecipes` | Permission for all actions related to creating and editing recipes |
