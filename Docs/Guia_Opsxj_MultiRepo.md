# Guia opsxj Multi-Repo

Esta guia define como trabajar tickets Jira en un entorno con repositorios separados usando `opsxj:new`, `opsxj:orchestrate:new`, `opsxj:orchestrate:publish`, `opsxj:archive` y `opsxj:orchestrate:archive`.

## Objetivo

- Evitar confusion sobre en que repo ejecutar comandos.
- Mantener trazabilidad por ticket y por repo.
- Sincronizar OpenSpec, GitHub PR y estado Jira.

## Esquema recomendado

- Repos ejecutores (codigo): `DocuArchi.Api`, `MiApp.Services`, `MiApp.Repository`, `MiApp.DTOs`, `MiApp.Models`, etc.
- Repo coordinador (documentacion/seguimiento): `DocuArchiCore` (OpenSpec y matriz de impacto).

Regla:
- `opsxj:new` se ejecuta en el repo donde vive el cambio de codigo.
- `opsxj:orchestrate:new` se ejecuta solo en `DocuArchiCore` cuando el ticket impacta varios repos y se quiere centralizar OpenSpec.
- `opsxj:orchestrate:new` deja el ticket abierto, prepara el branch del ticket en los repos principales limpios y difiere los PRs satelite hasta que exista diff real.
- `opsxj:orchestrate:publish` detecta repos satelite con implementacion real y publica desde el checkout principal cuando esta disponible; solo crea worktree/branch/PR si hace falta aislamiento.

## Configuracion minima por repo

Archivo: `Tools/jira-open/.jira-open.env`

```env
JIRA_BASE_URL=https://tu-org.atlassian.net
JIRA_EMAIL=tu.email@empresa.com
JIRA_API_TOKEN=tu_token_api

GIT_REMOTE_NAME=origin
GIT_BASE_BRANCH=
GITHUB_TOKEN=tu_github_token
GITHUBREPO=
OPSXJ_IMPACT_REPOS=
OPSXJ_IMPLEMENTATION_REPOS=
OPSXJ_TRACEABILITY_REPOS=
```

Notas:
- `GITHUBREPO` vacio: `opsxj` autodetecta `owner/repo` desde `git remote get-url origin`.
- Mantener el mismo `GITHUB_TOKEN` en todos los repos.
- `OPSXJ_IMPACT_REPOS` opcional: lista separada por coma para forzar repos impactados sin interaccion.
- Ejemplo: `OPSXJ_IMPACT_REPOS=DocuArchi.Api,MiApp.Services,MiApp.Repository`.
- `OPSXJ_IMPLEMENTATION_REPOS` opcional: subconjunto de repos impactados que si requieren branch, commit y PR satelite.
- `OPSXJ_TRACEABILITY_REPOS` opcional: etiqueta explicita para subconjunto de repos impactados que solo requieren trazabilidad.

## Flujo por ticket (ABC-123)

1. En coordinador, definir repos impactados en `sync.md` (matriz impacto).
2. Elegir modo:
   - repo unico: `npm.cmd --prefix Tools/jira-open run opsxj:new -- ABC-123`
   - multi-repo orquestado: `npm.cmd --prefix Tools/jira-open run opsxj:orchestrate:new -- ABC-123 -NonInteractive`
3. Trabajar codigo.
4. Publicar PRs reales:
   - multi-repo orquestado: `npm.cmd --prefix Tools/jira-open run opsxj:orchestrate:publish -- ABC-123 -NonInteractive`
5. Revisar PRs y mergear.
6. Archivar cuando todos los PRs impactados esten mergeados.
   - repo unico: `npm.cmd --prefix Tools/jira-open run opsxj:archive -- ABC-123 -NonInteractive`
   - multi-repo orquestado: `npm.cmd --prefix Tools/jira-open run opsxj:orchestrate:archive -- ABC-123 -NonInteractive`

## Clasificacion de impacto

La matriz `sync.md` ahora distingue:

- `implementation_required`: el repo requiere branch, commit, PR y merge
- `traceability_only`: el repo queda registrado en `sync.md`, pero no abre PR satelite si no hay diff funcional
- `no_code_change`: el repo queda explicitamente fuera de cambios

Reglas:

- `DocuArchiCore` normalmente queda como `implementation_required` porque centraliza OpenSpec y `sync.md`
- `opsxj:orchestrate:new` deja los satelites como `traceability_only`/diferidos hasta que exista diff real
- `opsxj:orchestrate:publish` promueve el repo a `implementation_required` solo cuando detecta implementacion publicable
- `OPSXJ_TRACEABILITY_REPOS` permite dejar trazabilidad explicita en `sync.md`
- `OPSXJ_READONLY_REPOS` sigue excluyendo repos de impacto real y los deja como `no_code_change`
- `opsxj:orchestrate:archive` acepta PR satelite `MERGED` aunque GitHub ya haya borrado la branch remota del cambio

