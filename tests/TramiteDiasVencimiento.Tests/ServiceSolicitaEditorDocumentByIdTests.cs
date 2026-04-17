using MiApp.DTOs.DTOs.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Repository.Repositorio.GestorDocumental.Editor;
using MiApp.Services.Service.GestorDocumental.Editor;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ServiceSolicitaEditorDocumentByIdTests
{
    private readonly Mock<ISolicitaEditorDocumentByIdRepository> _repoMock;
    private readonly ServiceSolicitaEditorDocumentById _service;

    public ServiceSolicitaEditorDocumentByIdTests()
    {
        _repoMock = new Mock<ISolicitaEditorDocumentByIdRepository>();
        _service = new ServiceSolicitaEditorDocumentById(_repoMock.Object);
    }

    [Fact]
    public async Task CuandoDocumentIdEsInvalido_RetornaErrorDeValidacion()
    {
        var result = await _service.SolicitaByIdAsync(0, "dbAlias");
        Assert.False(result.success);
        Assert.Contains("DocumentId requerido", result.message);
        _repoMock.Verify(r => r.SolicitaByIdAsync(It.IsAny<long>(), It.IsAny<string>(), null, null), Times.Never);
    }

    [Fact]
    public async Task CuandoDefaultDbAliasEsVacio_RetornaErrorDeValidacion()
    {
        var result = await _service.SolicitaByIdAsync(10, "");
        Assert.False(result.success);
        Assert.Contains("Alias de base de datos requerido", result.message);
        _repoMock.Verify(r => r.SolicitaByIdAsync(It.IsAny<long>(), It.IsAny<string>(), null, null), Times.Never);
    }

    [Fact]
    public async Task CuandoRepoRetornaOk_RetornaOk()
    {
        _repoMock
            .Setup(r => r.SolicitaByIdAsync(It.IsAny<long>(), It.IsAny<string>(), null, null))
            .ReturnsAsync(new AppResponses<EditorDocumentDetailResponseDto?> { success = true, data = new EditorDocumentDetailResponseDto { DocumentId = 10 } });

        var result = await _service.SolicitaByIdAsync(10, "dbAlias");

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Equal(10, result.data!.DocumentId);
    }
}

