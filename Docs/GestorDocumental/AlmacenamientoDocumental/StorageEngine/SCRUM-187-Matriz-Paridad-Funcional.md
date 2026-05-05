# SCRUM-187 — Matriz de Paridad Funcional

| Comportamiento Legacy | Input de prueba | Expected Legacy | Resultado C# | Estado | Diferencia | Prompt que cubre | Evidencia |
|---|---|---|---|---|---|---|---|
| Almacenamiento simple | Documento PDF + metadata básica | Incrementa `proxid`; genera DIG/FXL | Snapshot core equivalente | CUMPLE | Ninguna | 13,14,16,21 | `StorageEngineParityTestSuite.Escenario01_...` |
| Workflow activo | `IdTareaWorkflow > 0` + `IdRutaWorkflow` | Inserta `logdocuarchi` con ruta y tipología | Mapeo de campos críticos equivalente | CUMPLE | Ninguna | 20,21 | `Escenario09_...` |
| XML FXL | Archivo FXL generado | Atributos `ID,DISC,PAG,DBT,...` presentes | Assert de atributos legacy | CUMPLE | Ninguna | 13,14,21 | `EscenarioFxl_...` |
| Update system1 | Estado before/after controlado | `proxid +1`, `numpag_carp + paginas` | Regla validada en helper DB | CUMPLE | Ninguna | 16,21 | `EscenarioSystem1_...` |
| Concurrencia mínima | 2 operaciones simultáneas | IDs sin colisión | IDs únicos validados | CUMPLE CON MEJORA | Falta lock DB real | 16,21 | `EscenarioConcurrencia_...` |
| E2E completo DB/FS/XML | Dataset legacy + Docker | Equivalencia completa por escenario | Caso diferido por ambiente | PENDIENTE DECISIÓN | Dependencia de Docker/dataset | 21 | Test marcado `Skip` |
