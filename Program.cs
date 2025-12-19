using DocuArchiCore.Abstractions.Security;
using DocuArchiCore.Infrastructure.Security;
using MiApp.DTOs.DTOs.Account;
using MiApp.Repositorio.Account;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.Account;
using MiApp.Repository.Repositorio.DataAccess;
using MiApp.Repository.Repositorio.Docuarchi.Grupo;
using MiApp.Repository.Repositorio.Docuarchi.Usuario;
using MiApp.Repository.Repositorio.GestorDocumental.usuario;
using MiApp.Repository.Repositorio.Home.Menu;
using MiApp.Repository.Repositorio.Radicador.PlantillaRadicado;
using MiApp.Repository.Repositorio.Radicador.PlantillaValidacion;
using MiApp.Repository.Repositorio.Radicador.Tramite;
using MiApp.Repository.Repositorio.Radicador.Usuario;
using MiApp.Repository.Repositorio.Workflow.Eventos;
using MiApp.Repository.Repositorio.Workflow.Grupo;
using MiApp.Repository.Repositorio.Workflow.usuario;
using MiApp.Repository.Repositorio.Workflow.Usuario;
using MiApp.Services.Service.Account;
using MiApp.Services.Service.Crypto;
using MiApp.Services.Service.DateHelper;
using MiApp.Services.Service.Docuarchi.Inicio;
using MiApp.Services.Service.Docuarchi.Usuario;
using MiApp.Services.Service.General;
using MiApp.Services.Service.GestorDocumental.Inicio;
using MiApp.Services.Service.Home.Menu;
using MiApp.Services.Service.Radicacion.Inicio;
using MiApp.Services.Service.Radicacion.PlantillaRadicado;
using MiApp.Services.Service.SessionHelper;
using MiApp.Services.Service.Usuario;
using MiApp.Services.Service.Workflow.Inicio;
using MiApp.Services.Service.Workflow.Usuario;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();
// Agregar servicio en Program.cs
builder.Services.AddScoped<IGestorModuloR, GestorModuloR>();
// Asegúrate de registrar las dependencias adicionales
builder.Services.AddScoped<IDapperCrudEngine, DapperCrudEngine>();  // O el nombre de la implementación concreta
builder.Services.AddScoped<IEntidadBuilder, EntidadBuilder>();      // O el nombre de la implementación concreta
builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactoryImpl>();
builder.Services.AddScoped<IEmpresaGestionDocumentalR, EmpresaGestionDocumentalR>();
builder.Services.AddScoped<IGenerycService, GenerycService>();
builder.Services.AddScoped<ICryptoHelper, CryptoHelper>();
builder.Services.AddScoped<IDateHelper, DateHelper>();
builder.Services.AddScoped<IInicioSesionL, InicioSesionL>();
builder.Services.AddScoped<IRemitDestInternoR, RemitDestInternoR>();
builder.Services.AddScoped<IRemitDestInternoL, RemitDestInternoL>();
builder.Services.AddScoped<IRemitDestInternoPerfilProduccionR, RemitDestInternoPerfilProduccionR>();
builder.Services.AddScoped<IIpHelper, IpHelperL>();
builder.Services.AddScoped<IInicioModuloGestorL, InicioModuloGestorL>();
builder.Services.AddScoped<IIncioModuloWorkflowL, IncioModuloWorkflowL>();
builder.Services.AddScoped<IPermisosUsuarioWorkflowR, PermisosUsuarioWorkflowR>();
builder.Services.AddScoped<IWfPerfilDiagramadorR, WfPerfilDiagramadorR>();
builder.Services.AddScoped<IUsuarioWorkflowR, UsuarioWorkflowR>();
builder.Services.AddScoped<IIntervaloAlarmasUsuarioR, IntervaloAlarmasUsuarioR>();
builder.Services.AddScoped<IGruposWorkflowR, GruposWorkflowR>();
builder.Services.AddScoped<IScriptActividadesR, ScriptActividadesR>();
builder.Services.AddScoped<ILogUsuarioR, LogUsuarioR>();
builder.Services.AddScoped<IInicioModuloDocuarchiL, InicioModuloDocuarchiL>();
builder.Services.AddScoped<IInicioModuloRadicacionL, InicioModuloRadicacionL>();
builder.Services.AddScoped<IUsuariosDAL, UsuariosDAL>();
builder.Services.AddScoped<IUsuariosDAR, UsuariosDAR>();
builder.Services.AddScoped<ILogUsuarioDR, LogUsuarioDR>();
builder.Services.AddScoped<IRelacionUsuGrupR, RelacionUsuGrupR>();
builder.Services.AddScoped<IPerfilarUsuarioRadicadorR, PerfilarUsuarioRadicadorR>();
builder.Services.AddScoped<IUsuarioWorkflowL, UsuarioWorkflowL>();
builder.Services.AddScoped<IPermisosPlantillaR, PermisosPlantillaR>();
builder.Services.AddScoped<IMenuR, MenuR>();
builder.Services.AddScoped<IMenuL, MenuL>();
builder.Services.AddScoped<ISessionHelperService, SessionHelperService>();
builder.Services.AddScoped<ISesionActualCleaner, SesionActualCleaner>();
builder.Services.AddScoped<IDetallePlantillaRadicadoR, DetallePlantillaRadicadoR>();
builder.Services.AddScoped<IPermisosPlantillaR, PermisosPlantillaR>();
builder.Services.AddScoped<IRaRadEstadosModuloRadicacionR, RaRadEstadosModuloRadicacionR>();
builder.Services.AddScoped<IRaRadTipoRadicacionR, RaRadTipoRadicacionR>();
builder.Services.AddScoped<IRaScriptActividadesR, RaScriptActividadesR>();
builder.Services.AddScoped<IRaValoresCamposSeleccionPlantillaRadicadoR, RaValoresCamposSeleccionPlantillaRadicadoR>();
builder.Services.AddScoped<ICamposPlantillaValidacionR, CamposPlantillaValidacionR>();
builder.Services.AddScoped<IPlantillaValidacionR, PlantillaValidacionR>();
builder.Services.AddScoped<IRelacionScriptPlantillaR, RelacionScriptPlantillaR>();
builder.Services.AddScoped<IRelCamposValRadicR, RelCamposValRadicR>();
builder.Services.AddScoped<IRaRestriRelacionTramiteR, RaRestriRelacionTramiteR>();
builder.Services.AddScoped<ITipoDocEntranteR, TipoDocEntranteR>();
builder.Services.AddScoped<ISystemPlantillaRadicadoR, SystemPlantillaRadicadoR>();
builder.Services.AddScoped<IPlantillaRadicacionL, PlantillaRadicacionL>();
// Sesión   
// ===========================================================
// SESIÓN — REGISTRO DE TODAS LAS INTERFACES
// ===========================================================
builder.Services.AddHttpContextAccessor();

