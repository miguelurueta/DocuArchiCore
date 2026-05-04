## Context

- Jira issue key: SCRUM-177
- Jira summary: ACTUALIZACION-CAMPOS-OBLIGATORIOA
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-177

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE PROMPT 11 — Paridad Legacy de Metadata Gabinete + Campos Obligatorios (FASE CRÍTICA — BLOQUEANTE FUNCIONAL) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actúa como Arquitecto Master Backend .NET experto en: sistemas documentales legacy (DocuArchi) metadata dinámica por gabinete Dapper / QueryOptions validación de campos obligatorios integración preindex consistencia de datos antes de persistencia migración funcional exacta VB → C# ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Restaurar la paridad funcional del comportamiento legacy VB en: ✔ Consulta real de metadata del gabinete ( DETALLE_GABIENETE ) ✔ Obtención de campos obligatorios ✔ Validación contra preindex ✔ Alineación entre metadata y valores ✔ Integración de valores en el flujo de persistencia Este prompt es BLOQUEANTE para: Inserción en gabinete (Prompt 7) Preindex (Prompt 12) Validación (Prompt 4) Orchestrator completo (Prompt 10) CONTEXTO TABLAS DETALLE_GABIENETE Ruta : D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\DataModel\StorageEngine-Tables.md CONTEXTO LEGACY En VB: Consulta_Campos_Obligatorio(_Nombre_Gabienete, Matri_Campos_Obli) Ruta contexto: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\Architecture\Almacenamiento\funcion-almacena-consolidad.txt Validaba: - existencia de campos
- orden exacto
- obligatoriedad
- alineación con matriz de datos Errores típicos: "Matri Campos es nula"
"Las matrices no coinciden"
"Campo obligatorio vacío" UBICACIÓN MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Metadata/ Archivos: IStorageGabineteMetadataProvider.cs
StorageGabineteMetadataProvider.cs

