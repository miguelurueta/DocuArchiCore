using System.Collections.Generic;
using Xunit;

namespace TramiteDiasVencimiento.Tests.StorageEngine.Parity;

internal static class StorageEngineDatabaseAssert
{
    public static void AssertSystem1Update(
        IDictionary<string, long> before,
        IDictionary<string, long> after,
        long expectedProxIdIncrement,
        long expectedNumPagCarpIncrement)
    {
        Assert.Equal(before["proxid"] + expectedProxIdIncrement, after["proxid"]);
        Assert.Equal(before["numpag_carp"] + expectedNumPagCarpIncrement, after["numpag_carp"]);
        Assert.True(after["numcarp"] >= before["numcarp"]);
    }

    public static void AssertWorkflowLogRow(IDictionary<string, string?> row)
    {
        var required = new[]
        {
            "id_tran",
            "desc_op",
            "USER_OPER",
            "RUT_DOCU",
            "MODULO_REGISTRO",
            "GABINETE",
            "ID_TAREA_WF",
            "ID_RUTA_WF"
        };

        foreach (var key in required)
        {
            Assert.True(row.ContainsKey(key), $"Campo de logdocuarchi faltante: {key}");
            Assert.False(string.IsNullOrWhiteSpace(row[key]), $"Campo de logdocuarchi vacío: {key}");
        }
    }
}
