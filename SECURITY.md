# Security policy

Report vulnerabilities privately through GitHub Security Advisories. Do not attach proprietary assemblies.

## Trust boundaries

- Assemblies, PDBs, source, XML docs, resources, and AI responses are untrusted.
- Decompilation is static; DecompileForge never loads or executes a target assembly.
- AI is opt-in. Local Ollama is recommended for restricted intellectual property.
- Cloud requests must pass classification and redaction policy before enablement.
- AI output is never auto-applied. It must compile, pass tests, receive human review, and produce an auditable approval.
- Run hostile or obfuscated input in a Windows Sandbox/VM with CPU, memory, time, filesystem, and network limits.

## Secrets

Use environment variables for local development and a managed secret store in production. Never place keys in appsettings files, command lines, source, audit records, prompts, or exported reports.
