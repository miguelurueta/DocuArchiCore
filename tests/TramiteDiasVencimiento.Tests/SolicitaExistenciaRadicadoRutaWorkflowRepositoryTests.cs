using MiApp.Models.Models.Workflow.RutaTrabajo;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.Workflow.RutaTrabajo;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaExistenciaRadicadoRutaWorkflowRepositoryTests
{
    [Fact]
    public async Task SolicitaExistenciaRadicadoRutaWorkflowAsync_CuandoAliasVacio_RetornaValidacion()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var repository = new SolicitaExistenciaRadicadoRutaWorkflowRepository(dapper.Object);

        var result = await repository.SolicitaExistenciaRadicadoRutaWorkflowAsync("260001", "01", string.Empty);

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
        dapper.Verify(d => d.GetAllAsync<SolicitaExistenciaRadicadoRutaWorkflow>(It.IsAny<QueryOptions>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaExistenciaRadicadoRutaWorkflowAsync_CuandoNoHayRegistro_RetornaNoConYes()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        dapper.Setup(d => d.GetAllAsync<SolicitaExistenciaRadicadoRutaWorkflow>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<SolicitaExistenciaRadicadoRutaWorkflow>
            {
                Success = true,
                Message = "OK",
                Data = []
            });

        var repository = new SolicitaExistenciaRadicadoRutaWorkflowRepository(dapper.Object);
        var result = await repository.SolicitaExistenciaRadicadoRutaWorkflowAsync("260001", "01", "WF");

        Assert.True(result.success);
        Assert.Equal("YES", result.message);
        Assert.Equal("NO", result.data.EstadoExistenciaRadicado);
        Assert.Equal(0, result.data.IdTareaWorkflow);
    }
}
