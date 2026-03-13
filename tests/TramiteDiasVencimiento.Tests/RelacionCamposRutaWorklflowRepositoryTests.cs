using MiApp.DTOs.DTOs.Errors;
using MiApp.Models.Models.Radicacion.RelacionCamposRutaWorklflow;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class RelacionCamposRutaWorklflowRepositoryTests
{
    [Fact]
    public async Task SolicitaCamposRelacionRutaPlantillaAsync_CuandoAliasVacio_RetornaValidacion()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var repository = new RelacionCamposRutaWorklflowRepository(dapper.Object);

        var result = await repository.SolicitaCamposRelacionRutaPlantillaAsync(1, 1, string.Empty);

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
        Assert.Contains(result.errors!.Cast<AppError>(), e => e.Field == "defaultDbAlias");
        dapper.Verify(d => d.GetAllAsync<RelacionCamposRutaWorklflow>(It.IsAny<QueryOptions>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaCamposRelacionRutaPlantillaAsync_CuandoSinRegistros_RetornaSinResultados()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        QueryOptions? captured = null;
        dapper.Setup(d => d.GetAllAsync<RelacionCamposRutaWorklflow>(It.IsAny<QueryOptions>()))
            .Callback<QueryOptions>(q => captured = q)
            .ReturnsAsync(new QueryResult<RelacionCamposRutaWorklflow>
            {
                Success = true,
                Message = "OK",
                Data = []
            });

        var repository = new RelacionCamposRutaWorklflowRepository(dapper.Object);
        var result = await repository.SolicitaCamposRelacionRutaPlantillaAsync(22, 9, "DA");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.NotNull(result.data);
        Assert.Empty(result.data!);
        Assert.NotNull(captured);
        Assert.Equal("DA", captured!.DefaultAlias);
    }
}
