# How we would like patches made

## Size and scope of a fix

- Fix the root cause with as few changes as possible. Do not refactor or rename unrelated code.
- If several call sites share one faulty helper (e.g. in `src/Moryx/Serialization/`), fix the helper rather than each call site.
- Keep public APIs of shipped packages source-compatible. If a fix needs a breaking change, explain why in the report and offer the smallest variant.
- Do not weaken authorization, remove `[Authorize(Policy = ...)]` attributes, disable validation or skip failing tests to make a reproducer pass.

## Code style

- Follow `.editorconfig` at the repository root (file-scoped namespaces, 4-space indentation, `_camelCase` private fields).
- Keep the license header of existing files. New files start with:
  ```
  // Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
  // Licensed under the Apache License, Version 2.0
  ```
- Comment only where the code is not obvious.

## Tests

- Add a regression test using NUnit and Moq. Put it in the matching project under `src/Tests/` (e.g. `src/Tests/Moryx.Media.Endpoints.Tests/` for `src/Moryx.Media.Endpoints/`).
- Use the Arrange/Act/Assert layout and `Assert.That(...)` constraints, as the existing tests do.
- Name integration test classes or namespaces so that they contain `Integration`. The CI splits unit and integration tests on that name.
- The test must fail without the fix and pass with it (`moryx-oss-test <test.csproj> --filter <name>`).

## Branch and commit

- Base patches on the `dev` branch. Maintainers port fixes to supported release branches (see `SECURITY.md`).
- Give each patch one short, descriptive subject line that names the affected component, e.g. `Media.Endpoints: reject path segments in upload file names`.
