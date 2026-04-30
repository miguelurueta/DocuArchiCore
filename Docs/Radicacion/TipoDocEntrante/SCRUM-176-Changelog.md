# SCRUM-176 - Changelog

## 2026-04-30

### Funcional
- Se crea endpoint `GET /api/radicacion/tramite/tipo-doc-entrante/{idTipoDocEntrante}`.
- Se valida claim `defaulalias`.
- Se retorna `AppResponses<TipoDocEntrante>` (modelo completo).

### Backend
- Nuevo service `SolicitaEstructuraTipoDocEntranteService`.
- Nuevo controller `SolicitaEstructuraTipoDocEntranteController`.
- Registro DI en `DocuArchi.Api/Program.cs`.
- Reutilización de `ITipoDocEntranteR` para acceso de datos.

### Calidad
- Prueba unitaria focal del service actualizada al contrato full model.
- Documentación técnica expandida (arquitectura, implementación, integración, pruebas, observabilidad, metadata, diccionario, discrepancia y changelog).
