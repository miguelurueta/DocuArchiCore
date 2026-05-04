## Context

- Jira issue key: SCRUM-179
- Jira summary: IMPLEMENTACION-RESOLVE-NOMBRE-ARCHIVO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-179

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE PROMPT 13 — Naming Legacy (DIG + ceros + extensión + FXL XML) (FASE CRÍTICA — PARIDAD FUNCIONAL LEGACY) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actúa como Arquitecto Master Backend .NET experto en: sistemas documentales legacy DocuArchi nomenclatura documental (naming conventions) mapeo de extensiones por tipo documental generación determinística de nombres Clean Architecture migración exacta VB → C# ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Restaurar la nomenclatura legacy exacta utilizada en DocuArchi: ✔ Nombre de archivo principal: DIG + ceros + IdAlmacen + extensión ✔ Generación de ceros (padding) ✔ Resolución de extensión por tipo documental (DA_EXTENSION) ✔ Nombre XML: FXL + ceros + IdAlmacen + .xml ✔ Segundo nombre documental consistente ✔ Uso en FileSystem + XML + índice electrónico Este prompt elimina la desviación actual: alm_{id}.{ext} ❌ y la reemplaza por: DIG000001234.pdf ✔
FXL000001234.xml ✔ CONTEXTO TABLAS: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\DataModel\StorageEngine-Tables.md CONTEXTO LEGACY RUTACONTEXTO:D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\Architecture\Almacenamiento\funcion-almacena-consolidad.txt Funciones VB: Ceros_Imagen_Almacenada(...)
RetornaExtensionTipoDocumento(...)
Generando_Archivo_Dat_Xml(...) Reglas: - prefijo DIG para archivos
- prefijo FXL para XML
- padding con ceros a longitud fija
- extensión basada en tipo documental UBICACIÓN MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Naming/ Archivos: IStorageNamingService.cs
StorageNamingService.cs

IStorageExtensionResolver.cs
StorageExtensionResolver.cs Repositorio: MiApp.Repository/Repositorio/GestorDocumental/AlmacenamientoDocumental/Extension/ ================================ MODELOS ================================ StorageNamingResult public sealed class StorageNamingResult
{
    public required string NombreArchivoPrincipal { get; init; }
    public required string NombreXml { get; init; }
    public required string SegundoNombre { get; init; }
} ================================ INTERFACES ================================ IStorageNamingService public interface IStorageNamingService
{
    StorageNamingResult Generate(
        long idAlmacen,
        int tipoDocumento,
        string? segundoNombreOriginal = null
    );
} IStorageExtensionResolver public interface IStorageExtensionResolver
{
    Task<string> ResolveAsync(int tipoDocumento, string alias);
} ================================ REPOSITORIO — EXTENSIONES ================================ IStorageExtensionRepository public interface IStorageExtensionRepository
{
    Task<string?> GetExtensionAsync(int tipoDocumento, string alias);
} IMPLEMENTACIÓN public sealed class StorageExtensionRepository : IStorageExtensionRepository
{
    private readonly IDapperCrudEngine _engine;

    public StorageExtensionRepository(IDapperCrudEngine engine)
    {
        _engine = engine;
    }

    public async Task<string?> GetExtensionAsync(int tipoDocumento, string alias)
    {
        var query = new QueryOptions
        {
            TableName = "DA_EXTENSION",
            DefaultAlias = alias,
            Columns = new List<string> { "ESTENSION" },
            RawWhere = $"ESTADO_NORMAL = {tipoDocumento} OR ESTADO_ADJUNTO = {tipoDocumento} OR ESTADO_LINK = {tipoDocumento}"
        };

        var result = await _engine.GetAllAsync<ExtensionDto>(query);

        if (!result.Success || !result.Data.Any())
            return null;

        return result.Data.First().ESTENSION;
    }
} ================================ IMPLEMENTACIÓN — EXTENSION RESOLVER ================================ public sealed class StorageExtensionResolver : IStorageExtensionResolver
{
    private readonly IStorageExtensionRepository _repository;

    public StorageExtensionResolver(IStorageExtensionRepository repository)
    {
        _repository = repository;
    }

    public async Task<string> ResolveAsync(int tipoDocumento, string alias)
    {
        var ext = await _repository.GetExtensionAsync(tipoDocumento, alias);

        if (string.IsNullOrWhiteSpace(ext))
            throw new InvalidOperationException($"No se pudo determinar extensión para tipo {tipoDocumento}");

        return ext.StartsWith(".") ? ext : "." + ext;
    }
} ================================ IMPLEMENTACIÓN — NAMING SERVICE ================================ public sealed class StorageNamingService : IStorageNamingService
{
    private readonly IStorageExtensionResolver _resolver;

    public StorageNamingService(IStorageExtensionResolver resolver)
    {
        _resolver = resolver;
    }

    public StorageNamingResult Generate(
        long idAlmacen,
        int tipoDocumento,
        string? segundoNombreOriginal = null)
    {
        if (idAlmacen <= 0)
            throw new InvalidOperationException("IdAlmacen inválido");

        var ceros = GeneratePadding(idAlmacen);

        var extension = ".tmp"; // placeholder, se resuelve async en capa superior

        var nombrePrincipal = $"DIG{ceros}{idAlmacen}{extension}";
        var nombreXml = $"FXL{ceros}{idAlmacen}.xml";

        var segundoNombre = string.IsNullOrWhiteSpace(segundoNombreOriginal)
            ? nombrePrincipal
            : segundoNombreOriginal;

        return new StorageNamingResult
        {
            NombreArchivoPrincipal = nombrePrincipal,
            NombreXml = nombreXml,
            SegundoNombre = segundoNombre
        };
    }

    private static string GeneratePadding(long value)
    {
        var length = value.ToString().Length;

        return length switch
        {
            1 => "0000",
            2 => "000",
            3 => "00",
            4 => "0",
            _ => ""
        };
    }
} ================================ INTEGRACIÓN EN ORCHESTRATOR ================================ var naming = _namingService.Generate(
    txResult.IdentityReservation.Identity.IdAlmacen,
    context.Command.TipoDocumento
);

var extension = await _extensionResolver.ResolveAsync(
    context.Command.TipoDocumento,
    context.DefaultDbAlias
);

var nombreFinal = naming.NombreArchivoPrincipal.Replace(".tmp", extension); ================================ REGLAS CRÍTICAS ================================ SIEMPRE usar prefijo DIG SIEMPRE usar prefijo FXL para XML Padding obligatorio Extensión obligatoria desde DB No usar extensión fija No generar nombres dinámicos arbitrarios Nombre debe ser determinístico Compatible con visor legacy ================================ PRUEBAS ================================ Unitarias: idAlmacen 1 → DIG00001 idAlmacen 10 → DIG00010 idAlmacen 100 → DIG00100 idAlmacen 1000 → DIG01000 extensión correcta xml correcto segundo nombre correcto Integración: consulta real DA_EXTENSION extensión correcta por tipo compatibilidad con archivos reales ================================ CRITERIOS DE ACEPTACIÓN ================================ ✔ Naming idéntico a VB ✔ Prefijos correctos ✔ Padding correcto ✔ Extensión correcta ✔ XML correcto ✔ Determinístico ================================ INSTRUCCIÓN FINAL ================================ Implementar: ✔ NamingService ✔ ExtensionResolver ✔ ExtensionRepository Listo para: 👉 PROMPT 14 — Ruta física real SYSTEM1RUT

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-179-implementacion-resolve-nombre-archivo.