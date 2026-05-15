# SCRUM-201 - Implementación Detallada

## 1. Archivos implementados

## 1.1 API
- `DocuArchi.Api/Controllers/Workflow/UsuarioWorkflow/FirmaTemporalUsuarioWorkflowController.cs`
- `DocuArchi.Api/Program.cs` (registro DI del nuevo servicio)

## 1.2 Services
- `MiApp.Services/Service/Workflow/Usuario/ServiceFirmaTemporalUsuarioWorkflow.cs`

## 1.3 Repository
- `MiApp.Repository/Repositorio/Workflow/usuario/UsuarioWorkflowR.cs` (nuevo método de firma)

## 1.4 DTO
- `MiApp.DTOs/DTOs/Workflow/Usuario/FirmaTemporalUsuarioWorkflowDto.cs`

## 1.5 Tests
- `tests/TramiteDiasVencimiento.Tests/FirmaTemporalUsuarioWorkflowControllerTests.cs`
- `tests/TramiteDiasVencimiento.Tests/ServiceFirmaTemporalUsuarioWorkflowTests.cs`

## 2. Contrato técnico implementado

## 2.1 Endpoint metadata
- Método: `GET`
- Ruta: `/api/workflow/usuarios/firma-temporal`
- Respuesta: `AppResponses<FirmaTemporalUsuarioWorkflowDto?>`

## 2.2 Endpoint download
- Método: `GET`
- Ruta: `/api/workflow/usuarios/firma-temporal/download/{token}`
- Respuesta: binario (`File(...)`) con `Content-Type: image/png`.

## 2.3 Claims usados
- `defaulaliaswf`: alias de conexión workflow.
- `IdUsuarioWorkflow`: identidad workflow y control de descarga.

## 3. Lógica interna por capa

## 3.1 Controller
- Rechaza request si faltan claims requeridos.
- Parsea `IdUsuarioWorkflow` como entero positivo.
- Llama service para generar metadata temporal.
- Llama service para resolver token en descarga.
- Endpoints envueltos en `try/catch` para respuesta controlada de errores.

## 3.2 Service
- `SolicitaFirmaTemporalAsync`:
  - valida alias e id
  - consulta blob de firma
  - valida null/vacío/tamaño máximo
  - detecta formato de entrada por cabecera (`png/jpg/bmp`)
  - normaliza salida a `PNG` y aplica transparencia en píxeles cercanos a blanco
  - genera archivo temporal en `signatures` con extensión `.png`
  - genera token aleatorio y guarda cache con expiración absoluta
  - retorna DTO de metadata
- `TryResolveFirmaTemporal`:
  - valida token + id workflow + expiración + existencia de archivo
  - retorna `path/contentType/fileName` para descarga
  - envuelto en `try/catch` para evitar excepciones no controladas en descarga

## 3.3 Repository
- Método `SolicitaFirmaUsuarioWorkflowAsync(int idUsuarioWorkflow, string defaultDbAlias)`.
- Consulta `usuario_workflow` con filtros:
  - `idU_suario = @idUsuarioWorkflow`
- Columnas explícitas:
  - `idU_suario`
  - `Firma_Usuario`

## 4. Reglas de seguridad implementadas
- No se retorna `Firma_Usuario` en JSON.
- No se retorna path absoluto en DTO.
- Validación de path bajo root temporal para evitar traversal.
- Token temporal asociado al `IdUsuarioWorkflow` del claim.

## 5. Configuración funcional
- Root temporal: reusa `IStorageUploadPathResolver.GetTempRoot()`.
- Subcarpeta de firmas: `signatures`.
- TTL token: 15 minutos.
- Tamaño máximo firma: 5 MB.

## 6. Trazabilidad de errores
- `validation`:
  - claim faltante/invalidado
  - firma inexistente o vacía
  - formato no soportado
- `error`:
  - excepciones inesperadas de IO/infraestructura

## 7. Checklist de verificación manual
1. Token JWT contiene `defaulaliaswf` e `IdUsuarioWorkflow`.
2. `GET /firma-temporal` retorna `success=true`.
3. `UrlTemporal` responde binario con `200`.
4. Al expirar TTL, `download` retorna `404`.
