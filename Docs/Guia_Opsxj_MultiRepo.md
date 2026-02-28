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
```

Notas:
- `GITHUBREPO` vacio: `opsxj` autodetecta `owner/repo` desde `git remote get-url origin`.
- Mantener el mismo `GITHUB_TOKEN` en todos los repos.

## Flujo por ticket (ABC-123)

1. En coordinador, definir repos impactados en `sync.md` (matriz impacto).
2. En cada repo marcado `yes`:
   - `npm.cmd --prefix Tools/jira-open run opsxj:new -- ABC-123`
3. Trabajar codigo, abrir/revisar PR, mergear.
4. En cada repo mergeado:
   - `npm.cmd --prefix Tools/jira-open run opsxj:archive -- ABC-123`
5. Actualizar coordinador con links PR y estado final.

## Como saber en que repos correr opsxj:new

Usar plantilla:
- `openspec/context/REPO_IMPACT_TEMPLATE.md`

Regla:
- Solo correr `opsxj:new` en repos con `Impacta? = yes`.
- Repos `no` quedan `n/a` para dejar alcance explicito.

## Comandos utiles

Probar Jira:

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:test-jira
```

Crear cambio:

```powershell
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
