using AutoMapper;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.DTOs.DTOs.Workflow.RutaTrabajo;
using MiApp.Models.Models.Workflow.RutaTrabajo;
using MiApp.Repository.Repositorio.Workflow.RutaTrabajo;
using MiApp.Services.Service.Workflow.RutaTrabajo;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaExistenciaRadicadoRutaWorkflowServiceTests
{
    [Fact]
    public async Task SolicitaExistenciaRadicadoRutaWorkflowAsync_CuandoExiste_RetornaDtoMapeado()
    {
        var repository = new Mock<ISolicitaExistenciaRadicadoRutaWorkflowRepository>();
        var mapper = new Mock<IMapper>();

        var model = new SolicitaExistenciaRadicadoRutaWorkflow
        {
            Radicado = "260001",
            IdTareaWorkflow = 99,
            EstadoExistenciaRadicado = "YES"
        };

        repository
            .Setup(r => r.SolicitaExistenciaRadicadoRutaWorkflowAsync("260001", "01", "WF"))
            .ReturnsAsync(new AppResponses<SolicitaExistenciaRadicadoRutaWorkflow>
            {
                success = true,
                message = "YES",
                data = model,
                errors = []
            });

        mapper
            .Setup(m => m.Map<SolicitaExistenciaRadicadoRutaWorkflowDto>(model))
            .Returns(new SolicitaExistenciaRadicadoRutaWorkflowDto
            {
                Radicado = "260001",
                IdTareaWorkflow = 99,
                EstadoExistenciaRadicado = "YES"
            });

        var service = new SolicitaExistenciaRadicadoRutaWorkflowService(repository.Object, mapper.Object);
        var result = await service.SolicitaExistenciaRadicadoRutaWorkflowAsync("260001", "01", "WF");

        Assert.True(result.success);
        Assert.Equal("YES", result.message);
        Assert.Equal(99, result.data.IdTareaWorkflow);
    }

    [Fact]
    public async Task SolicitaExistenciaRadicadoRutaWorkflowAsync_CuandoErrorRepositorio_RetornaError()
    {
        var repository = new Mock<ISolicitaExistenciaRadicadoRutaWorkflowRepository>();
        var mapper = new Mock<IMapper>();

        repository
            .Setup(r => r.SolicitaExistenciaRadicadoRutaWorkflowAsync("260001", "01", "WF"))
            .ReturnsAsync(new AppResponses<SolicitaExistenciaRadicadoRutaWorkflow>
            {
                success = false,
                message = "fallo",
                data = new SolicitaExistenciaRadicadoRutaWorkflow(),
                errors = []
            });

        var service = new SolicitaExistenciaRadicadoRutaWorkflowService(repository.Object, mapper.Object);
        var result = await service.SolicitaExistenciaRadicadoRutaWorkflowAsync("260001", "01", "WF");

        Assert.False(result.success);
        Assert.Equal("fallo", result.message);
        Assert.Equal("NO", result.data.EstadoExistenciaRadicado);
        mapper.Verify(m => m.Map<SolicitaExistenciaRadicadoRutaWorkflowDto>(It.IsAny<SolicitaExistenciaRadicadoRutaWorkflow>()), Times.Never);
    }
}
