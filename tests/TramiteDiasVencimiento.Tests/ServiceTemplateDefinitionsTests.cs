using MiApp.DTOs.DTOs.GestorDocumental.Editor;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Repository.Repositorio.GestorDocumental.Editor;
using MiApp.Services.Service.GestorDocumental.Editor;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ServiceTemplateDefinitionsTests
{
    [Fact]
    public async Task CreateDefinition_CuandoAliasInvalido_NoInvocaRepo()
    {
        var repo = new Mock<ITemplateDefinitionsRepository>(MockBehavior.Strict);
        var service = new ServiceTemplateDefinitions(repo.Object);

        var res = await service.CreateDefinitionAsync(new CreateTemplateDefinitionDto { TemplateCode = "X", TemplateName = "N" }, "   ");

        Assert.False(res.success);
        repo.VerifyNoOtherCalls();
    }
}

