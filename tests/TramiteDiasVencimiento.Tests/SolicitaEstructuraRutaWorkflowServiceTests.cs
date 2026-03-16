using AutoMapper;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.DTOs.DTOs.Workflow.RutaTrabajo;
using MiApp.Models.Models.Workflow.RutaTrabajo;
using MiApp.Repository.Repositorio.Workflow.RutaTrabajo;
using MiApp.Services.Service.Workflow.RutaTrabajo;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaEstructuraRutaWorkflowServiceTests
{
    [Fact]
    public async Task SolicitaEstructuraRutaWorkflowAsync_CuandoHayDatos_RetornaListaMapeada()
    {
        var repository = new Mock<ISolicitaEstructuraRutaWorkflowRepository>();
        var mapper = new Mock<IMapper>();

        var model = new List<RutasWorkflow>
        {
            new()
            {
                id_Ruta = 1,
                Nombre_Ruta = "ENTRADA",
                Descripcion_Ruta = "Ruta de entrada",
                Fecha_Creacion = new DateTime(2026, 3, 16),
                Estado_Ruta = 1,
                Archivo_Plantilla = [1, 2],
                Ruta_Archivo_Server = "/tmp/ruta",
                Archivo_Plantilla_Mindifucion = "mind"
            }
        };

        var dto = new List<SolicitaEstructuraRutaWorkflowDto>
        {
            new()
            {
                id_Ruta = 1,
                Nombre_Ruta = "ENTRADA",
                Descripcion_Ruta = "Ruta de entrada",
                Fecha_Creacion = new DateTime(2026, 3, 16),
                Estado_Ruta = 1,
                Archivo_Plantilla = [1, 2],
                Ruta_Archivo_Server = "/tmp/ruta",
                Archivo_Plantilla_Mindifucion = "mind"
            }
        };

        repository
            .Setup(r => r.SolicitaEstructuraRutaWorkflowAsync("WF"))
            .ReturnsAsync(new AppResponses<List<RutasWorkflow>?>
            {
                success = true,
                message = "YES",
                data = model,
                errors = []
            });

        mapper
            .Setup(m => m.Map<List<SolicitaEstructuraRutaWorkflowDto>>(model))
            .Returns(dto);

        var service = new SolicitaEstructuraRutaWorkflowService(repository.Object, mapper.Object);
        var result = await service.SolicitaEstructuraRutaWorkflowAsync("WF");

        Assert.True(result.success);
        Assert.Equal("YES", result.message);
        Assert.NotNull(result.data);
        Assert.Single(result.data!);
        Assert.Equal("ENTRADA", result.data![0].Nombre_Ruta);
    }

    [Fact]
    public async Task SolicitaEstructuraRutaWorkflowAsync_CuandoSinResultados_RetornaDataNull()
    {
        var repository = new Mock<ISolicitaEstructuraRutaWorkflowRepository>();
        var mapper = new Mock<IMapper>();

        repository
            .Setup(r => r.SolicitaEstructuraRutaWorkflowAsync("WF"))
            .ReturnsAsync(new AppResponses<List<RutasWorkflow>?>
            {
                success = true,
                message = "Sin resultados",
                data = null,
                errors = []
            });

        var service = new SolicitaEstructuraRutaWorkflowService(repository.Object, mapper.Object);
        var result = await service.SolicitaEstructuraRutaWorkflowAsync("WF");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
        mapper.Verify(m => m.Map<List<SolicitaEstructuraRutaWorkflowDto>>(It.IsAny<List<RutasWorkflow>>()), Times.Never);
    }
}
