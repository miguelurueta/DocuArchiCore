using DocuArchi.Api.Controllers.GestorDocumental.Documentos;
using MiApp.DTOs.DTOs.Errors;
using MiApp.DTOs.DTOs.GestorDocumental.Documentos.ListaDocumentosRadicados;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.GestorDocumental.Documentos.ListaDocumentosRadicados;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ListaDocumentosRadicadoControllerTests
{
    [Fact]
    public async Task Query_CuandoClaimDefaulaliasEsInvalido_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation
            .Setup(service => service.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = false,
                ClaimValue = null,
                Response = Validation("defaulalias")
            });

        var controller = new ListaDocumentosRadicadoController(
            claimValidation.Object,
            Mock.Of<IListaDocumentosRadicadoService>());

        var result = await controller.Query(new ListaDocumentosRadicadosTreeQueryRequestDto());

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Query_CuandoClaimsSonValidos_DelegaAlServicio()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        var service = new Mock<IListaDocumentosRadicadoService>();

        claimValidation
            .Setup(svc => svc.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "DA", Response = null });
        claimValidation
            .Setup(svc => svc.ValidateClaim<string>("usuarioid"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "10", Response = null });

        service
            .Setup(svc => svc.SolicitaListaDocumentosRadicadosTreeAsync(It.IsAny<ListaDocumentosRadicadosTreeQueryRequestDto>(), 10, "DA"))
            .ReturnsAsync(new AppResponses<object>
            {
                success = true,
                message = "OK",
                data = new { TableId = "InboxListaRadicados" },
                errors = []
            });

        var controller = new ListaDocumentosRadicadoController(claimValidation.Object, service.Object);

        var result = await controller.Query(new ListaDocumentosRadicadosTreeQueryRequestDto());

        Assert.IsType<OkObjectResult>(result.Result);
        service.Verify(
            svc => svc.SolicitaListaDocumentosRadicadosTreeAsync(It.IsAny<ListaDocumentosRadicadosTreeQueryRequestDto>(), 10, "DA"),
            Times.Once);
    }

    [Fact]
    public async Task Action_CuandoUsuarioIdEsInvalido_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation
            .Setup(service => service.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "DA", Response = null });
        claimValidation
            .Setup(service => service.ValidateClaim<string>("usuarioid"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "abc", Response = null });

        var controller = new ListaDocumentosRadicadoController(
            claimValidation.Object,
            Mock.Of<IListaDocumentosRadicadoService>());

        var result = await controller.Action(new ListaDocumentosRadicadosTreeActionRequestDto { ActionId = "ver_documento" });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var payload = Assert.IsType<AppResponses<ListaDocumentosRadicadosTreeMutationResultDto?>>(badRequest.Value);
        Assert.False(payload.success);
        Assert.Equal("Error en action de ListaDocumentosRadicados", payload.message);
    }

    private static AppResponses<string> Validation(string field)
    {
        return new AppResponses<string>
        {
            success = false,
            message = "Validation",
            data = string.Empty,
            meta = new AppMeta { Status = "validation" },
            errors = [new AppError { Field = field, Type = "Validation", Message = "Claim invalido" }]
        };
    }
}
