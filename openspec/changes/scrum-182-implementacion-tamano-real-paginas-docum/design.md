## Context

- Jira issue key: SCRUM-182
- Jira summary: IMPLEMENTACION-TAMANO-REAL-PAGINAS-DOCUMENTO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-182

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE PROMPT 16 — Cálculo de tamaño real + conteo real de páginas (FASE CRÍTICA — PARIDAD FUNCIONAL LEGACY) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actúa como Arquitecto Master Backend .NET experto en: DocuArchi legacy análisis físico de documentos conteo real de páginas PDF/TIFF/imagen cálculo de tamaño documental normalización de formato FileSystem seguro Clean Architecture migración VB → C# con paridad funcional sistemas ECM críticos ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Restaurar la paridad funcional del cálculo físico documental que realizaba la función legacy Almacenamiento . La nueva implementación debe calcular correctamente: - tamaño total real del documento
- tamaño legible en Kb/Mb
- formato/extensión real
- número real de páginas cuando aplique El resultado debe alimentar: StorageMetadataModel
InventarioDocumental
Gabinete dinámico
Índice electrónico
XML documental
Log workflow ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ BRECHA FUNCIONAL DETECTADA Comparativo actual: VB legacy:
- Calcula tamaño acumulado de Matri_Dcoumentos
- Convierte tamaño a Kb/Mb
- Obtiene extensión real del archivo principal
- Calcula páginas reales mediante Retorna_numero_paginas_documentos_unificados
- Si puede calcular páginas reales, reemplaza Numero_Pag

C# actual:
- Tamano queda string.Empty
- Usa NumeroPaginasDeclaradas o suma DTO
- No lee páginas reales del archivo
- No replica formato Kb/Mb Esta brecha impacta: - inventario documental
- índice electrónico
- XML
- visor legacy
- auditoría documental ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REFERENCIA LEGACY CONTEXTO LEGAZY: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\Architecture\Almacenamiento\funcion-almacena-consolidad.txt VB: Dim tam_archivo As Object = 1024

For i As Integer = 0 To Matri_Dcoumentos.Length - 1
    Dim fi As New FileInfo(Matri_Dcoumentos(i))
    If fi.Exists Then
        tam_archivo = tam_archivo + fi.Length
    End If
Next

If (tam_archivo / 1024) > 1024 Then
    tamano = Math.Round(((tam_archivo / 1024) / 1024), 2).ToString() & " Mb"
Else
    tamano = Math.Round((tam_archivo / 1024), 2).ToString() & " Kb"
End If

Dim f2 As New FileInfo(Matri_Dcoumentos(0))
tipo = UCase(f2.Extension)

Result = Class_ItexShare.Retorna_numero_paginas_documentos_unificados(
    Matri_Dcoumentos(0),
    numero_pagina
)

If numero_pagina <> -1 Then
    pagi = numero_pagina
