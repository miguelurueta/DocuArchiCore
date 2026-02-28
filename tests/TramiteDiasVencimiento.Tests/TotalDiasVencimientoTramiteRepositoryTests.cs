using MiApp.DTOs.DTOs.Errors;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public class TotalDiasVencimientoTramiteRepositoryTests
{
    [Fact]
    public async Task SolicitaTotalDiasVencimientoTramite_CuandoDefaultDbAliasEsNulo_RetornaErrorControlado()
    {
        var dapperMock = new Mock<IDapperCrudEngine>();
        var repository = new TotalDiasVencimientoTramiteRepository(dapperMock.Object);

        var result = await repository.SolicitaTotalDiasVencimientoTramite(100, 200, null!);

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
        Assert.Equal(0, result.data);
        Assert.NotNull(result.errors);
        Assert.Contains(
            result.errors!.Cast<AppError>(),
            e => e.Field == "defaultDbAlias" && e.Message == "DefaultDbAlias requerido");
        dapperMock.Verify(
            d => d.GetAllAsync<TotalDiasVencimientoTramiteRow>(It.IsAny<QueryOptions>()),
            Times.Never);
    }
}
