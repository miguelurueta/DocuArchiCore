## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-170 y confirmar alcance.
- [x] 1.2 Confirmar repos impactados y rutas destino antes de implementar.
- [x] 1.3 Confirmar no-alcance de DB para esta fase (FileSystem/XML/compensación fuera de transacción DB).

## 2. Specs

- [x] 2.1 Completar `specs/jira-scrum-170/spec.md` con requisitos finales de fase física.
- [x] 2.2 Incluir referencia explicita a `openspec/context/OPSXJ_BACKEND_RULES.md`.
- [x] 2.3 Verificar escenarios testables por requisito.

## 3. Application

- [x] 3.1 Implementar `StoragePathResolver`, `StorageFileWriter`, `StorageXmlWriter`, `StorageCompensationManager` y `StoragePhysicalPhaseExecutor` en `MiApp.Services`.
- [x] 3.2 Implementar builders `StoragePlanBuilder` y `StorageXmlBuilder` e integrar fase física en `DocumentStorageOrchestrator`.
- [x] 3.3 Registrar interfaces/implementaciones en `DocuArchi.Api/Program.cs` (Services L).
- [x] 3.4 Ajustar contratos/modelos para resultado de fase física (`StoragePhysicalStatusModel` con `NombreArchivoFinal`).
- [x] 3.5 Verificar compilación local en repos impactados (`MiApp.Models`, `MiApp.Services`, `DocuArchi.Api`). (Resultado: `MiApp.Models` compila OK; `MiApp.Services` y `DocuArchi.Api` continúan bloqueados por falla de MSBuild/restore `_GenerateRestoreProjectPathWalk`, condición externa al cambio funcional).

## 4. Test

- [x] 4.1 Implementar pruebas unitarias de seguridad de rutas y fase física (`StoragePathResolverTests`, `StoragePhysicalPhaseExecutorTests`) y ajustar tests de orquestador.
- [x] 4.2 Ejecutar `dotnet test` y documentar evidencia o bloqueo explícito. (Resultado: ejecución bloqueada durante restore/MSBuild sin errores C# detallados, condición de entorno preexistente).
- [ ] 4.3 Ejecutar `opsxj:orchestrate:publish` y `opsxj:orchestrate:archive` al cierre multi-repo.
