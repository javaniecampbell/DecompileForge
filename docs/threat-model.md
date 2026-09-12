# Threat model

| Threat | Primary control |
|---|---|
| Target assembly exploits parser | Never execute/load it; isolate worker; enforce time/memory/output limits |
| Prompt injection in strings/comments/resources | Delimit untrusted content; fixed system policy; no autonomous tools or shell |
| Proprietary code exfiltration | Local provider default for restricted data; explicit cloud opt-in; redact and audit |
| Hallucinated business logic | Evidence-only prompt, confidence/assumptions, compile/test/IL validation, approval gate |
| Supply-chain compromise | Central versions, lock files in releases, vulnerability scan, CodeQL, signed artifacts/SBOM |
| Unauthorized reverse engineering | Mandatory authorization record and purpose before a job starts |
| Audit tampering | Ship JSONL to append-only/WORM storage and include artifact/proposal hashes |
