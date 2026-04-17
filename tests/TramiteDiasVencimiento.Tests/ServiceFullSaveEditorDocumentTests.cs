using MiApp.DTOs.DTOs.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.GestorDocumental.Editor;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.Editor;
using MiApp.Services.Service.GestorDocumental.Editor;
using Moq;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ServiceFullSaveEditorDocumentTests
{
    private readonly Mock<IGuardaEditorDocumentRepository> _guardaRepoMock;
    private readonly Mock<ISincronizaEditorDocumentImagesRepository> _sincronizaRepoMock;
    private readonly Mock<IDbConnectionFactory> _dbFactoryMock;
    private readonly Mock<IDbConnection> _dbConnMock;
    private readonly Mock<IDbTransaction> _dbTransMock;
    private readonly ServiceFullSaveEditorDocument _service;

    public ServiceFullSaveEditorDocumentTests()
    {
        _guardaRepoMock = new Mock<IGuardaEditorDocumentRepository>();
        _sincronizaRepoMock = new Mock<ISincronizaEditorDocumentImagesRepository>();
        _dbFactoryMock = new Mock<IDbConnectionFactory>();
        _dbConnMock = new Mock<IDbConnection>();
        _dbTransMock = new Mock<IDbTransaction>();

        _dbFactoryMock.Setup(f => f.GetOpenConnectionAsync(It.IsAny<string>())).ReturnsAsync(_dbConnMock.Object);
        _dbConnMock.Setup(c => c.BeginTransaction()).Returns(_dbTransMock.Object);

        _service = new ServiceFullSaveEditorDocument(
            _guardaRepoMock.Object,
            _sincronizaRepoMock.Object,
            _dbFactoryMock.Object
        );
    }

    [Fact]
    public async Task CuandoTodoEsCorrecto_HaceCommitYRetornaOk()
    {
        // Arrange
        var request = new FullSaveEditorDocumentRequestDto { DocumentHtml = "<p>HTML</p>", ImageUids = new List<string> { "uuid1" } };
        _guardaRepoMock.Setup(r => r.GuardaEditorDocumentAsync(It.IsAny<GuardaEditorDocumentRequestDto>(), It.IsAny<string>(), It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
            .ReturnsAsync(new AppResponses<RaEditorDocument?> { success = true, data = new RaEditorDocument { DocumentId = 100 } });
        
        _sincronizaRepoMock.Setup(r => r.SincronizaAsync(It.IsAny<long>(), It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<string>(), It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
            .ReturnsAsync(new AppResponses<bool> { success = true, data = true });

        // Act
        var result = await _service.FullSaveAsync(request, "dbAlias");

        // Assert
        Assert.True(result.success);
        _dbTransMock.Verify(t => t.Commit(), Times.Once);
    }

    [Fact]
    public async Task CuandoGuardarDocumentoFalla_HaceRollbackYRetornaError()
    {
        // Arrange
        var request = new FullSaveEditorDocumentRequestDto { DocumentHtml = "<p>HTML</p>" };
        _guardaRepoMock.Setup(r => r.GuardaEditorDocumentAsync(It.IsAny<GuardaEditorDocumentRequestDto>(), It.IsAny<string>(), It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
            .ReturnsAsync(new AppResponses<RaEditorDocument?> { success = false, message = "Error en Repo Guardar" });

        // Act
        var result = await _service.FullSaveAsync(request, "dbAlias");

        // Assert
        Assert.False(result.success);
        Assert.Contains("Error en Repo Guardar", result.message);
        _dbTransMock.Verify(t => t.Rollback(), Times.Once);
        _sincronizaRepoMock.Verify(r => r.SincronizaAsync(It.IsAny<long>(), It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<string>(), It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()), Times.Never);
    }

    [Fact]
    public async Task CuandoSincronizarImagenesFalla_HaceRollbackYRetornaError()
    {
        // Arrange
        var request = new FullSaveEditorDocumentRequestDto { DocumentHtml = "<p>HTML</p>", ImageUids = new List<string> { "uuid1" } };
        _guardaRepoMock.Setup(r => r.GuardaEditorDocumentAsync(It.IsAny<GuardaEditorDocumentRequestDto>(), It.IsAny<string>(), It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
            .ReturnsAsync(new AppResponses<RaEditorDocument?> { success = true, data = new RaEditorDocument { DocumentId = 100 } });
        
        _sincronizaRepoMock.Setup(r => r.SincronizaAsync(It.IsAny<long>(), It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<string>(), It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
            .ReturnsAsync(new AppResponses<bool> { success = false, message = "Error en Sync" });

        // Act
        var result = await _service.FullSaveAsync(request, "dbAlias");

        // Assert
        Assert.False(result.success);
        Assert.Contains("Fallo en la sincronización de imágenes", result.message);
        _dbTransMock.Verify(t => t.Rollback(), Times.Once);
    }
}
