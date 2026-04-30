# SCRUM-176 - Implementacion Detallada

## Archivos creados/modificados

### DocuArchi.Api
- `Controllers/Radicacion/Tramite/SolicitaEstructuraTipoDocEntranteController.cs` (nuevo)
- `Program.cs` (registro DI del nuevo service)

### MiApp.Services
- `Service/Radicacion/Tramite/SolicitaEstructuraTipoDocEntranteService.cs` (nuevo)

### DocuArchiCore
- `tests/TramiteDiasVencimiento.Tests/SolicitaEstructuraTipoDocEntranteServiceTests.cs` (nuevo)
- `Docs/Radicacion/TipoDocEntrante/SCRUM-176-CreaApi-SolicitaEstructuraTipoDocEntrante.md` (base)

## Endpoint
- `GET /api/radicacion/tramite/tipo-doc-entrante/{idTipoDocEntrante}`

## Contrato
- `AppResponses<TipoDocEntranteParametroDto>`

## Reutilizacion de datos
- Se reutiliza `ITipoDocEntranteR.SolicitaEstructuraTipoDoEntrante(...)` para acceso a `tipo_doc_entrante`.
