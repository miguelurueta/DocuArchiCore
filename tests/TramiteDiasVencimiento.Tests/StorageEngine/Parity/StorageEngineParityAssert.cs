using Xunit;

namespace TramiteDiasVencimiento.Tests.StorageEngine.Parity;

internal static class StorageEngineParityAssert
{
    public static void AssertCore(StorageEngineParitySnapshot expected, StorageEngineParitySnapshot actual)
    {
        Assert.Equal(expected.RequestId, actual.RequestId);
        Assert.Equal(expected.NombreGabinete, actual.NombreGabinete);
        Assert.Equal(expected.IdAlmacen, actual.IdAlmacen);
        Assert.Equal(expected.Disco, actual.Disco);
        Assert.Equal(expected.Carpeta, actual.Carpeta);
        Assert.Equal(expected.NumeroPaginas, actual.NumeroPaginas);
        Assert.Equal(expected.Formato, actual.Formato);
        Assert.Equal(expected.TamanoLegacy, actual.TamanoLegacy);
        Assert.Equal(expected.NombreDig, actual.NombreDig);
        Assert.Equal(expected.NombreFxl, actual.NombreFxl);
        Assert.Equal(expected.WorkflowActivo, actual.WorkflowActivo);
    }

    public static void AssertWorkflow(StorageEngineParitySnapshot expected, StorageEngineParitySnapshot actual)
    {
        AssertCore(expected, actual);
        Assert.Equal(expected.CamposLog.Count, actual.CamposLog.Count);

        foreach (var entry in expected.CamposLog)
        {
            Assert.True(actual.CamposLog.ContainsKey(entry.Key), $"Campo log faltante: {entry.Key}");
            Assert.Equal(entry.Value, actual.CamposLog[entry.Key]);
        }
    }
}
