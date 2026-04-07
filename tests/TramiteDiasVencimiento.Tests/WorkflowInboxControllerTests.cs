using System.Security;
using DocuArchi.Api.Controllers.WorkflowInboxGestion;
using MiApp.DTOs.DTOs.Errors;
using MiApp.DTOs.DTOs.UI.MuiTable;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.DTOs.DTOs.Workflow.BandejaCorrespondencia;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using MiApp.Services.Service.Workflow.BandejaCorrespondencia;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class WorkflowInboxControllerTests
{
    [Fact]
    public async Task SolicitaBandejaWorkflow_CuandoClaimDefaulaliasEsInvalido_RetornaBadRequest()
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

        var controller = new WorkflowInboxController(claimValidation.Object, Mock.Of<IWorkflowInboxService>());

        var result = await controller.SolicitaBandejaWorkflow(CreateRequest());

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SolicitaBandejaWorkflow_CuandoClaimUsuarioIdNoEsEntero_LanzaSecurityException()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        claimValidation
            .Setup(service => service.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "DA", Response = null });
        claimValidation
            .Setup(service => service.ValidateClaim<string>("usuarioid"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "abc", Response = null });

        var controller = new WorkflowInboxController(claimValidation.Object, Mock.Of<IWorkflowInboxService>());

        await Assert.ThrowsAsync<SecurityException>(() => controller.SolicitaBandejaWorkflow(CreateRequest()));
    }

    [Fact]
    public async Task SolicitaBandejaWorkflow_CuandoClaimsSonValidos_DelegaAlServicio()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        var service = new Mock<IWorkflowInboxService>();

        claimValidation
            .Setup(svc => svc.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "DA", Response = null });
        claimValidation
            .Setup(svc => svc.ValidateClaim<string>("usuarioid"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "10", Response = null });

        service
            .Setup(svc => svc.SolicitaBandejaWorkflowAsync(It.IsAny<WorkflowInboxApiRequestDto>(), 10, "DA"))
            .ReturnsAsync(new AppResponses<DynamicUiTableDto>
            {
                success = true,
                message = "OK",
                data = new DynamicUiTableDto { TableId = "workflowInboxgestion" },
                errors = []
            });

        var controller = new WorkflowInboxController(claimValidation.Object, service.Object);

        var result = await controller.SolicitaBandejaWorkflow(CreateRequest());

        Assert.IsType<OkObjectResult>(result.Result);
        service.Verify(svc => svc.SolicitaBandejaWorkflowAsync(It.IsAny<WorkflowInboxApiRequestDto>(), 10, "DA"), Times.Once);
    }

    [Fact]
    public async Task ExportaBandejaWorkflow_CuandoClaimsSonValidos_RetornaArchivo()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        var service = new Mock<IWorkflowInboxService>();

        claimValidation
            .Setup(svc => svc.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "DA", Response = null });
        claimValidation
            .Setup(svc => svc.ValidateClaim<string>("usuarioid"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "10", Response = null });

        service
            .Setup(svc => svc.ExportBandejaWorkflowAsync(It.IsAny<WorkflowInboxExportRequestDto>(), 10, "DA"))
            .ReturnsAsync(new AppResponses<WorkflowInboxExportFileDto>
            {
                success = true,
                message = "OK",
                data = new WorkflowInboxExportFileDto
                {
                    Format = WorkflowInboxExportFormats.Xlsx,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = "workflow_inbox_20260405_120000.xlsx",
                    FileBytes = [1, 2, 3]
                },
                errors = []
            });

        var controller = new WorkflowInboxController(claimValidation.Object, service.Object);

        var request = CreateExportRequest();
        request.Format = WorkflowInboxExportFormats.Xlsx;

        var result = await controller.ExportaBandejaWorkflow(request);

        var file = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file.ContentType);
        Assert.Equal("workflow_inbox_20260405_120000.xlsx", file.FileDownloadName);
        service.Verify(svc => svc.ExportBandejaWorkflowAsync(It.IsAny<WorkflowInboxExportRequestDto>(), 10, "DA"), Times.Once);
    }

    [Fact]
    public async Task AutocompleteBandejaWorkflow_CuandoClaimsSonValidos_DelegaAlServicio()
    {
        var claimValidation = new Mock<IClaimValidationService>();
        var service = new Mock<IWorkflowInboxService>();

        claimValidation
            .Setup(svc => svc.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "DA", Response = null });
        claimValidation
            .Setup(svc => svc.ValidateClaim<string>("usuarioid"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = "10", Response = null });

        service
            .Setup(svc => svc.AutocompleteBandejaWorkflowAsync(It.IsAny<WorkflowInboxAutocompleteRequestDto>(), 10, "DA"))
            .ReturnsAsync(new AppResponses<WorkflowInboxAutocompleteResponseDto>
            {
                success = true,
                message = "OK",
                data = new WorkflowInboxAutocompleteResponseDto
                {
                    Items =
                    [
                        new WorkflowInboxAutocompleteItemDto { Value = "ABC-123", Label = "ABC-123", Field = "asunto" }
                    ]
                },
                errors = []
            });

        var controller = new WorkflowInboxController(claimValidation.Object, service.Object);

        var result = await controller.AutocompleteBandejaWorkflow(new WorkflowInboxAutocompleteRequestDto { Search = "ABC", Limit = 5 });

        Assert.IsType<OkObjectResult>(result.Result);
        service.Verify(svc => svc.AutocompleteBandejaWorkflowAsync(It.IsAny<WorkflowInboxAutocompleteRequestDto>(), 10, "DA"), Times.Once);
    }

    [Fact]
    public void WorkflowInboxApiRequestDto_NoExponeCamposInternos()
    {
        var propertyNames = typeof(WorkflowInboxApiRequestDto)
            .GetProperties()
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);

        Assert.DoesNotContain("IdUsuarioGestion", propertyNames);
        Assert.DoesNotContain("IdUsuarioWorkflow", propertyNames);
        Assert.DoesNotContain("NombreRuta", propertyNames);
        Assert.DoesNotContain("IdActividad", propertyNames);
        Assert.DoesNotContain("DefaultDbAlias", propertyNames);
    }

    [Fact]
    public void WorkflowInboxExportRequestDto_NoExponeCamposInternos()
    {
        var propertyNames = typeof(WorkflowInboxExportRequestDto)
            .GetProperties()
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);

        Assert.DoesNotContain("IdUsuarioGestion", propertyNames);
        Assert.DoesNotContain("IdUsuarioWorkflow", propertyNames);
        Assert.DoesNotContain("NombreRuta", propertyNames);
        Assert.DoesNotContain("IdActividad", propertyNames);
        Assert.DoesNotContain("DefaultDbAlias", propertyNames);
    }

    private static WorkflowInboxApiRequestDto CreateRequest() => new()
    {
        ColumnMode = WorkflowColumnListMode.ListaGestionTramite,
        EstadoTramite = "Todos",
        SearchType = 1,
        Search = string.Empty,
        SortField = string.Empty,
        SortDir = "ASC",
        Page = 1,
        PageSize = 25,
        StructuredFilters = []
    };

    private static WorkflowInboxExportRequestDto CreateExportRequest() => new()
    {
        ColumnMode = WorkflowColumnListMode.ListaGestionTramite,
        EstadoTramite = "Todos",
        SearchType = 2,
        Search = "radicado",
        SortField = "fecha_inicio",
        SortDir = "DESC",
        Page = 7,
        PageSize = 10,
        Format = "csv",
        ExportMode = "allMatching",
        ReportTitle = "workflow inbox",
        StructuredFilters = []
    };

    private static AppResponses<string> Validation(string field) => new()
    {
        success = false,
        message = field,
        data = string.Empty,
        errors = [new AppError { Field = field, Message = field, Type = "Validation" }]
    };
}
