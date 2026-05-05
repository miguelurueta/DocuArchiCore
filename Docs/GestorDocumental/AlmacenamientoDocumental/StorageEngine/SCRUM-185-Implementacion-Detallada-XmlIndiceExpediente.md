# SCRUM-185 — Implementación Detallada XML Índice de Expediente

## Cambios por capa

## Models (`MiApp.Models`)
- `ExpedienteXml/ExpedienteIndiceXmlRouteModel.cs`
- `ExpedienteXml/ExpedienteIndiceXmlDocumentModel.cs`
- `ExpedienteXml/ExpedienteIndiceXmlUpdateResult.cs`
- `StorageCompensationPlan`:
  - `XmlIndiceExpedienteTemporales`
  - `XmlIndiceExpedienteFinales`
- `StoragePhysicalStatusModel`:
  - `ExpedienteIndiceXmlResult`
  - `InconsistenciaPostCommit`

## Repository (`MiApp.Repository`)
- `ExpedienteXml/IExpedienteIndiceXmlRepository.cs`
- Implementación `ExpedienteIndiceXmlRepository`:
  - consulta `ra_ruta_expediente` (`DISCO`, `RUTA`),
  - zero-fill `DISCO`/`ID_EXPEDIENTE` a 9,
  - arma `<ruta>\<disco>\{expediente}.xml`.

## Services (`MiApp.Services`)
- `ExpedienteXml/IExpedienteIndiceXmlBuilder.cs`
- `ExpedienteXml/ExpedienteIndiceXmlBuilder.cs`
- `ExpedienteXml/IExpedienteIndiceXmlWriter.cs`
- `ExpedienteXml/ExpedienteIndiceXmlWriter.cs`
- `ExpedienteXml/IExpedienteIndiceXmlService.cs`
- `ExpedienteXml/ExpedienteIndiceXmlService.cs`

## Integración fase física
- `Physical/StoragePhysicalPhaseExecutor.cs`
  - inyección opcional de `IExpedienteIndiceXmlService`,
  - ejecución post-copy/post-xml principal,
  - captura de error como `POST_COMMIT_INCONSISTENCY`.
- `Physical/StorageCompensationManager.cs`
  - borrado de artefactos XML índice en compensación.

## API (`DocuArchi.Api`)
- `Program.cs`: registro DI de repository + builder + writer + service.

## Reglas legacy preservadas
- Nombre documento `DIG...`, segundo nombre y tipología.
- Hash huella (`SHA256`) para valor de integridad.
- Inserción XML bajo `tipodocumentoFoliado`.
- No bloqueo de almacenamiento principal por falla de actualización de índice.

