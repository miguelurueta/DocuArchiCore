
using MiApp.Repository.ErrorController;
using MiApp.Repository.Repositorio.Account;
using Microsoft.AspNetCore.Mvc;

namespace DocuArchiCore.Controllers.Account
{
    public class AccountController : Controller
    {
        private readonly MiApp.Repository.Repositorio.Account.IEmpresaGestionDocumentalR _EmpresaGestionDocumentalR;

        public AccountController(IEmpresaGestionDocumentalR empresaGestionDocumentalR)
        {
            _EmpresaGestionDocumentalR = empresaGestionDocumentalR;
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

    }
}
