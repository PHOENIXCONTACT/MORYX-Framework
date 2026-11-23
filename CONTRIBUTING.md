# Contributing to MORYX Framework

Thank you for your interest in contributing to the MORYX Framework.
Contributions can include bug reports, feature proposals, documentation, tests, and code changes.

## Before you start

- Search the [existing issues](https://github.com/PHOENIXCONTACT/MORYX-Framework/issues) before opening a new one.
- For a bug, include a minimal reproduction, expected and actual behavior, and relevant environment details.
- Discuss substantial features, API changes, and breaking changes in an issue before implementation.
- Do not report security vulnerabilities in public issues. Follow [SECURITY.md](SECURITY.md) instead.

## Development setup

Prerequisites and supported IDEs are listed in the [README](README.md).

Clone the repository and create a branch from `dev`:

```bash
git clone https://github.com/PHOENIXCONTACT/MORYX-Framework.git
cd MORYX-Framework
git switch dev
git switch -c <type>/<short-description>
```

Build and test your changes from the repository root:

```bash
dotnet build MORYX-Framework.slnx
dotnet test MORYX-Framework.slnx --no-build
```

Some projects may require additional frontend build steps. Follow the instructions in the relevant project or documentation area.

## Making changes

- Keep each pull request focused. Avoid unrelated refactoring, formatting, or dependency updates.
- Follow the repository's [.editorconfig](/.editorconfig) as well as existing [guidelines](https://github.com/PHOENIXCONTACT/MORYX-Home/tree/main/guidelines) and [conventions](https://github.com/PHOENIXCONTACT/MORYX-Home/tree/main#coding-conventions).
- Add or update tests for behavior changes and bug fixes where practical.
- Update documentation when public behavior, configuration, APIs, or user workflows change.
- Preserve compatibility unless a breaking change has been discussed and accepted in advance.
- Do not commit generated build output, local configuration, credentials, or secrets.

## AI-assisted contributions

AI-assisted contributions are welcome when used responsibly.

- You remain accountable for every contribution, including code, tests, documentation, dependencies, licenses, security implications, and other content produced or modified with an agentic coding tool.
- Review, understand, and test all AI-assisted changes before submitting them. Do not submit generated content that you cannot explain and maintain.
- Do not provide confidential information, credentials, personal data, or other content you are not authorized to share to an AI tool.
- **Significantly AI-modified code must be marked in version control**, either at commit level or in the pull request description.
- Significantly AI-assisted contributions must identify the tool or framework and, if available, the specific model version using the following trailer or an equivalent field in the pull request description:

```text
Co-Authored-By: AGENT_NAME:MODEL_VERSION
```

Where:

- `AGENT_NAME` is the name of the AI tool or framework.
- `MODEL_VERSION` is the specific model version used, if available. Use `unknown` if the tool does not expose it.

Example:

```text
Co-Authored-By: ClaudeCode:claude-opus-5-5
```

The disclosure does not reduce the contributor's responsibility for correctness, originality, licensing, security, or compliance with this guide.

## Pull requests

Before submitting a pull request, make sure you comply with our [guideline on creating pull requests](https://github.com/PHOENIXCONTACT/MORYX-Home/blob/main/guidelines/pull-request-guidelines.md) and disclose significant AI assistance as described above.

Draft pull requests are welcome for early feedback. A pull request is ready for review when its scope is clear, required checks pass, and the contributor has completed a self-review.

By contributing, you agree that your contribution is licensed under the repository's [Apache License 2.0](LICENSE).
