# Media Endpoint

The Media Endpoint provides a REST API for managing media files, including uploading, viewing, and removing files.

## Facade

This endpoint is based on the `IMediaServer` facade.

## Controllers

- **MediaServerController**: Manages media files and their metadata

## Permissions

| Permission String | Description |
|-------------------|-------------|
| `Moryx.Media.CanView` | Permission for all actions related to viewing media files and metadata |
| `Moryx.Media.CanAdd` | Permission for all actions related to uploading and adding new media files |
| `Moryx.Media.CanRemove` | Permission for all actions related to removing media files |
