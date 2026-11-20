# MORYX Framework – Threat model

MORYX is a .NET framework for building manufacturing execution and shop-floor control applications. Applications built on it model machines, products and processes as digital twins, dispatch production jobs, and talk to field devices over OPC UA, MQTT, TCP and serial connections. They expose REST APIs and Angular web UIs to operators, engineers and ERP/MES integrations. A typical deployment is a single ASP.NET Core host in a plant's OT network that directly controls physical production equipment. A flaw in these shared packages affects every downstream application, and most of those applications are closed-source products.

## Scope and priorities

Scan the `dev` branch. Runtime code under `src/` that ships as a NuGet package is in scope. Prioritize, in this order:

1. **Authentication and authorization**: `src/Moryx.Identity/` (cookie/JWT handling in `MoryxIdentityHandler`, `MoryxAccessManagementClient`, `MoryxAuthorizationPolicyProvider`) and `src/Moryx.Identity.AccessManagement/` (sign-up, sign-in, refresh tokens, token verification, users/roles/permissions administration).
2. **HTTP APIs**: all `src/*.Endpoints/` projects and `src/Moryx.Runtime.Endpoints/`. The runtime endpoints are the most powerful: they start/stop modules, edit module configuration, and create, migrate, erase and restore databases (`Databases/DatabaseController.cs`, `Modules/`).
3. **Deserialization**: `src/Moryx/Serialization/` (`JsonSettings` uses `TypeNameHandling.Auto`, `EntryConvert` maps generic entry trees from requests onto typed objects), `src/Moryx.Orders.Management/Implementation/Assignment/DocumentAssignStep.cs` (`TypeNameHandling.All`), and every place where data that reached the system via HTTP or MQTT is later deserialized again (e.g. `ProcessStorage`, `OperationStorage`, `GenericEntityMapper`).
4. **File handling**: `src/Moryx.Media.Server/` and `src/Moryx.Media.Endpoints/` (upload validation, storage paths, download), `src/Moryx.ProcessData.SpreadsheetsListener/`.
5. **Device and broker communication**: `src/Moryx/Communication/` (TCP `BinaryConnection`, message header/delimiter interpreters), `src/Moryx.Communication.Serial/`, `src/Moryx.Drivers.Mqtt/`, `src/Moryx.AspNetCore.Mqtt/`, `src/Moryx.AbstractionLayer.Resources.Mqtt.Endpoints/`, `src/Moryx.Drivers.OpcUa/` (including certificate validation).
6. **Web UIs**: Angular apps in `src/*.Web/app/` and `src/Moryx.Launcher/app/` (XSS through product, resource, order, notification or instruction content rendered to operators).

## Attacker positions and trust boundaries

Consider these attackers:

- An unauthenticated client on the plant network that can reach the HTTP port.
- An authenticated user with ordinary permissions (e.g. a worker or operator) trying to gain permissions it was not granted.
- A compromised or malicious device or peer: another client of the MQTT broker, an OPC UA server, or a TCP/serial peer of a driver. Everything such a peer sends is untrusted input.
- A user who uploads media files or spreadsheets, or enters product/order/instruction content that is later rendered to other users.

Authorization works like this. Controllers declare `[Authorize(Policy = <Permission>)]`. `MoryxAuthorizationPolicyProvider` turns each policy into a requirement for a `Permission` claim. `MoryxIdentityHandler` collects these claims from the AccessManagement server using the caller's cookies. Endpoints without an `[Authorize]` attribute are anonymous by design of this handler. A state-changing or data-returning endpoint in a shipped package that lacks the permission it should require, or a way to obtain permissions without being granted them, is a vulnerability.

The following are trusted for their intended powers:

- Deployment operators with access to the host's file system. Plugins are loaded from the application directory (`src/Moryx/Tools/AppDomainBuilder.cs`). Module configuration files, connection strings and certificates are on disk.
- Administrators holding a permission, for what that permission is meant to do. A report that starts from an admin account must show what the attacker gains beyond that permission.
- The database. Database content is trusted only if no lower-privileged path can write it. If HTTP- or MQTT-supplied data is stored and later deserialized with type name handling, report it.

Application code that downstream developers write and register (resources, drivers, plugins, workplan steps) is trusted. A finding requires a path from one of the attacker positions above into the framework code.

## Out of scope

- `src/StartProject.Asp/` is a local development host. It disables authorization on purpose (`MapControllers().WithMetadata(new AllowAnonymousAttribute())`, `ExamplePolicyProvider`) and uses permissive CORS. Report it only if a packaged component inherits that behavior by default.
- Samples, benchmarks and test helpers: `src/*.Samples/`, `src/Moryx.Benchmarking*/`, `src/Moryx.Resources.Benchmarking/`, `src/*TestTools*/`, `src/Moryx.TestModule/`, `src/Moryx.DependentTestModule/`, `src/Tests/`. They may serve as reproducer infrastructure.
- `docs/`, build scripts and GitHub workflows.
- Known CVEs in dependencies without a demonstrated reachable path through MORYX code. Dependabot tracks those.
- Missing transport security when an application is configured without TLS. Plain-text MQTT or OPC UA security mode `None` count only if MORYX selects them silently against an explicit configuration.

## Build and offline reproduction

`.oss-scanner/Dockerfile` installs the .NET 10 SDK and Node.js 24, restores all NuGet and npm dependencies, and builds `MORYX-Framework.slnx` in Debug, including the Angular bundles. Everything runs offline afterwards.

- `moryx-oss-build [project.csproj]` rebuilds the solution or one project offline (`--no-restore`). After the first build, Debug builds skip the npm steps.
- `moryx-oss-test [test.csproj] [dotnet test args]` runs tests without a rebuild. Results go to `/tmp/moryx-oss-results`. Use `--filter FullyQualifiedName~Integration` or `!~Integration` to pick suites.
- Tests use in-memory or SQLite databases and loopback sockets. No external database, MQTT broker or OPC UA server is available.
- Useful harnesses: `src/Moryx.TestTools.IntegrationTest/` (module and endpoint hosting), `src/Tests/Moryx.Runtime.Endpoints.IntegrationTests/`, `src/Tests/Moryx.Media.Endpoints.Tests/`, `src/Tests/Moryx.AbstractionLayer.Products.Endpoints.Tests/`, `src/Tests/Moryx.Communication.Sockets.IntegrationTests/`.

Keep reproducers within dependencies already in the image.

## Severity

Availability matters as much as confidentiality here: a stopped MORYX host stops a production line.

- **Critical**: unauthenticated remote code execution; unauthenticated control of production (dispatching jobs, changing resource configuration, sending driver commands, erasing or replacing databases); authentication bypass for any permission-protected endpoint.
- **High**: privilege escalation from an ordinary user to permissions it was not granted; remote code execution or arbitrary file write that needs an ordinary authenticated user; unauthenticated remote crash or hang of the host; code execution or memory corruption triggered by a malicious device, broker client or peer; path traversal reading arbitrary files.
- **Medium**: stored XSS in the web UIs; denial of service that needs authentication; information disclosure of other users' or orders' data; OPC UA/TLS certificate validation flaws.
- **Low**: defense-in-depth issues, verbose errors, missing hardening headers.

Lower the rating when a non-default configuration is required, and state that configuration.
