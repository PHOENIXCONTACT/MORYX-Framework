# Security Integration Guidelines

These guidelines describe the security-relevant steps an integrator must take when building and deploying a product on top of the MORYX Framework.

MORYX is an open-source framework maintained by PHOENIX CONTACT as an [OSS Steward](https://eur-lex.europa.eu/legal-content/EN/TXT/?uri=CELEX:32024R2847) under the EU Cyber Resilience Act (CRA). The framework provides the building blocks for a secure application, but **the security of the final product is the responsibility of the integrator**. Products with digital elements that incorporate MORYX are themselves subject to CRA obligations (e.g. providing security updates for the expected product lifetime, minimum five years).

To report a vulnerability in the MORYX Framework itself, follow the process in [SECURITY.md](../SECURITY.md). Do **not** open a public GitHub issue for security reports.

---

## 1. Authentication and Authorization

By default, the reference application (`StartProject.Asp`) runs **without an Identity and Access Management (IAM) server**: every endpoint is exposed anonymously via the `ExamplePolicyProvider` and the `AllowAnonymousAttribute`. This is intended for development only.

> **We heavily discourage running an application without an IAM server in production environments** — it allows anyone with network access to use every endpoint of your application.

For production, connect your application to a MORYX Access Management (IAM) server:

- [Identity and Access Management overview](articles/moryx-accessmanagement/index.md) — the `Moryx.Identity` and `Moryx.Identity.AccessManagement` packages.
- [How to configure your application to connect to a running IAM server](articles/moryx-accessmanagement/how-to-integrate-in-your-application.md).
- [How to secure your endpoints and controllers with permissions](articles/moryx-accessmanagement/how-to-integrate-in-endpoints.md).

Key practices:

- Replace `ExamplePolicyProvider` with `MoryxAuthorizationPolicyProvider` and register the authentication/authorization middleware as described in the IAM integration guide.
- Protect every controller action with an `[Authorize(Policy = …)]` attribute and a dedicated permission — do not leave endpoints anonymous.
- Host the application and the IAM server under the same second-level domain so the shared authentication cookie works.

## 2. Credentials and Secrets

- The MORYX Framework ships **no default passwords**. All credentials belong to the systems you connect (IAM server, database, message brokers). Never introduce hard-coded credentials.
- Store connection strings and secrets outside of the source tree — use environment variables or a secret manager, not the JSON files in `Config/`.
- Restrict read access to the `Config/` directory to the service account only, since it may contain sensitive connection settings.

## 3. Transport Security (HTTPS)

- The reference application already calls `UseHttpsRedirection()`. In production, provision a certificate from a trusted CA (or your corporate PKI) — never a self-signed certificate.
- Terminate TLS at the application or at a reverse proxy in front of it and require TLS 1.2 as a minimum (TLS 1.3 recommended).
- Enable HSTS (`app.UseHsts()`) for browser-facing deployments.

## 4. CORS

The reference `Startup` enables a permissive CORS policy (`http://localhost:4200`, `AllowAnyMethod`, `AllowAnyHeader`, `AllowCredentials`) **only in the development environment**. For production:

- Do not enable CORS unless a cross-origin client genuinely requires it.
- If required, restrict `WithOrigins` to the exact production origin — never combine `AllowCredentials` with a wildcard origin.

## 5. Logging

MORYX logging is built on `Microsoft.Extensions.Logging` (see [Logging](articles/framework/logging.md)). For production:

- Use a minimum level of `Warning`; enable `Debug` only temporarily for incident investigation.
- Never log credentials, tokens, personal data, or raw request bodies.
- Forward logs to a centralised, access-controlled and tamper-resistant sink with an appropriate retention period.

## 6. File Uploads (Media Module)

If your application uses the Media module, follow the [MORYX-Media Security Guidelines](articles/module-media/security-guidelines.md):

- Store uploaded files in a directory tree separate from the application.
- Remove execution privileges from the upload directory.

## 7. Network Hardening and Firewall

- Expose only the ports the application actually needs (typically 443 for HTTPS). Place MORYX behind a reverse proxy rather than exposing Kestrel directly to a public network.
- Keep the database reachable only from the application host.
- If the OPC UA or MQTT drivers are enabled, restrict their ports to the industrial network segment.

## 8. Principle of Least Privilege

- Run the MORYX process under a dedicated service account without interactive login rights.
- Grant the database account only the permissions it needs on the application schema (no server-admin rights).
- The application requires write access only to its `Config/` and log directories; mount everything else read-only where possible.
- In container deployments, run as a non-root user.

## 9. Dependency Monitoring

MORYX centrally manages its dependencies, but you remain responsible for monitoring the dependencies of your own product for known CVEs:

- Watch [GitHub Security Advisories](https://github.com/PHOENIXCONTACT/MORYX-Framework/security/advisories) for MORYX-specific notifications.
- Enable Dependabot (or an equivalent scanner) on your own repository.
- Before each release, verify that no unresolved advisory with CVSS ≥ 7.0 affects your dependency tree without a documented risk decision.

## 10. Supported Versions and End-of-Life

Security updates for the MORYX Framework are provided according to the policy in [SECURITY.md](../SECURITY.md):

| Version | Status |
|---------|--------|
| 10.x (current) | Supported |
| < 10.0 | End of life — no security updates |

Plan upgrades to a supported major version before its end-of-life date. Under CRA Article 13, the product you ship on top of MORYX must receive security updates for its expected lifetime (minimum five years) — factor MORYX's support window into your own support commitments.

## 11. Incident Response Contact

For a security incident in a product built on MORYX, contact the Phoenix Contact PSIRT:

- Web: [phoenixcontact.com/psirt](https://www.phoenixcontact.com/psirt)
- E-mail: [psirt@phoenixcontact.com](mailto:psirt@phoenixcontact.com)

For responsible disclosure of a vulnerability in the MORYX Framework itself, see [SECURITY.md](../SECURITY.md).
