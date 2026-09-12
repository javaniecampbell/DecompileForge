# Operations runbook

## Workstation setup

1. Install Visual Studio 2022/2026 with .NET desktop development and .NET 8 SDK.
2. Run `./scripts/bootstrap.ps1` from PowerShell 7.
3. Set one provider configuration, for example `ANTHROPIC_API_KEY`, or start Ollama locally.
4. Run `./scripts/build.ps1`.
5. Start `src/DecompileForge.Wpf`, or use the CLI.

## Recovery workflow

1. Confirm ownership or written authorization and record its reference.
2. Hash and preserve the production artifacts read-only.
3. Collect matching PDBs, XML docs, runtimeconfig/deps files, NuGet lock files, build manifests, and Source Link data.
4. Decompile with embedded ILSpy and retain warnings; retry with ilspycmd or metadata backend when needed.
5. Match by assembly identity, fully qualified type/member signature, then path and content similarity.
6. Run source/semantic, strict API, and IL comparisons.
7. Select a proven gap and request an AI proposal only when deterministic evidence is insufficient.
8. Review assumptions, apply on a new branch, compile, run tests, scan, and compare the rebuilt assembly.
9. Obtain human approval and retain the audit JSONL plus exported comparison.

## Incident response

- Provider leak suspected: revoke API key, preserve audit log, identify request IDs, notify the provider/security owner, and classify affected source.
- Malicious assembly suspected: stop processing, preserve hashes, isolate the workstation, and inspect only in a sandbox.
- Invalid AI patch: reject it, retain evidence, add a regression test, and retry only with stronger deterministic context.
