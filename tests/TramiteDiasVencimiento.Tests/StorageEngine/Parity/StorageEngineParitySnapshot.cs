using System.Collections.Generic;

namespace TramiteDiasVencimiento.Tests.StorageEngine.Parity;

public sealed class StorageEngineParitySnapshot
{
    public string RequestId { get; init; } = string.Empty;
    public string NombreGabinete { get; init; } = string.Empty;
    public long IdAlmacen { get; init; }
    public int Disco { get; init; }
    public int Carpeta { get; init; }
    public int NumeroPaginas { get; init; }
    public string Formato { get; init; } = string.Empty;
    public string TamanoLegacy { get; init; } = string.Empty;
    public string NombreDig { get; init; } = string.Empty;
    public string NombreFxl { get; init; } = string.Empty;
    public bool WorkflowActivo { get; init; }
    public long? IdRegistroProduccionDocumental { get; init; }
    public Dictionary<string, string?> CamposLog { get; init; } = new();
}
