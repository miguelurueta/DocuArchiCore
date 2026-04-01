using MiApp.Models.Models.Workflow.Usuario;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.Workflow.usuario;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaConfiguracionListaUsuarioWorkflowRepositoryTests
{
    [Fact]
    public async Task SolicitaConfiguracionListaUsuarioWorkflowAsync_CuandoHayDatos_RetornaRegistro()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        QueryOptions? capturedOptions = null;

        dapper.Setup(engine => engine.GetAllAsync<configuracionUsuarioDTO>(It.IsAny<QueryOptions>()))
            .Callback<QueryOptions>(options => capturedOptions = options)
            .ReturnsAsync(new QueryResult<configuracionUsuarioDTO>
            {
                Success = true,
                Message = "YES",
                Data =
                [
                    new configuracionUsuarioDTO
                    {
                        id_config = 11,
                        Usuario_Workflow_idU_suario = 144,
                        Numero_Tarea_Lista = 25,
                        Fecha_ini_Lista = new DateTime(2026, 4, 1),
                        Fecha_Fin_Lista = new DateTime(2026, 4, 30)
                    }
                ]
            });

        var repository = new SolicitaConfiguracionListaUsuarioWorkflowRepository(dapper.Object);

        var result = await repository.SolicitaConfiguracionListaUsuarioWorkflowAsync(144, " WF ");

        Assert.True(result.success);
        Assert.Equal("YES", result.message);
        Assert.NotNull(result.data);
        Assert.Equal(11, result.data!.id_config);
        Assert.Equal(144, result.data.Usuario_Workflow_idU_suario);
        Assert.Equal(25, result.data.Numero_Tarea_Lista);
        Assert.NotNull(capturedOptions);
        Assert.Equal("configuracion_usuario", capturedOptions!.TableName);
        Assert.Equal("WF", capturedOptions.DefaultAlias);
        Assert.Equal(144, capturedOptions.Filters["Usuario_Workflow_idU_suario"]);
        Assert.Equal("id_config", capturedOptions.OrderByFields[0].Column);
    }

    [Fact]
    public async Task SolicitaConfiguracionListaUsuarioWorkflowAsync_CuandoNoHayDatos_RetornaSinResultados()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        dapper.Setup(engine => engine.GetAllAsync<configuracionUsuarioDTO>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<configuracionUsuarioDTO>
            {
                Success = true,
                Message = "YES",
                Data = []
            });

        var repository = new SolicitaConfiguracionListaUsuarioWorkflowRepository(dapper.Object);

        var result = await repository.SolicitaConfiguracionListaUsuarioWorkflowAsync(144, "WF");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
    }

    [Fact]
    public async Task SolicitaConfiguracionListaUsuarioWorkflowAsync_CuandoAliasEsVacio_RetornaValidacion()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var repository = new SolicitaConfiguracionListaUsuarioWorkflowRepository(dapper.Object);

        var result = await repository.SolicitaConfiguracionListaUsuarioWorkflowAsync(144, string.Empty);

        Assert.False(result.success);
        Assert.Equal("defaultDbAlias requerido", result.message);
        dapper.Verify(engine => engine.GetAllAsync<configuracionUsuarioDTO>(It.IsAny<QueryOptions>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaConfiguracionListaUsuarioWorkflowAsync_CuandoEngineFalla_RetornaErrorControlado()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        dapper.Setup(engine => engine.GetAllAsync<configuracionUsuarioDTO>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<configuracionUsuarioDTO>
            {
                Success = false,
                Message = "boom"
            });

        var repository = new SolicitaConfiguracionListaUsuarioWorkflowRepository(dapper.Object);

        var result = await repository.SolicitaConfiguracionListaUsuarioWorkflowAsync(144, "WF");

        Assert.False(result.success);
        Assert.Equal("boom", result.message);
        Assert.Null(result.data);
    }
}