## Checkout principal y worktrees gestionados

Cuando el repo satelite principal esta limpio y no esta en `HEAD` detached, `opsxj:orchestrate:new` intenta dejarlo directamente en la rama del ticket. Ese es el camino preferido para implementar localmente y revisar el codigo sin depender de un worktree temporal.

`opsxj:orchestrate:publish` reutiliza ese checkout principal cuando:

- el repo esta limpio al abrir el ticket
- la rama del ticket puede usarse en el repo principal
- no existe otra ocupacion que obligue a aislar el repo

Solo si eso no es posible, el flujo cae a worktree gestionado.

Cuando `opsxj:orchestrate:publish` encuentra un repo impactado en cualquiera de estos estados:

- checkout sucio
- rama distinta al branch del ticket
- `HEAD` detached

el comando no intenta reutilizar ese checkout. En su lugar:

1. crea o reutiliza un worktree gestionado bajo `.tmp/opsxj/`
2. registra metadata en `.opsxj/orchestrator/worktrees/<ISSUE>/`
3. escribe el marcador tecnico del repo satelite dentro del worktree reutilizable

Esto permite reruns idempotentes del comando sin mezclar tickets activos ni bloquear otros repositorios.

Reglas:

- el checkout principal del repo satelite se usa primero cuando esta limpio
- el checkout principal del repo satelite no se modifica si esta ocupado
- el worktree gestionado se reutiliza para el mismo `issue/repo`
- `opsxj` ignora la metadata interna de `.opsxj/orchestrator/` al validar limpieza efectiva del repo coordinador

## Flujo objetivo reforzado

1. `opsxj:new` consulta Jira y valida texto del ticket (`summary` o `description`).
2. IA detecta repos impactados por logica:
   - Si no hay confianza: aplica fallback automatico (catalogo completo) sin prompt.
3. El cambio OpenSpec debe quedar completo hasta `Application` y `Test`.
4. `opsxj:orchestrate:publish` genera PRs solo para repos con diff real.
5. `opsxj:archive` valida que todos los PRs impactados esten mergeados.
6. Solo con merge total:
   - archive local,
   - push del archive en `DocuArchiCore`,
   - cambio de estado en Jira.

Reglas de bloqueo:
- Si falta texto del ticket: no archiva.
- Si algun PR no esta mergeado: no archiva.

## Como saber en que repos correr opsxj:new

Usar plantilla:
- `openspec/context/REPO_IMPACT_TEMPLATE.md`

Regla:
- Solo correr `opsxj:new` en repos con `Impacta? = yes`.
- Repos `no` quedan `n/a` para dejar alcance explicito.
- Convencion de ramas recomendada por ticket: `ABC-123-descripcion-corta` (o `main/master` solo para mantenimiento).
- `opsxj:archive` ahora exige ejecutarse en la rama del cambio para evitar archivados en ramas incorrectas.

## Comandos utiles

Probar Jira:

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:test-jira
```

Probar aislamiento por worktree:

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:test-orchestrate-worktree
```

Probar publish orquestado:

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:test-orchestrate-publish
```

Probar clasificacion de impacto:

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:test-orchestrate-impact
```

Probar archive orquestado:

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:test-orchestrate-archive
```

Auditar ramas del multirepo:

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:branch-audit
```

Auditar ramas para un ticket esperado:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/jira-open/opsxj-branch-audit.ps1 -ExpectedIssue SCRUM-29
```

Crear cambio:

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:new -- SCRUM-23
```

Crear cambio (no interactivo):

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:new -- SCRUM-23
```

Forzar repos impactados (opcional) con `OPSXJ_IMPACT_REPOS`:

```powershell
$env:OPSXJ_IMPACT_REPOS="DocuArchi.Api,MiApp.Services,MiApp.Repository"
npm.cmd --prefix Tools/jira-open run opsxj:new -- SCRUM-23
```

Archivar cambio:

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:archive -- SCRUM-23
```

## Errores comunes

- `No change found for issue key ...`:
  - El cambio ya fue archivado o se creo con otro prefijo.
- `branch has no history in common with main`:
  - Repo/branch remoto incorrecto o `GITHUBREPO` apuntando a otro repo.
- Jira no cambia a terminado:
  - Verificar transiciones disponibles del workflow Jira del proyecto.
- `Policy enforced: interactive repo selection is disabled...`:
  - Quitar `-SelectRepos` y usar `OPSXJ_IMPACT_REPOS`.
- `El ticket no tiene texto, no puede archivarse.`:
  - Completar `summary` o `description` en Jira antes de ejecutar archive.
