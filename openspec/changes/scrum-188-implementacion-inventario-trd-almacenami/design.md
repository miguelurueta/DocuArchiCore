## Context

- Jira issue key: SCRUM-188
- Jira summary: IMPLEMENTACION-INVENTARIO-TRD-ALMACENAMIENTO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-188

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE PROMPT 15 — Opciones Legacy System1 (Inventario, TRD, Unidad) (FASE CRÍTICA — PARIDAD FUNCIONAL LEGACY) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actúa como Arquitecto Master Backend .NET experto en: DocuArchi legacy configuración documental por gabinete system1 opciones funcionales legacy inventario documental TRD expediente / unidad de conservación Dapper / QueryOptions Clean Architecture migración VB → C# con paridad funcional ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Restaurar la paridad funcional de las opciones legacy consultadas desde system1 o clases equivalentes del sistema antiguo. La función legacy Almacenamiento evalúa opciones como: INVENTARIO_DOCUMENTAL
APLICA_TRD
ASIGNA_UNIDAD / unidad de conservación Estas opciones definen si el flujo debe: - exigir usuario de gestión
- exigir empresa
- aplicar TRD
- asociar expediente
- asociar unidad de conservación
- insertar inventario documental
- activar validaciones archivísticas adicionales La implementación actual no debe usar valores por defecto falsos ni placeholders. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTEXTO LEGAZY VB D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\Architecture\Almacenamiento\funcion-almacena-consolidad.txt CONTEXTO TABLAS D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\DataModel\StorageEngine-Tables.md BRECHA FUNCIONAL DETECTADA Comparativo actual: VB legacy:
- VerificaOpcionAplicarInventarioDocumental(...)
- VerificaOpcionAplicarTablaRetencion(...)
- Verfica_opcion_seleccion_unidad(...)
- Decide reglas del flujo según configuración real del gabinete

