using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class LimpiezaEditorImagesRepositoryIntegrationTests
{
    [Fact(Skip = "Requiere MySQL Testcontainers/Docker para validar limpieza real de huérfanas en ra_editor_document_images.")]
    public void LimpiezaEditorImagesRepository_Integracion_MySqlTestcontainers_Pendiente()
    {
    }
}
