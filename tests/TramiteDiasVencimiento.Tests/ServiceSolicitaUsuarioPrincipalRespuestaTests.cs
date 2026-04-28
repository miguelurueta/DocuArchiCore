using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Models.Models.GestorDocumental.usuario;
using MiApp.Services.Service.GestionCorrespondencia.Firmas;
using MiApp.Services.Service.Usuario;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public sealed class ServiceSolicitaUsuarioPrincipalRespuestaTests
{
    [Fact]
    public async Task SolicitaUsuarioPrincipalRespuestaAsync_CuandoAliasEsVacio_RetornaValidacion()
    {
        var repository = new Mock<IRemitDestInternoR>();
        var service = new ServiceSolicitaUsuarioPrincipalRespuesta(repository.Object);

        var result = await service.SolicitaUsuarioPrincipalRespuestaAsync(10, 1, " ");

        Assert.False(result.success);
        Assert.Equal("DefaultDbAlias requerido", result.message);
        repository.Verify(
            r => r.SolicitaEstructuraIdUsuarioGestion(It.IsAny<int>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task SolicitaUsuarioPrincipalRespuestaAsync_CuandoUsuarioInactivo_RetornaNotFound()
    {
        var repository = new Mock<IRemitDestInternoR>();
        repository.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(10, "WF"))
            .ReturnsAsync(new AppResponses<RemitDestInterno>
            {
                success = true,
                message = "YES",
                data = BuildUsuario(10, 0, "Ana", "Analista"),
                errors = []
            });

        var service = new ServiceSolicitaUsuarioPrincipalRespuesta(repository.Object);
        var result = await service.SolicitaUsuarioPrincipalRespuestaAsync(10, 1, "WF");

        Assert.True(result.success);
        Assert.Null(result.data);
        Assert.Equal("not_found", result.meta?.Status);
    }

    [Fact]
    public async Task SolicitaUsuarioPrincipalRespuestaAsync_CuandoUsuarioActivo_MapeaDescripcion()
    {
        var repository = new Mock<IRemitDestInternoR>();
        repository.Setup(r => r.SolicitaEstructuraIdUsuarioGestion(10, "WF"))
            .ReturnsAsync(new AppResponses<RemitDestInterno>
            {
                success = true,
                message = "YES",
                data = BuildUsuario(10, 1, "Ana", "Analista"),
                errors = []
            });

        var service = new ServiceSolicitaUsuarioPrincipalRespuesta(repository.Object);
        var result = await service.SolicitaUsuarioPrincipalRespuestaAsync(10, 1, "WF");

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.Equal(10, result.data?.Id);
        Assert.Equal("Ana - Analista", result.data?.Descripcion);
        Assert.Equal("success", result.meta?.Status);
    }

    private static RemitDestInterno BuildUsuario(int id, int estado, string nombre, string cargo)
    {
        return new RemitDestInterno
        {
            Id_Remit_Dest_Int = id,
            Areas_Dep_Radicacion_Id_Areas_Dep = 1,
            Nombre_Remitente = nombre,
            Cargo_Remite = cargo,
            Login_Usuario = "login",
            Pasw_Usuario = "pass",
            Estado_Usuario = estado,
            Correo_Electronico = "a@a.com",
            Telefono_Usuario = "1",
            Firma_Usuario = [],
            Fecha_Creacion = null,
            Cambio_Clave = 0,
            Pasw_Encript = "enc",
            Empresa_Gestion_Documental_Id_Empresa = 1,
            Relacion_Workflow = 1,
            Id_Sedes_Empresa = 1,
            Relacion_Da = 1,
            Relacion_Workflow_Login = "wf",
            Relacion_Da_Login = "da",
            Relacion_Id_Usuario_Radicacion = 1,
            Relacion_Login_Radicacion = "rad",
            Estado_Usuario_Para_Gestion_Respuesta = 1,
            Estado_Usuario_Para_Gestion_Pqr = 1,
            Identificacion = "1",
            Direccion = "dir",
            Fecha_Limite_Acceso = null,
            Relacion_Workflow_Extend = 1,
            Relacion_Workflow_Login_Extend = "wfe",
            estado_radicacion_correspondencia = 1,
            estado_reasignacion_correspondencia = 1
        };
    }
}
