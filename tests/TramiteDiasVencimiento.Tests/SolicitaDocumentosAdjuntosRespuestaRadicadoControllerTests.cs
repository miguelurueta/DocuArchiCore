using DocuArchi.Api.Controllers.GestionCorrespondencia.GestionRespuesta;
using MiApp.DTOs.DTOs.Common;
using MiApp.DTOs.DTOs.GestionCorrespondencia.GestionRespuesta;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.GestionCorrespondencia.GestionRespuesta;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class SolicitaDocumentosAdjuntosRespuestaRadicadoControllerTests
{
    [Fact]
    public async Task Get_CuandoClaimAliasEsInvalido_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = false, ClaimValue = null, Response = new AppResponses<string>() });

        var service = new Mock<IServiceSolicitaDocumentosAdjuntosRespuestaRadicado>();
        var logger = new Mock<ILogger<SolicitaDocumentosAdjuntosRespuestaRadicadoController>>();
        var controller = new SolicitaDocumentosAdjuntosRespuestaRadicadoController(claimValidation.Object, service.Object, logger.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };

        var result = await controller.Get(10);

        Assert.IsType<BadRequestObjectResult>(result.Result);
        service.Verify(s => s.SolicitaDocumentosAdjuntosRespuestaRadicadoAsync(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Get_CuandoParametroEsInvalido_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "WF", Response = null });

        var service = new Mock<IServiceSolicitaDocumentosAdjuntosRespuestaRadicado>();
        var logger = new Mock<ILogger<SolicitaDocumentosAdjuntosRespuestaRadicadoController>>();
        var controller = new SolicitaDocumentosAdjuntosRespuestaRadicadoController(claimValidation.Object, service.Object, logger.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };

        var result = await controller.Get(0);

        Assert.IsType<BadRequestObjectResult>(result.Result);
        service.Verify(s => s.SolicitaDocumentosAdjuntosRespuestaRadicadoAsync(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Get_CuandoServiceRetornaSuccess_RetornaOk()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "WF", Response = null });

        var service = new Mock<IServiceSolicitaDocumentosAdjuntosRespuestaRadicado>();
        service.Setup(s => s.SolicitaDocumentosAdjuntosRespuestaRadicadoAsync(10, "WF"))
            .ReturnsAsync(new AppResponses<List<DocumentoAdjuntoRespuestaRadicadoDto>>
            {
                success = true,
                message = "YES",
                data =
                [
                    new DocumentoAdjuntoRespuestaRadicadoDto
                    {
                        IdRespuestaRadicado = 1,
                        IdImagen = 200,
                        TipoAdjunto = "DocumentoPrincipal"
                    }
                ],
                meta = new AppMeta { Status = "success" },
                errors = []
            });

        var logger = new Mock<ILogger<SolicitaDocumentosAdjuntosRespuestaRadicadoController>>();
        var controller = new SolicitaDocumentosAdjuntosRespuestaRadicadoController(claimValidation.Object, service.Object, logger.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };

        var result = await controller.Get(10);

        Assert.IsType<OkObjectResult>(result.Result);
    }
}
