# Build validation status

## Completed in packaging environment

- Generated-file inventory and placeholder scan.
- XML parsing of all project, props, and XAML files.
- Architecture, project-reference, configuration, and secret scans.
- ZIP integrity and checksum generation.

## Required on Windows before release

The packaging sandbox did not have a .NET SDK and had insufficient disk space to install a complete SDK. Therefore the package has not been claimed as compiled. Run:

```powershell
./scripts/bootstrap.ps1
./scripts/build.ps1
```

Then resolve any NuGet/API changes strictly within the corresponding adapters. Verify WPF startup, both CLI exit paths, sample decompilation, text/semantic diff, strict ApiCompat, IL diff, each enabled AI provider, cancellation, quotas, and audit persistence. A production release must not be signed until CI is green.
