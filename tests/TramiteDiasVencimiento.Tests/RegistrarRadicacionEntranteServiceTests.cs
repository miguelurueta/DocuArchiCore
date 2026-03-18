using MiApp.DTOs.DTOs.Radicacion.Configuracion;
using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.GestorDocumental.usuario;
using MiApp.Models.Models.Radicacion.PlantillaRadicado;
using MiApp.Models.Models.Radicacion.TipoTramite;
using MiApp.Repository.ErrorController;
using MiApp.Repository.Repositorio.Radicador.PlantillaRadicado;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using MiApp.Services.Service.Radicacion.Configuracion;
using MiApp.Services.Service.Radicacion.RelacionCamposRutaWorklflow;
using MiApp.Services.Service.Radicacion.PlantillaRadicado;
using MiApp.Services.Service.Radicacion.Tramite;
using MiApp.Services.Service.Usuario;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class RegistrarRadicacionEntranteServiceTests
{
    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoRequestValido_RetornaSuccess()
    {
        var detalleRepo = new Mock<IDetallePlantillaRadicadoR>();
        var remitRepo = new Mock<IRemitDestInternoR>();
        var plantillaRepo = new Mock<ISystemPlantillaRadicadoR>();
        var registrarRepo = new Mock<IRegistrarRadicacionEntranteRepository>();
        var parametrosService = new Mock<ISolicitaParametrosRadicadosService>();
        var tipoDocEntranteRepo = new Mock<ITipoDocEntranteR>();
        var validaCamposRadicacionService = new Mock<IValidaCamposRadicacionService>();
        var configuracionPlantillaService = new Mock<IConfiguracionPlantillaService>();
        var validaPreRegistroWorkflowService = new Mock<IValidaPreRegistroWorkflowService>();

        remitRepo
            .Setup(r => r.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno>
            {
                success = true,
                message = "OK",
                data = BuildRemitDestInterno(55)
            });

        plantillaRepo
            .Setup(r => r.SolicitaEstructuraPlantillaRadicacionDefault("DA"))
            .ReturnsAsync(new AppResponses<SystemPlantillaRadicado>
            {
                success = true,
                message = "YES",
                data = BuildPlantilla(100),
                errors = []
            });

        detalleRepo
            .Setup(r => r.SolicitaCamposDnamicos(100, "DA"))
            .ReturnsAsync(new AppResponse<DetallePlantillaRadicado[]>
            {
                Success = true,
                Message = "OK",
                Data = []
            });

        parametrosService
            .Setup(s => s.SolicitaParametrosRadicados(33, 302, 55, "DA"))
            .ReturnsAsync(new AppResponses<ParametrosRadicadosDto>
            {
                success = true,
                message = "YES",
                data = new ParametrosRadicadosDto
                {
                    NombreAreaRemitdest = new MiApp.DTOs.DTOs.GestorDocumental.usuario.NombreAreaRemitdestDto
                    {
                        IdArea = 1,
                        NombreArea = "AREA"
                    },
                    TipoDocEntrante = new TipoDocEntranteParametroDto
                    {
                        IdTipoDocEntrante = 302,
                        DescripcionDoc = "TRAMITE TEST",
                        SystemPlantillaRadicadoIdPlantilla = 100,
                        EstadoTipoDocumento = 1,
                        FlowTipo = 1,
                        RequiereRespuesta = 1,
                        CodigoGabineteWorkflow = 1,
                        TipoTramite = 1,
                        UtilRadicacionSimple = 1
                    },
                    IdSedeNombre = new MiApp.DTOs.DTOs.GestorDocumental.Sede.IdSedeNombreDto
                    {
                        IdSede = 1,
                        NombreSede = "SEDE"
                    }
                }
            });
        tipoDocEntranteRepo
            .Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante>
            {
                success = true,
                message = "YES",
                data = BuildTipoDocEntrante(302, 100)
            });

        registrarRepo
            .Setup(r => r.RegistrarRadicacionEntranteAsync(
                It.IsAny<RegistrarRadicacionEntranteRequestDto>(),
                55,
                10,
                "DA",
                It.IsAny<string>(),
                1,
                It.IsAny<string>(),
                It.IsAny<SystemPlantillaRadicado>(),
                It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>(),
                It.IsAny<ParametrosRadicadosDto>(),
                It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<RegistrarRadicacionEntranteResponseDto>
            {
                success = true,
                message = "OK",
                data = new RegistrarRadicacionEntranteResponseDto { ConsecutivoRadicado = "RAD-TEST-1" }
            });
        validaCamposRadicacionService
            .Setup(s => s.ValidaCamposRadicacionAsync(
                "DA",
                It.IsAny<RegistrarRadicacionEntranteRequestDto>(),
                It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>()))
            .ReturnsAsync(new AppResponses<List<ValidationError>?>
            {
                success = true,
                message = "Sin resultados",
                data = null
            });
        configuracionPlantillaService
            .Setup(s => s.SolicitaConfiguracionPlantillaAsync(100, 1, "DA"))
            .ReturnsAsync(new AppResponses<RaRadConfigPlantillaRadicacionDto?>
            {
                success = true,
                message = "OK",
                data = new RaRadConfigPlantillaRadicacionDto
                {
                    Descripcion_tipo_radicacion = "RADICACION"
                },
                errors = []
            });

        var service = new RegistrarRadicacionEntranteService(
            detalleRepo.Object,
            remitRepo.Object,
            plantillaRepo.Object,
            registrarRepo.Object,
            configuracionPlantillaService.Object,
            parametrosService.Object,
            tipoDocEntranteRepo.Object,
            validaCamposRadicacionService.Object,
            validaPreRegistroWorkflowService.Object);

        var result = await service.RegistrarRadicacionEntranteAsync(new RegistrarRadicacionEntranteRequestDto
        {
            IdPlantilla = 100,
            TipoRadicado = new TipoRadicadoEntradaDto { IdTipoRadicado = 1, TipoRadicacion = "ENTRANTE" },
            TipoPlantillaRadicado = new TipoPlantillaRadicadoDto { IdTipoPlantillaRdicado = 1 },
            Tipo_tramite = new TipoTramiteRadicacionDto { tipo_doc_entrante = 302, Descripcion = "TRAMITE" },
            Destinatario = new DestinatarioRadicacionDto { id_Remit_Dest_Int = 33 },
            Remitente = new RemitenteRadicacionDto { Nombre = "R" },
            ASUNTO = "A"
        }, 10, "DA", "127.0.0.1", 1);

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Equal("RAD-TEST-1", result.data!.ConsecutivoRadicado);
        parametrosService.Verify(s => s.SolicitaParametrosRadicados(33, 302, 55, "DA"), Times.Once);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoAliasVacio_RetornaValidacion()
    {
        var service = new RegistrarRadicacionEntranteService(
            Mock.Of<IDetallePlantillaRadicadoR>(),
            Mock.Of<IRemitDestInternoR>(),
            Mock.Of<ISystemPlantillaRadicadoR>(),
            Mock.Of<IRegistrarRadicacionEntranteRepository>(),
            Mock.Of<IConfiguracionPlantillaService>(),
            Mock.Of<ISolicitaParametrosRadicadosService>(),
            Mock.Of<ITipoDocEntranteR>(),
            Mock.Of<IValidaCamposRadicacionService>(),
            Mock.Of<IValidaPreRegistroWorkflowService>());

        var result = await service.RegistrarRadicacionEntranteAsync(
            new RegistrarRadicacionEntranteRequestDto(),
            10,
            string.Empty,
            "127.0.0.1",
            1);

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_PropagaAliasYUsuarioRadicadorAlRepositorio()
    {
        var detalleRepo = new Mock<IDetallePlantillaRadicadoR>();
        var remitRepo = new Mock<IRemitDestInternoR>();
        var plantillaRepo = new Mock<ISystemPlantillaRadicadoR>();
        var registrarRepo = new Mock<IRegistrarRadicacionEntranteRepository>();
        var parametrosService = new Mock<ISolicitaParametrosRadicadosService>();
        var tipoDocEntranteRepo = new Mock<ITipoDocEntranteR>();
        var validaCamposRadicacionService = new Mock<IValidaCamposRadicacionService>();
        var configuracionPlantillaService = new Mock<IConfiguracionPlantillaService>();
        var validaPreRegistroWorkflowService = new Mock<IValidaPreRegistroWorkflowService>();

        remitRepo
            .Setup(r => r.SolicitaEstructuraIdUsuarioGestion(25, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno>
            {
                success = true,
                message = "OK",
                data = BuildRemitDestInterno(77)
            });

        plantillaRepo
            .Setup(r => r.SolicitaEstructuraPlantillaRadicacionDefault("DA"))
            .ReturnsAsync(new AppResponses<SystemPlantillaRadicado>
            {
                success = true,
                message = "YES",
                data = BuildPlantilla(10),
                errors = []
            });

        detalleRepo
            .Setup(r => r.SolicitaCamposDnamicos(10, "DA"))
            .ReturnsAsync(new AppResponse<DetallePlantillaRadicado[]>
            {
                Success = true,
                Message = "OK",
                Data = []
            });

        parametrosService
            .Setup(s => s.SolicitaParametrosRadicados(45, 302, 77, "DA"))
            .ReturnsAsync(new AppResponses<ParametrosRadicadosDto>
            {
                success = true,
                message = "YES",
                data = new ParametrosRadicadosDto
                {
                    NombreAreaRemitdest = new MiApp.DTOs.DTOs.GestorDocumental.usuario.NombreAreaRemitdestDto(),
                    TipoDocEntrante = new TipoDocEntranteParametroDto
                    {
                        IdTipoDocEntrante = 302,
                        DescripcionDoc = "TRAMITE",
                        SystemPlantillaRadicadoIdPlantilla = 10,
                        EstadoTipoDocumento = 1,
                        FlowTipo = 1,
                        RequiereRespuesta = 1,
                        CodigoGabineteWorkflow = 1,
                        TipoTramite = 1,
                        UtilRadicacionSimple = 1
                    },
                    IdSedeNombre = new MiApp.DTOs.DTOs.GestorDocumental.Sede.IdSedeNombreDto()
                }
            });
        tipoDocEntranteRepo
            .Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante>
            {
                success = true,
                message = "YES",
                data = BuildTipoDocEntrante(302, 10)
            });

        registrarRepo
            .Setup(r => r.RegistrarRadicacionEntranteAsync(
                It.IsAny<RegistrarRadicacionEntranteRequestDto>(),
                77,
                25,
                "DA",
                It.IsAny<string>(),
                2,
                It.IsAny<string>(),
                It.IsAny<SystemPlantillaRadicado>(),
                It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>(),
                It.IsAny<ParametrosRadicadosDto>(),
                It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<RegistrarRadicacionEntranteResponseDto>
            {
                success = true,
                message = "OK",
                data = new RegistrarRadicacionEntranteResponseDto()
            });
        validaCamposRadicacionService
            .Setup(s => s.ValidaCamposRadicacionAsync(
                "DA",
                It.IsAny<RegistrarRadicacionEntranteRequestDto>(),
                It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>()))
            .ReturnsAsync(new AppResponses<List<ValidationError>?>
            {
                success = true,
                message = "Sin resultados",
                data = null
            });
        configuracionPlantillaService
            .Setup(s => s.SolicitaConfiguracionPlantillaAsync(10, 1, "DA"))
            .ReturnsAsync(new AppResponses<RaRadConfigPlantillaRadicacionDto?>
            {
                success = true,
                message = "OK",
                data = new RaRadConfigPlantillaRadicacionDto
                {
                    Descripcion_tipo_radicacion = "CORRESPONDENCIA"
                },
                errors = []
            });
        validaPreRegistroWorkflowService
            .Setup(s => s.ValidaPreRegistroWorkflowAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), "DA", It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<ValidaPreRegistroWorkflowResultDto>
            {
                success = true,
                message = "OK",
                data = new ValidaPreRegistroWorkflowResultDto(),
                errors = []
            });

        var service = new RegistrarRadicacionEntranteService(
            detalleRepo.Object,
            remitRepo.Object,
            plantillaRepo.Object,
            registrarRepo.Object,
            configuracionPlantillaService.Object,
            parametrosService.Object,
            tipoDocEntranteRepo.Object,
            validaCamposRadicacionService.Object,
            validaPreRegistroWorkflowService.Object);
        var request = new RegistrarRadicacionEntranteRequestDto
        {
            IdPlantilla = 10,
            TipoRadicado = new TipoRadicadoEntradaDto { IdTipoRadicado = 1, TipoRadicacion = "ENTRANTE" },
            TipoPlantillaRadicado = new TipoPlantillaRadicadoDto { IdTipoPlantillaRdicado = 1 },
            Tipo_tramite = new TipoTramiteRadicacionDto { tipo_doc_entrante = 302, Descripcion = "TRAMITE" },
            Destinatario = new DestinatarioRadicacionDto { id_Remit_Dest_Int = 45 },
            Remitente = new RemitenteRadicacionDto { Nombre = "R" },
            ASUNTO = "A"
        };

        var result = await service.RegistrarRadicacionEntranteAsync(request, 25, "DA", "127.0.0.1", 2);

        Assert.True(result.success);
        registrarRepo.Verify(r => r.RegistrarRadicacionEntranteAsync(
            It.Is<RegistrarRadicacionEntranteRequestDto>(x => x.IdPlantilla == 10 && x.tipoModuloRadicacion == 2),
            77,
            25,
            "DA",
            It.IsAny<string>(),
            2,
            "CORRESPONDENCIA",
            It.IsAny<SystemPlantillaRadicado>(),
            It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>(),
            It.IsAny<ParametrosRadicadosDto>(),
            It.IsAny<TipoDocEntrante>()),
            Times.Once);
        parametrosService.Verify(s => s.SolicitaParametrosRadicados(45, 302, 77, "DA"), Times.Once);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoFallaSolicitaParametros_RetornaErrorYNoRegistra()
    {
        var detalleRepo = new Mock<IDetallePlantillaRadicadoR>();
        var remitRepo = new Mock<IRemitDestInternoR>();
        var plantillaRepo = new Mock<ISystemPlantillaRadicadoR>();
        var registrarRepo = new Mock<IRegistrarRadicacionEntranteRepository>();
        var parametrosService = new Mock<ISolicitaParametrosRadicadosService>();
        var tipoDocEntranteRepo = new Mock<ITipoDocEntranteR>();
        var validaCamposRadicacionService = new Mock<IValidaCamposRadicacionService>();
        var configuracionPlantillaService = new Mock<IConfiguracionPlantillaService>();
        var validaPreRegistroWorkflowService = new Mock<IValidaPreRegistroWorkflowService>();

        remitRepo
            .Setup(r => r.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno>
            {
                success = true,
                message = "OK",
                data = BuildRemitDestInterno(55)
            });

        parametrosService
            .Setup(s => s.SolicitaParametrosRadicados(33, 302, 55, "DA"))
            .ReturnsAsync(new AppResponses<ParametrosRadicadosDto>
            {
                success = false,
                message = "No fue posible obtener parámetros backend",
                data = null!,
                errors = []
            });
        tipoDocEntranteRepo
            .Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante>
            {
                success = true,
                message = "YES",
                data = BuildTipoDocEntrante(302, 100)
            });

        var service = new RegistrarRadicacionEntranteService(
            detalleRepo.Object,
            remitRepo.Object,
            plantillaRepo.Object,
            registrarRepo.Object,
            configuracionPlantillaService.Object,
            parametrosService.Object,
            tipoDocEntranteRepo.Object,
            validaCamposRadicacionService.Object,
            validaPreRegistroWorkflowService.Object);

        var result = await service.RegistrarRadicacionEntranteAsync(new RegistrarRadicacionEntranteRequestDto
        {
            IdPlantilla = 100,
            TipoRadicado = new TipoRadicadoEntradaDto { IdTipoRadicado = 1, TipoRadicacion = "ENTRANTE" },
            Tipo_tramite = new TipoTramiteRadicacionDto { tipo_doc_entrante = 302, Descripcion = "TRAMITE" },
            Destinatario = new DestinatarioRadicacionDto { id_Remit_Dest_Int = 33 },
            Remitente = new RemitenteRadicacionDto { Nombre = "R" },
            ASUNTO = "A"
        }, 10, "DA", "127.0.0.1", 1);

        Assert.False(result.success);
        Assert.Equal("No fue posible obtener parámetros backend", result.message);
        registrarRepo.Verify(r => r.RegistrarRadicacionEntranteAsync(
            It.IsAny<RegistrarRadicacionEntranteRequestDto>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<SystemPlantillaRadicado>(),
            It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>(),
            It.IsAny<ParametrosRadicadosDto>(),
            It.IsAny<TipoDocEntrante>()),
            Times.Never);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoNoExisteConfiguracion_RetornaValidacionYNoRegistra()
    {
        var detalleRepo = new Mock<IDetallePlantillaRadicadoR>();
        var remitRepo = new Mock<IRemitDestInternoR>();
        var plantillaRepo = new Mock<ISystemPlantillaRadicadoR>();
        var registrarRepo = new Mock<IRegistrarRadicacionEntranteRepository>();
        var parametrosService = new Mock<ISolicitaParametrosRadicadosService>();
        var tipoDocEntranteRepo = new Mock<ITipoDocEntranteR>();
        var validaCamposRadicacionService = new Mock<IValidaCamposRadicacionService>();
        var configuracionPlantillaService = new Mock<IConfiguracionPlantillaService>();
        var validaPreRegistroWorkflowService = new Mock<IValidaPreRegistroWorkflowService>();

        remitRepo
            .Setup(r => r.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno>
            {
                success = true,
                message = "OK",
                data = BuildRemitDestInterno(55)
            });
        plantillaRepo
            .Setup(r => r.SolicitaEstructuraPlantillaRadicacionDefault("DA"))
            .ReturnsAsync(new AppResponses<SystemPlantillaRadicado> { success = true, data = BuildPlantilla(100), errors = [] });
        detalleRepo
            .Setup(r => r.SolicitaCamposDnamicos(100, "DA"))
            .ReturnsAsync(new AppResponse<DetallePlantillaRadicado[]> { Success = true, Data = [] });
        tipoDocEntranteRepo
            .Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante> { success = true, data = BuildTipoDocEntrante(302, 100) });
        parametrosService
            .Setup(s => s.SolicitaParametrosRadicados(33, 302, 55, "DA"))
            .ReturnsAsync(new AppResponses<ParametrosRadicadosDto> { success = true, data = BuildParametros(302, 100) });
        validaCamposRadicacionService
            .Setup(s => s.ValidaCamposRadicacionAsync("DA", It.IsAny<RegistrarRadicacionEntranteRequestDto>(), It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>()))
            .ReturnsAsync(new AppResponses<List<ValidationError>?> { success = true, data = null });
        configuracionPlantillaService
            .Setup(s => s.SolicitaConfiguracionPlantillaAsync(100, 1, "DA"))
            .ReturnsAsync(new AppResponses<RaRadConfigPlantillaRadicacionDto?> { success = true, message = "Sin resultados", data = null, errors = [] });

        var service = new RegistrarRadicacionEntranteService(
            detalleRepo.Object,
            remitRepo.Object,
            plantillaRepo.Object,
            registrarRepo.Object,
            configuracionPlantillaService.Object,
            parametrosService.Object,
            tipoDocEntranteRepo.Object,
            validaCamposRadicacionService.Object,
            validaPreRegistroWorkflowService.Object);

        var result = await service.RegistrarRadicacionEntranteAsync(new RegistrarRadicacionEntranteRequestDto
        {
            IdPlantilla = 100,
            TipoRadicado = new TipoRadicadoEntradaDto { IdTipoRadicado = 1, TipoRadicacion = "ENTRANTE" },
            TipoPlantillaRadicado = new TipoPlantillaRadicadoDto { IdTipoPlantillaRdicado = 1 },
            Tipo_tramite = new TipoTramiteRadicacionDto { tipo_doc_entrante = 302, Descripcion = "TRAMITE" },
            Destinatario = new DestinatarioRadicacionDto { id_Remit_Dest_Int = 33 },
            Remitente = new RemitenteRadicacionDto { Nombre = "R" },
            ASUNTO = "A"
        }, 10, "DA", "127.0.0.1", 1);

        Assert.False(result.success);
        Assert.Contains("No existe configuración", result.message);
        registrarRepo.Verify(r => r.RegistrarRadicacionEntranteAsync(
            It.IsAny<RegistrarRadicacionEntranteRequestDto>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<SystemPlantillaRadicado>(),
            It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>(),
            It.IsAny<ParametrosRadicadosDto>(),
            It.IsAny<TipoDocEntrante>()),
            Times.Never);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoFallaConfiguracion_RetornaErrorYNoRegistra()
    {
        var detalleRepo = new Mock<IDetallePlantillaRadicadoR>();
        var remitRepo = new Mock<IRemitDestInternoR>();
        var plantillaRepo = new Mock<ISystemPlantillaRadicadoR>();
        var registrarRepo = new Mock<IRegistrarRadicacionEntranteRepository>();
        var parametrosService = new Mock<ISolicitaParametrosRadicadosService>();
        var tipoDocEntranteRepo = new Mock<ITipoDocEntranteR>();
        var validaCamposRadicacionService = new Mock<IValidaCamposRadicacionService>();
        var configuracionPlantillaService = new Mock<IConfiguracionPlantillaService>();
        var validaPreRegistroWorkflowService = new Mock<IValidaPreRegistroWorkflowService>();

        remitRepo
            .Setup(r => r.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55) });
        plantillaRepo
            .Setup(r => r.SolicitaEstructuraPlantillaRadicacionDefault("DA"))
            .ReturnsAsync(new AppResponses<SystemPlantillaRadicado> { success = true, data = BuildPlantilla(100), errors = [] });
        detalleRepo
            .Setup(r => r.SolicitaCamposDnamicos(100, "DA"))
            .ReturnsAsync(new AppResponse<DetallePlantillaRadicado[]> { Success = true, Data = [] });
        tipoDocEntranteRepo
            .Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante> { success = true, data = BuildTipoDocEntrante(302, 100) });
        parametrosService
            .Setup(s => s.SolicitaParametrosRadicados(33, 302, 55, "DA"))
            .ReturnsAsync(new AppResponses<ParametrosRadicadosDto> { success = true, data = BuildParametros(302, 100) });
        validaCamposRadicacionService
            .Setup(s => s.ValidaCamposRadicacionAsync("DA", It.IsAny<RegistrarRadicacionEntranteRequestDto>(), It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>()))
            .ReturnsAsync(new AppResponses<List<ValidationError>?> { success = true, data = null });
        configuracionPlantillaService
            .Setup(s => s.SolicitaConfiguracionPlantillaAsync(100, 1, "DA"))
            .ReturnsAsync(new AppResponses<RaRadConfigPlantillaRadicacionDto?> { success = false, message = "Error de configuración", data = null, errors = [] });

        var service = new RegistrarRadicacionEntranteService(
            detalleRepo.Object,
            remitRepo.Object,
            plantillaRepo.Object,
            registrarRepo.Object,
            configuracionPlantillaService.Object,
            parametrosService.Object,
            tipoDocEntranteRepo.Object,
            validaCamposRadicacionService.Object,
            validaPreRegistroWorkflowService.Object);

        var result = await service.RegistrarRadicacionEntranteAsync(new RegistrarRadicacionEntranteRequestDto
        {
            IdPlantilla = 100,
            TipoRadicado = new TipoRadicadoEntradaDto { IdTipoRadicado = 1, TipoRadicacion = "ENTRANTE" },
            TipoPlantillaRadicado = new TipoPlantillaRadicadoDto { IdTipoPlantillaRdicado = 1 },
            Tipo_tramite = new TipoTramiteRadicacionDto { tipo_doc_entrante = 302, Descripcion = "TRAMITE" },
            Destinatario = new DestinatarioRadicacionDto { id_Remit_Dest_Int = 33 },
            Remitente = new RemitenteRadicacionDto { Nombre = "R" },
            ASUNTO = "A"
        }, 10, "DA", "127.0.0.1", 1);

        Assert.False(result.success);
        Assert.Equal("Error de configuración", result.message);
        registrarRepo.Verify(r => r.RegistrarRadicacionEntranteAsync(
            It.IsAny<RegistrarRadicacionEntranteRequestDto>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<SystemPlantillaRadicado>(),
            It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>(),
            It.IsAny<ParametrosRadicadosDto>(),
            It.IsAny<TipoDocEntrante>()),
            Times.Never);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoTipoModuloEnvioNoEsWorkflow_NoConsultaServiciosWorkflow()
    {
        var detalleRepo = new Mock<IDetallePlantillaRadicadoR>();
        var remitRepo = new Mock<IRemitDestInternoR>();
        var plantillaRepo = new Mock<ISystemPlantillaRadicadoR>();
        var registrarRepo = new Mock<IRegistrarRadicacionEntranteRepository>();
        var parametrosService = new Mock<ISolicitaParametrosRadicadosService>();
        var tipoDocEntranteRepo = new Mock<ITipoDocEntranteR>();
        var validaCamposRadicacionService = new Mock<IValidaCamposRadicacionService>();
        var configuracionPlantillaService = new Mock<IConfiguracionPlantillaService>();
        var validaPreRegistroWorkflowService = new Mock<IValidaPreRegistroWorkflowService>(MockBehavior.Strict);

        remitRepo
            .Setup(r => r.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55) });
        plantillaRepo
            .Setup(r => r.SolicitaEstructuraPlantillaRadicacionDefault("DA"))
            .ReturnsAsync(new AppResponses<SystemPlantillaRadicado> { success = true, data = BuildPlantilla(100), errors = [] });
        detalleRepo
            .Setup(r => r.SolicitaCamposDnamicos(100, "DA"))
            .ReturnsAsync(new AppResponse<DetallePlantillaRadicado[]> { Success = true, Data = [] });
        tipoDocEntranteRepo
            .Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante> { success = true, data = BuildTipoDocEntrante(302, 100, 1) });
        parametrosService
            .Setup(s => s.SolicitaParametrosRadicados(33, 302, 55, "DA"))
            .ReturnsAsync(new AppResponses<ParametrosRadicadosDto> { success = true, data = BuildParametros(302, 100) });
        validaCamposRadicacionService
            .Setup(s => s.ValidaCamposRadicacionAsync("DA", It.IsAny<RegistrarRadicacionEntranteRequestDto>(), It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>()))
            .ReturnsAsync(new AppResponses<List<ValidationError>?> { success = true, data = null });
        configuracionPlantillaService
            .Setup(s => s.SolicitaConfiguracionPlantillaAsync(100, 1, "DA"))
            .ReturnsAsync(new AppResponses<RaRadConfigPlantillaRadicacionDto?> { success = true, data = new RaRadConfigPlantillaRadicacionDto { Descripcion_tipo_radicacion = "RADICACION" } });
        registrarRepo
            .Setup(r => r.RegistrarRadicacionEntranteAsync(
                It.IsAny<RegistrarRadicacionEntranteRequestDto>(),
                55,
                10,
                "DA",
                It.IsAny<string>(),
                1,
                "RADICACION",
                It.IsAny<SystemPlantillaRadicado>(),
                It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>(),
                It.IsAny<ParametrosRadicadosDto>(),
                It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<RegistrarRadicacionEntranteResponseDto> { success = true, data = new RegistrarRadicacionEntranteResponseDto() });

        var service = new RegistrarRadicacionEntranteService(
            detalleRepo.Object,
            remitRepo.Object,
            plantillaRepo.Object,
            registrarRepo.Object,
            configuracionPlantillaService.Object,
            parametrosService.Object,
            tipoDocEntranteRepo.Object,
            validaCamposRadicacionService.Object,
            validaPreRegistroWorkflowService.Object);

        var result = await service.RegistrarRadicacionEntranteAsync(BuildRequest(100, 302, 33), 10, "DA", "127.0.0.1", 1);

        Assert.True(result.success);
        validaPreRegistroWorkflowService.Verify(s => s.ValidaPreRegistroWorkflowAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), It.IsAny<string>(), It.IsAny<TipoDocEntrante>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoWorkflowYNoExisteClaimDefaulaliaswf_RetornaValidacionYNoRegistra()
    {
        var detalleRepo = new Mock<IDetallePlantillaRadicadoR>();
        var remitRepo = new Mock<IRemitDestInternoR>();
        var plantillaRepo = new Mock<ISystemPlantillaRadicadoR>();
        var registrarRepo = new Mock<IRegistrarRadicacionEntranteRepository>();
        var parametrosService = new Mock<ISolicitaParametrosRadicadosService>();
        var tipoDocEntranteRepo = new Mock<ITipoDocEntranteR>();
        var validaCamposRadicacionService = new Mock<IValidaCamposRadicacionService>();
        var configuracionPlantillaService = new Mock<IConfiguracionPlantillaService>();
        var validaPreRegistroWorkflowService = new Mock<IValidaPreRegistroWorkflowService>();

        remitRepo
            .Setup(r => r.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ReturnsAsync(new AppResponses<RemitDestInterno> { success = true, data = BuildRemitDestInterno(55) });
        plantillaRepo
            .Setup(r => r.SolicitaEstructuraPlantillaRadicacionDefault("DA"))
            .ReturnsAsync(new AppResponses<SystemPlantillaRadicado> { success = true, data = BuildPlantilla(100), errors = [] });
        detalleRepo
            .Setup(r => r.SolicitaCamposDnamicos(100, "DA"))
            .ReturnsAsync(new AppResponse<DetallePlantillaRadicado[]> { Success = true, Data = [] });
        tipoDocEntranteRepo
            .Setup(r => r.SolicitaEstructuraTipoDoEntrante(302, "DA"))
            .ReturnsAsync(new AppResponses<TipoDocEntrante> { success = true, data = BuildTipoDocEntrante(302, 100, 2) });
        parametrosService
            .Setup(s => s.SolicitaParametrosRadicados(33, 302, 55, "DA"))
            .ReturnsAsync(new AppResponses<ParametrosRadicadosDto> { success = true, data = BuildParametros(302, 100) });
        validaCamposRadicacionService
            .Setup(s => s.ValidaCamposRadicacionAsync("DA", It.IsAny<RegistrarRadicacionEntranteRequestDto>(), It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>()))
            .ReturnsAsync(new AppResponses<List<ValidationError>?> { success = true, data = null });
        configuracionPlantillaService
            .Setup(s => s.SolicitaConfiguracionPlantillaAsync(100, 1, "DA"))
            .ReturnsAsync(new AppResponses<RaRadConfigPlantillaRadicacionDto?> { success = true, data = new RaRadConfigPlantillaRadicacionDto { Descripcion_tipo_radicacion = "RADICACION" } });
        validaPreRegistroWorkflowService
            .Setup(s => s.ValidaPreRegistroWorkflowAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), "DA", It.IsAny<TipoDocEntrante>()))
            .ReturnsAsync(new AppResponses<ValidaPreRegistroWorkflowResultDto>
            {
                success = false,
                message = "Claim defaulaliaswf requerido para continuar con workflow",
                data = null!,
                errors = []
            });

        var service = new RegistrarRadicacionEntranteService(
            detalleRepo.Object,
            remitRepo.Object,
            plantillaRepo.Object,
            registrarRepo.Object,
            configuracionPlantillaService.Object,
            parametrosService.Object,
            tipoDocEntranteRepo.Object,
            validaCamposRadicacionService.Object,
            validaPreRegistroWorkflowService.Object);

        var result = await service.RegistrarRadicacionEntranteAsync(BuildRequest(100, 302, 33), 10, "DA", "127.0.0.1", 2);

        Assert.False(result.success);
        Assert.Equal("Claim defaulaliaswf requerido para continuar con workflow", result.message);
        registrarRepo.Verify(r => r.RegistrarRadicacionEntranteAsync(
            It.IsAny<RegistrarRadicacionEntranteRequestDto>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<SystemPlantillaRadicado>(),
            It.IsAny<IReadOnlyCollection<DetallePlantillaRadicado>>(),
            It.IsAny<ParametrosRadicadosDto>(),
            It.IsAny<TipoDocEntrante>()), Times.Never);
        validaPreRegistroWorkflowService.Verify(s => s.ValidaPreRegistroWorkflowAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), "DA", It.IsAny<TipoDocEntrante>()), Times.Once);
    }

    private static SystemPlantillaRadicado BuildPlantilla(int idPlantilla)
    {
        return new SystemPlantillaRadicado
        {
            id_Plantilla = idPlantilla,
            Nombre_Plantilla_Radicado = "ra_plantilla_100",
            Tipo_Plantilla = "RAD",
            Fecha_Creacion = DateTime.UtcNow,
            Estado_Plantilla = 1,
            Consecutivo_Rad = 10,
            Consecutivo_CodBarra = 10,
            util_activo_plantilla_flujo = 1,
            Util_activo_plantilla_default_rad_interno = 0,
            util_evalua_periodo_general = 0,
            utill_evalua_periodo_pqr = 0,
            util_evalua_periodo_interno = 0,
            util_evalua_festivo = 0,
            util_tipo_modulo_envio = 0,
            util_consecutivo_general = 0,
            util_default_simple = 0,
            util_estado_pendiente_rad = 0,
            utilIncuyeConsecutivoArea = 0,
            utilConseTipoRad = 0,
            util_default_radicado = 1
        };
    }

    private static ParametrosRadicadosDto BuildParametros(int idTipoDocEntrante, int idPlantilla)
    {
        return new ParametrosRadicadosDto
        {
            NombreAreaRemitdest = new MiApp.DTOs.DTOs.GestorDocumental.usuario.NombreAreaRemitdestDto(),
            TipoDocEntrante = new TipoDocEntranteParametroDto
            {
                IdTipoDocEntrante = idTipoDocEntrante,
                DescripcionDoc = "TRAMITE",
                SystemPlantillaRadicadoIdPlantilla = idPlantilla,
                EstadoTipoDocumento = 1,
                FlowTipo = 1,
                RequiereRespuesta = 1,
                CodigoGabineteWorkflow = 1,
                TipoTramite = 1,
                UtilRadicacionSimple = 1
            },
            IdSedeNombre = new MiApp.DTOs.DTOs.GestorDocumental.Sede.IdSedeNombreDto()
        };
    }

    private static RemitDestInterno BuildRemitDestInterno(int idUsuarioRadicacion)
    {
        return new RemitDestInterno
        {
            Id_Remit_Dest_Int = 1,
            Relacion_Id_Usuario_Radicacion = idUsuarioRadicacion,
            Nombre_Remitente = string.Empty,
            Cargo_Remite = string.Empty,
            Login_Usuario = string.Empty,
            Pasw_Usuario = string.Empty,
            Correo_Electronico = string.Empty,
            Telefono_Usuario = string.Empty,
            Firma_Usuario = [],
            Pasw_Encript = string.Empty,
            Relacion_Workflow_Login = string.Empty,
            Relacion_Da_Login = string.Empty,
            Relacion_Login_Radicacion = string.Empty,
            Identificacion = string.Empty,
            Direccion = string.Empty,
            Relacion_Workflow_Login_Extend = string.Empty
        };
    }

    private static RegistrarRadicacionEntranteRequestDto BuildRequest(int idPlantilla, int tipoDocEntrante, int idDestinatario)
    {
        return new RegistrarRadicacionEntranteRequestDto
        {
            IdPlantilla = idPlantilla,
            TipoRadicado = new TipoRadicadoEntradaDto { IdTipoRadicado = 1, TipoRadicacion = "ENTRANTE" },
            TipoPlantillaRadicado = new TipoPlantillaRadicadoDto { IdTipoPlantillaRdicado = 1 },
            Tipo_tramite = new TipoTramiteRadicacionDto { tipo_doc_entrante = tipoDocEntrante, Descripcion = "TRAMITE" },
            Destinatario = new DestinatarioRadicacionDto { id_Remit_Dest_Int = idDestinatario },
            Remitente = new RemitenteRadicacionDto { Nombre = "R" },
            ASUNTO = "A"
        };
    }

    private static TipoDocEntrante BuildTipoDocEntrante(int idTipoDocEntrante, int idPlantilla, int utilTipoModuloEnvio = 1)
    {
        return new TipoDocEntrante
        {
            id_Tipo_Doc_Entrante = idTipoDocEntrante,
            Descripcion_Doc = "TRAMITE",
            system_plantilla_radicado_id_plantilla = idPlantilla,
            estado_tipo_documento = 1,
            flow_tipo = 1,
            requiere_respuesta = 1,
            codigo_gabinete_workflow = 1,
            resp_correo_fisico_electronico = 1,
            id_ruta = 1,
            tipo_tramite = 1,
            estado_ruta_open_close = 1,
            obliga_exp_radicado = 0,
            activo_modulo_respuesta = 1,
            util_tipo_modulo_envio = utilTipoModuloEnvio,
            util_producion_documental = 0,
            tipo_tramite_entrante_saliente = 1,
            wf_copia_doc_expediente_actualiza_exped_gabinete = 0,
            wf_auto_vincula_doc_expediente_actualiza_exped_gabinete = 0,
            wf_copia_doc_expediente_produc_actualiza_exped_gabinete = 0,
            util_auto_vincula_migracion = 0,
            id_gabinete = 1,
            util_radicacion_simple = 1,
            util_nivel_padre_auto_vincula = 0,
            util_opcion_auto_vincula = 0,
            util_Estado_Crea_ExpedienteSII = 0,
            util_Estado_Multiple_expedienteSII = 0
        };
    }
}
