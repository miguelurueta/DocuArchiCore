using MiApp.Models.Models.Radicacion.Configuracion;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.Radicador.PlantillaRadicado;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaListaEstructuraConfiguracionPlantillaRadicacionRepositoryTests
{
    [Fact]
    public async Task SolicitaListaEstructuraConfiguracionPlantillaRadicacionAsync_CuandoIdInvalido_RetornaError()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var repository = new SolicitaListaEstructuraConfiguracionPlantillaRadicacionRepository(dapper.Object);

        var result = await repository.SolicitaListaEstructuraConfiguracionPlantillaRadicacionAsync(0, "DA");

        Assert.False(result.success);
        Assert.Equal("IdPlantilla requerido", result.message);
        dapper.Verify(x => x.GetAllAsync<RaRadConfigPlantillaRadicacion>(It.IsAny<QueryOptions>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaListaEstructuraConfiguracionPlantillaRadicacionAsync_CuandoHayDatos_RetornaLista()
    {
        QueryOptions? captured = null;
        var dapper = new Mock<IDapperCrudEngine>();
        dapper.Setup(x => x.GetAllAsync<RaRadConfigPlantillaRadicacion>(It.IsAny<QueryOptions>()))
            .Callback<QueryOptions>(o => captured = o)
            .ReturnsAsync(new QueryResult<RaRadConfigPlantillaRadicacion>
            {
                Success = true,
                Message = "YES",
                Data =
                [
                    new RaRadConfigPlantillaRadicacion
                    {
                        id_rad_config_plantilla_radicacion = 1,
                        system_plantilla_radicado_id_Plantilla = 67,
                        Tipo_radicacion_plantilla = 1,
                        Descripcion_tipo_radicacion = "Externa"
                    },
                    new RaRadConfigPlantillaRadicacion
                    {
                        id_rad_config_plantilla_radicacion = 2,
                        system_plantilla_radicado_id_Plantilla = 67,
                        Tipo_radicacion_plantilla = 2,
                        Descripcion_tipo_radicacion = "Interna"
                    }
                ]
            });

        var repository = new SolicitaListaEstructuraConfiguracionPlantillaRadicacionRepository(dapper.Object);
        var result = await repository.SolicitaListaEstructuraConfiguracionPlantillaRadicacionAsync(67, "DA");

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
        Assert.NotNull(result.data);
        Assert.Equal(2, result.data!.Count);
        Assert.NotNull(captured);
        Assert.Equal("ra_rad_config_plantilla_radicacion", captured!.TableName);
        Assert.Equal(67, captured.Filters["system_plantilla_radicado_id_Plantilla"]);
        Assert.Equal("DA", captured.DefaultAlias);
    }

    [Fact]
    public async Task SolicitaListaEstructuraConfiguracionPlantillaRadicacionAsync_CuandoSinDatos_RetornaSinResultados()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        dapper.Setup(x => x.GetAllAsync<RaRadConfigPlantillaRadicacion>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<RaRadConfigPlantillaRadicacion>
            {
                Success = true,
                Message = "YES",
                Data = []
            });

        var repository = new SolicitaListaEstructuraConfiguracionPlantillaRadicacionRepository(dapper.Object);
        var result = await repository.SolicitaListaEstructuraConfiguracionPlantillaRadicacionAsync(999, "DA");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
    }

    [Fact]
    public async Task SolicitaListaEstructuraConfiguracionPlantillaRadicacionAsync_CuandoMotorFalla_RetornaErrorControlado()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        dapper.Setup(x => x.GetAllAsync<RaRadConfigPlantillaRadicacion>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<RaRadConfigPlantillaRadicacion>
            {
                Success = false,
                Message = "boom",
                Data = []
            });

        var repository = new SolicitaListaEstructuraConfiguracionPlantillaRadicacionRepository(dapper.Object);
        var result = await repository.SolicitaListaEstructuraConfiguracionPlantillaRadicacionAsync(67, "DA");

        Assert.False(result.success);
        Assert.Equal("Error al consultar ra_rad_config_plantilla_radicacion", result.message);
        Assert.Null(result.data);
    }
}
