# MarvinPlatform

Welcome to the MarvinPlatform
Currently the `preperation` branch is used to get rid of binaries.

## Binaries to remove

- \Documentation\MarvinPlatform.eap
- \Documentation\Doxygen\html\**\*.jpg

## ToDos for Platform 3

### Build

- Activate symbol store

### General

- Move config attributes to platform (possible values, cpu count, integer steps, ...)
- Remove '`IDisposable` from `IModulePlugin` and add `Stop()` -> `IDisposable` should be optional
- Remove `NoTracking` for container component lifecycle (?)

### Runtime

- Try to reduce assemblies
- Discuss about the shipped executable, maybe an approach like asp.net is
  - every application uses its own executable
  - matches nuget packacke structures
    - no start project is needed
    - clients can have its own icons / behavior
- Extend ExtendedHostConfig for configuring wcf service binding and timeouts

### Maintenance

- Refactor from requests for ReplaceEntry and RequestEntry to prototypes

### ClientFramework

- Find new name
- Add ScrollViewer to ConfigEditor-Control