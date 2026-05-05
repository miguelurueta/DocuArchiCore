using System.Threading.Tasks;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Metadata;
using Xunit;

namespace TramiteDiasVencimiento.Tests.StorageEngine.Parity;

public sealed class StorageEngineParityFixture : IAsyncLifetime
{
    public StorageEngineTestDataSeeder Seeder { get; } = new();
    public StorageEngineLegacyExpectedBuilder LegacyExpectedBuilder { get; } = new();
    public StorageEngineActualResultReader ActualResultReader { get; } = new();
    public StorageEngineFileSystemFixture FileSystem { get; private set; } = new();

    public Task InitializeAsync()
    {
        FileSystem = new StorageEngineFileSystemFixture();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        FileSystem.Dispose();
        return Task.CompletedTask;
    }

    public static StorageDocumentPhysicalMetadata BuildPhysical(string formato = ".PDF")
    {
        return new StorageDocumentPhysicalMetadata
        {
            TotalBytes = 123456,
            TamanoLegacy = "120.56 Kb",
            Formato = formato,
            NumeroPaginas = 3,
            PaginasCalculadasDesdeArchivo = true
        };
    }

    public static StorageNamingResult BuildNaming(long idAlmacen, string extension = ".pdf")
    {
        var suffix = idAlmacen.ToString().PadLeft(8, '0');
        return new StorageNamingResult
        {
            NombreArchivoPrincipal = $"DIG{suffix}{extension}",
            NombreXml = $"FXL{suffix}.xml",
            SegundoNombre = $"DIG{suffix}{extension}"
        };
    }
}
