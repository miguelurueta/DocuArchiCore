using MiApp.DTOs.DTOs.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.GestorDocumental.Editor;
using MiApp.Repository.Repositorio.GestorDocumental.Editor;
using MiApp.Services.Service.GestorDocumental.Editor;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ServiceGuardaEditorDocumentContextTests
{
    [Fact]
    public async Task CuandoContextCodeInexistente_RetornaErrorControlado()
    {
        var catalog = new Mock<ISolicitaEditorContextDefinitionRepository>();
        catalog.Setup(r => r.SolicitaPorContextCodeAsync("X", "db1", null, null))
            .ReturnsAsync(new AppResponses<RaEditorContextDefinition?>
            {
                success = false,
                message = "ContextCode no existe o está inactivo",
                data = null,
                errors = []
            });

        var repo = new Mock<IGuardaEditorDocumentContextRepository>(MockBehavior.Strict);
        var service = new ServiceGuardaEditorDocumentContext(catalog.Object, repo.Object);

        var res = await service.GuardaEditorDocumentContextAsync(new GuardaEditorDocumentContextDto
        {
            DocumentId = 1,
            ContextCode = "X",
            EntityId = 911
        }, "db1");

        Assert.False(res.success);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CuandoRequiresUniqueEntity_YaExisteOtroDocumento_RetornaValidation()
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
                    IsActive = true,
                    RequiresUniqueEntity = true,
                    AllowMultipleDocuments = true,
                    AllowMultipleContextsPerDocument = true
                },
                errors = []
            });

        var repo = new Mock<IGuardaEditorDocumentContextRepository>();
        repo.Setup(r => r.TryGetActiveByEntityAsync(1, 911, "db1", null, null))
            .ReturnsAsync(new AppResponses<RaEditorDocumentContext?> { success = true, message = "YES", data = new RaEditorDocumentContext { ContextId = 5, DocumentId = 99, ContextDefinitionId = 1, EntityId = 911, IsActive = true }, errors = [] });

        var service = new ServiceGuardaEditorDocumentContext(catalog.Object, repo.Object);

        var res = await service.GuardaEditorDocumentContextAsync(new GuardaEditorDocumentContextDto
        {
            DocumentId = 1,
            ContextCode = "RAD",
            EntityId = 911
        }, "db1");

        Assert.False(res.success);
        repo.Verify(r => r.GuardaEditorDocumentContextAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string?>(), It.IsAny<string>(), null, null), Times.Never);
    }

    [Fact]
    public async Task CuandoTodoOk_LlamaRepositorioYRetornaOk()
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
                    IsActive = true,
                    RequiresUniqueEntity = false,
                    AllowMultipleDocuments = true,
                    AllowMultipleContextsPerDocument = true
                },
                errors = []
            });

        var repo = new Mock<IGuardaEditorDocumentContextRepository>();
        repo.Setup(r => r.GuardaEditorDocumentContextAsync(1, 1, 911, "user1", "db1", null, null))
            .ReturnsAsync(new AppResponses<RaEditorDocumentContext?>
            {
                success = true,
                message = "YES",
                data = new RaEditorDocumentContext { ContextId = 10, DocumentId = 1, ContextDefinitionId = 1, EntityId = 911, IsActive = true },
                errors = []
            });

        var service = new ServiceGuardaEditorDocumentContext(catalog.Object, repo.Object);

        var res = await service.GuardaEditorDocumentContextAsync(new GuardaEditorDocumentContextDto
        {
            DocumentId = 1,
            ContextCode = "RAD",
            EntityId = 911,
            CreatedBy = "user1"
        }, "db1");

        Assert.True(res.success);
        repo.VerifyAll();
    }
}

