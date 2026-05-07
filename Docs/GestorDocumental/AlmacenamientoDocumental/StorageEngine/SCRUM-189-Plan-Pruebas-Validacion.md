# SCRUM-189 — Plan de Pruebas y Validación

## 1. Estrategia
- Unitarias: validadores, builders, policies, mappers.
- Integración: repos con MySQL/Testcontainers (cuando aplica).
- E2E: API -> UseCase -> Engine -> DB/FS/XML.
- Concurrencia: colisión `proxid`, bloqueo `FOR UPDATE`, consistencia de cuotas.
- Regresión: matriz de paridad VB vs C#.

## 2. Herramientas
- xUnit
- Moq
- Testcontainers.MySql
- Docker (runner habilitado)
- Fixtures temporales de FileSystem

## 3. Escenarios obligatorios
| Escenario | Objetivo | Estado objetivo |
|---|---|---|
| 1. Almacenamiento simple | validar `system1`, tabla gabinete, DIG/FXL | CUMPLE |
| 2. Batch preindex TXT | lectura e integración por orden | CUMPLE |
| 3. Batch preindex XMLS | parseo e integración equivalente | CUMPLE |
| 4. Inventario activo | insert en `registro_producion_documental` | CUMPLE |
| 5. TRD activa | persistencia de IDs/descr TRD | CUMPLE |
| 6. Expediente electrónico | folios + índice DB/XML | CUMPLE |
| 7. Unidad digitalizada | update folios digitalizados | CUMPLE |
| 8. Unidad electrónica | update folios electrónicos | CUMPLE |
| 9. Workflow activo | `logdocuarchi` completo | CUMPLE |
| 10. Falla FS post-commit | compensación y rastreo | CUMPLE CON MEJORA |
| 11. Falla XML índice | control de rollback lógico | CUMPLE CON MEJORA |
| 12. Concurrencia 2/5 | unicidad y estabilidad | PENDIENTE VALIDACIÓN |

## 4. Cobertura actual observada
- Pruebas unitarias extensas en `tests/TramiteDiasVencimiento.Tests`.
- Suite de paridad creada en SCRUM-187 (`StorageEngine/Parity`) con:
  - escenarios core/workflow/xml/system increment
  - escenarios Docker diferidos con `Skip` explícito.

## 5. Criterios de aceptación de pruebas
1. Sin errores en casos críticos de transacción y validación.
2. Evidencia de rutas y naming DIG/FXL.
3. Evidencia de inserciones condicionadas por options.
4. Evidencia de `logdocuarchi` con campos legacy críticos.
5. Matriz de paridad actualizada y trazable a prompts 2..21.

## 6. Criterios de bloqueo
- Falla en reserva de identidad concurrente.
- Falla en consistencia de `system1`/`disco_detalle`.
- Pérdida de trazabilidad entre DB y archivos.
- Divergencias no justificadas en matriz de paridad.

## 7. Comandos base
```powershell
dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj --filter "FullyQualifiedName~Storage"
dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj --filter "FullyQualifiedName~StorageEngine.Parity"
```
