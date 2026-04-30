using MiApp.DTOs.DTOs.Common;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.GestionCorrespondencia.TiposRespuesta;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaListaTiposRespuestaRepositoryTests
{
    [Fact]
    public async Task SolicitaListaTiposRespuestaAsync_ConstruyeQueryOptionsEsperadas()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        QueryOptions? captured = null;
        dapper
            .Setup(d => d.GetAllAsync<ResponseDropdownDto>(It.IsAny<QueryOptions>()))
            .Callback<QueryOptions>(q => captured = q)
            .ReturnsAsync(new QueryResult<ResponseDropdownDto>
            {
                Success = true,
                Message = "YES",
                Data =
                [
                    new ResponseDropdownDto { Id = 1, Descripcion = "Positiva" }
                ]
            });

        var repository = new SolicitaListaTiposRespuestaRepository(
            dapper.Object,
            Mock.Of<ILogger<SolicitaListaTiposRespuestaRepository>>());

        var result = await repository.SolicitaListaTiposRespuestaAsync("DA");

        Assert.Single(result);
        Assert.NotNull(captured);
        Assert.Equal("ra_tipo_respuesta", captured!.TableName);
        Assert.Equal("DA", captured.DefaultAlias);
        Assert.Equal(500, captured.Limit);
        Assert.True(captured.Filters.ContainsKey("estado"));
        Assert.Equal(1, captured.Filters["estado"]);
        Assert.Contains(captured.Columns, c => c.Contains("id_tipo_respuesta"));
        Assert.Contains(captured.Columns, c => c.Contains("descripcion_tipo_respuesta"));
    }
}
