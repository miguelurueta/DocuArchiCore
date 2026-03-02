using MiApp.DTOs.DTOs.Errors;
using MiApp.Models.Models.Radicacion.Tramite;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public class ListaRadicadosPendientesRepositoryTests
{
    [Fact]
    public async Task SolicitaListaRadicadosPendientes_CuandoDefaultDbAliasEsNulo_RetornaErrorControlado()
    {
        var dapperMock = new Mock<IDapperCrudEngine>();
        var repository = new ListaRadicadosPendientesRepository(dapperMock.Object);

        var result = await repository.SolicitaListaRadicadosPendientes(100, 10, null!);

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
        Assert.NotNull(result.errors);
        Assert.Contains(
            result.errors!.Cast<AppError>(),
            e => e.Field == "defaultDbAlias" && e.Message == "DefaultDbAlias requerido");

        dapperMock.Verify(
            d => d.GetAllAsync<raradestadosmoduloradicacion>(It.IsAny<QueryOptions>()),
            Times.Never);
    }

    [Fact]
    public async Task SolicitaListaRadicadosPendientes_CuandoNoHayRegistros_RetornaSinResultados()
    {
        var dapperMock = new Mock<IDapperCrudEngine>();
        QueryOptions? capturedOptions = null;

        dapperMock
            .Setup(d => d.GetAllAsync<raradestadosmoduloradicacion>(It.IsAny<QueryOptions>()))
            .Callback<QueryOptions>(q => capturedOptions = q)
            .ReturnsAsync(new QueryResult<raradestadosmoduloradicacion>
            {
                Success = true,
                Message = "OK",
                Data = []
            });

        var repository = new ListaRadicadosPendientesRepository(dapperMock.Object);

        var result = await repository.SolicitaListaRadicadosPendientes(100, 55, "DA");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
        Assert.NotNull(capturedOptions);
        Assert.Equal("DA", capturedOptions!.DefaultAlias);
        Assert.Equal(100, capturedOptions.Filters["system_plantilla_radicado_id_Plantilla"]);
        Assert.Equal(55, capturedOptions.Filters["id_usuario_radicado"]);
        Assert.Equal(1, capturedOptions.Filters["estado"]);
    }
}

