using MiApp.Models.Models.Workflow.RutaTrabajo;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.Workflow.RutaTrabajo;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaEstructuraRutaWorkflowRepositoryTests
{
    [Fact]
    public async Task SolicitaEstructuraRutaWorkflowAsync_CuandoAliasVacio_RetornaValidacion()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var repository = new SolicitaEstructuraRutaWorkflowRepository(dapper.Object);

        var result = await repository.SolicitaEstructuraRutaWorkflowAsync(string.Empty);

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
        Assert.Null(result.data);
        dapper.Verify(d => d.GetAllAsync<RutasWorkflow>(It.IsAny<QueryOptions>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaEstructuraRutaWorkflowAsync_CuandoHayDatos_RetornaRutasActivas()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        dapper.Setup(d => d.GetAllAsync<RutasWorkflow>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<RutasWorkflow>
            {
                Success = true,
                Message = "YES",
                Data =
                [
                    new RutasWorkflow
                    {
                        id_Ruta = 1,
                        Nombre_Ruta = "ENTRADA",
                        Descripcion_Ruta = "Ruta de entrada",
                        Fecha_Creacion = new DateTime(2026, 3, 16),
                        Estado_Ruta = 1,
                        Archivo_Plantilla = [1, 2, 3],
                        Ruta_Archivo_Server = "/tmp/ruta",
                        Archivo_Plantilla_Mindifucion = "mind"
                    }
                ]
            });

        var repository = new SolicitaEstructuraRutaWorkflowRepository(dapper.Object);
        var result = await repository.SolicitaEstructuraRutaWorkflowAsync("WF");

        Assert.True(result.success);
        Assert.Equal("YES", result.message);
        Assert.NotNull(result.data);
        Assert.Single(result.data!);
        Assert.Equal("ENTRADA", result.data![0].Nombre_Ruta);
    }

    [Fact]
    public async Task SolicitaEstructuraRutaWorkflowAsync_CuandoNoHayRegistros_RetornaSinResultados()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        dapper.Setup(d => d.GetAllAsync<RutasWorkflow>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<RutasWorkflow>
            {
                Success = true,
                Message = "YES",
                Data = []
            });

        var repository = new SolicitaEstructuraRutaWorkflowRepository(dapper.Object);
        var result = await repository.SolicitaEstructuraRutaWorkflowAsync("WF");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
    }
}
