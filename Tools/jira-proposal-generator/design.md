## Context

Actualmente las propuestas de OpenSpec se redactan manualmente, lo que introduce demora y variabilidad. Se quiere un flujo estandarizado que, dado un `issueKey` de Jira, consulte el ticket, construya una propuesta con IA y la guarde en `openspec/changes/<issueKey>/proposal.md`.

## Goals / Non-Goals

**Goals:**
- Definir un flujo reproducible para generar propuestas OpenSpec desde Jira.
- Establecer como se obtienen los datos del ticket (summary/description) y como se persistira la propuesta.
- Mantener el flujo compatible con la estructura de OpenSpec existente.
- Determinar automaticamente el repositorio objetivo del cambio a partir de la logica del requerimiento.
- Solicitar repo o plantilla al usuario cuando la IA no pueda clasificar con confianza.
- Completar el flujo OpenSpec hasta fases `Application` y `Test`.
- Garantizar que `archive` solo se ejecute cuando todos los PR de repos impactados esten mergeados.
- Asegurar cierre operacional: archive local, push en orquestador `DocuArchiCore` y transicion Jira.

**Non-Goals:**
- No implementar un UI para ejecutar el generador.
- No cambiar el esquema de OpenSpec ni la estructura de carpetas base.
- No cubrir autenticacion/rotacion de credenciales de Jira mas alla de lo necesario para el script.

## Decisions

- **Usar un script Node para consulta de Jira**: permite reutilizar la infraestructura existente en `scripts/` y mantiene el flujo automatizable en CI o local.
  - Alternativas: llamada directa desde una accion de OpenSpec o un servicio externo. Se descartan por mayor acoplamiento o complejidad.
- **Generacion con un paso de IA**: el contenido de la propuesta se construye a partir del summary y description del ticket.
  - Alternativas: plantilla estatica sin IA. Se descarta por menor calidad y mas edicion manual.
- **Salida en `openspec/changes/<issueKey>/proposal.md`**: alinea la propuesta con el flujo de cambios de OpenSpec.
  - Alternativas: guardar en una carpeta temporal o en un repositorio externo. Se descarta por dificultar el seguimiento del cambio.
- **Clasificacion de repo con fallback guiado**: la IA intenta detectar el repo; si la confianza no es suficiente, solicita confirmacion al usuario.
  - Alternativas: seleccionar siempre manualmente.
  - Razon: mejora velocidad sin perder control cuando el requerimiento es ambiguo.
- **Orquestacion de archive multi-repo**: validar estado de PR por cada repo impactado antes de cerrar ticket.
  - Alternativas: validar solo rama local.
  - Razon: el cierre de ticket debe reflejar integracion real en todos los repos afectados.
- **Separacion por modulos**:
  - `RepositoryModule`: deteccion repos, creacion PR, estado merge.
  - `LocalArchiveModule`: archive local OpenSpec.
  - `OrchestratorPushModule`: commit/push en `DocuArchiCore`.
  - `JiraModule`: validacion de texto del ticket y cambio de estado.
  - `LoggingModule`: trazabilidad paso a paso.

## AI Repository Context

- `DocuArchi.Api`: crear APIs y controllers.
- `DocuArchiCore`: repositorio orquestador, conserva informacion OpenSpec.
- `MiApp.DTOs`: contratos DTOs.
- `MiApp.Models`: clases Dapper que mapean tablas.
- `MiApp.Repository`: consultas y actualizaciones de base de datos.
- `MiApp.Services`: servicios y automapeos.

## Sequence Flow

```text
opsxj:new <ISSUE>
  -> JiraModule.getIssue()
  -> RepositoryModule.detectImpactedRepos()
    -> low confidence => prompt repo/template
  -> OpenSpecModule.generateArtifacts()
  -> OpenSpecModule.ensureApplicationAndTest()
  -> RepositoryModule.ensurePullRequests()
  -> LoggingModule.write(step=new, status=ok)

opsxj:archive <ISSUE>
  -> RepositoryModule.checkAllPrMerged()
    -> pending PR => block archive
  -> LocalArchiveModule.archiveChange()
  -> OrchestratorPushModule.pushArchive(DocuArchiCore)
  -> JiraModule.transitionDone()
  -> LoggingModule.write(step=archive, status=ok)
```

## Module Contracts

- `RepositoryModule`
  - Input: `issueKey`, `changeContext`
  - Output: `impactedRepos[]`, `pullRequests[]`, `allMerged:boolean`
- `OpenSpecModule`
  - Input: `issue`, `impactedRepos`
  - Output: `changeName`, `artifactStatus` (incluye `Application` y `Test`)
- `LocalArchiveModule`
  - Input: `changeName`
  - Output: `archivePath`
- `OrchestratorPushModule`
  - Input: `archivePath`, `branch`
  - Output: `commitSha`, `remoteBranch`
- `JiraModule`
  - Input: `issueKey`, `targetState`
  - Output: `newState`
- `LoggingModule`
  - Input: `step`, `context`, `result`, `error?`
  - Output: registro estructurado trazable

## Risks / Trade-offs

- [Dependencia de Jira/API] → Mitigacion: manejar errores y mensajes claros cuando la consulta falle.
- [Calidad variable de la propuesta generada] → Mitigacion: permitir edicion manual posterior antes de continuar el flujo.
- [Requisitos de credenciales/entorno] → Mitigacion: documentar variables necesarias y fallar de forma explicita.
- [Repositorio ambiguo] -> Mitigacion: fallback obligatorio solicitando repo/plantilla.
- [PR pendiente de merge] -> Mitigacion: bloquear archive y detallar PRs faltantes.
- [Ticket sin texto] -> Mitigacion: bloquear archive con mensaje explicito.
