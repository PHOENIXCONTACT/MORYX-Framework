# MarvinPlatform

Welcome to the MarvinPlatform
Currently the `preperation` branch is used to get rid of binaries.

## Binaries to remove

- \Documentation\MarvinPlatform.eap
- \Documentation\Doxygen\html\**\*.jpg

## ToDos for Platform 3

### General

- Move config attributes to platform (possible values, cpu count, integer steps, ...)

### Runtime

- Discuss about the shipped executable, maybe an approach like asp.net is
  - every application uses its own executable
  - matches nuget packacke structures
    - no start project is needed
    - clients can have its own icons / behavior
- Extend ExtendedHostConfig for configuring wcf service binding and timeouts

### ClientFramework

- Find new name
- Add ScrollViewer to ConfigEditor-Control