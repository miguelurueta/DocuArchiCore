using MiApp.Models.Models.Workflow.RutaTrabajo;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.Workflow.RutaTrabajo;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaCamposListaGestionCorrespondenciaRepositoryTests
{
    [Fact]
    public async Task SolicitaCamposListaGestionCorrespondenciaAsync_CuandoAliasEsVacio_RetornaValidacion()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var repository = new SolicitaCamposListaGestionCorrespondenciaRepository(dapper.Object);

        var result = await repository.SolicitaCamposListaGestionCorrespondenciaAsync(string.Empty, 7);

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
        Assert.Null(result.data);
        dapper.Verify(d => d.GetAllAsync<ConfiguracionListadoRuta>(It.IsAny<QueryOptions>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaCamposListaGestionCorrespondenciaAsync_CuandoIdRutaEsInvalido_RetornaValidacion()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var repository = new SolicitaCamposListaGestionCorrespondenciaRepository(dapper.Object);

        var result = await repository.SolicitaCamposListaGestionCorrespondenciaAsync("WF", 0);

        Assert.False(result.success);
        Assert.Equal("IdRutaWorkflow requerido", result.message);
        Assert.Null(result.data);
        dapper.Verify(d => d.GetAllAsync<ConfiguracionListadoRuta>(It.IsAny<QueryOptions>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaCamposListaGestionCorrespondenciaAsync_CuandoHayDatos_RetornaCsvOrdenado()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        QueryOptions? capturedOptions = null;

        dapper.Setup(d => d.GetAllAsync<ConfiguracionListadoRuta>(It.IsAny<QueryOptions>()))
            .Callback<QueryOptions>(options => capturedOptions = options)
            .ReturnsAsync(new QueryResult<ConfiguracionListadoRuta>
            {
                Success = true,
                Message = "YES",
                Data =
                [
                    new ConfiguracionListadoRuta
                    {
                        Nombre_Campo = "RADICADO",
                        Orden_lista_gestion_tamite = 1,
                        Lista_gestion_tamite = 1
                    },
                    new ConfiguracionListadoRuta
                    {
                        Nombre_Campo = "ASUNTO",
                        Orden_lista_gestion_tamite = 2,
                        Lista_gestion_tamite = 1
                    }
                ]
            });

        var repository = new SolicitaCamposListaGestionCorrespondenciaRepository(dapper.Object);
        var result = await repository.SolicitaCamposListaGestionCorrespondenciaAsync("WF", 7);

        Assert.True(result.success);
        Assert.Equal("YES", result.message);
        Assert.Equal("RADICADO,ASUNTO", result.data);
        Assert.NotNull(capturedOptions);
        Assert.Equal("configuracion_listado_ruta", capturedOptions!.TableName);
        Assert.Equal("WF", capturedOptions.DefaultAlias);
        Assert.Equal(7, capturedOptions.Filters["Rutas_Workflow_id_Ruta"]);
        Assert.Equal(1, capturedOptions.Filters["Lista_gestion_tamite"]);
        Assert.Contains("Nombre_Campo", capturedOptions.Columns);
        Assert.Single(capturedOptions.OrderByFields);
        Assert.Equal("Orden_lista_gestion_tamite", capturedOptions.OrderByFields[0].Column);
    }

    [Fact]
    public async Task SolicitaCamposListaGestionCorrespondenciaAsync_CuandoNoHayRegistros_RetornaSinResultados()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        dapper.Setup(d => d.GetAllAsync<ConfiguracionListadoRuta>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<ConfiguracionListadoRuta>
            {
                Success = true,
                Message = "YES",
                Data = []
            });

        var repository = new SolicitaCamposListaGestionCorrespondenciaRepository(dapper.Object);
        var result = await repository.SolicitaCamposListaGestionCorrespondenciaAsync("WF", 7);

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
    }

    [Fact]
    public async Task SolicitaCamposListaGestionCorrespondenciaAsync_CuandoDapperFalla_RetornaErrorControlado()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        dapper.Setup(d => d.GetAllAsync<ConfiguracionListadoRuta>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<ConfiguracionListadoRuta>
            {
                Success = false,
                Message = "boom"
            });

        var repository = new SolicitaCamposListaGestionCorrespondenciaRepository(dapper.Object);
        var result = await repository.SolicitaCamposListaGestionCorrespondenciaAsync("WF", 7);

        Assert.False(result.success);
        Assert.Contains("Error funcion SolicitaCamposListaGestionCorrespondencia", result.message);
        Assert.Null(result.data);
    }
}
