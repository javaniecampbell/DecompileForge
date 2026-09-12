# 01-core-contracts: Update shared contracts and core libraries

Update csproj TargetFramework for shared contracts and core libraries (src/DecompileForge.Contracts, src/DecompileForge.Core) to net10.0. Verify public APIs remain binary-compatible or apply necessary API shims.

Affected items: src/DecompileForge.Contracts, src/DecompileForge.Core

**Done when**: Projects target net10.0 and build cleanly with no errors; unit tests for core projects pass.
