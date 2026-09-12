# 05-tests: Update test projects and deprecated NuGet packages

Update test project TFMs (tests/DecompileForge.Core.Tests, tests/DecompileForge.Comparison.Tests) to net10.0. Replace deprecated NuGet packages identified in assessment (NuGet.0005) with supported alternatives or newer versions. Run full test suite and fix failures.

Affected items: tests/DecompileForge.Core.Tests, tests/DecompileForge.Comparison.Tests

**Done when**: All tests pass on CI locally.
