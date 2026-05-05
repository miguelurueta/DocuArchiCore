## Context

- Jira issue key: SCRUM-180
- Jira summary: IMPLEMENTACION-RUTA-FISICA-ALMACENAMIENTO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-180

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE PROMPT 14 — Ruta física real SYSTEM1RUT + StorageRoot por Gabinete/Disco (FASE CRÍTICA — PARIDAD FUNCIONAL LEGACY) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actúa como Arquitecto Master Backend .NET experto en: DocuArchi legacy resolución de rutas físicas documentales storage documental por gabinete/disco/carpeta hardening de rutas Dapper / QueryOptions Clean Architecture migración VB → C# con paridad funcional seguridad contra path traversal consistencia DB vs FileSystem ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Restaurar la paridad funcional de la ruta física legacy usada por la función Almacenamiento . La nueva implementación NO debe guardar en rutas temporales genéricas como: Path.GetTempPath()/docuarchi/storage-engine Debe resolver la ruta real de almacenamiento equivalente al comportamiento VB: Consulta_Ruta_Almacenamiento(_Ruta_Almacenamiento, _Nombre_Gabienete)
RutaCarpet = _Ruta_Almacenamiento & _Nombre_Gabienete & disc
Solicita_Carpeta_almacenamiento(carpealma, numcarpvar, rut2000, RutaCarpet)
Ruta_Alamce_Image = _Ruta_Almacenamiento & _Nombre_Gabienete & disc & "\" & carpealma & "\" Resultado esperado: StorageRoot real desde SYSTEM1RUT / configuración legacy
Ruta por gabinete + disco
Ruta por carpeta calculada
Ruta final segura validada ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTEXTO LEGAZY VB : D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\Architecture\Almacenamiento\funcion-almacena-consolidad.txt CONTEXTO TABLAS: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\DataModel\StorageEngine-Tables.md BRECHA FUNCIONAL DETECTADA Comparativo actual: VB legacy:
- Usa ruta real por gabinete desde Class_SYSTEM1RUT
- Construye ruta con gabinete + disco + carpeta
- Copia documentos en ubicación definitiva compatible con visor legacy

