using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using DecompileForge.Contracts;

namespace DecompileForge.Comparison;

public sealed class IlFingerprintService : IIlFingerprintService
{
    public Task<IReadOnlyList<IlFingerprint>> CreateAsync(string assemblyPath, CancellationToken cancellationToken = default)
    {
        using var stream = File.OpenRead(assemblyPath); using var pe = new PEReader(stream); var md = pe.GetMetadataReader(); var result = new List<IlFingerprint>();
        foreach (var th in md.TypeDefinitions)
        {
            var type = md.GetTypeDefinition(th); var typeName = $"{md.GetString(type.Namespace)}.{md.GetString(type.Name)}".Trim('.');
            foreach (var mh in type.GetMethods())
            {
                cancellationToken.ThrowIfCancellationRequested(); var method = md.GetMethodDefinition(mh); var name = md.GetString(method.Name);
                var signature = Convert.ToHexString(md.GetBlobBytes(method.Signature)); byte[] il = [];
                if (method.RelativeVirtualAddress != 0) il = pe.GetMethodBody(method.RelativeVirtualAddress).GetILBytes() ?? [];
                result.Add(new(Path.GetFileName(assemblyPath), typeName, name, signature, Convert.ToHexString(SHA256.HashData(il)).ToLowerInvariant(), CountInstructions(il)));
            }
        }
        return Task.FromResult<IReadOnlyList<IlFingerprint>>(result);
    }

    private static int CountInstructions(byte[] il)
    {
        var count = 0; for (var i = 0; i < il.Length; count++) { var op = il[i++]; if (op == 0xFE && i < il.Length) i++; i = Math.Min(il.Length, i + OperandSize(op, il, i)); }
        return count;
    }
    private static int OperandSize(byte op, byte[] il, int i) => op switch
    {
        0x20 or 0x28 or 0x6F or 0x72 or 0x73 or 0x74 or 0x7B or 0x7C or 0x7D or 0x7E or 0x7F or 0x80 or 0x8C or 0x8D or 0xA3 or 0xA4 or 0xA5 or 0xD0 => 4,
        0x21 or 0x23 => 8,
        0x1F or 0x0E or 0x10 or 0x11 or 0x12 or 0x13 or 0x1E => 1,
        0x22 => 4,
        0x38 or 0x39 or 0x3A or 0x3B or 0x3C or 0x3D or 0x3E or 0x3F or 0x40 or 0x41 or 0x42 or 0x43 or 0x44 or 0x45 => 4,
        0x2B or >= 0x2C and <= 0x37 => 1,
        _ => 0
    };
}
