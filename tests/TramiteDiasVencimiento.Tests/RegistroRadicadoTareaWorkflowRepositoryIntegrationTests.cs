using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class RegistroRadicadoTareaWorkflowRepositoryIntegrationTests
{
    [Fact(Skip = "Requiere MySQL Testcontainers/Docker para validar insercion atomica en INICIO_TAREAS_WORKFLOW, DAT_ADIC_TAR{ruta} y ESTADOS_TAREA_WORKFLOW.")]
    public void RegistrarTareaWorkflowAsync_Integration_PendienteDocker()
    {
    }
}
