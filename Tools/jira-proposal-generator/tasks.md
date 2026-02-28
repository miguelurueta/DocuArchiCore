## 1. Script y datos Jira

- [x] 1.1 Verificar/ajustar `scripts/fetch-jira.js` para devolver summary y description del issue
- [x] 1.2 Manejar errores de red/credenciales/issueKey inexistente con mensajes claros

## 2. Generacion de propuesta

- [x] 2.1 Implementar el paso de IA que transforma summary/description en propuesta OpenSpec
- [x] 2.2 Guardar la propuesta generada en `openspec/changes/<issueKey>/proposal.md`

## 3. Validacion y docs

- [x] 3.1 Agregar/actualizar tests de comportamiento para el generador (si aplica)
- [x] 3.2 Registrar evidencia de tests en OpenSpec

## 4. Comando unico

- [x] 4.1 Implementar `scripts/generate-proposal-from-jira.js`
- [x] 4.2 Agregar tests de `proposalGenerator`

## 5. Deteccion de repositorio por IA

- [ ] 5.1 Implementar deteccion automatica del repositorio objetivo usando la logica del requerimiento
- [ ] 5.2 Implementar fallback para solicitar repo o plantilla cuando no exista suficiente confianza
- [ ] 5.3 Incluir el contexto de clasificacion de repos (`DocuArchi.Api`, `DocuArchiCore`, `MiApp.DTOs`, `MiApp.Models`, `MiApp.Repository`, `MiApp.Services`) en la generacion
- [ ] 5.4 Agregar pruebas unitarias y de integracion para clasificacion + fallback

## 6. Flujo archive multi-repo

- [ ] 6.1 Detectar repos impactados y generar/verificar PR por repo
- [ ] 6.2 Bloquear archive cuando exista al menos un PR sin merge
- [ ] 6.3 Ejecutar archive local solo cuando todos los PR esten mergeados
- [ ] 6.4 Hacer push del archive en repositorio orquestador `DocuArchiCore`
- [ ] 6.5 Cambiar estado del ticket en Jira tras archive/push exitosos

## 7. Validaciones y trazabilidad

- [ ] 7.1 Bloquear flujo cuando ticket no tenga texto (summary/description vacios)
- [ ] 7.2 Registrar logs por etapa: deteccion, PR, merge, archive, push, Jira
- [ ] 7.3 Mostrar mensajes de error claros y accionables

## 8. Pruebas

- [ ] 8.1 Unit tests por modulo (Repository, LocalArchive, OrchestratorPush, Jira, Logging)
- [ ] 8.2 Integration tests del flujo completo multi-repo + Jira
- [ ] 8.3 Contract tests para transicion Jira y formato de PR
## Test Evidence

Run: 2026-02-24 local

```text
> npx vitest --run scripts/lib/jiraClient.test.js scripts/lib/proposalGenerator.test.js

 RUN  v4.0.18 D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore.react

✓ scripts/lib/proposalGenerator.test.js (1 test) 2ms
✓ scripts/lib/jiraClient.test.js (4 tests) 5ms

 Test Files  2 passed (2)
      Tests  5 passed (5)
   Start at  12:48:37
   Duration  1.03s (transform 80ms, setup 206ms, import 53ms, tests 7ms, environment 1.46s)
```
