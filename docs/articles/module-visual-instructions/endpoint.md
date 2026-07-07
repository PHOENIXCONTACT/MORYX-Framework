# Visual Instructions Endpoint

The Visual Instructions Endpoint provides a REST API for managing visual instructions displayed to operators.

## Facade

This endpoint is based on the [`IVisualInstructions`](/src/Moryx.VisualInstructions/IVisualInstructions.cs) facade for visual instructions.

## Controllers

- **VisualInstructionsController**: Manages visual instructions and their completion

## Permissions

Permissions are defined in [`VisualInstructionsPermissions`](/src/Moryx.VisualInstructions.Endpoints/VisualInstructionsPermissions.cs).

The `Moryx.VisualInstructions.CanView` permission controls access to the UI page.

| Permission String | Description |
|-------------------|-------------|
| `Moryx.VisualInstructions.CanView` | Permission for all actions related to viewing visual instructions |
| `Moryx.VisualInstructions.CanAdd` | Permission for all actions related to adding new visual instructions |
| `Moryx.VisualInstructions.CanComplete` | Permission for all actions related to completing visual instructions |
| `Moryx.VisualInstructions.CanClear` | Permission for all actions related to clearing visual instructions |
