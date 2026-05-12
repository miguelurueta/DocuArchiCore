## Context

- Jira issue key: SCRUM-199
- Jira summary: CORRECION-DA-EXTENSION-ALMACENAMIENTO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-199

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE Corrección de resolución técnica de archivo usando DA_EXTENSION (FASE CRÍTICA — PARIDAD LEGACY TÉCNICA VS TRD) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actúa como Arquitecto Master Backend .NET experto en: DocuArchi legacy DA_EXTENSION clasificación técnica de archivos Storage Engine Clean Architecture Dapper / QueryOptions validaciones tempranas separación archivística vs técnica migración VB → C# con paridad funcional ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTEXTO GENERAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Estamos corrigiendo un error funcional crítico del Storage Engine en DocuArchiCore (.NET), en el flujo de almacenamiento documental post-transacción. Error actual observado: Error fisico (FS/XML) Inner exception: No se pudo determinar extensión para tipo 43 Stack: StorageExtensionResolver.ResolveAsync(...)
StoragePlanBuilder.BuildFilePlanAsync(...)
StoragePhysicalPhaseExecutor.ExecuteAsync(...)
DocumentStorageOrchestrator.ExecuteAsync(...) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DIAGNÓSTICO CONFIRMADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ El flujo actual: usa TRD.IdTipoDocumento
para resolver extensión física/documental Eso es incorrecto. En legacy VB TRD.IdTipoDocumento
≠ tipo técnico de archivo La lógica correcta es: DA_EXTENSION Legacy correcto Buscar en: DA_EXTENSION por: ESTENSION = UCase(extensionArchivo) y usar: ESTADO_NORMAL
ESTADO_ADJUNTO
ESTADO_LINK ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLA FUNCIONAL OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ NO usar TRD.IdTipoDocumento
para:
- resolver extensión física
- resolver DBT
- naming físico SIEMPRE usar extensión real del archivo
+
lookup en DA_EXTENSION ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Alinear completamente el flujo C# con la lógica legacy VB para: ✔ Resolver clasificación técnica por extensión real ✔ Poblar DBT desde DA_EXTENSION.ESTADO_NORMAL ✔ Evitar fallo tardío en fase física ✔ Separar correctamente: TRD archivístico clasificación técnica de archivo ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REFERENCIA LEGACY OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Ruta: D:\imagenesda\GestorDocumental\promp\CORE-API\Almacenamiento\funcion-almacena-consolidad.txt :contentReference[oaicite:0]{index=0} FUNCIÓN VB BASE DE VERDAD SolicitaTipoArchivoDocuarchiExtension(
    EXTENSION,
    ByRef TIPO_ESTADO_NORMAL,
    ByRef TIPO_ESTADO_ADJUNTO,
    ByRef TYPO_ESTADO_LINK) QUERY LEGACY SELECT
    ESTADO_NORMAL,
    ESTADO_ADJUNTO,
    ESTADO_LINK,
    TIPO_ARCHIVO,
    PROGRAMA
FROM DA_EXTENSION
WHERE ESTENSION = UCase(EXTENSION) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ALCANCE TÉCNICO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Revisar flujo actual MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Builders/StoragePlanBuilder.cs

MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Naming/IStorageExtensionResolver.cs

