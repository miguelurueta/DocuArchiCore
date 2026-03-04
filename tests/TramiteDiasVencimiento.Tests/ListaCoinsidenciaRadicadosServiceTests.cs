using MiApp.DTOs.DTOs.Radicacion.ConsultaRadicacion;
using MiApp.DTOs.DTOs.UI.MuiTable;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Repository.Repositorio.Radicador.ConsultaRadicacion;
using MiApp.Services.Service.Radicacion.ConsultaRadicacion;
using MiApp.Services.Service.UI.MuiTable;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public class ListaCoinsidenciaRadicadosServiceTests
{
    [Fact]
    public async Task ServiceListaCoinsidenciaRadicados_CuandoHayDatos_RetornaTablaDinamica()
    {
        var repository = new Mock<IConsultaCoinsidenciaRadicadosRepository>();
        var builder = new Mock<IDynamicUiTableBuilder>();

        repository
            .Setup(r => r.SolicitaEstructuraCamposConsultaCoinsidenciaRadicados("DA", 2))
            .ReturnsAsync(new AppResponses<List<CamposConsultaCoinsidenciaRadicadosDTO>>
            {
                success = true,
                message = "OK",
                data =
                [
                    new CamposConsultaCoinsidenciaRadicadosDTO
                    {
                        Key = "Consecutivo_Rad",
                        ColumnName = "Consecutivo_Rad",
                        HeaderName = "Consecutivo",
                        Order = 1,
                        Visible = true
                    }
                ],
                errors = []
            });

        repository
            .Setup(r => r.SolicitaListaCoinsidenciaRadicadosRepository("DA", It.IsAny<List<UiColumnDto>>(), "RAD-"))
            .ReturnsAsync(new AppResponses<List<CoinsidenciaRadicadoDTO>>
            {
                success = true,
                message = "OK",
                data =
                [
                    new CoinsidenciaRadicadoDTO
                    {
                        Id = "1",
                        Valores = new Dictionary<string, object?>
                        {
                            ["Consecutivo_Rad"] = "RAD-001"
                        }
                    }
                ],
                errors = []
            });

        builder
            .Setup(b => b.BuildAsync(It.IsAny<DynamicUiTableBuildInput>()))
            .ReturnsAsync(new DynamicUiTableDto
            {
                TableId = "coinsidencia-radicados",
                Columns =
                [
                    new UiColumnDto
                    {
                        Key = "Consecutivo_Rad",
                        ColumnName = "Consecutivo_Rad",
                        HeaderName = "Consecutivo",
                        Order = 1
                    }
                ],
                Rows =
                [
                    new UiRowDto
                    {
                        Id = "1",
                        Values = new Dictionary<string, object?>
                        {
                            ["Consecutivo_Rad"] = "RAD-001"
                        }
                    }
                ]
            });

        var service = new ListaCoinsidenciaRadicadosService(repository.Object, builder.Object);

        var result = await service.ServiceListaCoinsidenciaRadicados("DA", 10, new ParametroCoinsidenciaRadicadoDTO
        {
            TextoBuscado = "RAD-",
            TipoModuloDeConsulta = 2
        });

        Assert.True(result.success);
        Assert.Equal("OK", result.message);
        Assert.NotNull(result.data);
        Assert.Equal("coinsidencia-radicados", result.data.TableId);
        Assert.Single(result.data.Rows);
    }

    [Fact]
    public async Task ServiceListaCoinsidenciaRadicados_CuandoIdUsuarioEsInvalido_RetornaErrorValidacion()
    {
        var repository = new Mock<IConsultaCoinsidenciaRadicadosRepository>();
        var builder = new Mock<IDynamicUiTableBuilder>();

        var service = new ListaCoinsidenciaRadicadosService(repository.Object, builder.Object);

        var result = await service.ServiceListaCoinsidenciaRadicados("DA", 0, new ParametroCoinsidenciaRadicadoDTO());

        Assert.False(result.success);
        Assert.Equal("IdUsuarioGestion requerido", result.message);
        repository.Verify(
            r => r.SolicitaEstructuraCamposConsultaCoinsidenciaRadicados(It.IsAny<string>(), It.IsAny<int>()),
            Times.Never);
    }
}
