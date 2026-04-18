using MiApp.DTOs.DTOs.GestorDocumental.Editor;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.Editor;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class GuardaEditorDocumentRepositoryTests
{
    [Fact]
    public async Task CuandoAliasInvalido_NoInvocaConexion()
    {
        var factory = new Mock<IDbConnectionFactory>(MockBehavior.Strict);
        var repo = new GuardaEditorDocumentRepository(factory.Object);

        var res = await repo.GuardaEditorDocumentAsync(new GuardaEditorDocumentRequestDto { DocumentHtml = "<p>x</p>" }, "   ");

        Assert.False(res.success);
        factory.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CuandoHtmlInvalido_NoInvocaConexion()
    {
        var factory = new Mock<IDbConnectionFactory>(MockBehavior.Strict);
        var repo = new GuardaEditorDocumentRepository(factory.Object);

        var res = await repo.GuardaEditorDocumentAsync(new GuardaEditorDocumentRequestDto { DocumentHtml = "   " }, "db1");

        Assert.False(res.success);
        factory.VerifyNoOtherCalls();
    }

    [Fact(Skip = "Requiere MySQL Testcontainers/Docker: validar INSERT/UPDATE real, fechas y existencia en update (SCRUM-146).")]
    public void GuardaEditorDocumentRepository_Integracion_MySqlTestcontainers_Pendiente()
    {
    }
}
