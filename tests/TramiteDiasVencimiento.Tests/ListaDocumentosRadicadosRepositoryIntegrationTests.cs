using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ListaDocumentosRadicadosRepositoryIntegrationTests
{
    [Fact(Skip = "Requiere MySQL Testcontainers/Docker para validar consulta legacy parametrizada sobre tabla real de gabinete.")]
    public void SolicitaDocumentosRelacionadosAsync_Integracion_MySqlTestcontainers_Pendiente()
    {
    }
}

