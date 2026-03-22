using MiApp.Models.Models.Workflow.Flujo;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.Workflow.Flujo;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaDatosActividadInicioFlujoRepositoryTests
{
    [Fact]
    public async Task SolicitaDatosActividadInicioFlujoAsync_CuandoIdFlujoEsInvalido_RetornaValidacion()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var repository = new SolicitaDatosActividadInicioFlujoRepository(dapper.Object);

        var result = await repository.SolicitaDatosActividadInicioFlujoAsync(0, "WF");

        Assert.False(result.success);
        Assert.Equal("IdFlujoTrabajo requerido", result.message);
        Assert.Equal(0, result.data.IdRegistroActividadFlujoTrabajo);
        dapper.Verify(d => d.GetAllAsync<SolicitaDatosActividadInicioFlujo>(It.IsAny<QueryOptions>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaDatosActividadInicioFlujoAsync_CuandoAliasEsVacio_RetornaValidacion()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var repository = new SolicitaDatosActividadInicioFlujoRepository(dapper.Object);

        var result = await repository.SolicitaDatosActividadInicioFlujoAsync(7, string.Empty);

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
        dapper.Verify(d => d.GetAllAsync<SolicitaDatosActividadInicioFlujo>(It.IsAny<QueryOptions>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaDatosActividadInicioFlujoAsync_CuandoNoHayRegistro_RetornaSinResultadosConCeros()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        dapper.Setup(d => d.GetAllAsync<SolicitaDatosActividadInicioFlujo>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<SolicitaDatosActividadInicioFlujo>
            {
                Success = true,
                Message = "YES",
                Data = []
            });

        var repository = new SolicitaDatosActividadInicioFlujoRepository(dapper.Object);
        var result = await repository.SolicitaDatosActividadInicioFlujoAsync(7, "WF");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Equal(0, result.data.IdRegistroActividadFlujoTrabajo);
        Assert.Equal(0, result.data.IdActividadFlujoTrabajo);
        Assert.Equal(0, result.data.IdUsuarioWorkflowFlujoTrabajo);
    }

    [Fact]
    public async Task SolicitaDatosActividadInicioFlujoAsync_CuandoHayDatos_RetornaRegistro()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        dapper.Setup(d => d.GetAllAsync<SolicitaDatosActividadInicioFlujo>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<SolicitaDatosActividadInicioFlujo>
            {
                Success = true,
                Message = "YES",
                Data =
                [
                    new SolicitaDatosActividadInicioFlujo
                    {
                        IdRegistroActividadFlujoTrabajo = 11,
                        IdActividadFlujoTrabajo = 22,
                        IdUsuarioWorkflowFlujoTrabajo = 0
                    }
                ]
            });

        var repository = new SolicitaDatosActividadInicioFlujoRepository(dapper.Object);
        var result = await repository.SolicitaDatosActividadInicioFlujoAsync(7, "WF");

        Assert.True(result.success);
        Assert.Equal("YES", result.message);
        Assert.Equal(11, result.data.IdRegistroActividadFlujoTrabajo);
        Assert.Equal(22, result.data.IdActividadFlujoTrabajo);
        Assert.Equal(0, result.data.IdUsuarioWorkflowFlujoTrabajo);
    }
}
