using System;
using System.IO;

namespace TramiteDiasVencimiento.Tests.StorageEngine.Parity;

public sealed class StorageEngineFileSystemFixture : IDisposable
{
    public string RootPath { get; }

    public StorageEngineFileSystemFixture()
    {
        RootPath = Path.Combine(Path.GetTempPath(), "storage-parity-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(RootPath);
    }

    public (string DigPath, string FxlPath, string FolderPath) CreateLegacyStorageFiles(
        string gabinete,
        int disco,
        int carpeta,
        string digName,
        string fxlName,
        string fxlContent)
    {
        var folder = Path.Combine(RootPath, $"{gabinete}{disco}", carpeta.ToString("D5"));
        Directory.CreateDirectory(folder);

        var digPath = Path.Combine(folder, digName);
        var fxlPath = Path.Combine(folder, fxlName);

        File.WriteAllText(digPath, "DIG-MOCK");
        File.WriteAllText(fxlPath, fxlContent);

        return (digPath, fxlPath, folder);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(RootPath))
            {
                Directory.Delete(RootPath, true);
            }
        }
        catch
        {
            // Fixture best-effort cleanup.
        }
    }
}
