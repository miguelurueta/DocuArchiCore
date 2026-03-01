# Guia opsxj Multi-Repo

Esta guia define como trabajar tickets Jira en un entorno con repositorios separados usando `opsxj:new` y `opsxj:archive`.

## Objetivo

- Evitar confusion sobre en que repo ejecutar comandos.
- Mantener trazabilidad por ticket y por repo.
- Sincronizar OpenSpec, GitHub PR y estado Jira.

## Esquema recomendado

- Repos ejecutores (codigo): `DocuArchi.Api`, `MiApp.Services`, `MiApp.Repository`, `MiApp.DTOs`, `MiApp.Models`, etc.
- Repo coordinador (documentacion/seguimiento): `DocuArchiCore` (OpenSpec y matriz de impacto).

Regla: `opsxj` se ejecuta en el repo donde vive el cambio de codigo.

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
```

Notas:
- `GITHUBREPO` vacio: `opsxj` autodetecta `owner/repo` desde `git remote get-url origin`.
- Mantener el mismo `GITHUB_TOKEN` en todos los repos.
- `OPSXJ_IMPACT_REPOS` opcional: lista separada por coma para forzar repos impactados sin interaccion.
- Ejemplo: `OPSXJ_IMPACT_REPOS=DocuArchi.Api,MiApp.Services,MiApp.Repository`.

## Flujo por ticket (ABC-123)

1. En coordinador, definir repos impactados en `sync.md` (matriz impacto).
2. En cada repo marcado `yes`:
   - `npm.cmd --prefix Tools/jira-open run opsxj:new -- ABC-123`
3. Trabajar codigo, abrir/revisar PR, mergear.
4. En cada repo mergeado:
   - `npm.cmd --prefix Tools/jira-open run opsxj:archive -- ABC-123`
5. Actualizar coordinador con links PR y estado final.

## Flujo objetivo reforzado

1. `opsxj:new` consulta Jira y valida texto del ticket (`summary` o `description`).
2. IA detecta repos impactados por logica:
   - Si no hay confianza: aplica fallback automatico (catalogo completo) sin prompt.
3. El cambio OpenSpec debe quedar completo hasta `Application` y `Test`.
4. Se generan PRs por repo impactado.
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
