using MiApp.DTOs.DTOs.Errors;
using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.Radicacion.PlantillaRadicado;
using MiApp.Repository.Repositorio.Radicador.PlantillaRadicado;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using MiApp.Services.Service.Radicacion.Tramite;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public class FechaLimiteRespuestaServiceTests
{
    [Fact]
    public async Task SolicitaFechaLimiteRespuesta_CuandoHayDatos_RetornaSuccessConFechaCalculada()
    {
        var plantillaRepo = new Mock<ISystemPlantillaRadicadoR>();
        var diasRepo = new Mock<ITotalDiasVencimientoTramiteRepository>();
        var feriadosRepo = new Mock<IListaDiasFeriadosTramiteRepository>();

        plantillaRepo
            .Setup(r => r.SolicitaEstructuraPlantillaRadicacionDefault("DA"))
            .ReturnsAsync(new AppResponses<SystemPlantillaRadicado>
            {
                success = true,
                message = "YES",
                data = new SystemPlantillaRadicado
                {
                    id_Plantilla = 100,
                    Nombre_Plantilla_Radicado = "Default",
                    Tipo_Plantilla = "GEN"
                },
                errors = []
            });

        diasRepo
            .Setup(r => r.SolicitaTotalDiasVencimientoTramite(100, 200, "DA"))
            .ReturnsAsync(new AppResponses<int>
            {
                success = true,
                message = "OK",
                data = 3,
                errors = []
            });

        var holiday = NextWeekday(DateTime.Today.AddDays(1));
        feriadosRepo
            .Setup(r => r.SolicitaListaDiasFeriados("DA"))
            .ReturnsAsync(new AppResponses<List<string>>
            {
                success = true,
                message = "OK",
                data = [holiday.ToString("yyyy-MM-dd")],
                errors = []
            });

        var mapper = BuildMapper();
        var service = new FechaLimiteRespuestaService(
            plantillaRepo.Object,
            diasRepo.Object,
            feriadosRepo.Object,
            mapper.Object);

        var result = await service.SolicitaFechaLimiteRespuesta(200, "DA");

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
        Assert.NotNull(result.data);
        Assert.Equal(200, result.data.IdTipoTramite);
        Assert.Equal(100, result.data.IdPlantilla);
        Assert.Equal(3, result.data.DiasVencimiento);

        var expected = CalculateBusinessDueDate(DateTime.Today, 3, [holiday]);
        Assert.Equal(expected.ToString("yyyy-MM-dd"), result.data.FechaLimiteRespuesta);
    }

    [Fact]
    public async Task SolicitaFechaLimiteRespuesta_CuandoNoHayDias_RetornaSinResultados()
    {
        var plantillaRepo = new Mock<ISystemPlantillaRadicadoR>();
        var diasRepo = new Mock<ITotalDiasVencimientoTramiteRepository>();
        var feriadosRepo = new Mock<IListaDiasFeriadosTramiteRepository>();

        plantillaRepo
            .Setup(r => r.SolicitaEstructuraPlantillaRadicacionDefault("DA"))
            .ReturnsAsync(new AppResponses<SystemPlantillaRadicado>
            {
                success = true,
                message = "YES",
                data = new SystemPlantillaRadicado
                {
                    id_Plantilla = 100,
                    Nombre_Plantilla_Radicado = "Default",
                    Tipo_Plantilla = "GEN"
                },
                errors = []
            });

        diasRepo
            .Setup(r => r.SolicitaTotalDiasVencimientoTramite(100, 999, "DA"))
            .ReturnsAsync(new AppResponses<int>
            {
                success = true,
                message = "Sin resultados",
                data = 0,
                errors = []
            });

        var mapper = BuildMapper();
        var service = new FechaLimiteRespuestaService(
            plantillaRepo.Object,
            diasRepo.Object,
            feriadosRepo.Object,
            mapper.Object);

        var result = await service.SolicitaFechaLimiteRespuesta(999, "DA");

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Null(result.data);
    }

    [Fact]
    public async Task SolicitaFechaLimiteRespuesta_CuandoHayExcepcion_RetornaErrorControlado()
    {
        var plantillaRepo = new Mock<ISystemPlantillaRadicadoR>();
        var diasRepo = new Mock<ITotalDiasVencimientoTramiteRepository>();
        var feriadosRepo = new Mock<IListaDiasFeriadosTramiteRepository>();

        plantillaRepo
            .Setup(r => r.SolicitaEstructuraPlantillaRadicacionDefault("DA"))
            .ReturnsAsync(new AppResponses<SystemPlantillaRadicado>
            {
                success = true,
                message = "YES",
                data = new SystemPlantillaRadicado
                {
                    id_Plantilla = 100,
                    Nombre_Plantilla_Radicado = "Default",
                    Tipo_Plantilla = "GEN"
                },
                errors = []
            });

        diasRepo
            .Setup(r => r.SolicitaTotalDiasVencimientoTramite(100, 200, "DA"))
            .ThrowsAsync(new Exception("fallo simulado"));

        var mapper = BuildMapper();
        var service = new FechaLimiteRespuestaService(
            plantillaRepo.Object,
            diasRepo.Object,
            feriadosRepo.Object,
            mapper.Object);

        var result = await service.SolicitaFechaLimiteRespuesta(200, "DA");

        Assert.False(result.success);
        Assert.NotNull(result.errors);
        Assert.Contains(
            result.errors!.Cast<AppError>(),
            e => e.Field == "IdTipoTramite" && e.Message.Contains("fallo simulado"));
    }

    private static Mock<AutoMapper.IMapper> BuildMapper()
    {
        var mapper = new Mock<AutoMapper.IMapper>();
        mapper
            .Setup(m => m.Map<FechaLimiteRespuestaDto>(It.IsAny<object>()))
            .Returns((object source) =>
            {
                var model = (FechaLimiteRespuestaModel)source;
                return new FechaLimiteRespuestaDto
                {
                    IdTipoTramite = model.IdTipoTramite,
                    IdPlantilla = model.IdPlantilla,
                    DiasVencimiento = model.DiasVencimiento,
                    FechaLimiteRespuesta = model.FechaLimiteRespuesta
                };
            });

        return mapper;
    }

    private static DateTime NextWeekday(DateTime candidate)
    {
        var value = candidate.Date;
        while (value.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            value = value.AddDays(1);
        }

        return value;
    }

    private static DateTime CalculateBusinessDueDate(DateTime startDate, int businessDays, IEnumerable<DateTime> holidays)
    {
        var holidaySet = new HashSet<DateTime>(holidays.Select(h => h.Date));
        var current = startDate.Date;
        var added = 0;

        while (added < businessDays)
        {
            current = current.AddDays(1);
            if (current.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                continue;
            }

            if (holidaySet.Contains(current))
            {
                continue;
            }

            added++;
        }

        return current;
    }
}
