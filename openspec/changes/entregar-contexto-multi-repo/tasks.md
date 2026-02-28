## 1. Contexto consolidado

- [x] 1.1 Crear `openspec/context/multi-repo-context.md` con inventario de repos.
- [x] 1.2 Incluir branch, ultimo commit y remotos por repo.
- [x] 1.3 Incluir artefactos de build y referencias `ProjectReference`.

## 2. Entrega a OpenSpec

- [x] 2.1 Crear `proposal.md` para el cambio `entregar-contexto-multi-repo`.
- [x] 2.2 Crear `design.md` y referenciar el contexto consolidado.
- [x] 2.3 Dejar este cambio listo para validacion OpenSpec.

## 3. Operacion continua

- [x] 3.1 Automatizar la regeneracion con `Tools/Generate-OpenSpecMultiRepoContext.ps1`.
- [x] 3.2 Exigir referencia al contexto con reglas en `openspec/config.yaml` y gate local `Tools/Validate-OpenSpecContextReference.ps1`.
