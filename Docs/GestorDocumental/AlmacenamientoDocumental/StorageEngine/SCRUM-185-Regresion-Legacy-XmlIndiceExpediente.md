# SCRUM-185 — Regresión Legacy XML Índice Expediente

## Función legacy de referencia
- `Solicita_archivo_indice_expediente`
- `Actualiza_archivo_xml_indice_expediente`
- `Registra_archivo_xml_indice_expediente`

## Paridad VB vs C#
| Comportamiento legacy VB | Implementación C# |
|---|---|
| Resolver ruta de índice por estructura de expedientes | `ExpedienteIndiceXmlRepository.GetXmlRouteAsync` |
| Crear/actualizar XML de índice con nodo de documento | `ExpedienteIndiceXmlWriter.UpdateAsync` |
| Incorporar metadatos documentales (tipología, folios, hash, ruta) | `ExpedienteIndiceXmlBuilder.Build` |
| Ejecutar solo para expediente electrónico | `ExpedienteIndiceXmlService.ExecuteAsync` |
| Si falla XML índice, no perder almacenamiento principal | `StoragePhysicalPhaseExecutor` marca inconsistencia post-commit |

## Diferencia controlada
- Legacy podía escribir XML en el mismo bloque de transacción lógica del proceso completo.
- En C# se conserva documento físico y se reporta inconsistencia post-commit para observabilidad y remediación.

## Riesgo residual
- Si la ruta XML legacy no existe o está inconsistente, el documento principal queda persistido y el índice requiere corrección operativa.

