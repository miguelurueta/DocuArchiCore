## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-169 y confirmar alcance.
- [x] 1.2 Confirmar repos impactados y rutas destino antes de implementar.
- [x] 1.3 Confirmar estructura de tablas objetivo (`expediente_archivo`, `unidad_conservacion`, `ra_cert_indice_expediente`) en `Docs/DataModel/StorageEngine-Tables.md`.

## 2. Specs

- [x] 2.1 Completar `specs/jira-scrum-169/spec.md` con requisitos finales de fase archivistica transaccional.
- [x] 2.2 Incluir referencia explicita a `openspec/context/OPSXJ_BACKEND_RULES.md`.
- [x] 2.3 Verificar escenarios testables por requisito.

## 3. Application

- [x] 3.1 Implementar modelos `ExpedienteInfoModel`, `UnidadConservacionInfoModel`, `IndiceElectronicoInsertModel` y `IndiceElectronicoCalculationResult` en `MiApp.Models`.
- [x] 3.2 Implementar `IExpedienteRepository`/`ExpedienteRepository`, `IUnidadConservacionRepository`/`UnidadConservacionRepository` e `IIndiceElectronicoRepository`/`IndiceElectronicoRepository` en `MiApp.Repository` usando `DapperCrudEngine + QueryOptions`.
- [x] 3.3 Implementar `IIndiceElectronicoCalculator`/`IndiceElectronicoCalculator` e `IIndiceElectronicoBuilder`/`IndiceElectronicoBuilder` en `MiApp.Services`.
- [x] 3.4 Integrar la fase archivistica en `StorageTransactionCoordinator` con orden fijo: lock expediente -> lock unidad -> calcular indice -> update folios/indice -> insert indice -> commit.
- [x] 3.5 Registrar DI en `DocuArchi.Api/Program.cs` para repositorios y servicios nuevos.
- [x] 3.6 Verificar compilacion local en repos impactados (`MiApp.Models`, `MiApp.Repository`, `MiApp.Services`, `DocuArchi.Api`). (Resultado: `MiApp.Models` compila OK; los otros 3 proyectos están bloqueados por fallo de MSBuild `_GetProjectReferenceTargetFrameworkProperties` sin errores de código reportados, condición preexistente del entorno).

## 4. Test

- [x] 4.1 Implementar pruebas unitarias de calculator/builder y coordinator para casos success/rollback.
- [x] 4.2 Implementar pruebas de repository para lock/update/insert con validaciones de `QueryOptions` y propagacion transaccional.
- [x] 4.3 Ejecutar `dotnet test` y registrar evidencia (o bloqueo explicito externo al alcance). (Resultado: ejecución intentada; bloqueo en build MSBuild `_GetProjectReferenceTargetFrameworkProperties` con salida `ERROR al compilar` y sin errores C# detallados).
- [ ] 4.4 Actualizar `sync.md` y estado multi-repo con PRs satelite reales.
- [x] 4.5 Completar documentacion tecnica SCRUM-169 (arquitectura, implementacion, pruebas, observabilidad, concurrencia, seguridad hash, metadata).
- [ ] 4.6 Ejecutar `opsxj:orchestrate:publish` y `opsxj:orchestrate:archive` al cierre de merges.
