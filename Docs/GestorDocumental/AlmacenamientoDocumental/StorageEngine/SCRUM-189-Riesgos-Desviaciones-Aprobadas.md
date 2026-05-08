# SCRUM-189 - Riesgos y Desviaciones Aprobadas

## Riesgos Identificados
| ID | Riesgo | Severidad | Evidencia | Impacto | Mitigación | Estado |
|---|---|---|---|---|---|---|
| R-01 | `StorageEngineV2=false` no enruta a adapter legacy (retorna error controlado) | Alta | [AlmacenamientoDocumentalController.cs:69](D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\Controllers\GestorDocumental\AlmacenamientoDocumental\AlmacenamientoDocumentalController.cs:69), [AlmacenamientoDocumentalController.cs:74](D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\Controllers\GestorDocumental\AlmacenamientoDocumental\AlmacenamientoDocumentalController.cs:74) | Riesgo de continuidad si se requiere rollback funcional inmediato | Implementar adapter legacy o ruta de rollback operativa en runbook | Abierto |
| R-02 | Escenarios parity integración/concurrencia dependen de Docker y quedaron SKIP | Media | Ejecución de `dotnet test` StorageEngine (2026-05-07), 129 OK / 2 SKIP | Cobertura E2E incompleta para comportamiento concurrente real | Ejecutar en runner CI con Docker/Testcontainers | Abierto |
| R-03 | Uso de `Task.CompletedTask` en componentes async sin operación await real | Baja | [TipoAlmacenamientoValidator.cs:18](D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Services\Service\GestorDocumental\AlmacenamientoDocumental\Validation\TipoAlmacenamientoValidator.cs:18), [ExpedienteIndiceXmlWriter.cs:101](D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Services\Service\GestorDocumental\AlmacenamientoDocumental\ExpedienteXml\ExpedienteIndiceXmlWriter.cs:101) | Riesgo bajo de mantenibilidad (no funcional) | Refactor menor de estilo async | Aceptado |

## Desviaciones Aprobadas
| ID | Desviación | Clasificación | Justificación | Revisión |
|---|---|---|---|---|
| D-01 | `Task.CompletedTask` en validadores/writer | CUMPLE CON DESVIACIÓN APROBADA | No altera lógica; patrón sync envuelto en contrato async | Próximo hardening técnico |
| D-02 | E2E parity suite parcial en ambiente local | CUMPLE CON DESVIACIÓN APROBADA | Pruebas pasaron sin fallos; faltan solo escenarios Docker | Cerrar en CI Docker |

## Riesgos Críticos
- Riesgos críticos abiertos: **0**
