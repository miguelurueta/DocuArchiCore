## 1. Discovery and Contracts

- [x] 1.1 Confirmar alcance funcional final: dos endpoints (`por radicado` y `por idTareaWorkflow`) sin `nombreRuta` en request.
- [x] 1.2 Confirmar reutilización de `ISolicitaEstructuraRutaWorkflowService` para resolver la única ruta activa.
- [x] 1.3 Confirmar estrategia de alias: `defaulaliaswf` obligatorio para workflow y para consulta de `configuracion_gabinete` en esta API.
- [x] 1.4 Definir contrato de salida `RadicadoGabineteWorkflowDto` con campos: `Radicado`, `IdTareaWorkflow`, `IdGabinete`, `NombreGabinete`, `EstadoExistenciaRadicado`.

## 2. OpenSpec Refinement

- [x] 2.1 Refinar `design.md` con arquitectura, flujo, validaciones, riesgos y criterios de aceptación.
- [x] 2.2 Refinar `specs/jira-scrum-207/spec.md` con escenarios testables por endpoint, validación, claims y compatibilidad.
- [x] 2.3 Ajustar `tasks.md` por fases implementables (contrato, código, pruebas, documentación).

## 3. Implementation - API Layer

- [x] 3.1 Crear `RadicadoGabineteWorkflowController` con:
- [x] 3.1.1 `GET /api/workflow/ruta-trabajo/radicados/{consecutivoRadicado}/gabinete`
- [x] 3.1.2 `GET /api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete`
- [x] 3.2 Validar claims requeridos (`defaulaliaswf`) y devolver `BadRequest` controlado cuando aplique.
- [x] 3.3 Mantener patrón `ActionResult<AppResponses<T>>` con semántica `YES/NO`.

## 4. Implementation - Service Layer

- [x] 4.1 Crear `IRadicadoGabineteWorkflowService` y `RadicadoGabineteWorkflowService`.
- [x] 4.2 Resolver ruta activa con `SolicitaEstructuraRutaWorkflowAsync` y `FirstOrDefault()`.
- [x] 4.3 Validar `Nombre_Ruta` con regex `^[A-Za-z0-9_]+$`.
- [x] 4.4 Orquestar consulta de fila workflow y consulta de `configuracion_gabinete`.
- [x] 4.5 Implementar fallback funcional:
- [x] 4.5.1 encontrado -> `EstadoExistenciaRadicado="YES"`
- [x] 4.5.2 no encontrado -> `EstadoExistenciaRadicado="NO"` con defaults

## 5. Implementation - Repository Layer

- [x] 5.1 Crear `IRadicadoGabineteWorkflowRepository` y `RadicadoGabineteWorkflowRepository`.
- [x] 5.2 Implementar consulta por radicado en `dat_adic_tar{ruta}` usando `QueryOptions`.
- [x] 5.3 Implementar consulta por `idTareaWorkflow` en `dat_adic_tar{ruta}` usando `QueryOptions`.
- [x] 5.4 Implementar consulta de `Nombre_Gabinete` en `configuracion_gabinete` por `id_Gabinete`.
- [x] 5.5 Garantizar cero SQL concatenando valores de entrada de usuario.
- [x] 5.6 Forzar error controlado cuando `EstadoExistenciaRadicado=YES` y `NombreGabinete` no se resuelve.

## 6. Cross-cutting and Registration

- [x] 6.1 Registrar interfaces y clases nuevas en `Program.cs`.
- [x] 6.2 Agregar mapeo AutoMapper si se usa modelo interno adicional.
- [x] 6.3 Añadir comentarios XML en contratos públicos nuevos.

## 7. Tests

- [x] 7.1 Unit tests Controller:
- [x] 7.1.1 claim faltante
- [x] 7.1.2 success por radicado
- [x] 7.1.3 success por idTareaWorkflow
- [x] 7.2 Unit tests Service:
- [x] 7.2.1 ruta activa no encontrada
- [x] 7.2.2 regex de `Nombre_Ruta` inválida
- [x] 7.2.3 resultado encontrado/no encontrado
- [x] 7.3 Unit tests Repository:
- [x] 7.3.1 consulta por radicado
- [x] 7.3.2 consulta por idTareaWorkflow
- [x] 7.3.3 consulta de gabinete
- [x] 7.4 Ejecutar `dotnet test` en proyectos impactados o documentar bloqueo real.

## 8. Documentation and Validation

- [x] 8.1 Crear documentación técnica de integración frontend con ejemplos request/response para ambos endpoints.
- [x] 8.2 Documentar reglas de claims, alias y validación obligatoria de `NombreGabinete`.
- [x] 8.3 Ejecutar validación OpenSpec del change antes de cierre.
