# SCRUM-187 — Evidencia de Paridad

## Evidencia automatizada
- Suite: `tests/TramiteDiasVencimiento.Tests/StorageEngine/Parity/`
- Casos activos:
  - `Escenario01_AlmacenamientoSimple_DebeMantenerParidadCore`
  - `Escenario09_WorkflowActivo_DebeMantenerParidadLogdocuarchi`
  - `EscenarioFxl_DebeValidarAtributosLegacy`
  - `EscenarioSystem1_DebeValidarEstructuraMinimaDeIncrementos`
  - `EscenarioConcurrencia_DosSolicitudes_DebeGenerarIdentidadesUnicas`

## Evidencia de cobertura por artefacto
- DB:
  - verificación de incrementos `system1` (proxid/numpag_carp).
  - verificación de columnas críticas `logdocuarchi`.
- FileSystem:
  - fixture temporal aislada para estructura `{gabinete}{disco}/{carpeta}`.
- XML:
  - validación FXL por atributos requeridos legacy.

## Evidencia diferida
- Integración MySQL real/Testcontainers:
  - no ejecutada automáticamente en este entorno.
  - cubierta con tests `Skip` explícitos para ejecución en pipeline habilitado.
