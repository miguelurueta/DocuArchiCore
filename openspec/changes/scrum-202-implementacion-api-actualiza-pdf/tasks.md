## 1. Refinamiento (analysis)

- [x] 1.1 Confirmar alcance: reemplazo de PDF existente desde temporal, sin migrar versionado legacy completo.
- [x] 1.2 Confirmar reuso obligatorio de StorageEngine (rutas físicas/temporales, validación traversal, path resolver).
- [x] 1.3 Confirmar tabla de auditoría existente `logdocuarchi` (sin crear nueva tabla).

## 2. Design/Spec

- [x] 2.1 Refinar `design.md` con arquitectura Controller->Service->Repository y modelo híbrido DB/FS.
- [x] 2.2 Refinar `specs/jira-scrum-202/spec.md` con requisitos funcionales medibles y escenarios testeables.
- [x] 2.3 Definir estrategia de backup temporal en `replacement-versions/{gabinete}/{idDocumento}/{timestamp}`.

## 3. Backend Implementation

- [x] 3.1 Crear endpoint `POST /api/gestor-documental/documentos/reemplazopdf` con `AppResponses` y `try/catch`.
- [x] 3.2 Implementar service de reemplazo reutilizando resolución de ruta temporal/física existente.
- [x] 3.3 Implementar backup previo a overwrite en root temporal existente.
- [x] 3.4 Implementar cálculo de `hashAnterior` y `hashNuevo` sobre archivos físico/temporal.
- [x] 3.5 Implementar repositorio de auditoría `logdocuarchi` con `DapperCrudEngine + QueryOptions`.
- [x] 3.6 Registrar DI en `Program.cs` para controller/service/repository nuevos.

## 4. Validation & Testing

- [x] 4.1 Unit tests: request inválido, extensión no PDF, backup/replace OK.
- [x] 4.2 Integration test: flujo completo temporal -> replace -> auditoría `logdocuarchi` (validado en pruebas manuales E2E).
- [x] 4.3 Ejecutar `dotnet test` en suites impactadas y registrar evidencia.

## 5. Documentation

- [x] 5.1 Documentar contrato API y flujo frontend de reemplazo.
- [x] 5.2 Documentar observabilidad y troubleshooting (fallo físico, fallo auditoría, inconsistencia híbrida).
- [x] 5.3 Documentar runbook de recuperación manual con respaldo temporal.

## 6. OpenSpec Flow

- [x] 6.1 Ejecutar `openspec validate scrum-202-implementacion-api-actualiza-pdf`.
- [x] 6.2 Preparar `orchestrate:publish` con tasks completos.
- [x] 6.3 Cerrar con `orchestrate:archive` tras merge multi-repo.
