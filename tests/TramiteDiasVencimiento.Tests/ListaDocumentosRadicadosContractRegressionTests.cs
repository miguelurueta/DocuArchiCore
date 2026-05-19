using System.Reflection;
using DocuArchi.Api.Controllers.GestorDocumental.Documentos;
using DocuArchi.Api.Controllers.WorkflowInboxGestion;
using MiApp.DTOs.DTOs.GestorDocumental.Documentos.ListaDocumentosRadicados;
using MiApp.DTOs.DTOs.UI.MuiTable;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.DTOs.DTOs.Workflow.BandejaCorrespondencia;
using MiApp.Services.Service.GestorDocumental.Documentos.ListaDocumentosRadicados;
using MiApp.Services.Service.Seguridad.Autorizacion.CurrentClaim;
using MiApp.Services.Service.Workflow.BandejaCorrespondencia;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ListaDocumentosRadicadosContractRegressionTests
{
    [Fact]
    public async Task ListaDocumentosRadicados_Query_CuandoOk_RetornaEnvelopeAppResponses()
    {
        var claims = BuildClaimService("DA", "10");
        var service = new Mock<IListaDocumentosRadicadoService>();
        service
            .Setup(s => s.SolicitaListaDocumentosRadicadosTreeAsync(It.IsAny<ListaDocumentosRadicadosTreeQueryRequestDto>(), 10, "DA"))
            .ReturnsAsync(new AppResponses<object>
            {
                success = true,
                message = "OK",
                data = new DynamicUiRowsOnlyDto
                {
                    TableId = "InboxListaRadicados",
                    Rows = []
                },
                meta = new AppMeta { Status = "success" },
                errors = []
            });

        var controller = new ListaDocumentosRadicadoController(claims.Object, service.Object);

        var result = await controller.Query(new ListaDocumentosRadicadosTreeQueryRequestDto());

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AppResponses<object>>(ok.Value);
        Assert.True(payload.success);
        Assert.Equal("OK", payload.message);
        Assert.Equal("success", payload.meta?.Status);
    }

    [Fact]
    public async Task ListaDocumentosRadicados_Action_CuandoOk_RetornaEnvelopeAppResponses()
    {
        var claims = BuildClaimService("DA", "10");
        var service = new Mock<IListaDocumentosRadicadoService>();
        service
            .Setup(s => s.EjecutaAccionListaDocumentosRadicadosTreeAsync(It.IsAny<ListaDocumentosRadicadosTreeActionRequestDto>(), 10, "DA"))
            .ReturnsAsync(new AppResponses<ListaDocumentosRadicadosTreeMutationResultDto?>
            {
                success = true,
                message = "OK",
                data = new ListaDocumentosRadicadosTreeMutationResultDto
                {
                    Operation = "view",
                    AffectedRowId = "doc-15416",
                    RequiresReloadNode = false
                },
                meta = new AppMeta { Status = "success" },
                errors = []
            });

        var controller = new ListaDocumentosRadicadoController(claims.Object, service.Object);

        var result = await controller.Action(new ListaDocumentosRadicadosTreeActionRequestDto { ActionId = "ver_documento" });

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AppResponses<ListaDocumentosRadicadosTreeMutationResultDto?>>(ok.Value);
        Assert.True(payload.success);
        Assert.NotNull(payload.data);
        Assert.Equal("view", payload.data!.Operation);
    }

    [Fact]
    public void WorkflowInbox_RutasPublicas_NoCambian()
    {
        var classRoute = typeof(WorkflowInboxController).GetCustomAttribute<RouteAttribute>()?.Template;
        Assert.Equal("api/workflowInboxgestion", classRoute);

        var inboxRoute = typeof(WorkflowInboxController)
            .GetMethod(nameof(WorkflowInboxController.SolicitaBandejaWorkflow))?
            .GetCustomAttribute<HttpPostAttribute>()?
            .Template;
        var autocompleteRoute = typeof(WorkflowInboxController)
            .GetMethod(nameof(WorkflowInboxController.AutocompleteBandejaWorkflow))?
            .GetCustomAttribute<HttpPostAttribute>()?
            .Template;
        var exportRoute = typeof(WorkflowInboxController)
            .GetMethod(nameof(WorkflowInboxController.ExportaBandejaWorkflow))?
            .GetCustomAttribute<HttpPostAttribute>()?
            .Template;

        Assert.Equal("inboxgestion", inboxRoute);
        Assert.Equal("inboxgestion/autocomplete", autocompleteRoute);
        Assert.Equal("/api/AppTable/export", exportRoute);
    }

    [Fact]
    public async Task WorkflowInbox_Regression_EndpointsBaseSiguenRespondiendoConContratosEsperados()
    {
        var claims = BuildClaimService("DA", "10");
        var service = new Mock<IWorkflowInboxService>();
        service
            .Setup(s => s.SolicitaBandejaWorkflowAsync(It.IsAny<WorkflowInboxApiRequestDto>(), 10, "DA"))
            .ReturnsAsync(new AppResponses<DynamicUiTableDto>
            {
                success = true,
                message = "OK",
                data = new DynamicUiTableDto { TableId = "workflowInboxgestion", Rows = [] },
                errors = []
            });
        service
            .Setup(s => s.AutocompleteBandejaWorkflowAsync(It.IsAny<WorkflowInboxAutocompleteRequestDto>(), 10, "DA"))
            .ReturnsAsync(new AppResponses<WorkflowInboxAutocompleteResponseDto>
            {
                success = true,
                message = "OK",
                data = new WorkflowInboxAutocompleteResponseDto { Items = [] },
                errors = []
            });
        service
            .Setup(s => s.ExportBandejaWorkflowAsync(It.IsAny<WorkflowInboxExportRequestDto>(), 10, "DA"))
            .ReturnsAsync(new AppResponses<WorkflowInboxExportFileDto>
            {
                success = true,
                message = "OK",
                data = new WorkflowInboxExportFileDto
                {
                    FileBytes = [1, 2, 3],
                    FileName = "export.xlsx",
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    Format = WorkflowInboxExportFormats.Xlsx
                },
                errors = []
            });

        var controller = new WorkflowInboxController(claims.Object, service.Object);

        var inbox = await controller.SolicitaBandejaWorkflow(new WorkflowInboxApiRequestDto());
        var autocomplete = await controller.AutocompleteBandejaWorkflow(new WorkflowInboxAutocompleteRequestDto { Search = "ABC", Limit = 5 });
        var export = await controller.ExportaBandejaWorkflow(new WorkflowInboxExportRequestDto());

        Assert.IsType<OkObjectResult>(inbox.Result);
        Assert.IsType<OkObjectResult>(autocomplete.Result);
        Assert.IsType<FileContentResult>(export);
    }

    private static Mock<IClaimValidationService> BuildClaimService(string alias, string usuarioId)
    {
        var claimService = new Mock<IClaimValidationService>();
        claimService
            .Setup(service => service.ValidateClaim<string>("defaulalias"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = alias, Response = null });
        claimService
            .Setup(service => service.ValidateClaim<string>("usuarioid"))
            .Returns(new ClaimValidationResult<string> { Success = true, ClaimValue = usuarioId, Response = null });
        return claimService;
    }
}
