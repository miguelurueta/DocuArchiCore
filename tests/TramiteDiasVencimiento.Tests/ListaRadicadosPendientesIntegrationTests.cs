using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ListaRadicadosPendientesIntegrationTests
{
    [Fact(Skip = "Requiere MySQL Testcontainers/Docker para validar la lectura real y el payload DynamicUiTable de radicados pendientes.")]
    public void SolicitaListaRadicadosPendientes_Integracion_MySqlTestcontainers_Pendiente()
    {
    }
}
