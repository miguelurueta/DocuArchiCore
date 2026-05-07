## 1. Preparación de Auditoría

- [ ] 1.1 Confirmar alcance de `SCRUM-192` como auditoría/cierre (sin desarrollo nuevo).
- [ ] 1.2 Levantar fuentes obligatorias: legacy consolidado, OpenSpec 177-191, docs `SCRUM-189-*`.
- [ ] 1.3 Confirmar repos impactados y marcar `implementation_required` vs `traceability_only` en `sync.md`.

## 2. Matriz de Paridad VB vs C#

- [ ] 2.1 Construir matriz completa por comportamiento legacy con columnas obligatorias (estado/severidad/evidencia/riesgo/acción).
- [ ] 2.2 Trazar función legacy origen por cada fila y su equivalente C#.
- [ ] 2.3 Clasificar desviaciones y registrar justificación técnica cuando aplique `CUMPLE CON DESVIACIÓN APROBADA`.
- [ ] 2.4 Validar regla crítica: cero `NO CUMPLE` críticos para habilitar `GO`.

## 3. Evidencia de Pruebas

- [ ] 3.1 Ejecutar/verificar escenarios E2E obligatorios del ticket (simple, preindex, inventario, TRD, expediente/unidad, workflow, fallos, concurrencia).
- [ ] 3.2 Consolidar evidencias de prueba (resultado, logs, archivos afectados, evidencia de rollback/compensación).
- [ ] 3.3 Verificar paridad de mensajes/efectos críticos vs legacy en escenarios de error.

## 4. Controles Técnicos Transversales

- [ ] 4.1 Verificar observabilidad mínima (`requestId`, `idAlmacen`, `idRegistroProduccionDocumental`, fase, duración, código error).
- [ ] 4.2 Verificar feature flag `StorageEngineV2` y estrategia de fallback/rollback operativo.
- [ ] 4.3 Verificar DI completo y ausencia de stubs/placeholders activos.

## 5. Documentación de Cierre (SCRUM-189)

- [ ] 5.1 Actualizar `SCRUM-189-Arquitectura-Cierre-Integral.md`.
- [ ] 5.2 Actualizar `SCRUM-189-Matriz-Paridad-StorageEngine.md`.
- [ ] 5.3 Actualizar `SCRUM-189-Pruebas-E2E-StorageEngine.md`.
- [ ] 5.4 Actualizar `SCRUM-189-Riesgos-Desviaciones-Aprobadas.md`.
- [ ] 5.5 Actualizar `SCRUM-189-Go-NoGo-StorageEngine.md`.
- [ ] 5.6 Actualizar `SCRUM-189-Metadata.md`.

## 6. Cierre OpenSpec

- [ ] 6.1 Validar change con `openspec validate scrum-192-implementacion-auditoria-integral-almace`.
- [ ] 6.2 Confirmar checklist de revisión OpenSpec previo a publish/archive.
- [ ] 6.3 Publicar (`opsxj:orchestrate:publish`) y cerrar (`opsxj:orchestrate:archive`) según estado de merges.
