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
