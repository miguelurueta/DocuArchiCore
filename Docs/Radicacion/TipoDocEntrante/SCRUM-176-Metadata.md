# SCRUM-176 - Metadata

- Ticket: `SCRUM-176`
- Modulo: `Radicacion / Tramite`
- Tabla relacionada: `tipo_doc_entrante`
- Endpoint: `GET /api/radicacion/tramite/tipo-doc-entrante/{idTipoDocEntrante}`
- Repos impactados:
  - `DocuArchi.Api`
  - `MiApp.Services`
  - `DocuArchiCore` (tests/docs)
- Estado: `En revisión` (hasta merge/archivo)
- Fecha: `2026-04-30`
- Observacion: `RemitDestInterno` no aplica para esta API; el origen correcto es `tipo_doc_entrante`.
