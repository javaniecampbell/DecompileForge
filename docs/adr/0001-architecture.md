# ADR 0001: Layered recovery architecture

**Status:** Accepted

## Decision

Use ports and adapters: Contracts defines stable records/interfaces; Decompilers hosts embedded ILSpy, ilspycmd, and metadata adapters; Comparison performs text, Roslyn-normalized semantic, ApiCompat, and IL fingerprint analysis; AI consumes `IChatClient`; WPF and CLI are thin hosts.

## Consequences

Backends and AI providers are replaceable. Deterministic evidence is collected before probabilistic inference. No AI proposal is treated as recovered truth, and no candidate is applied without independent compilation, tests, and approval.
