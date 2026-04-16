# SCRUM-139 — Implementación Detallada — Configuración Upload

## Archivos

- `DocuArchi.Api/Controllers/GestorDocumental/ConfiguracionUpload/SolicitaEstructuraConfiguracionUploadController.cs`
- `MiApp.Services/Service/GestorDocumental/ConfiguracionUpload/ServiceSolicitaEstructuraConfiguracionUpload.cs`
- Reusa repository/model existentes del flujo SCRUM-138.

## Controller

- Ruta base: `api/gestor-documental/configuracion-upload`
- Validaciones:
  - Claim `defaulalias`
  - `nameProceso` no vacío
- Respuestas:
  - `BadRequest` si claim o `nameProceso` inválidos o si `service` retorna `success=false`
  - `Ok` si `success=true`

## Service

- Valida `defaultDbAlias` y `nameProceso`
- Llama repository `SolicitaEstructuraConfiguracionUploadNameProcesoAsync`
- Normaliza salida a `AppResponses<List<RaConfiguracionUploadModel>>`