C# actual:
- StorageOptionsResolver retorna defaults false
- No consulta system1
- Desactiva flujos que en legacy podían estar activos Esta brecha es bloqueante porque puede omitir inventario, TRD, expediente o unidad. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UBICACIÓN ESPERADA Services / Options MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Options/ Archivos esperados: IStorageOptionsResolver.cs
StorageOptionsResolver.cs
IStorageOptionsValidator.cs
StorageOptionsValidator.cs Repository MiApp.Repository/Repositorio/GestorDocumental/AlmacenamientoDocumental/SystemOptions/ Archivos esperados: IStorageSystemOptionsRepository.cs
StorageSystemOptionsRepository.cs Models MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/Options/ Archivos esperados: StorageSystemOptionsModel.cs
StorageResolvedOptionsModel.cs ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ MODELOS StorageSystemOptionsModel public sealed class StorageSystemOptionsModel
{
    public string NombreGabinete { get; init; } = "";

    public int AplicaInventarioDocumental { get; init; }
    public int AplicaTablaRetencion { get; init; }
    public int AplicaUnidadConservacion { get; init; }
} StorageResolvedOptionsModel public sealed class StorageResolvedOptionsModel
{
    public string NombreGabinete { get; init; } = "";

    public bool AplicaInventarioDocumental { get; init; }
    public bool AplicaTrd { get; init; }
    public bool AplicaUnidadConservacion { get; init; }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTERFACES IStorageSystemOptionsRepository public interface IStorageSystemOptionsRepository
{
    Task<StorageSystemOptionsModel?> GetOptionsAsync(
        string nombreGabinete,
        string defaultDbAlias
    );
} IStorageOptionsResolver public interface IStorageOptionsResolver
{
    Task<StorageResolvedOptionsModel> ResolveAsync(
        StorageContext context
    );
} IStorageOptionsValidator public interface IStorageOptionsValidator
{
    void Validate(
        StorageResolvedOptionsModel options,
        StorageContext context
    );
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — StorageSystemOptionsRepository Reglas Debe consultar configuración real del gabinete. No retornar defaults falsos. No hardcodear opciones. No inventar configuración. Debe usar QueryOptions o Dapper parametrizado. Debe validar nombreGabinete . Debe validar defaultDbAlias . Implementación esperada public sealed class StorageSystemOptionsRepository : IStorageSystemOptionsRepository
{
    private readonly IDapperCrudEngine _engine;

    public StorageSystemOptionsRepository(IDapperCrudEngine engine)
    {
        _engine = engine;
    }

    public async Task<StorageSystemOptionsModel?> GetOptionsAsync(
        string nombreGabinete,
        string defaultDbAlias)
    {
        if (string.IsNullOrWhiteSpace(nombreGabinete))
            throw new ArgumentException("NombreGabinete requerido", nameof(nombreGabinete));

        if (string.IsNullOrWhiteSpace(defaultDbAlias))
            throw new ArgumentException("DefaultDbAlias requerido", nameof(defaultDbAlias));

        var query = new QueryOptions
        {
            TableName = "system1",
            DefaultAlias = defaultDbAlias,
            Columns = new List<string>
            {
                "nombre AS NombreGabinete",
                "INVENTARIO_DOCUMENTAL AS AplicaInventarioDocumental",
                "APLICA_TRD AS AplicaTablaRetencion",
                "ASIGNA_UNIDAD AS AplicaUnidadConservacion"
            },
            Filters = new Dictionary<string, object>
            {
                { "nombre", nombreGabinete }
            },
            Limit = 1
        };

        var result = await _engine.GetAllAsync<StorageSystemOptionsModel>(query);

        if (!result.Success)
            throw new InvalidOperationException(result.ErrorMessage);

        return result.Data.FirstOrDefault();
    }
} Si los nombres reales de columnas difieren, ajustar al esquema real, pero conservar la intención: replicar las funciones legacy: VerificaOpcionAplicarInventarioDocumental
VerificaOpcionAplicarTablaRetencion
Verfica_opcion_seleccion_unidad ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — StorageOptionsResolver Responsabilidad Resolver opciones reales del gabinete y convertirlas a booleans de dominio. public sealed class StorageOptionsResolver : IStorageOptionsResolver
{
    private readonly IStorageSystemOptionsRepository _repository;
    private readonly ILogger<StorageOptionsResolver> _logger;

    public StorageOptionsResolver(
        IStorageSystemOptionsRepository repository,
        ILogger<StorageOptionsResolver> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<StorageResolvedOptionsModel> ResolveAsync(StorageContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var nombreGabinete = context.Command.NombreGabinete;

        var options = await _repository.GetOptionsAsync(
            nombreGabinete,
            context.DefaultDbAlias
        );

        if (options == null)
            throw new InvalidOperationException("No se encontraron opciones system1 para gabinete");

        var resolved = new StorageResolvedOptionsModel
        {
            NombreGabinete = options.NombreGabinete,
            AplicaInventarioDocumental = options.AplicaInventarioDocumental == 1,
            AplicaTrd = options.AplicaTablaRetencion == 1,
            AplicaUnidadConservacion = options.AplicaUnidadConservacion == 1
        };

        _logger.LogInformation(
            "Opciones legacy resueltas requestId={requestId} gabinete={gabinete} inventario={inventario} trd={trd} unidad={unidad}",
            context.RequestId,
            nombreGabinete,
            resolved.AplicaInventarioDocumental,
            resolved.AplicaTrd,
            resolved.AplicaUnidadConservacion
        );

        return resolved;
    }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — StorageOptionsValidator Responsabilidad Validar que el request cumpla reglas según las opciones reales del gabinete. public sealed class StorageOptionsValidator : IStorageOptionsValidator
{
    public void Validate(StorageResolvedOptionsModel options, StorageContext context)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var command = context.Command;

        if (options.AplicaInventarioDocumental)
        {
            if (command.Inventario == null)
                throw new InvalidOperationException("Inventario documental requerido para este gabinete");

            if (command.Inventario.IdUsuarioGestion <= 0)
                throw new InvalidOperationException("El usuario DocuArchi debe estar asociado a usuario de gestión");

            if (command.Inventario.IdEmpresa <= 0)
                throw new InvalidOperationException("El usuario de gestión debe tener empresa asociada");
        }

        if (options.AplicaTrd)
        {
            if (command.Trd == null)
                throw new InvalidOperationException("TRD requerida para este gabinete");

            if (!command.Trd.IdArea.HasValue || command.Trd.IdArea <= 0)
                throw new InvalidOperationException("Área requerida para TRD");

            if (!command.Trd.IdSerie.HasValue || command.Trd.IdSerie <= 0)
                throw new InvalidOperationException("Serie requerida para TRD");

            if (!command.Trd.IdTipoDocumento.HasValue || command.Trd.IdTipoDocumento <= 0)
                throw new InvalidOperationException("Tipo documental requerido para TRD");
        }

        if (options.AplicaUnidadConservacion)
        {
            if (command.Expediente == null)
                throw new InvalidOperationException("Expediente o unidad de conservación requerida para este gabinete");

            var tieneExpediente = command.Expediente.IdExpediente.HasValue && command.Expediente.IdExpediente > 0;
            var tieneUnidad = command.Expediente.IdUnidadConservacion.HasValue && command.Expediente.IdUnidadConservacion > 0;

            if (!tieneExpediente && !tieneUnidad)
                throw new InvalidOperationException("Debe seleccionar expediente o unidad de conservación");

            if (!command.Expediente.IdClaseDocumento.HasValue || command.Expediente.IdClaseDocumento <= 0)
                throw new InvalidOperationException("Clase de documento requerida para expediente/unidad");
        }
    }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTEGRACIÓN EN VALIDATION PIPELINE Crear validator: StorageOptionsPipelineValidator.cs public sealed class StorageOptionsPipelineValidator : BaseStorageValidator
{
    private readonly IStorageOptionsResolver _resolver;
    private readonly IStorageOptionsValidator _validator;

    public StorageOptionsPipelineValidator(
        IStorageOptionsResolver resolver,
        IStorageOptionsValidator validator)
    {
        _resolver = resolver;
        _validator = validator;
    }

    public override async Task ValidateAsync(StorageContext context, List<StorageError> errors)
    {
        try
        {
            var options = await _resolver.ResolveAsync(context);

            _validator.Validate(options, context);

            context.ResolvedOptions = options;
        }
        catch (Exception ex)
        {
            errors.Add(new StorageError
            {
                Code = "STORAGE_OPTIONS",
                Message = ex.Message
            });
        }
    }
} Si StorageContext aún no tiene ResolvedOptions , agregar: public StorageResolvedOptionsModel? ResolvedOptions { get; set; } ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTEGRACIÓN CON TRANSACTION COORDINATOR El StorageTransactionCoordinator debe decidir si ejecuta o no operaciones según: context.ResolvedOptions?.AplicaInventarioDocumental == true
context.ResolvedOptions?.AplicaTrd == true
context.ResolvedOptions?.AplicaUnidadConservacion == true Reglas: - Inventario solo si AplicaInventarioDocumental = true.
- TRD solo si AplicaTrd = true.
- Expediente/unidad solo si AplicaUnidadConservacion = true.
- No exigir inventario si la opción está apagada.
- No omitir inventario si la opción está encendida. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DEPENDENCY INJECTION services.AddScoped<IStorageSystemOptionsRepository, StorageSystemOptionsRepository>();
services.AddScoped<IStorageOptionsResolver, StorageOptionsResolver>();
services.AddScoped<IStorageOptionsValidator, StorageOptionsValidator>();
services.AddScoped<IStorageValidator, StorageOptionsPipelineValidator>(); ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS CRÍTICAS No usar defaults falsos. No omitir consulta a DB. No hardcodear opciones. No confiar en frontend para decidir si aplica inventario/TRD/unidad. Las opciones reales gobiernan el flujo. Las reglas deben ejecutarse antes de persistencia. El resultado debe quedar disponible en StorageContext . No ejecutar inventario si la opción no aplica. No ejecutar expediente/unidad si la opción no aplica. No exigir TRD si el gabinete no la aplica. No romper flujos donde VB permitía operar sin TRD/inventario/unidad. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS Unitarias — StorageOptionsResolver gabinete vacío → error alias vacío → error sin registro system1 → error inventario = 1 → true inventario = 0 → false trd = 1 → true trd = 0 → false unidad = 1 → true unidad = 0 → false Unitarias — StorageOptionsValidator inventario activo sin Inventario → error inventario activo sin usuario gestión → error inventario activo sin empresa → error trd activo sin Trd → error trd activo sin área → error trd activo sin serie → error trd activo sin tipo documental → error unidad activa sin expediente/unidad → error unidad activa sin clase documento → error opciones apagadas no exigen datos → OK Integración consulta real a system1 gabinete con inventario activo gabinete con TRD activo gabinete con unidad activa gabinete sin opciones activas comportamiento equivalente a VB Regresión Legacy Comparar contra: VerificaOpcionAplicarInventarioDocumental
VerificaOpcionAplicarTablaRetencion
Verfica_opcion_seleccion_unidad C# debe decidir igual que VB. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBSERVABILIDAD Logs obligatorios: requestId
nombreGabinete
aplicaInventarioDocumental
aplicaTrd
aplicaUnidadConservacion
estado resolución
duración No loguear: datos sensibles
fulltext
contenido documental ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN ENTERPRISE Ruta: Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/ Crear: SCRUM-[ID]-Arquitectura-OpcionesLegacySystem1.md
SCRUM-[ID]-Implementacion-Detallada-OpcionesLegacySystem1.md
SCRUM-[ID]-Pruebas-OpcionesLegacySystem1.md
SCRUM-[ID]-Observabilidad-OpcionesLegacySystem1.md
SCRUM-[ID]-Regresion-Legacy-OpcionesSystem1.md
SCRUM-[ID]-Metadata.md Debe incluir: comparación VB vs C# tabla de equivalencia de opciones decisión de no usar defaults falsos relación con inventario relación con TRD relación con expediente/unidad impacto en TransactionCoordinator riesgos de activar o desactivar opciones incorrectamente ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN Consulta opciones reales desde system1 . No retorna defaults falsos. Resuelve inventario correctamente. Resuelve TRD correctamente. Resuelve unidad correctamente. Valida request según opciones reales. Guarda resultado en StorageContext . TransactionCoordinator usa las opciones. No rompe flujos legacy donde las opciones están apagadas. No omite flujos legacy donde las opciones están encendidas. Hay pruebas unitarias. Hay pruebas de integración. Hay regresión comparativa con VB. Hay documentación enterprise. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RESTRICCIONES No implementar inventario aquí. No implementar TRD aquí. No implementar expediente aquí. No implementar unidad aquí. No modificar FileSystem. No modificar XML. No hardcodear opciones. No modificar función legacy. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INSTRUCCIÓN FINAL Implementar: StorageSystemOptionsRepository
StorageOptionsResolver
StorageOptionsValidator
StorageOptionsPipelineValidator
Modelos de opciones
DI
Pruebas
Documentación El resultado debe quedar listo para: PROMPT 16 — Cálculo tamaño + conteo real de páginas

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-188-implementacion-inventario-trd-almacenami.