IStorageRequiredFieldsValidator.cs
StorageRequiredFieldsValidator.cs Repositorio: MiApp.Repository/Repositorio/GestorDocumental/AlmacenamientoDocumental/GabineteMetadata/ ================================ MODELOS ================================ GabineteCampoMetadata public sealed class GabineteCampoMetadata
{
    public required string NombreCampo { get; init; }
    public required bool EsObligatorio { get; init; }
    public required int Orden { get; init; }
} GabineteMetadataResult public sealed class GabineteMetadataResult
{
    public required List<GabineteCampoMetadata> Campos { get; init; }
} ================================ INTERFACES ================================ IStorageGabineteMetadataProvider public interface IStorageGabineteMetadataProvider
{
    Task<GabineteMetadataResult> GetMetadataAsync(
        string nombreGabinete,
        string defaultDbAlias
    );
} IStorageRequiredFieldsValidator public interface IStorageRequiredFieldsValidator
{
    void Validate(
        List<GabineteCampoMetadata> metadata,
        List<CampoIndexacionDto> camposEntrada
    );
} ================================ REPOSITORIO (CONSULTA REAL) ================================ IStorageGabineteMetadataRepository public interface IStorageGabineteMetadataRepository
{
    Task<List<GabineteCampoMetadata>> GetCamposAsync(
        string nombreGabinete,
        string alias
    );
} IMPLEMENTACIÓN public sealed class StorageGabineteMetadataRepository : IStorageGabineteMetadataRepository
{
    private readonly IDapperCrudEngine _engine;

    public StorageGabineteMetadataRepository(IDapperCrudEngine engine)
    {
        _engine = engine;
    }

    public async Task<List<GabineteCampoMetadata>> GetCamposAsync(
        string nombreGabinete,
        string alias)
    {
        var query = new QueryOptions
        {
            TableName = "gabinete_detalle",
            DefaultAlias = alias,
            Columns = new List<string>
            {
                "NOMBRE_CAMPO",
                "OBLIGATORIO",
                "ORDEN"
            },
            Filters = new Dictionary<string, object>
            {
                { "NOMBRE_GABINETE", nombreGabinete }
            }
        };

        var result = await _engine.GetAllAsync<GabineteCampoMetadata>(query);

        if (!result.Success)
            throw new Exception(result.ErrorMessage);

        return result.Data.ToList();
    }
} ================================ PROVIDER ================================ public sealed class StorageGabineteMetadataProvider : IStorageGabineteMetadataProvider
{
    private readonly IStorageGabineteMetadataRepository _repository;

    public StorageGabineteMetadataProvider(IStorageGabineteMetadataRepository repository)
    {
        _repository = repository;
    }

    public async Task<GabineteMetadataResult> GetMetadataAsync(
        string nombreGabinete,
        string alias)
    {
        var campos = await _repository.GetCamposAsync(nombreGabinete, alias);

        if (campos == null || campos.Count == 0)
            throw new InvalidOperationException("No existe metadata para gabinete");

        return new GabineteMetadataResult
        {
            Campos = campos.OrderBy(c => c.Orden).ToList()
        };
    }
} ================================ VALIDADOR DE CAMPOS ================================ public sealed class StorageRequiredFieldsValidator : IStorageRequiredFieldsValidator
{
    public void Validate(
        List<GabineteCampoMetadata> metadata,
        List<CampoIndexacionDto> camposEntrada)
    {
        if (metadata == null)
            throw new InvalidOperationException("Metadata nula");

        if (camposEntrada == null)
            throw new InvalidOperationException("Campos entrada nulos");

        if (metadata.Count != camposEntrada.Count)
            throw new InvalidOperationException("Cantidad de campos no coincide con metadata");

        for (int i = 0; i < metadata.Count; i++)
        {
            var meta = metadata[i];
            var input = camposEntrada[i];

            if (!string.Equals(meta.NombreCampo, input.NombreCampo, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Desalineación campo: {meta.NombreCampo}");

            if (meta.EsObligatorio && string.IsNullOrWhiteSpace(input.Valor))
                throw new InvalidOperationException($"Campo obligatorio vacío: {meta.NombreCampo}");
        }
    }
} ================================ INTEGRACIÓN EN VALIDATION PIPELINE ================================ Agregar nuevo validator: public sealed class GabineteRequiredFieldsValidator : BaseStorageValidator
{
    private readonly IStorageGabineteMetadataProvider _provider;
    private readonly IStorageRequiredFieldsValidator _validator;

    public GabineteRequiredFieldsValidator(
        IStorageGabineteMetadataProvider provider,
        IStorageRequiredFieldsValidator validator)
    {
        _provider = provider;
        _validator = validator;
    }

    public override async Task ValidateAsync(StorageContext context, List<StorageError> errors)
    {
        try
        {
            var metadata = await _provider.GetMetadataAsync(
                context.Command.NombreGabinete,
                context.DefaultDbAlias
            );

            _validator.Validate(metadata.Campos, context.Command.Campos.ToList());
        }
        catch (Exception ex)
        {
            errors.Add(new StorageError
            {
                Code = "GABINETE_METADATA",
                Message = ex.Message
            });
        }
    }
} ================================ DEPENDENCY INJECTION ================================ services.AddScoped<IStorageGabineteMetadataRepository, StorageGabineteMetadataRepository>();
services.AddScoped<IStorageGabineteMetadataProvider, StorageGabineteMetadataProvider>();
services.AddScoped<IStorageRequiredFieldsValidator, StorageRequiredFieldsValidator>();
services.AddScoped<IStorageValidator, GabineteRequiredFieldsValidator>(); ================================ REGLAS CRÍTICAS ================================ Orden de campos debe coincidir exactamente Nombre de campo debe coincidir Cantidad debe coincidir Obligatorios deben validarse No permitir campos adicionales No permitir campos faltantes No confiar en frontend Siempre consultar DB ================================ PRUEBAS ================================ Unitarias: metadata null → error metadata vacío → error campos entrada null → error desalineación → error campo obligatorio vacío → error validación OK Integración: consulta real gabinete_detalle orden correcto validación contra datos reales ================================ CRITERIOS DE ACEPTACIÓN ================================ ✔ Metadata real desde DB ✔ Validación exacta como VB ✔ Orden respetado ✔ Campos obligatorios validados ✔ Integrado al pipeline ✔ Sin placeholders ================================ INSTRUCCIÓN FINAL ================================ Implementar: ✔ Consulta metadata gabinete ✔ Validación obligatoria ✔ Integración pipeline Listo para: 👉 PROMPT 12 — Integración completa de Preindex

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-177-actualizacion-campos-obligatorioa.