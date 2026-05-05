using System.Security.Claims;
using DocuArchi.Api.Controllers.GestorDocumental.AlmacenamientoDocumental;
using DocuArchi.Api.Infrastructure.Features;
using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Services.Service.SessionHelper;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class AlmacenamientoDocumentalControllerTests
    {
        [Fact]
        public async Task ShouldReturnBadRequest_WhenAliasClaimMissing()
        {
            var claimValidation = new Mock<IClaimValidationService>();
            claimValidation.Setup(x => x.ValidateClaim<string>("defaulalias"))
                .Returns(new ClaimValidationResult<string>
                {
                    Success = false,
                    Response = new AppResponses<string> { success = false, message = "missing alias" }
                });

            var controller = BuildController(claimValidation.Object);
            var result = await controller.AlmacenarDocumento(BuildRequest());

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task ShouldReturnBadRequest_WhenUsuarioIdClaimInvalid()
        {
            var claimValidation = new Mock<IClaimValidationService>();
            claimValidation.Setup(x => x.ValidateClaim<string>("defaulalias"))
                .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "tenantA" });
            claimValidation.Setup(x => x.ValidateClaim<string>("usuarioid"))
                .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "abc" });

            var controller = BuildController(claimValidation.Object);
            var result = await controller.AlmacenarDocumento(BuildRequest());

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var body = Assert.IsType<AppResponses<AlmacenarDocumentoResponse?>>(badRequest.Value);
            Assert.False(body.success);
            Assert.Equal("validation", body.meta?.Status);
        }

        [Fact]
        public async Task ShouldReturnFeatureDisabled_WhenFlagIsOff()
        {
            var claimValidation = new Mock<IClaimValidationService>();
            claimValidation.Setup(x => x.ValidateClaim<string>("defaulalias"))
                .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "tenantA" });
            claimValidation.Setup(x => x.ValidateClaim<string>("usuarioid"))
                .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "25" });

            var featureToggle = new Mock<IFeatureToggleService>();
            featureToggle.Setup(x => x.IsEnabledAsync("StorageEngineV2")).ReturnsAsync(false);

            var controller = BuildController(claimValidation.Object, featureToggle: featureToggle.Object);
            var result = await controller.AlmacenarDocumento(BuildRequest());

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var body = Assert.IsType<AppResponses<AlmacenarDocumentoResponse?>>(badRequest.Value);
            Assert.False(body.success);
            Assert.Equal("feature_disabled", body.meta?.Status);
        }

        [Fact]
        public async Task ShouldReturnBadRequest_WhenUseCaseReturnsFailure()
        {
            var claimValidation = new Mock<IClaimValidationService>();
            claimValidation.Setup(x => x.ValidateClaim<string>("defaulalias"))
                .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "tenantA" });
            claimValidation.Setup(x => x.ValidateClaim<string>("usuarioid"))
                .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "25" });

            var useCase = new Mock<IAlmacenarDocumentoUseCase>();
            useCase.Setup(x => x.ExecuteAsync(
                    It.IsAny<AlmacenarDocumentoRequest>(),
                    "tenantA",
                    It.IsAny<string>(),
                    25,
                    It.IsAny<string?>()))
                .ReturnsAsync(new AppResponses<AlmacenarDocumentoResponse?>
                {
                    success = false,
                    message = "validation error",
                    data = null
                });

            var controller = BuildController(claimValidation.Object, useCase.Object);
            var result = await controller.AlmacenarDocumento(BuildRequest());

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task ShouldReturnOk_WhenUseCaseReturnsSuccess()
        {
            var claimValidation = new Mock<IClaimValidationService>();
            claimValidation.Setup(x => x.ValidateClaim<string>("defaulalias"))
                .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "tenantA" });
            claimValidation.Setup(x => x.ValidateClaim<string>("usuarioid"))
                .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "99" });

            var useCase = new Mock<IAlmacenarDocumentoUseCase>();
            useCase.Setup(x => x.ExecuteAsync(
                    It.IsAny<AlmacenarDocumentoRequest>(),
                    "tenantA",
                    "qa.user",
                    99,
                    It.IsAny<string?>()))
                .ReturnsAsync(new AppResponses<AlmacenarDocumentoResponse?>
                {
                    success = true,
                    message = "OK",
                    data = new AlmacenarDocumentoResponse { IdAlmacen = 10, RequestId = "req-1" }
                });

            var controller = BuildController(claimValidation.Object, useCase.Object, identityName: "qa.user");
            var result = await controller.AlmacenarDocumento(BuildRequest());

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var body = Assert.IsType<AppResponses<AlmacenarDocumentoResponse?>>(ok.Value);
            Assert.True(body.success);
        }

        private static AlmacenarDocumentoRequest BuildRequest()
        {
            return new AlmacenarDocumentoRequest
            {
                NombreGabinete = "GAB-01",
                RutaTemporalId = "TMP-01",
                NombreDocumento = "doc.pdf",
                RequestId = "req-1",
                Documentos = new List<DocumentoEntradaDto>
                {
                    new()
                    {
                        IdDocumento = "DOC-1",
                        ArchivoTemporalId = "file-1",
                        NombreOriginal = "doc.pdf",
                        Extension = "pdf",
                        NumeroPaginas = 1
                    }
                }
            };
        }

        private static AlmacenamientoDocumentalController BuildController(
            IClaimValidationService claimValidationService,
            IAlmacenarDocumentoUseCase? useCase = null,
            IFeatureToggleService? featureToggle = null,
            string identityName = "")
        {
            useCase ??= Mock.Of<IAlmacenarDocumentoUseCase>();
            featureToggle ??= Mock.Of<IFeatureToggleService>(x => x.IsEnabledAsync("StorageEngineV2") == Task.FromResult(true));
            var ipHelper = new Mock<IIpHelper>();
            ipHelper.Setup(x => x.ObtenerDireccionIP(It.IsAny<HttpContext>())).Returns("10.0.0.10");

            var controller = new AlmacenamientoDocumentalController(
                claimValidationService,
                useCase,
                featureToggle,
                ipHelper.Object,
                Mock.Of<ILogger<AlmacenamientoDocumentalController>>());

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
}
