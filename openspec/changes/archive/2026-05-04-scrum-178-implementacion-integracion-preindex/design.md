## Context

- Jira issue key: SCRUM-178
- Jira summary: IMPLEMENTACION-INTEGRACION-PREINDEX
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-178

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE PROMPT 12 — Integración completa de Preindex (LECTURA + NORMALIZACIÓN + USO REAL) (FASE CRÍTICA — PARIDAD FUNCIONAL LEGACY) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actúa como Arquitecto Master Backend .NET experto en: sistemas documentales legacy (DocuArchi) preindexación documental (TXT/XMLS) normalización de datos de entrada pipelines de ingestión Clean Architecture validación estructural avanzada migración VB → C# con paridad exacta ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Restaurar la paridad funcional completa del Preindex legacy , asegurando: ✔ Lectura real de archivos preindex (TXT/XMLS) ✔ Construcción del nombre de archivo legacy ✔ Interpretación correcta de contenido ✔ Integración real con campos de indexación ✔ Alineación con metadata de gabinete (PROMPT 11) ✔ Uso efectivo en persistencia (PROMPT 7) Este prompt elimina la brecha crítica detectada: Preindex se lee pero NO se usa ❌ Y la convierte en: Preindex es fuente primaria de datos ✔ CONTEXTO LEGACY RUTA : D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\Architecture\Almacenamiento\funcion-almacena-consolidad.txt En VB: Contruye_Nombre_Archvio_Index(...)
Leer_Archivo_Preindex(...) El flujo: 1. Construye nombre archivo preindex
2. Busca .xmls o .txt
3. Lee contenido
4. Llena matriz de datos
5. Se usa en validación y persistencia UBICACIÓN MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Preindex/ Archivos: IStoragePreindexResolver.cs
StoragePreindexResolver.cs

IStoragePreindexReader.cs
StoragePreindexReader.cs

IStoragePreindexIntegrator.cs
StoragePreindexIntegrator.cs ================================ MODELOS ================================ StoragePreindexFile public sealed class StoragePreindexFile
{
    public required string RutaCompleta { get; init; }
    public required string Extension { get; init; }
} StoragePreindexResult public sealed class StoragePreindexResult
{
    public required List<string> Valores { get; init; }
} ================================ INTERFACES ================================ IStoragePreindexResolver public interface IStoragePreindexResolver
{
    StoragePreindexFile Resolve(
        string rutaTemporal,
        string nombreDocumento
    );
} IStoragePreindexReader public interface IStoragePreindexReader
{
    StoragePreindexResult Read(StoragePreindexFile file);
} IStoragePreindexIntegrator public interface IStoragePreindexIntegrator
{
    void Integrate(
        StorageContext context,
        StoragePreindexResult preindex
    );
} ================================ IMPLEMENTACIÓN — RESOLVER ================================ public sealed class StoragePreindexResolver : IStoragePreindexResolver
{
    public StoragePreindexFile Resolve(string ruta, string nombre)
    {
        var baseName = nombre;

        var xmls = Path.Combine(ruta, baseName + ".xmls");
        var txt = Path.Combine(ruta, baseName + ".txt");

        if (File.Exists(xmls))
        {
            return new StoragePreindexFile
            {
                RutaCompleta = xmls,
                Extension = ".xmls"
            };
        }

        if (File.Exists(txt))
        {
            return new StoragePreindexFile
            {
                RutaCompleta = txt,
                Extension = ".txt"
            };
        }

        throw new InvalidOperationException("Documento sin preindex");
    }
} ================================ IMPLEMENTACIÓN — READER ================================ public sealed class StoragePreindexReader : IStoragePreindexReader
{
    public StoragePreindexResult Read(StoragePreindexFile file)
    {
        if (!File.Exists(file.RutaCompleta))
            throw new FileNotFoundException("Preindex no encontrado");

        var lines = File.ReadAllLines(file.RutaCompleta);

        var values = new List<string>();

        foreach (var line in lines)
        {
            values.Add(line.Trim());
        }

        return new StoragePreindexResult
        {
            Valores = values
        };
    }
} ================================ IMPLEMENTACIÓN — INTEGRATOR ================================ public sealed class StoragePreindexIntegrator : IStoragePreindexIntegrator
{
    public void Integrate(StorageContext context, StoragePreindexResult preindex)
    {
        if (context.Command.Campos == null)
            throw new InvalidOperationException("Campos no definidos");

        if (preindex.Valores.Count != context.Command.Campos.Count)
            throw new InvalidOperationException("Preindex no coincide con metadata");

        for (int i = 0; i < context.Command.Campos.Count; i++)
        {
            var campo = context.Command.Campos[i];

            if (string.IsNullOrWhiteSpace(campo.Valor))
            {
                campo = campo with
                {
                    Valor = preindex.Valores[i]
                };

                context.Command.Campos[i] = campo;
            }
        }
    }
} ================================ INTEGRACIÓN EN PIPELINE ================================ Agregar validator: public sealed class PreindexValidator : BaseStorageValidator
{
    private readonly IStoragePreindexResolver _resolver;
    private readonly IStoragePreindexReader _reader;
    private readonly IStoragePreindexIntegrator _integrator;

    public PreindexValidator(
        IStoragePreindexResolver resolver,
        IStoragePreindexReader reader,
        IStoragePreindexIntegrator integrator)
    {
        _resolver = resolver;
        _reader = reader;
        _integrator = integrator;
    }

    public override async Task ValidateAsync(StorageContext context, List<StorageError> errors)
    {
        try
        {
            if (context.Command.TipoAlmacenamiento != TipoAlmacenamientoEnum.BatchPreindex)
                return;

            var file = _resolver.Resolve(
                context.Command.RutaTemporalId,
                context.Command.NombreDocumento
            );

            var data = _reader.Read(file);

            _integrator.Integrate(context, data);
        }
        catch (Exception ex)
        {
            errors.Add(new StorageError
            {
                Code = "PREINDEX_ERROR",
                Message = ex.Message
            });
        }

        await Task.CompletedTask;
    }
} ================================ DEPENDENCY INJECTION ================================ services.AddScoped<IStoragePreindexResolver, StoragePreindexResolver>();
services.AddScoped<IStoragePreindexReader, StoragePreindexReader>();
services.AddScoped<IStoragePreindexIntegrator, StoragePreindexIntegrator>();
services.AddScoped<IStorageValidator, PreindexValidator>(); ================================ REGLAS CRÍTICAS ================================ Solo aplica a BatchPreindex Preindex debe existir Cantidad de valores debe coincidir No sobrescribir valores manuales Validar contra metadata (PROMPT 11) Integrarse antes de persistencia ================================ PRUEBAS ================================ Unitarias: archivo inexistente → error cantidad mismatch → error integración correcta no sobrescribe valores existentes Integración: preindex real TXT preindex real XMLS flujo completo con persistencia ================================ CRITERIOS DE ACEPTACIÓN ================================ ✔ Preindex leído correctamente ✔ Integrado en pipeline ✔ Valores usados en persistencia ✔ Paridad con VB ✔ Validación estricta ================================ INSTRUCCIÓN FINAL ================================ Implementar completamente: ✔ Resolver ✔ Reader ✔ Integrator ✔ Validator ✔ DI Listo para: 👉 PROMPT 13 — Naming legacy (DIG + ceros + extensión)

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-178-implementacion-integracion-preindex.