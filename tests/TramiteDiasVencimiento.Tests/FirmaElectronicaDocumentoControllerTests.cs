using DocuArchi.Api.Controllers.GestorDocumental.Documentos;
using Microsoft.AspNetCore.Mvc;
using MiApp.DTOs.DTOs.GestorDocumental.Documentos.FirmaElectronica;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.GestorDocumental.Documentos.FirmaElectronica;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class FirmaElectronicaDocumentoControllerTests
{
    [Fact]
    public async Task GetFirmaElectronicaDocumento_SinClaimAlias_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<FirmaElectronicaDocumentoResponseDto>("defaulalias"))
            .Returns(new ClaimValidationResult<FirmaElectronicaDocumentoResponseDto>
            {
                Success = false,
                Response = new AppResponses<FirmaElectronicaDocumentoResponseDto>
                {
                    success = false,
                    message = "No claim",
                    data = new FirmaElectronicaDocumentoResponseDto()
                }
            });
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = false,
                Response = new AppResponses<string>
                {
                    success = false,
                    message = "No claim",
                    data = string.Empty
                }
            });

        var controller = new FirmaElectronicaDocumentoController(
            claimValidation.Object,
            Mock.Of<IFirmaElectronicaDocumentoService>());

        var result = await controller.GetFirmaElectronicaDocumento(123, "WF_DOCS");

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetFirmaElectronicaDocumento_CuandoOk_RetornaOk()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(c => c.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "DA" });

        var service = new Mock<IFirmaElectronicaDocumentoService>();
        service.Setup(s => s.GetFirmaElectronicaDocumentoAsync(123, "WF_DOCS", "DA"))
            .ReturnsAsync(new AppResponses<FirmaElectronicaDocumentoResponseDto>
            {
                success = true,
                message = "OK",
                data = new FirmaElectronicaDocumentoResponseDto
                {
                    IdArchivo = 123,
                    NombreGabinete = "WF_DOCS",
                    FirmadoElectronico = true,
                    IdCertificado = 9876
                }
            });

        var controller = new FirmaElectronicaDocumentoController(claimValidation.Object, service.Object);
        var result = await controller.GetFirmaElectronicaDocumento(123, "WF_DOCS");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AppResponses<FirmaElectronicaDocumentoResponseDto>>(ok.Value);
        Assert.True(payload.success);
        Assert.True(payload.data.FirmadoElectronico);
        Assert.Equal(9876, payload.data.IdCertificado);
    }
}
