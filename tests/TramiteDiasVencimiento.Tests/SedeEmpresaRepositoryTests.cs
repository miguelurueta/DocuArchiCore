using MiApp.DTOs.DTOs.GestorDocumental.Sede;
using MiApp.DTOs.DTOs.Errors;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.GestorDocumental.Sede;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public class SedeEmpresaRepositoryTests
{
    [Fact]
    public async Task RetornaIdNombreSedeEmpresa_CuandoHayDatos_RetornaYes()
    {
        var dapper = new Mock<IDapperCrudEngine>();

        dapper.Setup(d => d.GetAllAsync<IdSedeNombreDto>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<IdSedeNombreDto>
            {
                Success = true,
                Message = "OK",
                Data =
                [
                    new IdSedeNombreDto
                    {
                        IdSede = 4,
                        NombreSede = "SEDE PRINCIPAL"
                    }
                ]
            });

        var repository = new SedeEmpresaR(dapper.Object);

        var result = await repository.RetornaIdNombreSedeEmpresa(10, "DA");

        Assert.True(result.success);
        Assert.Equal("YES", result.message);
        Assert.NotNull(result.data);
        Assert.Equal(4, result.data!.IdSede);
        Assert.Equal("SEDE PRINCIPAL", result.data.NombreSede);
    }

    [Fact]
    public async Task RetornaIdNombreSedeEmpresa_CuandoNoHayDatos_RetornaError()
    {
        var dapper = new Mock<IDapperCrudEngine>();

        dapper.Setup(d => d.GetAllAsync<IdSedeNombreDto>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<IdSedeNombreDto>
            {
                Success = true,
                Message = "OK",
                Data = []
            });

        var repository = new SedeEmpresaR(dapper.Object);

        var result = await repository.RetornaIdNombreSedeEmpresa(999, "DA");

        Assert.False(result.success);
        Assert.Contains("Imposible encontrar id sede nombre sede", result.message);
        Assert.Null(result.data);
        Assert.NotNull(result.errors);
        Assert.Contains(result.errors!, e => e is AppError err && err.Field == "idUsuarioRadicador");
    }

    [Fact]
    public async Task RetornaIdNombreSedeEmpresa_ConstruyeQueryEsperada()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        QueryOptions? captured = null;

        dapper.Setup(d => d.GetAllAsync<IdSedeNombreDto>(It.IsAny<QueryOptions>()))
            .Callback<QueryOptions>(q => captured = q)
            .ReturnsAsync(new QueryResult<IdSedeNombreDto>
            {
                Success = true,
                Message = "OK",
                Data = []
            });

        var repository = new SedeEmpresaR(dapper.Object);

        await repository.RetornaIdNombreSedeEmpresa(123, "DA");

        Assert.NotNull(captured);
        Assert.Equal("usuario_radicador AS ur", captured!.TableName);
        Assert.Equal("DA", captured.DefaultAlias);
        Assert.NotNull(captured.Joins);
        Assert.Contains("sedes_empresa", string.Join(" ", captured.Joins!));
        Assert.NotNull(captured.Filters);
        Assert.Equal(123, captured.Filters!["id_usuario"]);
        Assert.Equal(1, captured.Limit);
    }
}
