## Context

- Jira issue key: SCRUM-204
- Jira summary: IMPLEMENTACION-API-SOLICITA-DOCUMENTO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-204

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

Se requiere una API de visualización de documento para frontend que reemplace el comportamiento de
`Genera_Matris_Documentos_Almacenados` del legacy VB, manteniendo compatibilidad funcional con naming/ruta legacy,
pero con arquitectura por capas y contrato tipado.

Regla central de negocio: el frontend siempre debe recibir una única URL temporal para visualizar/descargar un único
archivo. Si el documento físico está compuesto por matriz TIF/multi-imagen, el backend debe consolidar a PDF temporal
y entregar esa URL.

## Legacy Reference

- `D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\Docuarchi\ClassVisualisaDocumento.vb`
  - `Genera_Matris_Documentos_Almacenados`
- `D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\Docuarchi\ClassDaGabinete.vb`
  - `Genera_Matriz_Documentos`
  - `Consulta_Documentos_Añadidos`
  - `Suma_Numero_Documentos_Añadidos`

## Scope

### In Scope

- Endpoint de resolución de documento visualizable (URL temporal única).
- Endpoint de descarga por token temporal.
- Resolución de documento principal y añadidos por `DBT`.
- Consolidación TIF/multi-imagen a PDF temporal.
- Reutilización explícita de servicios/repositorios actuales.
- Documentación técnica y contrato frontend.

### Out of Scope

- Cambios en estructura de tablas legacy de gabinete.
- Reescritura del motor de almacenamiento principal.
- Persistencia permanente de PDF consolidado (solo temporal).
- Cambios globales de autenticación/autorización.

## Reuse Strategy

Se debe reutilizar obligatoriamente:

- `MiApp.Services\Service\GestorDocumental\AlmacenamientoDocumental\Physical\StoragePhysicalPathService.cs`
- `MiApp.Services\Service\GestorDocumental\AlmacenamientoDocumental\Physical\StorageFolderLegacyPolicy.cs`
- `MiApp.Services\Service\GestorDocumental\AlmacenamientoDocumental\Physical\StoragePathResolver.cs`
- `MiApp.Repository\Repositorio\GestorDocumental\AlmacenamientoDocumental\StorageRoute\StorageRouteRepository.cs`
- `MiApp.Repository\Repositorio\GestorDocumental\Documentos\ReemplazoPdf\IReemplazoPdfDocumentLocationRepository.cs`
- `MiApp.Services\Service\GestorDocumental\AlmacenamientoDocumental\Naming\IStorageExtensionResolver.cs`
- `MiApp.Repository\Repositorio\GestorDocumental\AlmacenamientoDocumental\Extension\IStorageExtensionRepository.cs`
- `MiApp.Services\Service\GestorDocumental\AlmacenamientoDocumental\Naming\IStorageNamingService.cs`
- `MiApp.Services\Service\GestorDocumental\AlmacenamientoDocumental\TemporaryUpload\StorageUploadPathResolver.cs`
- `AppResponses<T>`, `AppMeta`, `AppError`

## Architecture Decisions

### AD-01 Controller especializado en Documentos

Crear `VisualizacionDocumentoController` en `GestorDocumental/Documentos`, separado del controller de almacenamiento.

### AD-02 Servicio orquestador único de visualización

Crear `VisualizacionDocumentoService` para encapsular:

- validaciones
- resolución de rutas legacy
- decisión ORIGINAL vs TIF_CONSOLIDADO
- emisión de token temporal
- respuesta final para frontend

### AD-03 Token temporal para descarga

No exponer rutas absolutas. El endpoint de resolución devuelve `UrlTemporal` y `ExpiresAt`.

### AD-04 Contrato único para frontend

La respuesta de resolución siempre representa un único archivo final. Para TIF/multiimagen, el archivo final debe ser
`application/pdf`.

## Main Flow

1. Validar claims (`defaulalias` y reglas de seguridad vigentes).
2. Validar `nombreGabinete` e `idDocumento`.
3. Obtener ubicación base (`ID`, `DISC`, `PAG`, `IDEX`, `DBT`) en gabinete.
4. Resolver ruta física legacy del documento.
5. Detectar origen:
   - PDF/original único: publicar URL temporal del archivo.
   - TIF/multiimagen: generar PDF temporal y publicar URL temporal del consolidado.
6. Retornar `AppResponses<VisualizacionDocumentoResponseDto>`.

## Risk Assessment

- Riesgo de path traversal: mitigar con `StoragePathResolver` + validación root.
- Riesgo de consumo alto en consolidación: mitigar con límites de tamaño/páginas y stream.
- Riesgo de inconsistencia legacy de nombres: mitigar reutilizando naming y folder policies actuales.
- Riesgo de exposición de rutas físicas: mitigar con token temporal obligatorio.

## Validation Strategy

- Unit tests de service:
  - PDF original
  - TIF consolidado
  - archivo inexistente
  - token expirado
- Tests de controller:
  - claim inválido
  - request inválido
  - success
- Prueba integración E2E de descarga temporal.

## Impacted Repositories

- `DocuArchi.Api` (controller nuevo)
- `MiApp.Services` (servicio visualización + consolidación PDF temporal)
- `MiApp.Repository` (consulta añadidos por DBT si no existe)
- `MiApp.DTOs` (DTOs request/response)
- `DocuArchiCore` (documentación técnica/OpenSpec)

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de arquitectura y pruebas de `OPSXJ_BACKEND_RULES`.
- Definir alcance/no-alcance antes de implementar.
- Validar con `openspec.cmd validate scrum-204-implementacion-api-solicita-documento`.
