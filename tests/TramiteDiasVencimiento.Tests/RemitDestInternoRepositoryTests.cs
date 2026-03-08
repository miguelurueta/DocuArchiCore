using MiApp.DTOs.DTOs.GestorDocumental.usuario;
using MiApp.DTOs.DTOs.Errors;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Services.Service.Usuario;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public class RemitDestInternoRepositoryTests
{
    [Fact]
    public async Task SolicitaIdAreaNombreAreaDestinatario_CuandoHayDatos_RetornaYes()
    {
        var entidadBuilder = new Mock<IEntidadBuilder>();
        var dapper = new Mock<IDapperCrudEngine>();

        dapper.Setup(d => d.GetAllAsync<NombreAreaRemitdestDto>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<NombreAreaRemitdestDto>
            {
                Success = true,
                Message = "OK",
                Data =
                [
                    new NombreAreaRemitdestDto
                    {
                        IdArea = 7,
                        NombreArea = "AREA PRUEBA",
                        CargoRemite = "PROFESIONAL"
                    }
                ]
            });

        var repository = new RemitDestInternoR(entidadBuilder.Object, dapper.Object);

        var result = await repository.SolicitaIdAreaNombreAreaDestinatario(55, "DA");

        Assert.True(result.success);
        Assert.Equal("YES", result.message);
        Assert.NotNull(result.data);
        Assert.Equal(7, result.data!.IdArea);
        Assert.Equal("AREA PRUEBA", result.data.NombreArea);
        Assert.Equal("PROFESIONAL", result.data.CargoRemite);
    }

    [Fact]
    public async Task SolicitaIdAreaNombreAreaDestinatario_CuandoNoHayDatos_RetornaError()
    {
        var entidadBuilder = new Mock<IEntidadBuilder>();
        var dapper = new Mock<IDapperCrudEngine>();

        dapper.Setup(d => d.GetAllAsync<NombreAreaRemitdestDto>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<NombreAreaRemitdestDto>
            {
                Success = true,
                Message = "OK",
                Data = []
            });

        var repository = new RemitDestInternoR(entidadBuilder.Object, dapper.Object);

        var result = await repository.SolicitaIdAreaNombreAreaDestinatario(99, "DA");

        Assert.False(result.success);
        Assert.Contains("Imposible encontrar el area", result.message);
        Assert.Null(result.data);
        Assert.NotNull(result.errors);
        Assert.Contains(result.errors!, e => e is AppError err && err.Field == "idUsuarioDestinatario");
    }

    [Fact]
    public async Task SolicitaIdAreaNombreAreaDestinatario_ConstruyeQueryEsperada()
    {
        var entidadBuilder = new Mock<IEntidadBuilder>();
        var dapper = new Mock<IDapperCrudEngine>();
        QueryOptions? captured = null;

        dapper.Setup(d => d.GetAllAsync<NombreAreaRemitdestDto>(It.IsAny<QueryOptions>()))
            .Callback<QueryOptions>(q => captured = q)
            .ReturnsAsync(new QueryResult<NombreAreaRemitdestDto>
            {
                Success = true,
                Message = "OK",
                Data = []
            });

        var repository = new RemitDestInternoR(entidadBuilder.Object, dapper.Object);

        await repository.SolicitaIdAreaNombreAreaDestinatario(123, "DA");

        Assert.NotNull(captured);
        Assert.Equal("remit_dest_interno AS rdi", captured!.TableName);
        Assert.Equal("DA", captured.DefaultAlias);
        Assert.NotNull(captured.Joins);
        Assert.Contains("areas_depart_radicacion", string.Join(" ", captured.Joins!));
        Assert.NotNull(captured.Filters);
        Assert.Equal(123, captured.Filters!["id_Remit_Dest_Int"]);
    }
}
