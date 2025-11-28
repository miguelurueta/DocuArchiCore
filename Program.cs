using DocuArchiCore.Repositorio.Account;
using MiApp.Repository.DataAccess;
using MiApp.Repository.Repositorio.Account;
using MiApp.Repository.Repositorio.DataAccess;
using Microsoft.AspNetCore.Connections;

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
//IEmpresaGestionDocumentalR

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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();
