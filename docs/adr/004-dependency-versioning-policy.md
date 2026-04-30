# ADR-004: Dependency Versioning Policy

**Date:** 2026-01-16 \
**Status:** Accepted \
**Context:** Packages

The MORYX Framework and its ecosystem should maintain consistent and intentional dependency versioning. The established but not documented decision was to reference the lowest compatible minor version, only increasing it when a higher version was strictly necessary for security or feature requirements. While this approach was consistently applied in earlier releases, it became less enforced in recent major versions of the MORYX Framework.

This ADR formalizes that principle to prevent unnecessary dependency drift and forced upgrades.

## Motivation

Recent practices have led to dependencies being updated to the latest minor version without technical justification.

**Uncontrolled version increases create:**

- Unnecessary forced upgrades for users.
- Higher risk of version conflicts across projects.
- Increased maintenance overhead.
- Reduced stability and predictability.

**Maintaining the lowest possible minor version ensures:**

- Maximum compatibility across the ecosystem.
- Intentional upgrades for security or new features.
- Better semantic versioning discipline.

**Example:**

The MqttDriver project uses MQTTnet 10.1.x, but the MqttDriver now requires 10.8.x, even though no features introduced after 10.1.x are used. This forces users to upgrade for no reason other than version alignment.

## Decision

Projects must reference the lowest possible minor version of a dependency that satisfies the required functionality.

## Exceptions

Higher minor versions should only be referenced for the follwing reasons:

- If a vulnerability is addressed in a newer minor version, upgrading is mandatory.
- If the code actively uses functionality introduced in a later minor version, the dependency should reflect that.
- When adopting a new major version (e.g., from 8.x to 10.x), start with the base version (e.g., 10.0.0) unless higher versions are required for stability or compatibility.
- When packages depend on other packages within the same repository, referencing the minimal minor version is not possible because only one version exists internally. In such cases:
  - Evaluate whether these packages truly need to reside in the monorepo.
  - Consider moving them to a separate repository to avoid version mixing.
  - Mixing multiple versions within the same repository should be avoided to maintain consistency and prevent dependency drift.

## Consequences

- Users benefit from greater flexibility and stability.
- Maintainers must justify version increases explicitly.
- Security and feature upgrades become deliberate, not incidental.
- Monorepo management requires vigilance to prevent dependency drift.
- Ecosystem remains more predictable and easier to integrate.

## References

- [SemVer](https://semver.org/)
