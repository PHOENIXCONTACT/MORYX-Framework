# How we would like reports written

## Layout and length

- Plain English, at most about one page plus the reproducer, with these sections in this order:
  1. **Summary**: one or two sentences. Start the title with the affected package, e.g. `Moryx.Media.Endpoints: path traversal in file download`.
  2. **Affected**: NuGet package(s), commit or branch, and source locations (`path:line`).
  3. **Attacker position**: unauthenticated network client, authenticated user (name the permissions it holds), or malicious device/broker client/peer, as defined in `threat_model.md`.
  4. **Entry point**: the HTTP route and verb, MQTT topic, or protocol message that reaches the faulty code, plus any non-default configuration needed.
  5. **Reproduction**: a self-contained NUnit test or script that runs in the scanner image without network access.
  6. **Impact and severity**: use the rubric in `threat_model.md` and say what happens to the production system, not only to the process.
  7. **Suggested fix**: a short description. Add a patch following `patch_guidelines.md` if you have one.

## What every report should state

- Whether the issue is demonstrated (the reproducer shows it) or inferred from reading the code. Label inferred findings clearly.
- For authorization findings, the `[Authorize(Policy = ...)]` the endpoint has or lacks and the permission the attacker should have needed.
- For deserialization findings, the full data flow from the untrusted input to the deserializer, and the type or gadget that is reached.

## Grouping

- One root cause per report. Put several entry points that reach the same faulty helper in one report.
- Report the same flaw found in several `*.Endpoints` or `*.Web` projects once, and list all affected locations.
