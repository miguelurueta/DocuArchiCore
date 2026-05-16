# SCRUM-202 Implementación Técnica Reemplazo PDF

## 1. Resumen de Implementación
Se implementó un endpoint dedicado para reemplazo de PDF en módulo `Documentos`, desacoplado de `AlmacenamientoDocumental`, con auditoría transversal reutilizable.

## 2. Endpoint
- Ruta: `/api/gestor-documental/documentos/reemplazopdf`
- Método: `POST`
- Controller: `ReemplazoPdfController`
- Autorización: JWT Bearer

## 3. Artefactos Implementados
### 3.1 API
- `DocuArchi.Api/Controllers/GestorDocumental/Documentos/ReemplazoPdfController.cs`

### 3.2 Services
- `MiApp.Services/Service/GestorDocumental/Documentos/ReemplazoPdf/IReemplazoPdfService.cs`

### 3.3 Repository
- `MiApp.Repository/Repositorio/GestorDocumental/Documentos/ReemplazoPdf/IReemplazoPdfDocumentLocationRepository.cs`
- `MiApp.Repository/Repositorio/GestorDocumental/Common/Audit/ILogDocuarchiRepository.cs`

### 3.4 Models
- `MiApp.Models/Models/GestorDocumental/Documentos/ReemplazoPdf/StorageDocumentLocationModel.cs`
- `MiApp.Models/Models/GestorDocumental/Common/Audit/LogDocuarchiEntryModel.cs`

### 3.5 Wiring DI
- `DocuArchi.Api/Program.cs`

## 4. Reglas funcionales implementadas
- Solo acepta reemplazo de archivos PDF.
- Exige sesión temporal `Completed`.
- Exige documento existente en gabinete (`ID`).
- Ubica ruta final con política legacy de disco/carpeta.
- Ejecuta backup previo antes del overwrite.
- Verifica hash posterior para confirmar integridad.
- Inserta auditoría técnica en `logdocuarchi`.
- Permite que frontend envíe campos de auditoría (`DescOp`, `ModuloRegistro`, `Radicado`, `IdTareaWorkflow`, `IdRutaWorkflow`, `TipologiaDocumental`) y usa defaults solo cuando no llegan.

## 5. Refactor aplicado en SCRUM-202
- Se removió repositorio de log específico de `ReemplazoPdf`.
- Se creó repositorio genérico `ILogDocuarchiRepository` para reutilización transversal.
- `WorkflowStorageLogRepository` pasó a consumir `ILogDocuarchiRepository` con inserción transaccional (`InsertBeginTransAsync`).

## 6. Manejo de Excepciones
- Se aplicó patrón `try/catch` en funciones nuevas de controller, service y repositorios nuevos del SCRUM.
- Errores de validación regresan `AppResponses` con `Status=validation`.
- Errores no controlados regresan `Status=error` y quedan en log.

## 7. Contrato de auditoría `logdocuarchi`
### 7.1 Campos recibidos desde request
- `DescOp`
- `ModuloRegistro`
- `Radicado`
- `IdTareaWorkflow`
- `IdRutaWorkflow`
- `TipologiaDocumental`

### 7.2 Reglas de mapeo
- `DescOp`: request o default `REEMPLAZO_PDF_STORAGE_ENGINE`.
- `ModuloRegistro`: request o default `DOCUARCHI`.
- `Radicado`: request (nullable).
- `IdTareaWorkflow`: request o `0`.
- `IdRutaWorkflow`: request o `0`.
- `TipologiaDocumental`: request o fallback `TIPODOCUMENTO` consultado del gabinete.
## 8. Tabla de compatibilidad
| Componente | Antes | Después |
|---|---|---|
| Endpoint reemplazo | Dentro de almacenamiento | Controller dedicado `Documentos/ReemplazoPdf` |
| Auditoría logdocuarchi | Acoplada a ReemplazoPdf | Repositorio genérico común |
| Workflow log | Insert directo en repo workflow | Delegación a `ILogDocuarchiRepository` |

## 9. Casos de error cubiertos
- Claim `defaulalias` ausente
- Claim `usuarioid` inválido
- Archivo temporal no existe
- Archivo temporal no es PDF
- Documento no existe en gabinete
- Carpeta física no existe
- Archivo `DIG########` no existe
- Hash final distinto al temporal
- Falla al insertar auditoría
- `IdTareaWorkflow` o `IdRutaWorkflow` negativos
## 10. Pruebas unitarias relacionadas
- `ReemplazoPdfControllerTests`
- `StorageDocumentReplacementServiceTests`
- `WorkflowStorageLogRepositoryTests`
- `WorkflowStorageLogServiceTests`

## 11. Evidencia de verificación
Comando ejecutado:
```powershell
dotnet test tests\TramiteDiasVencimiento.Tests\TramiteDiasVencimiento.Tests.csproj --filter "FullyQualifiedName~ReemplazoPdfControllerTests|FullyQualifiedName~StorageDocumentReplacementServiceTests|FullyQualifiedName~WorkflowStorageLogRepositoryTests|FullyQualifiedName~WorkflowStorageLogServiceTests"
```
Resultado:
- `10/10` pruebas superadas.
