# SCRUM-176 - API SolicitaEstructuraTipoDocEntrante

## Objetivo

Exponer endpoint GET para consultar la estructura de `tipo_doc_entrante` por id, reutilizando la arquitectura actual `Controller -> Service -> Repository`.

## Implementacion

- Controller:
  - `DocuArchi.Api/Controllers/Radicacion/Tramite/SolicitaEstructuraTipoDocEntranteController.cs`
- Service:
  - `MiApp.Services/Service/Radicacion/Tramite/SolicitaEstructuraTipoDocEntranteService.cs`
- Repository:
  - Reutiliza `ITipoDocEntranteR.SolicitaEstructuraTipoDoEntrante(...)` en `MiApp.Repository/Repositorio/Radicador/Tramite/TipoDocEntranteR.cs`
- DI:
  - Registro en `DocuArchi.Api/Program.cs` de `ISolicitaEstructuraTipoDocEntranteService`

## Endpoint

- `GET /api/radicacion/tramite/tipo-doc-entrante/{idTipoDocEntrante}`
- Claim requerido: `defaulalias`

## Contrato

- Respuesta: `AppResponses<TipoDocEntranteParametroDto>`
- Incluye flags:
  - `UtilEnvioCorreoCertificado`
  - `UtilFirmaDigitalProtocoloRespuesta`
  - `UtilAgregaDigitalProtocoloRespuesta`

## Notas

- Se valida explícitamente que `RemitDestInterno` no aplica para esta API.
- Se mantiene compatibilidad con contratos actuales de radicación.
