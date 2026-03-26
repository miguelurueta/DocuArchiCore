using MiApp.DTOs.DTOs.Errors;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.Radicador.PlantillaRadicado;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class RaRadEstadosModuloRadicacionRepositoryTests
{
    [Fact]
    public async Task ActualizaEstadoModuloRadicacio_CuandoAliasInvalido_RetornaValidacion()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var repository = new RaRadEstadosModuloRadicacionR(dapper.Object);

        var result = await repository.ActualizaEstadoModuloRadicacio(string.Empty, 10, 2, 321L);

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
        dapper.Verify(x => x.UpdateDynamicWithValidationAsync(It.IsAny<QueryOptions>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ActualizaEstadoModuloRadicacio_CuandoUpdateExitoso_RetornaYes()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        dapper.Setup(x => x.UpdateDynamicWithValidationAsync(It.IsAny<QueryOptions>(), It.IsAny<string>()))
            .ReturnsAsync(new QueryResult<bool>
            {
                Success = true,
                Message = "YES",
                Data = [true]
            });

        var repository = new RaRadEstadosModuloRadicacionR(dapper.Object);
        var result = await repository.ActualizaEstadoModuloRadicacio("DA", 10, 2, 321L);

        Assert.True(result.success);
        Assert.True(result.data);
        Assert.Equal("YES", result.message);
        dapper.Verify(x => x.UpdateDynamicWithValidationAsync(
            It.Is<QueryOptions>(q =>
                q.TableName == "ra_rad_estados_modulo_radicacion"
                && q.DefaultAlias == "DA"
                && (long)q.Filters["id_estado_radicado"] == 10
                && (int)q.ReglasValidacionCampo["estado"] == 2
                && (long)q.ReglasValidacionCampo["id_tarea_workflow"] == 321L),
            "ActualizaEstadoModuloRadicacio"), Times.Once);
    }

    [Fact]
    public async Task ActualizaEstadoModuloRadicacio_CuandoDapperLanzaExcepcion_RetornaErrorControlado()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        dapper.Setup(x => x.UpdateDynamicWithValidationAsync(It.IsAny<QueryOptions>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("db failed"));

        var repository = new RaRadEstadosModuloRadicacionR(dapper.Object);
        var result = await repository.ActualizaEstadoModuloRadicacio("DA", 10, 2, 321L);

        Assert.False(result.success);
        Assert.Equal("Error al actualizar estado del modulo de radicacion", result.message);
        Assert.Contains(result.errors!.Cast<AppError>(), error => error.Field == "ActualizaEstadoModuloRadicacio" && error.Message.Contains("db failed"));
    }

    [Fact]
    public async Task ActualizaEstadoModuloRadicacio_CuandoIdTareaWorkflowInvalido_RetornaValidacion()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var repository = new RaRadEstadosModuloRadicacionR(dapper.Object);

        var result = await repository.ActualizaEstadoModuloRadicacio("DA", 10, 2, 0L);

        Assert.False(result.success);
        Assert.Equal("IdTareaWorkflow requerido", result.message);
        dapper.Verify(x => x.UpdateDynamicWithValidationAsync(It.IsAny<QueryOptions>(), It.IsAny<string>()), Times.Never);
    }
}