End If ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UBICACIÓN ESPERADA Services / Metadata MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Metadata/ Archivos esperados: IStorageDocumentMetadataAnalyzer.cs
StorageDocumentMetadataAnalyzer.cs
IStoragePageCountReader.cs
StoragePageCountReader.cs
IStorageSizeFormatter.cs
StorageSizeFormatter.cs Models MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/Metadata/ Archivos esperados: StorageDocumentPhysicalMetadata.cs ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ MODELOS StorageDocumentPhysicalMetadata public sealed class StorageDocumentPhysicalMetadata
{
    public long TotalBytes { get; init; }

    public string TamanoLegacy { get; init; } = "";

    public string Formato { get; init; } = "";

    public int NumeroPaginas { get; init; }

    public bool PaginasCalculadasDesdeArchivo { get; init; }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTERFACES IStorageDocumentMetadataAnalyzer public interface IStorageDocumentMetadataAnalyzer
{
    Task<StorageDocumentPhysicalMetadata> AnalyzeAsync(
        StorageContext context,
        IReadOnlyList<string> archivosOrigen
    );
} IStoragePageCountReader public interface IStoragePageCountReader
{
    Task<int?> TryReadPageCountAsync(string filePath);
} IStorageSizeFormatter public interface IStorageSizeFormatter
{
    string FormatLegacy(long totalBytes);
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — StorageSizeFormatter Reglas Debe replicar formato legacy: - Si tamaño > 1024 Kb → "X Mb"
- Si tamaño <= 1024 Kb → "X Kb"
- Redondeo a 2 decimales
- Mantener sufijos "Mb" y "Kb" Implementación esperada public sealed class StorageSizeFormatter : IStorageSizeFormatter
{
    public string FormatLegacy(long totalBytes)
    {
        if (totalBytes < 0)
            throw new InvalidOperationException("Tamaño inválido");

        var kb = totalBytes / 1024d;

        if (kb > 1024)
        {
            var mb = kb / 1024d;
            return $"{Math.Round(mb, 2)} Mb";
        }

        return $"{Math.Round(kb, 2)} Kb";
    }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — StoragePageCountReader Reglas Debe intentar obtener páginas reales de: - PDF
- TIFF/TIF multipágina
- imágenes simples como 1 página Si no puede determinar: return null; No debe lanzar excepción por formato no soportado, salvo archivo inexistente. Implementación esperada conceptual public sealed class StoragePageCountReader : IStoragePageCountReader
{
    private readonly ILogger<StoragePageCountReader> _logger;

    public StoragePageCountReader(ILogger<StoragePageCountReader> logger)
    {
        _logger = logger;
    }

    public Task<int?> TryReadPageCountAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Archivo no encontrado", filePath);

        var ext = Path.GetExtension(filePath).ToLowerInvariant();

        try
        {
            if (ext == ".pdf")
            {
                // Implementar con librería disponible del proyecto.
                // Ejemplo: PdfPig, iTextSharp/iText7 o lector existente.
                // return Task.FromResult<int?>(pdfPageCount);
                return Task.FromResult<int?>(null);
            }

            if (ext == ".tif" || ext == ".tiff")
            {
                // Implementar conteo multipágina si la plataforma lo soporta.
                // return Task.FromResult<int?>(frameCount);
                return Task.FromResult<int?>(null);
            }

            if (ext is ".jpg" or ".jpeg" or ".png" or ".bmp")
            {
                return Task.FromResult<int?>(1);
            }

            return Task.FromResult<int?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "No fue posible calcular páginas reales file={file}",
                Path.GetFileName(filePath)
            );

            return Task.FromResult<int?>(null);
        }
    }
} ⚠️ Si existe una clase equivalente a Class_ItexShare.Retorna_numero_paginas_documentos_unificados , reutilizarla mediante adapter: IStoragePageCountReader → LegacyPageCountAdapter ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — StorageDocumentMetadataAnalyzer Responsabilidad Construir metadata física consolidada: TotalBytes
TamanoLegacy
Formato
NumeroPaginas
PaginasCalculadasDesdeArchivo Reglas Debe validar lista de archivos. Debe sumar tamaños reales. Debe sumar desde todos los archivos de entrada. Debe usar el archivo principal para formato. Debe intentar conteo real del archivo principal. Si no puede contar páginas reales, usar context.Command.NumeroPaginasDeclaradas . Si ambos fallan, error. Debe actualizar StorageMetadataModel o dejar resultado listo para builder. Implementación esperada public sealed class StorageDocumentMetadataAnalyzer : IStorageDocumentMetadataAnalyzer
{
    private readonly IStoragePageCountReader _pageCountReader;
    private readonly IStorageSizeFormatter _sizeFormatter;
    private readonly ILogger<StorageDocumentMetadataAnalyzer> _logger;

    public StorageDocumentMetadataAnalyzer(
        IStoragePageCountReader pageCountReader,
        IStorageSizeFormatter sizeFormatter,
        ILogger<StorageDocumentMetadataAnalyzer> logger)
    {
        _pageCountReader = pageCountReader;
        _sizeFormatter = sizeFormatter;
        _logger = logger;
    }

    public async Task<StorageDocumentPhysicalMetadata> AnalyzeAsync(
        StorageContext context,
        IReadOnlyList<string> archivosOrigen)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (archivosOrigen == null || archivosOrigen.Count == 0)
            throw new InvalidOperationException("No existen archivos para analizar");

        long totalBytes = 1024; // paridad legacy: VB inicia en 1024

        foreach (var file in archivosOrigen)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException("Archivo no encontrado", file);

            var info = new FileInfo(file);
            totalBytes += info.Length;
        }

        var principal = archivosOrigen[0];

        var formato = Path.GetExtension(principal).ToUpperInvariant();

        var pageCount = await _pageCountReader.TryReadPageCountAsync(principal);

        var paginas = pageCount ?? context.Command.NumeroPaginasDeclaradas;

        if (paginas <= 0)
            throw new InvalidOperationException("No fue posible determinar número de páginas");

        var tamano = _sizeFormatter.FormatLegacy(totalBytes);

        _logger.LogInformation(
            "Metadata física calculada requestId={requestId} formato={formato} paginas={paginas} totalBytes={totalBytes}",
            context.RequestId,
            formato,
            paginas,
            totalBytes
        );

        return new StorageDocumentPhysicalMetadata
        {
            TotalBytes = totalBytes,
            TamanoLegacy = tamano,
            Formato = formato,
            NumeroPaginas = paginas,
            PaginasCalculadasDesdeArchivo = pageCount.HasValue
        };
    }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTEGRACIÓN CON STORAGE METADATA Actualizar StorageMetadataModel para que pueda recibir: Tamano = physicalMetadata.TamanoLegacy
