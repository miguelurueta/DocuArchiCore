using MiApp.DTOs.DTOs.Radicacion.Tramite;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Repository.Repositorio.Radicador.PlantillaRadicado;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using MiApp.Services.Service.Radicacion.Tramite;
using Moq;

namespace TramiteDiasVencimiento.Tests;

public sealed class RegistrarRadicacionEntranteServiceTests
{
    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoRequestValido_RetornaSuccess()
    {
        var remitRepo = new Mock<IRemitDestInternoR>();
        var registrarRepo = new Mock<IRegistrarRadicacionEntranteRepository>();

        remitRepo
            .Setup(r => r.SolicitaEstructuraIdUsuarioGestion(10, "DA"))
            .ReturnsAsync(new AppResponses<MiApp.Models.Models.Radicacion.PlantillaRadicado.remit_dest_interno>
            {
                success = true,
                message = "OK",
                data = new MiApp.Models.Models.Radicacion.PlantillaRadicado.remit_dest_interno
                {
                    Relacion_Id_Usuario_Radicacion = 55
                }
            });

        registrarRepo
            .Setup(r => r.RegistrarRadicacionEntranteAsync(It.IsAny<RegistrarRadicacionEntranteRequestDto>(), 55, "DA"))
            .ReturnsAsync(new AppResponses<RegistrarRadicacionEntranteResponseDto>
            {
                success = true,
                message = "OK",
                data = new RegistrarRadicacionEntranteResponseDto { ConsecutivoRadicado = "RAD-TEST-1" }
            });

        var service = new RegistrarRadicacionEntranteService(remitRepo.Object, registrarRepo.Object);

        var result = await service.RegistrarRadicacionEntranteAsync(new RegistrarRadicacionEntranteRequestDto
        {
            IdPlantilla = 100,
            TipoRadicacion = "ENTRANTE",
            Remitente = "R",
            Asunto = "A"
        }, 10, "DA");

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Equal("RAD-TEST-1", result.data!.ConsecutivoRadicado);
    }

    [Fact]
    public async Task RegistrarRadicacionEntranteAsync_CuandoAliasVacio_RetornaValidacion()
    {
        var service = new RegistrarRadicacionEntranteService(
            Mock.Of<IRemitDestInternoR>(),
            Mock.Of<IRegistrarRadicacionEntranteRepository>());

        var result = await service.RegistrarRadicacionEntranteAsync(
            new RegistrarRadicacionEntranteRequestDto { IdPlantilla = 100 },
            10,
            string.Empty);

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
    }
}