C# actual:
- Usa Path.GetTempPath()/docuarchi/storage-engine
- No usa SYSTEM1RUT
- No respeta gabinete + disco + carpeta
- Rompe compatibilidad con visor/rutas legacy Esta brecha es bloqueante para producción. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UBICACIÓN ESPERADA Services / Physical MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Physical/ Archivos esperados: IStoragePhysicalPathService.cs
StoragePhysicalPathService.cs
IStorageFolderLegacyPolicy.cs
StorageFolderLegacyPolicy.cs Repository MiApp.Repository/Repositorio/GestorDocumental/AlmacenamientoDocumental/StorageRoute/ Archivos esperados: IStorageRouteRepository.cs
StorageRouteRepository.cs
StorageRouteModel.cs Models MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/Physical/ Archivos esperados: StoragePhysicalPathModel.cs
StorageRouteModel.cs
StorageFolderResult.cs ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ MODELOS StorageRouteModel public sealed class StorageRouteModel
{
    public string NombreGabinete { get; init; } = "";
    public string RutaAlmacenamiento { get; init; } = "";
} StorageFolderResult public sealed class StorageFolderResult
{
    public string CarpetaLegacy { get; init; } = "";
    public string RutaGabineteDisco { get; init; } = "";
    public string RutaFinal { get; init; } = "";
} StoragePhysicalPathModel public sealed class StoragePhysicalPathModel
{
    public string StorageRoot { get; init; } = "";
    public string NombreGabinete { get; init; } = "";
    public int Disco { get; init; }
    public int Carpeta { get; init; }

    public string RutaGabineteDisco { get; init; } = "";
    public string CarpetaLegacy { get; init; } = "";
    public string RutaFinal { get; init; } = "";
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTERFACES IStorageRouteRepository public interface IStorageRouteRepository
{
    Task<StorageRouteModel?> GetRouteAsync(
        string nombreGabinete,
        string defaultDbAlias
    );
} IStorageFolderLegacyPolicy public interface IStorageFolderLegacyPolicy
{
    StorageFolderResult ResolveFolder(
        string rutaAlmacenamiento,
        string nombreGabinete,
        int disco,
        int numCarpeta
    );
} IStoragePhysicalPathService public interface IStoragePhysicalPathService
{
    Task<StoragePhysicalPathModel> ResolveAsync(
        StorageContext context,
        StorageIdentityModel identity
    );
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — StorageRouteRepository Reglas Debe consultar la ruta real equivalente a Consulta_Ruta_Almacenamiento . No debe retornar rutas vacías. No debe usar valores hardcodeados. Debe usar QueryOptions o Dapper parametrizado. No debe concatenar SQL. Debe validar el nombre del gabinete. Implementación esperada public sealed class StorageRouteRepository : IStorageRouteRepository
{
    private readonly IDapperCrudEngine _engine;

    public StorageRouteRepository(IDapperCrudEngine engine)
    {
        _engine = engine;
    }

    public async Task<StorageRouteModel?> GetRouteAsync(
        string nombreGabinete,
        string defaultDbAlias)
    {
        if (string.IsNullOrWhiteSpace(nombreGabinete))
            throw new ArgumentException("NombreGabinete requerido", nameof(nombreGabinete));

        if (string.IsNullOrWhiteSpace(defaultDbAlias))
            throw new ArgumentException("DefaultDbAlias requerido", nameof(defaultDbAlias));

        var query = new QueryOptions
        {
            TableName = "system1rut",
            DefaultAlias = defaultDbAlias,
            Columns = new List<string>
            {
                "NOMBRE AS NombreGabinete",
                "RUTA AS RutaAlmacenamiento"
            },
            Filters = new Dictionary<string, object>
            {
                { "NOMBRE", nombreGabinete }
            },
            Limit = 1
        };

        var result = await _engine.GetAllAsync<StorageRouteModel>(query);

        if (!result.Success)
            throw new InvalidOperationException(result.ErrorMessage);

        return result.Data.FirstOrDefault();
    }
} Si la tabla real o columnas reales no se llaman system1rut , NOMBRE , RUTA , ajustar contra el esquema real, pero conservar la intención: replicar Consulta_Ruta_Almacenamiento . ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — StorageFolderLegacyPolicy Objetivo Replicar la construcción de ruta legacy: RutaGabineteDisco = RutaAlmacenamiento + NombreGabinete + Disco
CarpetaLegacy = carpeta calculada
RutaFinal = RutaGabineteDisco + CarpetaLegacy Implementación esperada public sealed class StorageFolderLegacyPolicy : IStorageFolderLegacyPolicy
{
    public StorageFolderResult ResolveFolder(
        string rutaAlmacenamiento,
        string nombreGabinete,
        int disco,
        int numCarpeta)
    {
        if (string.IsNullOrWhiteSpace(rutaAlmacenamiento))
            throw new InvalidOperationException("RutaAlmacenamiento requerida");

        if (string.IsNullOrWhiteSpace(nombreGabinete))
            throw new InvalidOperationException("NombreGabinete requerido");

        if (disco <= 0)
            throw new InvalidOperationException("Disco inválido");

        if (numCarpeta <= 0)
            throw new InvalidOperationException("Carpeta inválida");

        var rutaGabineteDisco = Path.Combine(
            rutaAlmacenamiento,
            $"{nombreGabinete}{disco}"
        );

        var carpetaLegacy = numCarpeta.ToString();

        var rutaFinal = Path.Combine(
            rutaGabineteDisco,
            carpetaLegacy
        );

        return new StorageFolderResult
        {
            CarpetaLegacy = carpetaLegacy,
            RutaGabineteDisco = rutaGabineteDisco,
            RutaFinal = rutaFinal
        };
    }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — StoragePhysicalPathService Responsabilidad Resolver la ruta física final del documento usando: StorageContext
StorageIdentityModel
StorageRouteRepository
StorageFolderLegacyPolicy
Path hardening Implementación esperada public sealed class StoragePhysicalPathService : IStoragePhysicalPathService
{
    private readonly IStorageRouteRepository _routeRepository;
    private readonly IStorageFolderLegacyPolicy _folderPolicy;
    private readonly IStorageFileValidator _fileValidator;
    private readonly ILogger<StoragePhysicalPathService> _logger;

    public StoragePhysicalPathService(
        IStorageRouteRepository routeRepository,
        IStorageFolderLegacyPolicy folderPolicy,
        IStorageFileValidator fileValidator,
        ILogger<StoragePhysicalPathService> logger)
    {
        _routeRepository = routeRepository;
        _folderPolicy = folderPolicy;
        _fileValidator = fileValidator;
        _logger = logger;
    }

    public async Task<StoragePhysicalPathModel> ResolveAsync(
        StorageContext context,
        StorageIdentityModel identity)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (identity == null)
            throw new ArgumentNullException(nameof(identity));

        var nombreGabinete = context.Command.NombreGabinete;

        var route = await _routeRepository.GetRouteAsync(
            nombreGabinete,
            context.DefaultDbAlias
        );

        if (route == null || string.IsNullOrWhiteSpace(route.RutaAlmacenamiento))
            throw new InvalidOperationException("No se encontró ruta de almacenamiento para gabinete");

        var folder = _folderPolicy.ResolveFolder(
            route.RutaAlmacenamiento,
            nombreGabinete,
            identity.Disco,
            identity.Carpeta
        );

        _fileValidator.ValidateSafePath(
            folder.RutaFinal,
            route.RutaAlmacenamiento
        );

        _logger.LogInformation(
            "Ruta física legacy resuelta requestId={requestId} gabinete={gabinete} disco={disco} carpeta={carpeta}",
            context.RequestId,
            nombreGabinete,
            identity.Disco,
            identity.Carpeta
        );

        return new StoragePhysicalPathModel
        {
            StorageRoot = route.RutaAlmacenamiento,
            NombreGabinete = nombreGabinete,
            Disco = identity.Disco,
            Carpeta = identity.Carpeta,
            RutaGabineteDisco = folder.RutaGabineteDisco,
            CarpetaLegacy = folder.CarpetaLegacy,
            RutaFinal = folder.RutaFinal
        };
    }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTEGRACIÓN CON PROMPT 9 Actualizar StorageFilePlanModel para que use: StorageRoot = physicalPath.StorageRoot
RutaFinal = Path.GetRelativePath(physicalPath.StorageRoot, physicalPath.RutaFinal)
NombreArchivoPrincipal = naming.NombreArchivoPrincipal El DocumentFileWriter debe resolver ruta final segura mediante: IStoragePathResolver.ResolveSafePath(StorageRoot, RutaFinal) Nunca debe recibir rutas arbitrarias externas. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTEGRACIÓN CON PROMPT 13 El nombre del archivo final debe venir de: StorageNamingService Y la ruta final debe venir de: StoragePhysicalPathService Separación obligatoria: NamingService → nombre
PhysicalPathService → ruta
FileWriter → copia No mezclar responsabilidades. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS CRÍTICAS No usar Path.GetTempPath() para destino final. No hardcodear ruta física. No confiar en ruta enviada por frontend. Ruta final debe salir de configuración DB. Debe respetar gabinete + disco + carpeta. Debe validar StorageRoot . Debe prevenir path traversal. No crear directorios fuera del root permitido. No loguear rutas completas si contienen información sensible. Mantener compatibilidad con visor legacy. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS Unitarias — StorageFolderLegacyPolicy ruta válida + gabinete + disco + carpeta → ruta correcta disco <= 0 → error carpeta <= 0 → error ruta vacía → error gabinete vacío → error Unitarias — StoragePhysicalPathService route null → error ruta almacenamiento vacía → error path traversal → error ruta válida → StoragePhysicalPathModel correcto usa identity.Disco usa identity.Carpeta Unitarias — StorageRouteRepository consulta por gabinete alias requerido gabinete requerido sin resultados → null error DB → excepción controlada Integración consulta real a tabla de rutas ruta real de gabinete creación de carpeta permitida bloqueo de ruta fuera de root compatibilidad con FileWriter Regresión Legacy Comparar contra VB: _Ruta_Almacenamiento
_Nombre_Gabienete + disc
carpealma + numcarpvar
Ruta_Alamce_Image Resultado C# debe producir ruta equivalente. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBSERVABILIDAD Logs obligatorios: requestId
nombreGabinete
disco
carpeta
estado resolución ruta
duración No loguear: ruta física completa sensible
contenido de documentos
fulltext ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN ENTERPRISE Ruta: Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/ Crear: SCRUM-[ID]-Arquitectura-RutaFisicaLegacy.md
SCRUM-[ID]-Implementacion-Detallada-RutaFisicaLegacy.md
SCRUM-[ID]-Pruebas-RutaFisicaLegacy.md
SCRUM-[ID]-Observabilidad-RutaFisicaLegacy.md
SCRUM-[ID]-Compatibilidad-Visor-Legacy.md
SCRUM-[ID]-Metadata.md Debe incluir: comparación VB vs C# tabla de equivalencia de rutas diagrama de secuencia diagrama de clases decisión de seguridad de path hardening relación con Prompt 9 y Prompt 13 riesgos de compatibilidad con visor legacy ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN Consulta ruta real desde DB. No usa Path.GetTempPath() como destino final. Ruta final respeta gabinete + disco + carpeta. Ruta final es segura. Ruta final es compatible con legacy. FileWriter usa esta ruta. NamingService se mantiene separado. No hay path traversal. Hay pruebas unitarias. Hay pruebas de integración. Hay documentación enterprise. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RESTRICCIONES No implementar naming aquí. No implementar XML aquí. No implementar copia física aquí. No modificar lógica de transacción DB. No hardcodear rutas. No aceptar rutas finales desde frontend. No modificar función legacy. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INSTRUCCIÓN FINAL Implementar: StorageRouteRepository
StorageFolderLegacyPolicy
StoragePhysicalPathService
Modelos físicos
DI
Pruebas
Documentación El resultado debe quedar listo para: PROMPT 15 — Opciones Legacy System1 (Inventario, TRD, Unidad)

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-180-implementacion-ruta-fisica-almacenamient.