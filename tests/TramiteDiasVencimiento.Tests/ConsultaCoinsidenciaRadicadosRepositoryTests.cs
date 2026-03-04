using MiApp.DTOs.DTOs.Radicacion.ConsultaRadicacion;
using MiApp.Models.Models.Radicacion.PlantillaRadicado;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.Radicador.ConsultaRadicacion;
using MiApp.Repository.Repositorio.Radicador.PlantillaRadicado;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public class ConsultaCoinsidenciaRadicadosRepositoryTests
{
    [Fact]
    public async Task SolicitaEstructuraCamposConsultaCoinsidenciaRadicados_CuandoHayDatos_RetornaColumnasBaseYDinamicas()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var plantilla = new Mock<ISystemPlantillaRadicadoR>();
        var dbFactory = new Mock<IDbConnectionFactory>();

        plantilla
            .Setup(p => p.SolicitaEstructuraPlantillaRadicacionDefault("DA"))
            .ReturnsAsync(new MiApp.DTOs.DTOs.Utilidades.AppResponses<SystemPlantillaRadicado>
            {
                success = true,
                message = "YES",
                data = new SystemPlantillaRadicado
                {
                    id_Plantilla = 1,
                    Nombre_Plantilla_Radicado = "ra_radicados",
                    Tipo_Plantilla = "GEN"
                },
                errors = []
            });

        dapper
            .Setup(d => d.GetAllAsync<DetallePlantillaRadicado>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<DetallePlantillaRadicado>
            {
                Success = true,
                Message = "OK",
                Data =
                [
                    new DetallePlantillaRadicado
                    {
                        System_Plantilla_Radicado_id_Plantilla = 1,
                        Campo_Plantilla = "Campo_Dinamico",
                        Tipo_Campo = "VARCHAR",
                        Comportamiento_Campo = "DIGITACION",
                        Alias_Campo = "Campo Dinamico",
                        Orden_Campo = 20,
                        Estado_Campo = 1,
                        Descripcion_Campo = "x",
                        Campo_Obligatorio = 0,
                        Campo_rad_interno = 1,
                        Campo_rad_externo = 1,
                        Campo_rad_simple = 1,
                        tam_campo = 50,
                        id_detalle_plantilla_radicado = 1,
                        TagSesion = "TAG"
                    }
                ]
            });

        var repository = new ConsultaCoinsidenciaRadicadosRepository(dapper.Object, plantilla.Object, dbFactory.Object);

        var result = await repository.SolicitaEstructuraCamposConsultaCoinsidenciaRadicados("DA", 2);

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Contains(result.data!, c => c.ColumnName == "Consecutivo_Rad");
        Assert.Contains(result.data!, c => c.ColumnName == "Campo_Dinamico");
        Assert.DoesNotContain(result.data!, c => c.ColumnName == "estado_validacion");
    }

    [Fact]
    public async Task SolicitaEstructuraCamposConsultaCoinsidenciaRadicados_TipoValidacion_NoIncluyeEstadoValidacion()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var plantilla = new Mock<ISystemPlantillaRadicadoR>();
        var dbFactory = new Mock<IDbConnectionFactory>();

        plantilla
            .Setup(p => p.SolicitaEstructuraPlantillaRadicacionDefault("DA"))
            .ReturnsAsync(new MiApp.DTOs.DTOs.Utilidades.AppResponses<SystemPlantillaRadicado>
            {
                success = true,
                message = "YES",
                data = new SystemPlantillaRadicado
                {
                    id_Plantilla = 1,
                    Nombre_Plantilla_Radicado = "ra_radicados",
                    Tipo_Plantilla = "GEN"
                },
                errors = []
            });

        dapper
            .Setup(d => d.GetAllAsync<DetallePlantillaRadicado>(It.IsAny<QueryOptions>()))
            .ReturnsAsync(new QueryResult<DetallePlantillaRadicado>
            {
                Success = true,
                Message = "OK",
                Data = []
            });

        var repository = new ConsultaCoinsidenciaRadicadosRepository(dapper.Object, plantilla.Object, dbFactory.Object);
        var result = await repository.SolicitaEstructuraCamposConsultaCoinsidenciaRadicados("DA", 1);

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.DoesNotContain(result.data!, c => c.ColumnName == "estado_validacion");
    }

    [Fact]
    public async Task SolicitaListaCoinsidenciaRadicadosRepository_CuandoNoHayColumnas_RetornaErrorValidacion()
    {
        var dapper = new Mock<IDapperCrudEngine>();
        var plantilla = new Mock<ISystemPlantillaRadicadoR>();
        var dbFactory = new Mock<IDbConnectionFactory>();

        var repository = new ConsultaCoinsidenciaRadicadosRepository(dapper.Object, plantilla.Object, dbFactory.Object);

        var result = await repository.SolicitaListaCoinsidenciaRadicadosRepository("DA", [], "abc");

        Assert.False(result.success);
        Assert.Equal("Se requiere al menos una columna visible para consulta", result.message);
        dbFactory.Verify(f => f.GetOpenConnectionAsync(It.IsAny<string>()), Times.Never);
    }
}
