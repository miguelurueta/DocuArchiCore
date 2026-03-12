using MiApp.Repository.Repositorio.Radicador.Configuracion;
using Moq;
using Xunit;
using MiApp.Models.Models.Radicacion.Configuracion;
using MiApp.Repository.DataAccess;

namespace TramiteDiasVencimiento.Tests;

public sealed class ConfiguracionPlantillaRepositoryTests
{
    [Fact]
    public async Task SolicitaConfiguracionPlantillaAsync_CuandoAliasInvalido_RetornaErrorValidacion()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var repository = new ConfiguracionPlantillaRepository(dapper.Object);

        var result = await repository.SolicitaConfiguracionPlantillaAsync(10, 1, string.Empty);

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
        dapper.Verify(x => x.GetAllAsync<RaRadConfigPlantillaRadicacion>(It.IsAny<QueryOptions>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaConfiguracionPlantillaAsync_CuandoSinDatos_RetornaSinResultados()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        dapper.Setup(x => x.GetAllAsync<RaRadConfigPlantillaRadicacion>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<RaRadConfigPlantillaRadicacion>
            {
                Success = true,
                Message = "YES",
                Data = []
            });

        var repository = new ConfiguracionPlantillaRepository(dapper.Object);
        var result = await repository.SolicitaConfiguracionPlantillaAsync(10, 1, "DA");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
    }
}
