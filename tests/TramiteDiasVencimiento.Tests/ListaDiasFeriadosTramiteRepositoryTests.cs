using MiApp.DTOs.DTOs.Errors;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public class ListaDiasFeriadosTramiteRepositoryTests
{
    [Fact]
    public async Task SolicitaListaDiasFeriados_CuandoDefaultDbAliasEsNulo_RetornaErrorControlado()
    {
        var dapperMock = new Mock<IDapperCrudEngine>();
        var repository = new ListaDiasFeriadosTramiteRepository(dapperMock.Object);

        var result = await repository.SolicitaListaDiasFeriados(null!);

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
        Assert.Empty(result.data);
        Assert.NotNull(result.errors);
        Assert.Contains(
            result.errors!.Cast<AppError>(),
            e => e.Field == "defaultDbAlias" && e.Message == "DefaultDbAlias requerido");
        dapperMock.Verify(
            d => d.GetAllAsync<ListaDiasFeriadosRow>(It.IsAny<QueryOptions>()),
            Times.Never);
    }
}
