# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade DecompileForge solution to .NET 10 (net10.0)
**Scope**: Multi-project solution containing libraries, CLI, WPF app, and tests (projects currently target .NET 8).

## Tasks

### 01-core-contracts: Update shared contracts and core libraries

Update csproj TargetFramework for shared contracts and core libraries (src/DecompileForge.Contracts, src/DecompileForge.Core) to net10.0. Verify public APIs remain binary-compatible or apply necessary API shims.

Affected items: src/DecompileForge.Contracts, src/DecompileForge.Core

**Done when**: Projects target net10.0 and build cleanly with no errors; unit tests for core projects pass.

---

### 02-ai-and-decompilers: Upgrade AI and decompiler projects

Update project TFMs for src/DecompileForge.AI and src/DecompileForge.Decompilers to net10.0. Identify and fix API incompatibilities and behavioral changes flagged in assessment (Api.0002/Api.0003). Replace or update any NuGet packages that lack net10.0 support.

Affected items: src/DecompileForge.AI, src/DecompileForge.Decompilers

**Done when**: Projects target net10.0, build succeeds, and core integration tests pass.

---

### 03-cli: Upgrade CLI project

Change TFM for src/DecompileForge.Cli to net10.0, update CLI-related package references as needed, and verify command-line behavior.

Affected items: src/DecompileForge.Cli

**Done when**: CLI builds and runs basic commands locally without regressions.

---

### 04-wpf: Migrate WPF application and address binary incompatibilities

Migrate src/DecompileForge.Wpf to net10.0. Investigate and resolve binary-incompatibility and behavioral-change issues (Api.0001, Api.0003). This may include updating WPF-specific references, replacing APIs, and testing UI behavior.

Affected items: src/DecompileForge.Wpf

**Done when**: WPF project targets net10.0, application launches, and key UI flows function as expected.

---

### 05-tests: Update test projects and deprecated NuGet packages

Update test project TFMs (tests/DecompileForge.Core.Tests, tests/DecompileForge.Comparison.Tests) to net10.0. Replace deprecated NuGet packages identified in assessment (NuGet.0005) with supported alternatives or newer versions. Run full test suite and fix failures.

Affected items: tests/DecompileForge.Core.Tests, tests/DecompileForge.Comparison.Tests

**Done when**: All tests pass on CI locally.

---

### 06-integration-validation: Full-solution build, run tests, and QA

Perform a full solution build, run all tests, and perform runtime checks (smoke tests). Fix any remaining compatibility issues and ensure packages are updated with secure versions.

**Done when**: Solution builds, all tests pass, and smoke tests complete without regressions.

---

### 07-release-prep: Update documentation and finalize changes

Update README and any developer docs noting new target framework and any required runtimes. Prepare changes for release (changelog, notes on breaking changes).

**Done when**: Documentation updated and repository ready for release.
