# Security Policy

PHOENIX CONTACT takes the security of the MORYX Framework and its downstream products seriously.
We follow the principle of [Coordinated Vulnerability Disclosure (CVD)](https://www.bsi.bund.de/SharedDocs/Downloads/DE/BSI/CVD/CVD-Leitlinie.html) and are committed to addressing reported vulnerabilities in a timely manner.

## Supported Versions

MORYX follows the [.NET LTS release schedule](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core).
Security updates are provided during both the **Active Support** and **Maintenance Support** phases.
For exact dates see the [MORYX Release Schedule](https://github.com/PHOENIXCONTACT/MORYX-Home/blob/main/processes/release-schedule.md).

| Version | Status              | Security updates until |
|---------|---------------------|------------------------|
| 10.x    | Active Support      | ~ Nov 2030             |
| 8.x     | Maintenance Support | ~ Nov 2028             |
| 6.x     | Maintenance Support | ~ Nov 2026             |
| < 6.0   | End of Life         | —                      |

## Reporting a Vulnerability

**Please do not report security vulnerabilities through public GitHub issues, pull requests, or discussions.**

Report potential vulnerabilities to the Phoenix Contact Product Security Incident Response Team (PSIRT) using one of the following channels:

- **GitHub Private Vulnerability Reporting** (preferred): Use the [Report a vulnerability](../../security/advisories/new) button in the *Security* tab of this repository.
- **PHOENIX CONTACT PSIRT**: Visit [phoenixcontact.com/psirt](https://www.phoenixcontact.com/psirt) or send an e-mail to [psirt@phoenixcontact.com](mailto:psirt@phoenixcontact.com).

### What to Include

To help us triage your report quickly, please provide as much of the following as possible:

- Type of vulnerability (e.g. SQL injection, authentication bypass, path traversal)
- Affected component(s) and version(s)
- The location of the affected source code (tag, branch, commit, or direct URL)
- Step-by-step instructions to reproduce the issue
- Proof-of-concept or exploit code (if available)
- Impact assessment: how could an attacker exploit this issue?
- Any special configuration required to reproduce the issue

## Response Process

| Step | Target timeframe |
| ---- | ---------------- |
| Acknowledgement of receipt | within 72 hours |
| Initial severity assessment | within 7 days |
| Fix or workaround available | depends on severity and complexity |
| Public disclosure (Security Advisory) | coordinated with the reporter |

We will keep you informed about the progress of the fix and agree on a disclosure date with you before publishing a Security Advisory.

## Preferred Language

We prefer all communications to be in English or German.

## Security Advisories

Published security advisories for MORYX Framework are available in the [GitHub Security Advisories](../../security/advisories) section of this repository.

## Regulatory Reporting (CRA Art. 14)

For vulnerabilities actively exploited in the wild, PHOENIX CONTACT's PSIRT reports to the competent national authority (BSI / CERT-Bund) within the timeframes mandated by EU Cyber Resilience Act Article 14: 24-hour early warning, 72-hour full notification, 14-day final report.

## Scope

This policy covers the source code in this repository (`PHOENIXCONTACT/MORYX-Framework`).

If you are building a product or extension on top of MORYX, you are responsible for the security of your own code and for monitoring your additional dependencies for known CVEs. Please refer to our [security integration guidelines](docs/security.md) for recommended practices.

Under the [EU Cyber Resilience Act (CRA)](https://eur-lex.europa.eu/legal-content/EN/TXT/?uri=CELEX:32024R2847), products with digital elements that incorporate MORYX are subject to Article 13 obligations, including providing security updates for the expected product lifetime (minimum five years).