Formato = physicalMetadata.Formato
NumeroPaginas = physicalMetadata.NumeroPaginas El resultado debe alimentar: - GabineteInsertModel.Paginas
- InventarioInsertModel.NumeroFolios
- InventarioInsertModel.Tamano
- InventarioInsertModel.Formato
- IndiceElectronicoInsertModel.NumeroFolios
- IndiceElectronicoInsertModel.Tamano
- IndiceElectronicoInsertModel.Formato
- XML documental
- Log workflow ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTEGRACIÓN CON IDENTITY ALLOCATOR ⚠️ Importante: La reserva de identidad usa número de páginas. Por tanto, el cálculo de páginas debe ocurrir ANTES de: IdentityAllocator Ajustar flujo: ValidationPipeline
→ Preindex
→ MetadataAnalyzer
→ IdentityAllocator
→ TransactionCoordinator
→ FileSystem/XML Si el TransactionCoordinator actualmente usa: context.Command.NumeroPaginasDeclaradas Debe cambiar a: context.PhysicalMetadata.NumeroPaginas Agregar en StorageContext : public StorageDocumentPhysicalMetadata? PhysicalMetadata { get; set; } ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTEGRACIÓN EN ORCHESTRATOR Actualizar Orchestrator: var physicalMetadata = await _metadataAnalyzer.AnalyzeAsync(
    context,
    archivosOrigen
);

context.PhysicalMetadata = physicalMetadata; Debe ejecutarse antes de TransactionCoordinator . ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DEPENDENCY INJECTION services.AddScoped<IStorageDocumentMetadataAnalyzer, StorageDocumentMetadataAnalyzer>();
services.AddScoped<IStoragePageCountReader, StoragePageCountReader>();
services.AddScoped<IStorageSizeFormatter, StorageSizeFormatter>(); ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS CRÍTICAS No dejar Tamano vacío. No dejar Formato vacío. No usar páginas declaradas si se pueden calcular páginas reales. No reservar identidad antes de conocer páginas reales. Mantener paridad de tamaño con VB: iniciar total en 1024 bytes. Formato debe estar en mayúscula. Si no hay páginas reales, usar páginas declaradas. Si no hay páginas válidas, fallar antes de transacción. No calcular páginas dentro de transacción. No mantener archivos abiertos. No loguear rutas completas sensibles. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS Unitarias — StorageSizeFormatter 1024 bytes → 1 Kb 1048576 bytes → 1024 Kb redondeo a 2 decimales tamaño negativo → error Unitarias — StorageDocumentMetadataAnalyzer lista vacía → error archivo inexistente → error suma tamaños correcta inicia suma en 1024 bytes formato mayúscula páginas reales reemplazan declaradas páginas null usa declaradas páginas <= 0 → error Unitarias — StoragePageCountReader imagen simple → 1 formato no soportado → null archivo inexistente → error PDF con lector real → páginas correctas si aplica Integración PDF real multipágina imagen real TIFF multipágina si aplica flujo completo antes de IdentityAllocator Regresión Legacy Comparar contra VB: tam_archivo
tamano
tipo
numero_pagina
pagi Resultado C# debe ser equivalente. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBSERVABILIDAD Logs obligatorios: requestId
formato
numeroPaginas
paginasCalculadasDesdeArchivo
totalBytes
tamanoLegacy
duración No loguear: ruta completa
contenido del documento
fulltext
datos sensibles ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN ENTERPRISE Ruta: Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/ Crear: SCRUM-[ID]-Arquitectura-MetadataFisica.md
SCRUM-[ID]-Implementacion-Detallada-MetadataFisica.md
SCRUM-[ID]-Pruebas-MetadataFisica.md
SCRUM-[ID]-Observabilidad-MetadataFisica.md
SCRUM-[ID]-Regresion-Legacy-MetadataFisica.md
SCRUM-[ID]-Metadata.md Debe incluir: comparación VB vs C# explicación de suma inicial 1024 bytes explicación del formato Kb/Mb flujo antes de IdentityAllocator impacto en system1 / NUMPAG_CARP impacto en inventario impacto en índice electrónico impacto en XML limitaciones de librería para conteo PDF/TIFF deuda técnica si no existe lector real ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN Calcula tamaño real. Formatea tamaño como VB. Calcula o resuelve páginas antes de IdentityAllocator. No deja Tamano vacío. No deja Formato vacío. Actualiza StorageContext.PhysicalMetadata . TransactionCoordinator usa páginas reales. Inventario usa tamaño/formato reales. Índice usa tamaño/formato reales. XML usa tamaño/formato reales. Log workflow puede usar tamaño/formato reales. Hay pruebas unitarias. Hay pruebas de integración. Hay regresión comparativa con VB. Hay documentación enterprise. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RESTRICCIONES No implementar inventario aquí. No implementar expediente aquí. No implementar XML aquí. No implementar naming aquí. No abrir transacción aquí. No acceder a DB aquí. No modificar función legacy. No usar rutas arbitrarias desde frontend. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INSTRUCCIÓN FINAL Implementar: StorageDocumentMetadataAnalyzer
StoragePageCountReader
StorageSizeFormatter
StorageDocumentPhysicalMetadata
Actualización StorageContext
Integración Orchestrator antes de TransactionCoordinator
Pruebas
Documentación El resultado debe quedar listo para:

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-182-implementacion-tamano-real-paginas-docum.