using MiApp.Models.Models.Workflow.RutaTrabajo;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.Workflow.RutaTrabajo;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaGabineteRadicadoWorkflowRepositoryTests
{
    [Fact]
    public async Task SolicitaPorRadicadoAsync_CuandoAliasWorkflowVacio_RetornaValidacion()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var repository = new SolicitaGabineteRadicadoWorkflowRepository(dapper.Object);

        var result = await repository.SolicitaPorRadicadoAsync("2500466700035", "workflow", string.Empty, "DA");

        Assert.False(result.success);
        Assert.Equal("DefaultDbAliasWorkflow requerido", result.message);
        dapper.Verify(d => d.GetAllAsync<RadicadoGabineteWorkflow>(It.IsAny<QueryOptions>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaPorRadicadoAsync_CuandoNoExisteRegistro_RetornaNoConYes()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        dapper.Setup(d => d.GetAllAsync<RadicadoGabineteWorkflow>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<RadicadoGabineteWorkflow>
            {
                Success = true,
                Message = "YES",
                Data = []
            });

        var repository = new SolicitaGabineteRadicadoWorkflowRepository(dapper.Object);
        var result = await repository.SolicitaPorRadicadoAsync("2500466700035", "workflow", "WF", "DA");

        Assert.True(result.success);
        Assert.Equal("YES", result.message);
        Assert.Equal("NO", result.data.EstadoExistenciaRadicado);
        Assert.Equal("2500466700035", result.data.Radicado);
    }

    [Fact]
    public async Task SolicitaPorIdTareaWorkflowAsync_CuandoExisteRegistroYGabinete_RetornaYesConNombre()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        dapper.SetupSequence(d => d.GetAllAsync<RadicadoGabineteWorkflow>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<RadicadoGabineteWorkflow>
            {
                Success = true,
                Message = "YES",
                Data =
                [
                    new RadicadoGabineteWorkflow
                    {
                        Radicado = "2500466700035",
                        IdTareaWorkflow = 777,
                        IdGabinete = 12
                    }
                ]
            })
            .ReturnsAsync(new QueryResult<RadicadoGabineteWorkflow>
            {
                Success = true,
                Message = "YES",
                Data =
                [
                    new RadicadoGabineteWorkflow
                    {
                        NombreGabinete = "CORRESPO"
                    }
                ]
            });

        var repository = new SolicitaGabineteRadicadoWorkflowRepository(dapper.Object);
        var result = await repository.SolicitaPorIdTareaWorkflowAsync(777, "workflow", "WF", "DA");

        Assert.True(result.success);
        Assert.Equal("YES", result.data.EstadoExistenciaRadicado);
        Assert.Equal("CORRESPO", result.data.NombreGabinete);
    }
}