// La clase concreta
builder.Services.AddScoped<SesionActual>();

// Todas las interfaces redirigen a la misma SesionActual
builder.Services.AddScoped<ISesionActual>(sp =>
    sp.GetRequiredService<SesionActual>());

builder.Services.AddScoped<ISesionGeneral>(sp =>
    sp.GetRequiredService<SesionActual>());

builder.Services.AddScoped<ISesionDocuArchi>(sp =>
    sp.GetRequiredService<SesionActual>());

builder.Services.AddScoped<ISesionGestionDocumental>(sp =>
    sp.GetRequiredService<SesionActual>());

builder.Services.AddScoped<ISesionRadicacion>(sp =>
    sp.GetRequiredService<SesionActual>());

builder.Services.AddScoped<ISesionWorkflow>(sp =>
    sp.GetRequiredService<SesionActual>());

// ===========================================================
// SESSION
// ===========================================================


var sessionConfig = new SessionConfigDTO();
builder.Configuration.GetSection("SessionConfig").Bind(sessionConfig);

// Valor por defecto si no viene por configuración
if (sessionConfig.IdleTimeoutMinutes <= 0)
    sessionConfig.IdleTimeoutMinutes = 20;

// Registrar DTO en DI
builder.Services.AddSingleton(sessionConfig);
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(sessionConfig.IdleTimeoutMinutes);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});
// CONFIGURAR SESIÓN — VERSIÓN FINAL
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromMinutes(sessionConfig.IdleTimeoutMinutes);
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;

//    // 🔥 MUY IMPORTANTE PARA ASP.NET CORE 8/9
//    // Garantiza que el middleware escriba la sesión SIEMPRE.
//    options.IOTimeout = TimeSpan.FromSeconds(10);

//    // Opcional: evita compresión en cookies (más rápido)
//    options.Cookie.SameSite = SameSiteMode.Lax;
//});

// Configurar JSON para mantener PascalCase en lugar de camelCase
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Esto mantiene PascalCase
    });
if (args.Contains("--generate-jsdoc") || args.Contains("generate-jsdoc"))
{
    var output = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "js", "dtos.js");
    DocuArchiCore.Tools.DtoJsDocGenerator.DtoJsDocGenerator.Generate(output);
    Console.WriteLine("DTO JSDoc generado en: " + output);
    return;
}


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();          // <-- ✔ Lugar correcto para que funcione


app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();
