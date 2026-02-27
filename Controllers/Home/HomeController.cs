using DocuArchiCore.Abstractions.Security;
using MiApp.DTOs.DTOs.Account;
using MiApp.Repository.ErrorController;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.Home.Menu;
using MiApp.Services.Service.Account;
using MiApp.Services.Service.Home.Menu;
using MiApp.Services.Service.SessionHelper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.Json;
[ApiController]
[Route("[controller]/[action]")]
public class HomeController : Controller
{
    private readonly IMenuR _menuR;
    private readonly IMenuL menuL;
    private readonly ISesionActual _sesionActual;
    private readonly ISesionGeneral sesionGeneral;
    private readonly IInicioSesionL inicioSesionL;
    private readonly ISessionHelperService sessionHelperService;
    private readonly SessionConfigDTO _sessionConfig;
    public HomeController(IMenuR menuR, ISesionActual sesionActual, IMenuL menuL, IInicioSesionL inicioSesionL, ISessionHelperService sessionHelperService, ISesionGeneral sesionGeneral, SessionConfigDTO sessionConfig)
    {
        _menuR = menuR;
        _sesionActual = sesionActual;
        this.menuL = menuL;
        this.inicioSesionL = inicioSesionL;
        this.sessionHelperService = sessionHelperService;
        this.sesionGeneral = sesionGeneral;
        _sessionConfig = sessionConfig;
    }

    [HttpGet]
    public IActionResult Home()
    {
        return View();
    }
    // 🔑 AQUÍ VA, NO DENTRO DEL MÉTODO
    private static JsonSerializerOptions GetPascalCaseJsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            DictionaryKeyPolicy = null
        };
    }
    [HttpGet]
    public async Task<IActionResult> ServiceSolicitaEstructuraMenuPrincipals()
    {
        try
        {
            string tipo = _sesionActual.TIPOMODULO switch
            {
                "WORKFLOW DOCUMENTAL" => "WF",
                "GESTOR DOCUMENTAL" => "",
                "DOCUARCHI CONTENEDOR" => "DA",
                _ => ""
            };

            var result = await _menuR.SolicitaEstructuraMenuPrincipal(
                tipo,
                _sesionActual.DefaultDbAlias
            );

            if (result == null || result.Success != true || result.Data == null)
            {
                var errorResponse = new AppResponse<object>
                {
                    Success = false,
                    Message = result?.Message ?? "No se pudo obtener la estructura del menú principal.",
                    ErrorMessage = result?.ErrorMessage ?? result?.Message ?? "DATA_NULL",
                    Data = null
                };

                return new JsonResult(errorResponse, GetPascalCaseJsonOptions());
            }

            var restMenu = await menuL.FiltraEstructuraPermisosMenuPrincipal(
                _sesionActual.TIPOMODULO,
                0,
                result.Data,
                _sesionActual.DefaultDbAlias
            );

            var response = new AppResponse<object>
            {
                Success = restMenu.Success,
                Message = restMenu.Message,
                ErrorMessage = restMenu.Message,
                Data = restMenu.Data,
                Meta = restMenu.Meta
            };

            return new JsonResult(response, GetPascalCaseJsonOptions());
        }
        catch (Exception ex)
        {
            var errorResponse = new AppResponse<object>
            {
                Success = false,
                Message = "Excepción: " + ex.Message,
                Errors = new List<object> { ex.Message },
                ErrorMessage = "Inconsistencia general SolicitaEstructuraEmpresa " + ex.Message
            };

            return new JsonResult(errorResponse, GetPascalCaseJsonOptions());
        }
    }

    

    [HttpPost]
    public async Task<IActionResult> ServiceSolicitaCaraterizacionUsuarioLogueado()
    {
        try
        {
            var result = await inicioSesionL
                .SolicitaCaraterizacionUsuarioLogueado(_sesionActual.TIPOMODULO);

            // 👉 Instanciación correcta del convertidor
            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
            {
                new JsonDateConverter()   // 👈 Aquí se instancia
            },
                Formatting = Formatting.None
            };

            var json = JsonConvert.SerializeObject(
                new AppResponse<object>
                {
                    Success = result.Success,
                    Message = result.Message,
                    ErrorMessage = result.ErrorMessage,
                    Data = result.Data,
                    Meta = result.Meta,
                    AuxData = result.AuxData
                },
                settings
            );

            return Content(json, "application/json");
        }
        catch (Exception ex)
        {
            return Json(new AppResponse<object>
            {
                Success = false,
                Message = "Excepción: " + ex.Message,
                ErrorMessage = "Inconsistencia general ServiceSolicitaCaraterizacionUsuarioLogueado " + ex.Message,
                Errors = new List<object> { ex.Message }
            });
        }
    }
    // -------------------------------------------------------------
    // 🔁 1. KEEPALIVE – RENUEVA LA SESIÓN POR ACTIVIDAD REAL
    // -------------------------------------------------------------
    [HttpPost]
    public IActionResult KeepAlive()
    {
        if (!HttpContext.Session.IsAvailable)
        {
            return Json(new { success = false, message = "Sesión no disponible" });
        }

        var now = DateTime.UtcNow;

        // Marca de último acceso
        HttpContext.Session.SetString("Sesion_UltimoAcceso", now.ToString("o"));

        // Contador solo para debug
        var counter = HttpContext.Session.GetInt32("KeepAlive_Counter") ?? 0;
        counter++;
        HttpContext.Session.SetInt32("KeepAlive_Counter", counter);

        return Json(new
        {
            success = true,
            counter,
            sessionId = HttpContext.Session.Id,
            lastAccess = now.ToString("o")
        });
    }

    // -------------------------------------------------------------
    // ⏱ 2. TIEMPO RESTANTE – FUENTE OFICIAL PARA EL FRONT
    // -------------------------------------------------------------
    [HttpPost]
    public IActionResult TiempoRestante()
    {
        if (!HttpContext.Session.IsAvailable)
            return Json(new { restanteSegundos = 0 });

        var raw = HttpContext.Session.GetString("Sesion_UltimoAcceso");

        if (string.IsNullOrEmpty(raw))
            return Json(new { restanteSegundos = 0 });

        if (!DateTime.TryParse(raw, null, DateTimeStyles.RoundtripKind, out var last))
            return Json(new { restanteSegundos = 0 });

        var elapsed = (DateTime.UtcNow - last).TotalSeconds;
        var total = _sessionConfig.IdleTimeoutMinutes * 60;
        var restante = total - elapsed;

        if (restante < 0) restante = 0;

        return Json(new { restanteSegundos = (int)restante });
    }

    // -------------------------------------------------------------
    // ✅ 3. VALIDAR SESIÓN (USADO POR SessionManager.js)
    // -------------------------------------------------------------
    [HttpPost]
    public async Task<IActionResult> ServiceIsSessionTimedOut()
    {
        var result = await sessionHelperService.IsSessionTimedOut();
        return Json(result);
    }

    // -------------------------------------------------------------
    // 🚪 4. CERRAR SESIÓN DESDE EL FRONT
    // -------------------------------------------------------------
    [HttpPost]
    public async Task<IActionResult> ServiceCerrarSesion()
    {
        var result = await sessionHelperService.CerrarSesion();
        return Json(result);
    }
}

