using AutoMapper;
using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.Radicacion.RelacionCamposRutaWorklflow;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using MiApp.Services.Service.Radicacion.Tramite;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class RelacionCamposRutaWorklflowServiceTests
{
    [Fact]
    public async Task SolicitaCamposRelacionRutaPlantillaAsync_CuandoExistenDatos_RetornaOk()
    {
        var repository = new Mock<IRelacionCamposRutaWorklflowRepository>();
        var mapper = new Mock<IMapper>();

        var raw = new List<RelacionCamposRutaWorklflow>
        {
            new()
            {
                NombreCampoPlantilla = "Asunto",
                TipoCampoPlantilla = "varchar",
                DimensionCampoPlantilla = "255",
                NombreCampoRuta = "WF_ASUNTO",
                TipoCampoRuta = "varchar",
                DimensionCampoRuta = "255"
            }
        };

        repository
            .Setup(r => r.SolicitaCamposRelacionRutaPlantillaAsync(10, 2, "DA"))
            .ReturnsAsync(new AppResponses<List<RelacionCamposRutaWorklflow>>
            {
                success = true,
                message = "OK",
                data = raw,
                errors = []
            });

        mapper
            .Setup(m => m.Map<List<RelacionCamposRutaWorklflowDto>>(raw))
            .Returns(new List<RelacionCamposRutaWorklflowDto>
            {
                new() { NombreCampoPlantilla = "Asunto", NombreCampoRuta = "WF_ASUNTO" }
            });

        var service = new RelacionCamposRutaWorklflowService(repository.Object, mapper.Object);
        var result = await service.SolicitaCamposRelacionRutaPlantillaAsync(10, 2, "DA");

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
        Assert.Single(result.data!);
        Assert.Equal("Asunto", result.data![0].NombreCampoPlantilla);
    }

    [Fact]
    public async Task SolicitaCamposRelacionRutaPlantillaAsync_CuandoNoHayDatos_RetornaSinResultados()
    {
        var repository = new Mock<IRelacionCamposRutaWorklflowRepository>();
        var mapper = new Mock<IMapper>();

        repository
            .Setup(r => r.SolicitaCamposRelacionRutaPlantillaAsync(10, 2, "DA"))
            .ReturnsAsync(new AppResponses<List<RelacionCamposRutaWorklflow>>
            {
                success = true,
                message = "Sin resultados",
                data = [],
                errors = []
            });

        var service = new RelacionCamposRutaWorklflowService(repository.Object, mapper.Object);
        var result = await service.SolicitaCamposRelacionRutaPlantillaAsync(10, 2, "DA");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.NotNull(result.data);
        Assert.Empty(result.data!);
        mapper.Verify(m => m.Map<List<RelacionCamposRutaWorklflowDto>>(It.IsAny<List<RelacionCamposRutaWorklflow>>()), Times.Never);
    }
}
