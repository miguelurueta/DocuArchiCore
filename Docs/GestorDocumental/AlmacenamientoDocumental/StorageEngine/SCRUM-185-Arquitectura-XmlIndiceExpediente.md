# SCRUM-185 — Arquitectura XML Índice de Expediente

## Objetivo
Restaurar el comportamiento legacy de actualización del XML índice de expediente después de la persistencia física del documento.

## Componentes
- `IExpedienteIndiceXmlRepository` / `ExpedienteIndiceXmlRepository`
- `IExpedienteIndiceXmlBuilder` / `ExpedienteIndiceXmlBuilder`
- `IExpedienteIndiceXmlWriter` / `ExpedienteIndiceXmlWriter`
- `IExpedienteIndiceXmlService` / `ExpedienteIndiceXmlService`
- Integración en `StoragePhysicalPhaseExecutor`

## Decisiones
- La actualización del XML se ejecuta en fase física post-escritura.
- Se activa solo bajo condiciones legacy:
  - existe `ExpedienteUnidadResult`,
  - `TieneExpediente = true`,
  - `EstadoExpedienteElectronico = 2`,
  - `IdRegistroProduccionDocumental > 0`.
- La ruta XML se resuelve vía `ra_ruta_expediente` y zero-fill a 9 dígitos (disco/expediente).
- Fallas de XML índice se tratan como inconsistencia post-commit, sin revertir archivo principal.

## Flujo
1. `StoragePhysicalPhaseExecutor` finaliza copia + XML FXL principal.
2. Ejecuta `IExpedienteIndiceXmlService.ExecuteAsync`.
3. El service resuelve ruta (`ra_ruta_expediente`) y construye modelo documental.
4. Writer agrega nodo `DocumentoIndizado` en `tipodocumentoFoliado`.
5. El resultado se guarda en `StoragePhysicalStatusModel.ExpedienteIndiceXmlResult`.

