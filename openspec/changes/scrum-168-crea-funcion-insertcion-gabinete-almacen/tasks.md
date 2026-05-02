## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-168 y confirmar alcance.
- [x] 1.2 Confirmar repos impactados y rutas destino antes de crear Controllers/DTOs/Models/funciones.
- [x] 1.3 Solicitar estructura de tabla si se requiere nuevo modelo.

## 2. Specs

- [x] 2.1 Completar specs/jira-scrum-168/spec.md con requisitos finales.
- [x] 2.2 Incluir referencia explicita a openspec/context/OPSXJ_BACKEND_RULES.md.
- [x] 2.3 Verificar escenarios testables por requisito.

## 3. Application

- [ ] 3.1 Implementar `GabineteInsertModel` y `InventarioInsertModel` en `MiApp.Models`.
- [ ] 3.2 Implementar `IGabineteStorageRepository`/`GabineteStorageRepository` y `IInventarioDocumentalRepository`/`InventarioDocumentalRepository` en `MiApp.Repository` con DapperCrudEngine + QueryOptions.
- [ ] 3.3 Integrar ambos repositorios en `StorageTransactionCoordinator` para commit/rollback unico y retorno de `IdRegistroProduccionDocumental`.
- [ ] 3.4 Registrar DI de repositorios/servicios en `DocuArchi.Api/Program.cs` y validar compilacion en repos impactados.

## 4. Test

- [ ] 4.1 Implementar unit tests para validaciones de gabinete/inventario y flujo transaccional coordinator (success + rollback).
- [ ] 4.2 Implementar integration/concurrency tests para insercion en gabinete + inventario dentro de misma transaccion.
- [ ] 4.3 Ejecutar suites `dotnet test` y registrar evidencia (o skip explicito por Docker no disponible).
- [ ] 4.4 Actualizar `sync.md` y publicar PR de `MiApp.Repository` cuando exista diff funcional real.
- [ ] 4.5 Ejecutar `opsxj:orchestrate:archive` al completar merges y validaciones finales.
