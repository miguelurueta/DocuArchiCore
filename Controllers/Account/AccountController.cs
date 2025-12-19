

using MiApp.DTOs.DTOs.Account;
using MiApp.Repository.ErrorController;
using MiApp.Repository.Repositorio.Account;
using MiApp.Services.Service.Account;
using MiApp.Services.Service.SessionHelper;
using Microsoft.AspNetCore.Mvc;


namespace DocuArchiCore.Controllers.Account
{

    public class AccountController : Controller
    {
        private readonly IInicioSesionL _InicioSesion;
        private readonly MiApp.Repository.Repositorio.Account.IEmpresaGestionDocumentalR _EmpresaGestionDocumentalR;
        private readonly ISesionActualCleaner sesionActualCleaner;
        public AccountController(IEmpresaGestionDocumentalR empresaGestionDocumentalR, IInicioSesionL inicioSesion, ISesionActualCleaner sesionActualCleaner)
        {
            _EmpresaGestionDocumentalR = empresaGestionDocumentalR;
            _InicioSesion = inicioSesion;
            this.sesionActualCleaner = sesionActualCleaner;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> SolicitaEstructuraEmpresa()
        {
            try
            {
                var result = await _EmpresaGestionDocumentalR.SolicitaEstructuraEmpresa();

                if (result != null)
                {
                    return Json(new AppResponse<object>
                    {
                        Success = result.Success,
                        Message = result.Message,
                        ErrorMessage = result.Message,
                        Data = result.Data,
                        Meta = result.Meta
                    });
                }
                else
                {
                    return Json(new AppResponse<object>
                    {
                        Success = false,
                        Message = "Error inesperado al consultar usuario",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new AppResponse<object>
                {
                    Success = false,
                    Message = "Excepción: " + ex.Message,
                    Errors = new List<string> { ex.Message },
                    ErrorMessage = "Inconsistencia general SolicitaEstructuraEmpresa " + ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<ActionResult> ValidaUserAplicacion([FromBody] ValidaUsuarioDTO ValidaUsuarioDTO)
        {
            try
            {
                var errores = new List<ValidationError>();
                // ✅ Validaciones básicas
                if (ValidaUsuarioDTO.IdEmpresa == 0)
                {
                    return Json(new AppResponse<object>
                    {
                        Success = false,
                        Message = "El sistema no detecta la selección de una entidad o empresa",
                        ErrorMessage = "Entidad no seleccionada"
                    });
                }

                if (ValidaUsuarioDTO.IdModulo == 0)
                    errores.Add(ValidationHelper.CreateError("selectModulos", "Debe seleccionar el módulo de ingreso.", "Required",    ValidaUsuarioDTO.IdModulo));

                if (string.IsNullOrWhiteSpace(ValidaUsuarioDTO.User))
                    errores.Add(ValidationHelper.CreateError("usuario", "Debe informar el campo usuario.", "Required", ValidaUsuarioDTO.User));

                if (string.IsNullOrWhiteSpace(ValidaUsuarioDTO.Pasword))
                    errores.Add(ValidationHelper.CreateError("password", "Debe informar el campo password.", "Required", ValidaUsuarioDTO.Pasword));

                if (errores.Any())
                {
                    return Json(ValidationHelper.BuildValidationErrorResponse(
                        "Hay errores de validación en el formulario.",
                        errores.ToArray()
                    ));
                }

                // ✅ Validar credenciales con el servicio
                var ResultValidaUser = await _InicioSesion.ValidaUserAplicacion(ValidaUsuarioDTO.IdEmpresa, ValidaUsuarioDTO.IdModulo, ValidaUsuarioDTO.User, ValidaUsuarioDTO.Pasword);

                if (!ResultValidaUser.Success)
                {
                    return Json(new AppResponse<object>
                    {
                        Success = false,
                        Message = ResultValidaUser.Message,
                        ErrorMessage = ResultValidaUser.ErrorMessage,
                        Data = ResultValidaUser.Data
                    });
                }

                // ============================================
                //     🧩 Consultar estructura de empresa
                // ============================================
                var RestEmpresa = await _EmpresaGestionDocumentalR.SolicitaEstructuraEmpresaPorId(ValidaUsuarioDTO.IdEmpresa);

                if (!RestEmpresa.Success)
                {
                    return Json(RestEmpresa);
                }

                // ✔ INICIAR SESIÓN SOLO AQUÍ
                HttpContext.Session.SetString("SesionActiva", "true");
                HttpContext.Session.SetString("Sesion_UltimoAcceso", DateTime.UtcNow.ToString("o"));

                await HttpContext.Session.CommitAsync(); // forza escritura
                return Json(new AppResponse<object>
                {
                    Success = true,
                    Message = "YES",
                    ErrorMessage = "YES",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return Json(new AppResponse<object>
                {
                    Success = false,
                    Message = "Excepción: " + ex.Message,
                    ErrorMessage = "Inconsistencia general en ValidaUserAplicacion: " + ex.Message
                });
            }
        }

    }
}