MiApp.Repository/Repositorio/GestorDocumental/AlmacenamientoDocumental/Extension/IStorageExtensionRepository.cs Implementar estrategia legacy Resolver extensión desde: 1. metadata física
2. fallback request.Documentos[].Extension
3. normalización estricta Normalización: - mayúsculas/minúsculas
- con/sin punto Consultar DA_EXTENSION Usar: ESTENSION = extensión normalizada Obtener: ESTADO_NORMAL
ESTADO_ADJUNTO
ESTADO_LINK
TIPO_ARCHIVO
PROGRAMA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ MODELO OBLIGATORIO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Crear: TipoArchivoDocuArchiModel.cs Estructura sugerida: public sealed class TipoArchivoDocuArchiModel
{
    public string Extension { get; init; } = "";

    public string EstadoNormal { get; init; } = "";

    public string EstadoAdjunto { get; init; } = "";

    public string EstadoLink { get; init; } = "";

    public string TipoArchivo { get; init; } = "";

    public string Programa { get; init; } = "";
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLA DE NOMENCLATURA (OBLIGATORIA) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PROHIBIDO Resolve
Handler
Manager
Helper
Generic OBLIGATORIO Nombres relacionados con la funcionalidad. Ejemplos válidos: ConsultarTipoArchivoDocuArchiPorExtensionAsync
RepositorioTipoArchivoDocuArchi
ValidarExtensionDocumentoStorage IMPORTANTE NO usar términos en inglés Toda clase/interfaz/método nuevo debe escribirse en español técnico consistente. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CORRECCIÓN DE DBT EN GABINETE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Validar insert en: contabil Campos obligatorios: ID
DISC
PAG
DBT
IDEX
USER
DATE1
TIME1 Regla crítica DBT
=
DA_EXTENSION.ESTADO_NORMAL NO: TRD.IdTipoDocumento TIME1 Debe poblarse SIEMPRE. NOT NULL en DDL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ VALIDACIÓN TEMPRANA (CRÍTICO) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Mover validación de DA_EXTENSION a: ValidationPipeline
o
MetadataAnalyzer NO esperar a: StoragePhysicalPhaseExecutor Si no existe mapeo Fallar ANTES del commit: No existe mapeo en DA_EXTENSION para extensión X Debe ser: error de negocio
NO StoragePhysicalException post-commit ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBSERVABILIDAD ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Logs obligatorios: requestId
extension detectada
alias DB
nombreGabinete
idTipoDocumentoTRD (solo informativo)
resultado lookup DA_EXTENSION Mensajes claros: No existe mapeo en DA_EXTENSION para extensión PDF ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UNITARIAS — NORMALIZACIÓN Validar: pdf
.PDF
Pdf
.Pdf Resultado: PDF UNITARIAS — LOOKUP DA_EXTENSION Casos: - extensión existente → OK
- extensión inexistente → error
- DA_EXTENSION inconsistente → error UNITARIAS — DBT Caso crítico: TRD.IdTipoDocumento = 43
archivo = documento.pdf
DA_EXTENSION válido Resultado esperado: DBT = DA_EXTENSION.ESTADO_NORMAL
NO 43 UNITARIAS — MODELO GABINETE Validar: ID
DISC
PAG
DBT
IDEX
USER
DATE1
TIME1 INTEGRACIÓN — STORAGE ENGINE Validar: - flujo exitoso con extensión válida
- falla temprana sin DA_EXTENSION
- NO commit + compensación REGRESIÓN LEGACY Comparar contra: SolicitaTipoArchivoDocuarchiExtension(...) Validar: - DBT correcto
- naming correcto
- separación TRD vs técnico ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS ARQUITECTÓNICAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ TRD:
- clasificación archivística

DA_EXTENSION:
- clasificación técnica del archivo NO mezclar responsabilidades. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actualizar: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\GestorDocumental\AlmacenamientoDocumental\StorageEngine\ Archivos: SCRUM-189-Correccion-DA_EXTENSION.md
SCRUM-189-Pruebas-DA_EXTENSION.md
SCRUM-189-Regresion-Legacy-DA_EXTENSION.md
SCRUM-189-Metadata.md Debe incluir: - separación TRD vs DA_EXTENSION
- origen de DBT
- tabla de campos obligatorios contabil
- casos OK / error
- evidencia de pruebas ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ✔ lookup DA_EXTENSION correcto
✔ DBT correcto
✔ falla temprana sin DA_EXTENSION
✔ no más error físico tardío
✔ contabil correcto
✔ separación TRD vs técnico respetada
✔ pruebas en verde
✔ documentación actualizada
✔ sin regresiones ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RESTRICCIONES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ - NO cambiar contratos HTTP
- NO reescribir arquitectura completa
- NO introducir lógica de negocio no relacionada
- NO usar TRD para DBT ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ENTREGABLES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 1. lista de archivos modificados
2. resumen de cambios
3. resultado de pruebas
4. riesgos residuales
5. nota operativa:
   cómo mantener DA_EXTENSION sincronizada ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INSTRUCCIÓN FINAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Implementar la resolución técnica de archivos usando DA_EXTENSION como fuente de verdad para DBT y clasificación técnica documental, garantizando validación temprana y paridad exacta con el comportamiento legacy VB.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-199-correcion-da-extension-almacenamiento.