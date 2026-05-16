using System.Security.Claims;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.TemporaryUpload;
using DocuArchi.Api.Controllers.GestorDocumental.Documentos;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.TemporaryUpload;
using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental.TemporaryUpload;
using MiApp.DTOs.DTOs.GestorDocumental.Documentos.ReemplazoPdf;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.GestorDocumental.Documentos.ReemplazoPdf;
using MiApp.Services.Service.SessionHelper;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ReemplazoPdfControllerTests
{
    [Fact]
    public async Task Post_CuandoFaltaAlias_RetornaBadRequest()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(x => x.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string>
            {
                Success = false,
                Response = new AppResponses<string> { success = false, message = "missing alias" }
            });

        var controller = BuildController(claimValidation.Object);
        var result = await controller.Post(BuildRequest());

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Post_CuandoServicioOk_RetornaOk()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(x => x.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "tenantA" });
        claimValidation.Setup(x => x.ValidateClaim<string>("usuarioid"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "99" });

        var service = new Mock<IReemplazoPdfService>();
        service.Setup(x => x.ExecuteAsync(
                It.IsAny<ReemplazarDocumentoPdfRequest>(),
                "tenantA",
                "qa.user",
                99,
                It.IsAny<string?>()))
            .ReturnsAsync(new AppResponses<ReemplazarDocumentoPdfResponse?>
            {
                success = true,
                message = "OK",
                data = new ReemplazarDocumentoPdfResponse
                {
                    IdDocumento = 10,
                    NombreGabinete = "contabil",
                    RutaArchivoFinal = "D:/imagenes/discos/CONTABIL7/00093/DIG00000010.PDF",
                    RutaRespaldo = "D:/temp/replacement-versions/contabil/10/20260516/DIG00000010.PDF",
                    HashAnteriorSha256 = "aa",
                    HashNuevoSha256 = "bb",
                    RequestId = "req-1"
                }
            });

        var controller = BuildController(
            claimValidation.Object,
            service.Object,
            identityName: "qa.user");
        var result = await controller.Post(BuildRequest());

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<AppResponses<ReemplazarDocumentoPdfResponse?>>(ok.Value);
        Assert.True(body.success);
    }

    [Fact]
    public async Task InitUploadTemporal_CuandoServicioOk_RetornaOk()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation.Setup(x => x.ValidateClaim<string>("usuarioid"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "141" });

        var upload = new Mock<IStorageLargeUploadService>();
        upload.Setup(x => x.InitAsync(It.IsAny<StorageUploadInitRequestDto>(), 141))
            .ReturnsAsync(new StorageUploadInitResult
            {
                RutaTemporalId = "ruta-1",
                ArchivoTemporalId = "archivo-1",
                ChunkSizeBytes = 2097152,
                Estado = StorageTemporaryUploadState.InProgress
            });

        var controller = BuildController(claimValidation.Object, uploadService: upload.Object);
        var result = await controller.InitUploadTemporal(new StorageUploadInitRequestDto
        {
            NombreOriginal = "doc.pdf",
            Extension = ".PDF",
            TamanoBytes = 1024,
            NumeroChunks = 1
        });

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<AppResponses<StorageUploadInitResponseDto?>>(ok.Value);
        Assert.True(body.success);
        Assert.Equal("ruta-1", body.data?.RutaTemporalId);
    }

    private static ReemplazarDocumentoPdfRequest BuildRequest()
    {
        return new ReemplazarDocumentoPdfRequest
        {
            NombreGabinete = "contabil",
            IdDocumento = 10,
            RutaTemporalId = "usr_10_abc",
            ArchivoTemporalId = "af_abc.pdf",
            Motivo = "ajuste"
        };
    }

    private static ReemplazoPdfController BuildController(
        IClaimValidationService claimValidationService,
        IReemplazoPdfService? service = null,
        IStorageLargeUploadService? uploadService = null,
        string identityName = "")
    {
        service ??= Mock.Of<IReemplazoPdfService>();
        uploadService ??= Mock.Of<IStorageLargeUploadService>();
        var ipHelper = new Mock<IIpHelper>();
        ipHelper.Setup(x => x.ObtenerDireccionIP(It.IsAny<HttpContext>())).Returns("10.0.0.10");

        var controller = new ReemplazoPdfController(
            claimValidationService,
            service,
            uploadService,
            ipHelper.Object,
            Mock.Of<ILogger<ReemplazoPdfController>>());

        var httpContext = new DefaultHttpContext();
        if (!string.IsNullOrWhiteSpace(identityName))
        {
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.Name, identityName)],
                "TestAuth"));
        }

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        return controller;
    }
}
