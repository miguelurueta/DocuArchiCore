using MiApp.Models.Models.Radicacion.RelacionCamposRutaWorklflow;
using MiApp.Repository.ErrorController;
using MiApp.Services.Service.Radicacion.RelacionCamposRutaWorklflow;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ValidaDatosRadicacionTareaWorkflowServiceTests
{
    [Fact]
    public async Task ValidaDatosRadicacionTareaWorkflowAsync_CuandoNoHayErrores_RetornaValidacionExitosa()
    {
        var service = new ValidaDatosRadicacionTareaWorkflowService();

        var result = await service.ValidaDatosRadicacionTareaWorkflowAsync(
        [
            new RelacionCamposRutaWorklflow
            {
                NombreCampoPlantilla = "Asunto",
                TipoCampoPlantilla = "varchar",
                DimensionCampoPlantilla = "255",
                DatoCampoPlantilla = "Workflow correcto"
            },
            new RelacionCamposRutaWorklflow
            {
                NombreCampoPlantilla = "IdTercero",
                TipoCampoRuta = "int",
                DatoCampoPlantilla = "123"
            }
        ]);

        Assert.True(result.success);
        Assert.Equal("Validación exitosa", result.message);
        Assert.NotNull(result.data);
        Assert.Empty(result.data!);
    }

    [Fact]
    public async Task ValidaDatosRadicacionTareaWorkflowAsync_CuandoSuperaMaxLength_RetornaError()
    {
        var service = new ValidaDatosRadicacionTareaWorkflowService();

        var result = await service.ValidaDatosRadicacionTareaWorkflowAsync(
        [
            new RelacionCamposRutaWorklflow
            {
                NombreCampoPlantilla = "Asunto",
                TipoCampoPlantilla = "varchar",
                DimensionCampoPlantilla = "5",
                DatoCampoPlantilla = "123456"
            }
        ]);

        var error = Assert.Single(result.data!);
        Assert.False(result.success);
        Assert.Equal("MaxLength", error.Type);
    }

    [Fact]
    public async Task ValidaDatosRadicacionTareaWorkflowAsync_CuandoFormatoEsInvalido_RetornaError()
    {
        var service = new ValidaDatosRadicacionTareaWorkflowService();

        var result = await service.ValidaDatosRadicacionTareaWorkflowAsync(
        [
            new RelacionCamposRutaWorklflow
            {
                NombreCampoPlantilla = "IdTercero",
                TipoCampoRuta = "int",
                DatoCampoPlantilla = "ABC123"
            }
        ]);

        var error = Assert.Single(result.data!);
        Assert.False(result.success);
        Assert.Equal("InvalidType", error.Type);
    }

    [Fact]
    public async Task ValidaDatosRadicacionTareaWorkflowAsync_CuandoCampoRequeridoEstaVacio_RetornaError()
    {
        var service = new ValidaDatosRadicacionTareaWorkflowService();

        var result = await service.ValidaDatosRadicacionTareaWorkflowAsync(
        [
            new RelacionCamposRutaWorklflow
            {
                NombreCampoPlantilla = "NumeroRadicado",
                TipoCampoPlantilla = "required|varchar",
                DatoCampoPlantilla = ""
            }
        ]);

        var error = Assert.Single(result.data!);
        Assert.False(result.success);
        Assert.Equal("Required", error.Type);
    }

    [Fact]
    public async Task ValidaDatosRadicacionTareaWorkflowAsync_CuandoCampoRequeridoEsNull_RetornaError()
    {
        var service = new ValidaDatosRadicacionTareaWorkflowService();

        var result = await service.ValidaDatosRadicacionTareaWorkflowAsync(
        [
            new RelacionCamposRutaWorklflow
            {
                NombreCampoPlantilla = "NumeroRadicado",
                TipoCampoRuta = "not null",
                DatoCampoPlantilla = null!
            }
        ]);

        var error = Assert.Single(result.data!);
        Assert.False(result.success);
        Assert.Equal("Required", error.Type);
    }

    [Fact]
    public async Task ValidaDatosRadicacionTareaWorkflowAsync_CuandoHayMultiplesErrores_LosRetornaTodos()
    {
        var service = new ValidaDatosRadicacionTareaWorkflowService();

        var result = await service.ValidaDatosRadicacionTareaWorkflowAsync(
        [
            new RelacionCamposRutaWorklflow
            {
                NombreCampoPlantilla = "Asunto",
                TipoCampoPlantilla = "varchar",
                DimensionCampoPlantilla = "3",
                DatoCampoPlantilla = "12345"
            },
            new RelacionCamposRutaWorklflow
            {
                NombreCampoPlantilla = "IdTercero",
                TipoCampoRuta = "int",
                DatoCampoPlantilla = "ABC"
            }
        ]);

        Assert.False(result.success);
        Assert.Equal(2, result.data!.Count);
        Assert.Contains(result.data!, x => x.Type == "MaxLength");
        Assert.Contains(result.data!, x => x.Type == "InvalidType");
    }

    [Fact]
    public async Task ValidaDatosRadicacionTareaWorkflowAsync_CuandoListaEsVacia_RetornaSinResultados()
    {
        var service = new ValidaDatosRadicacionTareaWorkflowService();

        var result = await service.ValidaDatosRadicacionTareaWorkflowAsync([]);

        Assert.True(result.success);
        Assert.Equal("Sin resultados", result.message);
        Assert.Empty(result.data!);
    }

    [Fact]
    public async Task ValidaDatosRadicacionTareaWorkflowAsync_CuandoListaEsNula_RetornaValidationError()
    {
        var service = new ValidaDatosRadicacionTareaWorkflowService();

        var result = await service.ValidaDatosRadicacionTareaWorkflowAsync(null);

        var error = Assert.Single(result.data!);
        Assert.False(result.success);
        Assert.Equal("Required", error.Type);
    }
}
