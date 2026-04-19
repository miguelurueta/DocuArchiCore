using MiApp.DTOs.DTOs.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.GestorDocumental.Editor;
using MiApp.Repository.Repositorio.GestorDocumental.Editor;
using MiApp.Services.Service.GestorDocumental.Editor;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ServiceSolicitaEditorDocumentByContextTests
{
    [Fact]
    public async Task CuandoContextCodeInvalido_RetornaValidation()
    {
        var catalog = new Mock<ISolicitaEditorContextDefinitionRepository>(MockBehavior.Strict);
        var repo = new Mock<ISolicitaEditorDocumentByContextRepository>(MockBehavior.Strict);
        var service = new ServiceSolicitaEditorDocumentByContext(catalog.Object, repo.Object);

        var res = await service.SolicitaEditorDocumentByContextAsync("   ", 1, "db1");

        Assert.False(res.success);
        catalog.VerifyNoOtherCalls();
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CuandoCatalogoInvalido_RetornaErrorControlado()
    {
        var catalog = new Mock<ISolicitaEditorContextDefinitionRepository>();
        catalog.Setup(r => r.SolicitaPorContextCodeAsync("RAD", "db1", null, null))
            .ReturnsAsync(new AppResponses<RaEditorContextDefinition?>
            {
                success = false,
                message = "ContextCode no existe o está inactivo",
                data = null,
                errors = []
            });

        var repo = new Mock<ISolicitaEditorDocumentByContextRepository>(MockBehavior.Strict);
        var service = new ServiceSolicitaEditorDocumentByContext(catalog.Object, repo.Object);

        var res = await service.SolicitaEditorDocumentByContextAsync("rad", 911, "db1");

        Assert.False(res.success);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CuandoRepoOk_EnriqueceContext()
    {
        var catalog = new Mock<ISolicitaEditorContextDefinitionRepository>();
        catalog.Setup(r => r.SolicitaPorContextCodeAsync("RAD", "db1", null, null))
            .ReturnsAsync(new AppResponses<RaEditorContextDefinition?>
            {
                success = true,
                message = "YES",
                data = new RaEditorContextDefinition
                {
                    ContextDefinitionId = 1,
                    ContextCode = "RAD",
                    EntityName = "rad_gestion",
                    RelationType = "documento_respuesta",
                    IsActive = true
                },
                errors = []
            });

        var repo = new Mock<ISolicitaEditorDocumentByContextRepository>();
        repo.Setup(r => r.SolicitaEditorDocumentByContextAsync(1, 911, "db1", null, null))
            .ReturnsAsync(new AppResponses<EditorDocumentDetailByContextResponseDto?>
            {
                success = true,
                message = "OK",
                data = new EditorDocumentDetailByContextResponseDto
                {
                    DocumentId = 10,
                    FormatCode = "html",
                    DocumentHtml = "<p>x</p>",
                    StatusCode = "saved",
                    Context = new EditorDocumentContextResponseDto { ContextId = 5, ContextDefinitionId = 1, ContextCode = "", EntityName = "", RelationType = "", EntityId = 911 }
                },
                errors = []
            });

        var service = new ServiceSolicitaEditorDocumentByContext(catalog.Object, repo.Object);

        var res = await service.SolicitaEditorDocumentByContextAsync("rad", 911, "db1");

        Assert.True(res.success);
        Assert.NotNull(res.data?.Context);
        Assert.Equal("RAD", res.data!.Context!.ContextCode);
        Assert.Equal("rad_gestion", res.data.Context.EntityName);
        Assert.Equal("documento_respuesta", res.data.Context.RelationType);
        Assert.Equal(5, res.data.Context.ContextId);
    }
}

