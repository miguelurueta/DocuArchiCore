using MiApp.Repositorio.Account;

namespace DocuArchiCore.Controllers.Account
{
    using MiApp.DTOs.DTOs.Account;
    using MiApp.Repository.ErrorController;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    public class ModulosController : Controller
    {
        private readonly IGestorModuloR _GestorModuloR;

        public ModulosController(IGestorModuloR gestorModuloR)
        {
            if (gestorModuloR == null)
            {
                throw new ArgumentNullException(nameof(gestorModuloR));
            }
            _GestorModuloR = gestorModuloR;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        public class IdEmpresaRequest
        {
            public int IdEmpresa { get; set; }
        }


        [HttpPost]
        public async Task<ActionResult> SolicitaModulosEmpresa(
    [FromBody] MiApp.DTOs.Account.EmpresaGestionDocumentalDto IdEmpresa)
        {
            AppResponse<List<ModuloDTO>> result;

            try
            {
                result = await _GestorModuloR.SolicitaModulosEmpresa(IdEmpresa.IdEmpresa);
                if (result != null)
                {
                    return new JsonResult(new AppResponse<object>
                    {
                        Success = result.Success,
                        Message = result.Message,
                        ErrorMessage = result.Success ? null : result.ErrorMessage,
                        Data = result.Data,
                        Meta = result.Meta
                    },
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNamingPolicy = null,
                        DictionaryKeyPolicy = null
                    });
                }
                else
                {
                    return new JsonResult(new AppResponse<object>
                    {
                        Success = false,
                        Message = "No se recibió respuesta del repositorio de módulos.",
                        ErrorMessage = "Error inesperado en SolicitaModulosEmpresa.",
                        Data = null
                    },
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNamingPolicy = null,
                        DictionaryKeyPolicy = null
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new AppResponse<object>
                {
                    Success = false,
                    Message = "Error al consultar los módulos de la empresa.",
                    ErrorMessage = "Excepción: " + ex.Message,
                    Errors = new List<object> { ex.Message }
                });
            }
        }

       
    }

}
