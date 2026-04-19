using MiApp.DTOs.DTOs.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.GestorDocumental.Editor;
using MiApp.Models.Models.GestionCorrespondencia;
using MiApp.Repository.Repositorio.GestorDocumental.Editor;
using MiApp.Services.Service.GestorDocumental;
using MiApp.Services.Service.GestorDocumental.Editor;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ServiceInitialContentEditorTests
{
    [Fact]
    public async Task CuandoContextCodeInvalido_RetornaValidation()
    {
        var estructura = new Mock<IServiceSolicitaEstructuraRespuesta>(MockBehavior.Strict);
        var ctx = new Mock<ISolicitaEditorContextDefinitionRepository>(MockBehavior.Strict);
        var templates = new Mock<ITemplateDefinitionsRepository>(MockBehavior.Strict);

        var service = new ServiceInitialContentEditor(estructura.Object, ctx.Object, templates.Object);

        var res = await service.GetInitialContentAsync(1, "   ", 1, "db1");

        Assert.False(res.success);
        ctx.VerifyNoOtherCalls();
        templates.VerifyNoOtherCalls();
        estructura.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CuandoFaltanTokens_RetornaValidation()
    {
        var estructura = new Mock<IServiceSolicitaEstructuraRespuesta>();
        estructura.Setup(s => s.SolicitaEstructuraRespuestaIdTareaAsync(1, "db1"))
            .ReturnsAsync(new AppResponses<List<RaRespuestaRadicado>> { success = true, message = "YES", data = [new RaRespuestaRadicado()], errors = [] });

        var ctx = new Mock<ISolicitaEditorContextDefinitionRepository>();
        ctx.Setup(r => r.SolicitaPorContextCodeAsync("RAD", "db1", null, null))
            .ReturnsAsync(new AppResponses<RaEditorContextDefinition?> { success = true, message = "YES", data = new RaEditorContextDefinition { ContextDefinitionId = 1, ContextCode = "RAD", EntityName = "x", RelationType = "y", IsActive = true }, errors = [] });

        var templates = new Mock<ITemplateDefinitionsRepository>();
        templates.Setup(r => r.GetDefinitionByCodeAsync("RAD", "db1", null, null))
            .ReturnsAsync(new AppResponses<TemplateDefinitionDto?> { success = true, message = "OK", data = new TemplateDefinitionDto { TemplateDefinitionId = 1, TemplateCode = "RAD", TemplateName = "N", IsActive = true }, errors = [] });
        templates.Setup(r => r.GetVersionsByCodeAsync("RAD", "db1", null, null))
            .ReturnsAsync(new AppResponses<IReadOnlyCollection<TemplateVersionDto>> { success = true, message = "OK", data = [new TemplateVersionDto { TemplateVersionId = 1, TemplateDefinitionId = 1, VersionNumber = 1, TemplateHtml = \"<p>{{TOKEN_NO_EXISTE}}</p>\", IsActive = true, IsPublished = true }], errors = [] });

        var service = new ServiceInitialContentEditor(estructura.Object, ctx.Object, templates.Object);

        var res = await service.GetInitialContentAsync(1, "rad", 911, "db1");

        Assert.False(res.success);
    }
}

