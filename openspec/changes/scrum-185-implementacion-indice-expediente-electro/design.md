## Context

- Jira issue key: SCRUM-185
- Jira summary: IMPLEMENTACION-INDICE-EXPEDIENTE-ELECTRONICO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-185

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE PROMPT 19 — XML Índice Expediente Legacy-Compatible (FASE CRÍTICA — PARIDAD FUNCIONAL VB → C#) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actúa como Arquitecto Master Backend .NET experto en: DocuArchi legacy expedientes electrónicos índice electrónico documental XML archivístico actualización segura de archivos XML existentes consistencia DB + XML Dapper / SQL transaccional FileSystem seguro Clean Architecture migración VB → C# con paridad funcional sistemas ECM críticos ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Restaurar la paridad funcional del bloque legacy que actualiza el archivo XML del índice electrónico del expediente . La nueva implementación debe cubrir: - Solicitar ruta real del archivo XML índice del expediente.
- Validar existencia del XML.
- Cargar XML existente.
- Insertar nodo DocumentoIndizado.
- Mantener estructura legacy.
- Guardar XML de forma segura.
- Coordinar la actualización XML con el índice registrado en DB. Este prompt corrige la brecha: C# actual:
- Inserta ra_cert_indice_expediente parcialmente.
- NO ejecuta equivalente a:
  Solicita_archivo_indice_expediente
  Actualiza_archivo_xml_indice_expediente ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTEXTO LEGAZY VB D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\Architecture\Almacenamiento\funcion-almacena-consolidad.txt CONTEXTO TABLAS D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\DataModel\StorageEngine-Tables.md CONTEXTO LEGACY En VB, cuando: id_expediente <> 0 And estado_expediente_electronico = 2 se ejecutaba: Solicita_archivo_indice_expediente(id_expediente, Ruta_archivo_xml)

Actualiza_archivo_xml_indice_expediente(
    Ruta_archivo_xml,
    stru_produccion_indice,
    xmlArchivo
) Y al final: If estado_exml_archivo = "YES" Then
    xmlArchivo.Save(Ruta_archivo_xml)
End If ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ BRECHA FUNCIONAL DETECTADA Comparativo actual: VB legacy:
- Obtiene ruta XML del expediente.
- Actualiza archivo XML físico del índice.
- Agrega información del documento indizado.
- Guarda XML después del commit lógico.

C# actual:
- No existe equivalente completo.
- Solo inserta índice en DB.
- No actualiza el XML físico del expediente. Impacto: - Índice electrónico incompleto.
- Visor/consulta legacy puede no ver el documento.
- Auditoría archivística incompleta.
- Paridad funcional fallida. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UBICACIÓN ESPERADA Services / ExpedienteXml MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/ExpedienteXml/ Archivos esperados: IExpedienteIndiceXmlService.cs
ExpedienteIndiceXmlService.cs
IExpedienteIndiceXmlBuilder.cs
ExpedienteIndiceXmlBuilder.cs
IExpedienteIndiceXmlWriter.cs
ExpedienteIndiceXmlWriter.cs Repository MiApp.Repository/Repositorio/GestorDocumental/AlmacenamientoDocumental/ExpedienteXml/ Archivos esperados: IExpedienteIndiceXmlRepository.cs
ExpedienteIndiceXmlRepository.cs Models MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/ExpedienteXml/ Archivos esperados: ExpedienteIndiceXmlRouteModel.cs
ExpedienteIndiceXmlDocumentModel.cs
ExpedienteIndiceXmlUpdateResult.cs ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ MODELOS ExpedienteIndiceXmlRouteModel public sealed class ExpedienteIndiceXmlRouteModel
{
    public int IdExpediente { get; init; }

    public string RutaArchivoXml { get; init; } = "";
} ExpedienteIndiceXmlDocumentModel public sealed class ExpedienteIndiceXmlDocumentModel
{
    public long IdRegistroProduccionDocumental { get; init; }

    public long IdAlmacen { get; init; }

    public int IdExpediente { get; init; }

    public string NombreDocumento { get; init; } = "";

    public string SegundoNombreDocumento { get; init; } = "";

    public string TipologiaDocumental { get; init; } = "";

    public DateTime FechaDeclaracionDocumento { get; init; }

    public DateTime FechaIncorporacionDocumento { get; init; }

    public string ValorHuella { get; init; } = "";

    public string FuncionResumen { get; init; } = "SHA256";

    public int OrdenDocumentoExpediente { get; init; }

    public int PaginaInicial { get; init; }

    public int PaginaFinal { get; init; }

    public string Formato { get; init; } = "";

    public string Tamano { get; init; } = "";

    public string RutaDocumento { get; init; } = "";

    public int NumeroFolios { get; init; }

    public int? Origen { get; init; }
} ExpedienteIndiceXmlUpdateResult public sealed class ExpedienteIndiceXmlUpdateResult
{
    public bool Updated { get; init; }

    public string RutaArchivoXml { get; init; } = "";

    public string Estado { get; init; } = "";
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTERFACES IExpedienteIndiceXmlRepository public interface IExpedienteIndiceXmlRepository
{
    Task<ExpedienteIndiceXmlRouteModel?> GetXmlRouteAsync(
        int idExpediente,
        string defaultDbAlias
    );
} IExpedienteIndiceXmlBuilder public interface IExpedienteIndiceXmlBuilder
{
    ExpedienteIndiceXmlDocumentModel Build(
        StorageContext context,
        StorageTransactionResult transactionResult,
        StorageNamingResult naming,
        StoragePhysicalPathModel physicalPath
    );
} IExpedienteIndiceXmlWriter public interface IExpedienteIndiceXmlWriter
{
    Task<ExpedienteIndiceXmlUpdateResult> UpdateAsync(
        string rutaArchivoXml,
        ExpedienteIndiceXmlDocumentModel document
    );
} IExpedienteIndiceXmlService public interface IExpedienteIndiceXmlService
{
    Task<ExpedienteIndiceXmlUpdateResult> ExecuteAsync(
        StorageContext context,
        StorageTransactionResult transactionResult,
        StorageNamingResult naming,
        StoragePhysicalPathModel physicalPath
    );
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLA LEGACY DE ACTIVACIÓN Este flujo solo debe ejecutarse si: - Existe expediente.
- Existe IdRegistroProduccionDocumental.
- EstadoExpedienteElectronico == 2, si esa es la semántica confirmada legacy. Regla: if (transactionResult.ExpedienteUnidadResult?.TieneExpediente != true)
    return not executed;

if (transactionResult.IdRegistroProduccionDocumental is null or <= 0)
    return not executed;

if (transactionResult.ExpedienteUnidadResult.EstadoExpedienteElectronico != 2)
    return not executed; ⚠️ No reemplazar 2 por 1 si la paridad legacy exige 2 . ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — ExpedienteIndiceXmlRepository Objetivo Replicar: Solicita_archivo_indice_expediente(id_expediente, Ruta_archivo_xml) Reglas Debe consultar la ruta real del XML del expediente. No debe hardcodear rutas. No debe retornar ruta vacía. Debe validar alias e id expediente. Debe usar QueryOptions o SQL parametrizado. Implementación conceptual public sealed class ExpedienteIndiceXmlRepository : IExpedienteIndiceXmlRepository
{
    private readonly IDapperCrudEngine _engine;

    public ExpedienteIndiceXmlRepository(IDapperCrudEngine engine)
    {
        _engine = engine;
    }

    public async Task<ExpedienteIndiceXmlRouteModel?> GetXmlRouteAsync(
        int idExpediente,
        string defaultDbAlias)
    {
        if (idExpediente <= 0)
            throw new ArgumentException("IdExpediente inválido", nameof(idExpediente));

        if (string.IsNullOrWhiteSpace(defaultDbAlias))
            throw new ArgumentException("DefaultDbAlias requerido", nameof(defaultDbAlias));

        var query = new QueryOptions
        {
            TableName = "expediente_archivo",
            DefaultAlias = defaultDbAlias,
            Columns = new List<string>
            {
                "ID_EXPEDIENTE AS IdExpediente",
                "RUTA_ARCHIVO_XML AS RutaArchivoXml"
            },
            Filters = new Dictionary<string, object>
            {
                { "ID_EXPEDIENTE", idExpediente }
            },
            Limit = 1
        };

        var result = await _engine.GetAllAsync<ExpedienteIndiceXmlRouteModel>(query);

        if (!result.Success)
            throw new InvalidOperationException(result.ErrorMessage);

        return result.Data.FirstOrDefault();
    }
} Si la columna real no se llama RUTA_ARCHIVO_XML , ajustar al esquema real, pero conservar la intención: obtener la ruta del XML índice del expediente . ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — ExpedienteIndiceXmlBuilder Responsabilidad Construir el modelo XML usando: StorageContext
StorageTransactionResult
StorageNamingResult
StoragePhysicalPathModel
PhysicalMetadata
IdRegistroProduccionDocumental
ExpedienteUnidadResult Implementación esperada public sealed class ExpedienteIndiceXmlBuilder : IExpedienteIndiceXmlBuilder
{
    public ExpedienteIndiceXmlDocumentModel Build(
        StorageContext context,
        StorageTransactionResult transactionResult,
        StorageNamingResult naming,
        StoragePhysicalPathModel physicalPath)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (transactionResult == null)
            throw new ArgumentNullException(nameof(transactionResult));

        if (naming == null)
            throw new ArgumentNullException(nameof(naming));

        if (physicalPath == null)
            throw new ArgumentNullException(nameof(physicalPath));

        var expediente = context.Command.Expediente
            ?? throw new InvalidOperationException("Expediente requerido para XML índice");

        var physical = context.PhysicalMetadata
            ?? throw new InvalidOperationException("Metadata física requerida");

        var idRegistro = transactionResult.IdRegistroProduccionDocumental
            ?? throw new InvalidOperationException("IdRegistroProduccionDocumental requerido");

        var idExpediente = expediente.IdExpediente
            ?? throw new InvalidOperationException("IdExpediente requerido");

        var rutaDocumento = Path.Combine(
            physicalPath.RutaFinal,
            naming.NombreArchivoPrincipal
        );

        var numeroFolios = physical.NumeroPaginas;

        var paginaInicial = 1;
        var paginaFinal = numeroFolios;

        if (transactionResult.IndiceElectronicoResult != null)
        {
            paginaInicial = transactionResult.IndiceElectronicoResult.PaginaInicial;
            paginaFinal = transactionResult.IndiceElectronicoResult.PaginaFinal;
        }

        return new ExpedienteIndiceXmlDocumentModel
        {
            IdRegistroProduccionDocumental = idRegistro,
            IdAlmacen = transactionResult.IdentityReservation.Identity.IdAlmacen,
            IdExpediente = idExpediente,
            NombreDocumento = naming.NombreArchivoPrincipal,
            SegundoNombreDocumento = naming.SegundoNombre,
            TipologiaDocumental = context.Command.Trd?.NombreTipoDocumento ?? "NA",
            FechaDeclaracionDocumento = context.FechaEjecucion,
            FechaIncorporacionDocumento = context.FechaEjecucion,
            ValorHuella = GenerateSha256(idRegistro.ToString()),
            FuncionResumen = "SHA256",
            OrdenDocumentoExpediente = transactionResult.IndiceElectronicoResult?.OrdenDocumento ?? 0,
            PaginaInicial = paginaInicial,
            PaginaFinal = paginaFinal,
            Formato = physical.Formato,
            Tamano = physical.TamanoLegacy,
            RutaDocumento = rutaDocumento,
            NumeroFolios = numeroFolios,
            Origen = expediente.IdClaseDocumento
        };
    }

    private static string GenerateSha256(string input)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — ExpedienteIndiceXmlWriter Objetivo Actualizar XML existente de forma segura. Reglas Validar existencia del XML. Cargar XML existente. Preservar estructura actual. Agregar nodo DocumentoIndizado . Usar escritura temporal .tmp . Reemplazar archivo final de forma segura. No corromper XML si falla. No escribir fuera de ruta permitida. No loguear contenido completo. Implementación esperada public sealed class ExpedienteIndiceXmlWriter : IExpedienteIndiceXmlWriter
{
    private readonly ILogger<ExpedienteIndiceXmlWriter> _logger;

    public ExpedienteIndiceXmlWriter(
        ILogger<ExpedienteIndiceXmlWriter> logger)
    {
        _logger = logger;
    }

    public async Task<ExpedienteIndiceXmlUpdateResult> UpdateAsync(
        string rutaArchivoXml,
        ExpedienteIndiceXmlDocumentModel document)
    {
        if (string.IsNullOrWhiteSpace(rutaArchivoXml))
            throw new InvalidOperationException("RutaArchivoXml requerida");

        if (!File.Exists(rutaArchivoXml))
            throw new FileNotFoundException("Archivo XML índice no existe", rutaArchivoXml);

        if (document == null)
            throw new ArgumentNullException(nameof(document));

        var temp = rutaArchivoXml + ".tmp";

        var xml = new XmlDocument
        {
            PreserveWhitespace = true
        };

        xml.Load(rutaArchivoXml);

        var ns = xml.DocumentElement?.NamespaceURI ?? "";

        var parent = xml.GetElementsByTagName("tipodocumentoFoliado")
            .OfType<XmlElement>()
            .FirstOrDefault();

        if (parent == null)
            parent = xml.DocumentElement;

        if (parent == null)
            throw new InvalidOperationException("XML índice no tiene nodo raíz válido");

        var node = xml.CreateElement("DocumentoIndizado", ns);

        Append(xml, node, "Id", document.IdRegistroProduccionDocumental.ToString(), ns);
        Append(xml, node, "NombreDocumento", document.NombreDocumento, ns);
        Append(xml, node, "TipologiaDocumental", document.TipologiaDocumental, ns);
        Append(xml, node, "FechaDeclaracionDocumento", document.FechaDeclaracionDocumento.ToString("yyyy-MM-dd"), ns);
        Append(xml, node, "FechaIncorporacionDocumento", document.FechaIncorporacionDocumento.ToString("yyyy-MM-dd"), ns);
        Append(xml, node, "ValorHuella", document.ValorHuella, ns);
        Append(xml, node, "FuncionResumen", document.FuncionResumen, ns);
        Append(xml, node, "OrdenDocumentoExpediente", document.OrdenDocumentoExpediente.ToString(), ns);
        Append(xml, node, "PaginaInicial", document.PaginaInicial.ToString(), ns);
        Append(xml, node, "PaginaFinal", document.PaginaFinal.ToString(), ns);
        Append(xml, node, "Formato", document.Formato, ns);
        Append(xml, node, "Tamano", document.Tamano, ns);
        Append(xml, node, "RutaDocumento", document.RutaDocumento, ns);
        Append(xml, node, "NumeroFolios", document.NumeroFolios.ToString(), ns);
        Append(xml, node, "SegundoNombre", document.SegundoNombreDocumento, ns);

        parent.AppendChild(node);

        xml.Save(temp);

        File.Copy(temp, rutaArchivoXml, overwrite: true);
        File.Delete(temp);

        _logger.LogInformation(
            "XML índice expediente actualizado idExpediente={idExpediente} idRegistro={idRegistro}",
            document.IdExpediente,
            document.IdRegistroProduccionDocumental
        );

        await Task.CompletedTask;

        return new ExpedienteIndiceXmlUpdateResult
        {
            Updated = true,
            RutaArchivoXml = rutaArchivoXml,
            Estado = "UPDATED"
        };
    }

    private static void Append(
        XmlDocument xml,
        XmlElement parent,
        string name,
        string? value,
        string ns)
    {
        var child = xml.CreateElement(name, ns);
        child.InnerText = value ?? "";
        parent.AppendChild(child);
    }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — ExpedienteIndiceXmlService public sealed class ExpedienteIndiceXmlService : IExpedienteIndiceXmlService
{
    private readonly IExpedienteIndiceXmlRepository _repository;
    private readonly IExpedienteIndiceXmlBuilder _builder;
    private readonly IExpedienteIndiceXmlWriter _writer;
    private readonly ILogger<ExpedienteIndiceXmlService> _logger;

    public ExpedienteIndiceXmlService(
        IExpedienteIndiceXmlRepository repository,
        IExpedienteIndiceXmlBuilder builder,
        IExpedienteIndiceXmlWriter writer,
        ILogger<ExpedienteIndiceXmlService> logger)
    {
        _repository = repository;
        _builder = builder;
        _writer = writer;
        _logger = logger;
    }

    public async Task<ExpedienteIndiceXmlUpdateResult> ExecuteAsync(
        StorageContext context,
        StorageTransactionResult transactionResult,
        StorageNamingResult naming,
        StoragePhysicalPathModel physicalPath)
    {
        var expResult = transactionResult.ExpedienteUnidadResult;

        if (expResult?.TieneExpediente != true)
        {
            return new ExpedienteIndiceXmlUpdateResult
            {
                Updated = false,
                Estado = "NO_EXPEDIENTE"
            };
        }

        if (transactionResult.IdRegistroProduccionDocumental is null or <= 0)
        {
            return new ExpedienteIndiceXmlUpdateResult
            {
                Updated = false,
                Estado = "NO_REGISTRO_PRODUCCION"
            };
        }

        if (expResult.EstadoExpedienteElectronico != 2)
        {
            return new ExpedienteIndiceXmlUpdateResult
            {
                Updated = false,
                Estado = "NO_EXPEDIENTE_ELECTRONICO_LEGACY"
            };
        }

        var idExpediente = context.Command.Expediente?.IdExpediente
            ?? throw new InvalidOperationException("IdExpediente requerido");

        var route = await _repository.GetXmlRouteAsync(
            idExpediente,
            context.DefaultDbAlias
        );

        if (route == null || string.IsNullOrWhiteSpace(route.RutaArchivoXml))
            throw new InvalidOperationException("No se encontró ruta XML del expediente");

        var model = _builder.Build(
            context,
            transactionResult,
            naming,
            physicalPath
        );

        var result = await _writer.UpdateAsync(
            route.RutaArchivoXml,
            model
        );

        _logger.LogInformation(
            "Servicio XML índice expediente finalizado requestId={requestId} estado={estado}",
            context.RequestId,
            result.Estado
        );

        return result;
    }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTEGRACIÓN EN ORCHESTRATOR Este flujo debe ejecutarse después de: - Commit DB exitoso
- FileSystem exitoso
- XML documental FXL generado Y antes del cierre final del Storage Engine. Ejemplo: var expedienteXmlResult = await _expedienteIndiceXmlService.ExecuteAsync(
    context,
    txResult,
    naming,
    physicalPath
); Si falla: - Ejecutar CompensationManager si aplica.
- Registrar inconsistencia post-commit.
- No dejar error silencioso. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTEGRACIÓN CON COMPENSATION MANAGER Agregar soporte para XML índice expediente: public List<string> XmlIndiceExpedienteTemporales { get; init; } = new();
public List<string> XmlIndiceExpedienteFinales { get; init; } = new(); Regla: Si falla la actualización del XML índice después del commit DB,
se debe registrar estado de inconsistencia para reconciliación. No asumir rollback DB porque ya hubo commit. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS CRÍTICAS Activar solo con expediente electrónico legacy. Respetar EstadoExpedienteElectronico == 2 si esa es la regla VB. No actualizar XML si no hay IdRegistroProduccion. No actualizar XML si no hay expediente. No crear XML nuevo si legacy exige XML existente. Usar escritura segura .tmp . No corromper XML existente. Preservar namespace. Preservar estructura. No loguear XML completo. No guardar rutas sensibles completas en logs. Manejar fallo post-commit como inconsistencia recuperable. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DEPENDENCY INJECTION services.AddScoped<IExpedienteIndiceXmlRepository, ExpedienteIndiceXmlRepository>();
services.AddScoped<IExpedienteIndiceXmlBuilder, ExpedienteIndiceXmlBuilder>();
services.AddScoped<IExpedienteIndiceXmlWriter, ExpedienteIndiceXmlWriter>();
services.AddScoped<IExpedienteIndiceXmlService, ExpedienteIndiceXmlService>(); ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS Unitarias — Service sin expediente → no ejecuta sin IdRegistroProduccion → no ejecuta EstadoExpedienteElectronico != 2 → no ejecuta ruta XML no encontrada → error ruta XML vacía → error ejecución válida → writer llamado Unitarias — Builder context null → error transactionResult null → error naming null → error physicalPath null → error sin PhysicalMetadata → error sin IdRegistroProduccion → error sin IdExpediente → error genera SHA256 mapea páginas desde IndiceElectronicoResult si existe mapea ruta documento mapea segundo nombre Unitarias — Writer ruta vacía → error archivo no existe → error XML sin raíz → error XML con tipodocumentoFoliado → inserta bajo ese nodo XML sin tipodocumentoFoliado → inserta bajo raíz preserva namespace genera .tmp reemplaza final elimina .tmp Integración XML real de expediente inserción real de nodo DocumentoIndizado preservación de estructura fallo XML → compensación/reporte compatibilidad con visor legacy si aplica Regresión Legacy Comparar contra: Solicita_archivo_indice_expediente
Actualiza_archivo_xml_indice_expediente
DocumentoIndizado
ValorHuella
FuncionResumen
OrdenDocumentoExpediente
PaginaInicial
PaginaFinal
RutaDocumento
SegundoNombre ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBSERVABILIDAD Logs obligatorios: requestId
idExpediente
idRegistroProduccionDocumental
estadoExpedienteElectronico
estadoXml
updated
duración No loguear: contenido XML completo
rutas completas sensibles
fulltext
contenido documental ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN ENTERPRISE Ruta: Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/ Crear: SCRUM-[ID]-Arquitectura-XmlIndiceExpediente.md
SCRUM-[ID]-Implementacion-Detallada-XmlIndiceExpediente.md
SCRUM-[ID]-Pruebas-XmlIndiceExpediente.md
SCRUM-[ID]-Observabilidad-XmlIndiceExpediente.md
SCRUM-[ID]-Regresion-Legacy-XmlIndiceExpediente.md
SCRUM-[ID]-Metadata.md Debe incluir: comparación VB vs C# regla EstadoExpedienteElectronico == 2 estructura XML esperada ejemplo de XML antes/después estrategia .tmp manejo de fallos post-commit integración con CompensationManager riesgos de corrupción XML estrategia de recuperación ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN Obtiene ruta real XML expediente. Valida existencia del XML. Actualiza XML existente. Inserta nodo DocumentoIndizado . Preserva estructura y namespace. Usa SHA256. Usa datos reales del índice. Respeta EstadoExpedienteElectronico == 2 . No ejecuta si no hay expediente. No ejecuta si no hay IdRegistroProduccion. Maneja fallo post-commit. Integra CompensationManager. Hay pruebas unitarias. Hay pruebas de integración. Hay regresión comparativa con VB. Hay documentación enterprise. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RESTRICCIONES No modificar lógica de expediente/unidad aquí. No modificar inventario aquí. No modificar naming aquí. No modificar FileWriter principal aquí. No abrir transacción DB aquí. No modificar función legacy. No crear XML nuevo salvo decisión explícita. No usar MD5. No loguear XML completo. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INSTRUCCIÓN FINAL Implementar: ExpedienteIndiceXmlRepository
ExpedienteIndiceXmlBuilder
ExpedienteIndiceXmlWriter
ExpedienteIndiceXmlService
Modelos
DI
Integración Orchestrator
Integración CompensationManager
Pruebas
Documentación El resultado debe quedar listo para: PROMPT 20 — Logdocuarchi Workflow Legacy-Compatible

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-185-implementacion-indice-expediente-electro.