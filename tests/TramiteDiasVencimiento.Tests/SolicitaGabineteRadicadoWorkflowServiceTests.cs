using AutoMapper;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.DTOs.DTOs.Workflow.RutaTrabajo;
using MiApp.Models.Models.Workflow.RutaTrabajo;
using MiApp.Repository.Repositorio.Workflow.RutaTrabajo;
using MiApp.Services.Service.Workflow.RutaTrabajo;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaGabineteRadicadoWorkflowServiceTests
{
    [Fact]
    public async Task SolicitaPorRadicadoAsync_CuandoExiste_RetornaDtoMapeado()
    {
        var repository = new Mock<ISolicitaGabineteRadicadoWorkflowRepository>();
        var rutaService = new Mock<ISolicitaEstructuraRutaWorkflowService>();
        var mapper = new Mock<IMapper>();

        rutaService
            .Setup(s => s.SolicitaEstructuraRutaWorkflowAsync("WF"))
            .ReturnsAsync(new AppResponses<List<SolicitaEstructuraRutaWorkflowDto>?>
            {
                success = true,
                message = "YES",
                data =
                [
                    new SolicitaEstructuraRutaWorkflowDto
                    {
                        id_Ruta = 1,
                        Nombre_Ruta = "workflow"
                    }
                ],
                errors = []
            });

        var model = new RadicadoGabineteWorkflow
        {
            Radicado = "2500466700035",
            IdTareaWorkflow = 999,
            IdGabinete = 12,
            NombreGabinete = "CORRESPO",
            EstadoExistenciaRadicado = "YES"
        };

        repository
            .Setup(r => r.SolicitaPorRadicadoAsync("2500466700035", "workflow", "WF", "DA"))
            .ReturnsAsync(new AppResponses<RadicadoGabineteWorkflow>
            {
                success = true,
                message = "YES",
                data = model,
                errors = []
            });

        mapper
            .Setup(m => m.Map<RadicadoGabineteWorkflowDto>(model))
            .Returns(new RadicadoGabineteWorkflowDto
            {
                Radicado = "2500466700035",
                IdTareaWorkflow = 999,
                IdGabinete = 12,
                NombreGabinete = "CORRESPO",
                EstadoExistenciaRadicado = "YES"
            });

        var service = new SolicitaGabineteRadicadoWorkflowService(repository.Object, rutaService.Object, mapper.Object);
        var result = await service.SolicitaPorRadicadoAsync("2500466700035", "WF", "DA");

        Assert.True(result.success);
        Assert.Equal("YES", result.message);
        Assert.Equal("CORRESPO", result.data.NombreGabinete);
    }

    [Fact]
    public async Task SolicitaPorIdTareaWorkflowAsync_CuandoIdInvalido_RetornaValidacion()
    {
        var repository = new Mock<ISolicitaGabineteRadicadoWorkflowRepository>();
        var rutaService = new Mock<ISolicitaEstructuraRutaWorkflowService>();
        var mapper = new Mock<IMapper>();

        var service = new SolicitaGabineteRadicadoWorkflowService(repository.Object, rutaService.Object, mapper.Object);
        var result = await service.SolicitaPorIdTareaWorkflowAsync(0, "WF", null);

        Assert.False(result.success);
        Assert.Equal("IdTareaWorkflow requerido", result.message);
        repository.Verify(r => r.SolicitaPorIdTareaWorkflowAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaPorRadicadoAsync_CuandoNoHayRutaActiva_RetornaErrorControlado()
    {
        var repository = new Mock<ISolicitaGabineteRadicadoWorkflowRepository>();
        var rutaService = new Mock<ISolicitaEstructuraRutaWorkflowService>();
        var mapper = new Mock<IMapper>();

        rutaService
            .Setup(s => s.SolicitaEstructuraRutaWorkflowAsync("WF"))
            .ReturnsAsync(new AppResponses<List<SolicitaEstructuraRutaWorkflowDto>?>
            {
                success = true,
                message = "Sin resultados",
                data = null,
                errors = []
            });

        var service = new SolicitaGabineteRadicadoWorkflowService(repository.Object, rutaService.Object, mapper.Object);
        var result = await service.SolicitaPorRadicadoAsync("2500466700035", "WF", "DA");

        Assert.False(result.success);
        Assert.Equal("No existe ruta workflow activa valida", result.message);
        repository.Verify(r => r.SolicitaPorRadicadoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SolicitaPorRadicadoAsync_CuandoNombreRutaInvalido_RetornaValidacion()
    {
        var repository = new Mock<ISolicitaGabineteRadicadoWorkflowRepository>();
        var rutaService = new Mock<ISolicitaEstructuraRutaWorkflowService>();
        var mapper = new Mock<IMapper>();

        rutaService
            .Setup(s => s.SolicitaEstructuraRutaWorkflowAsync("WF"))
            .ReturnsAsync(new AppResponses<List<SolicitaEstructuraRutaWorkflowDto>?>
            {
                success = true,
                message = "YES",
                data =
                [
                    new SolicitaEstructuraRutaWorkflowDto
                    {
                        id_Ruta = 1,
                        Nombre_Ruta = "workflow-01"
                    }
                ],
                errors = []
            });

        var service = new SolicitaGabineteRadicadoWorkflowService(repository.Object, rutaService.Object, mapper.Object);
        var result = await service.SolicitaPorRadicadoAsync("2500466700035", "WF", "DA");

        Assert.False(result.success);
        Assert.Equal("Nombre_Ruta invalido", result.message);
        repository.Verify(r => r.SolicitaPorRadicadoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
