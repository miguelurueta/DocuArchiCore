## 1. Coordinacion multi-repo

- [x] 1.1 Confirmar `DocuArchiCore` como repositorio coordinador del cambio SCRUM-23.
- [x] 1.2 Crear tablero de sincronizacion de PRs y evidencia por repositorio en `sync.md`.
- [x] 1.3 Definir estado inicial (todo/open) para cada repo ejecutor.

## 2. Contexto OpenSpec en configuracion

- [x] 2.1 Ampliar `openspec/config.yaml` con politica multi-repo y referencia obligatoria a `openspec/context/multi-repo-context.md`.
- [x] 2.2 Declarar explicitamente en configuracion que SCRUM-23 es `context-only` en este repositorio.
- [x] 2.3 Agregar reglas por artefacto (proposal/design) para reflejar alcance y exclusiones.

## 3. Alineacion de artefactos del cambio

- [x] 3.1 Ajustar `proposal.md`, `design.md` y `spec.md` para dejar alcance en contexto OpenSpec.
- [x] 3.2 Verificar que no se incluyan tareas de implementacion de codigo ejecutor en este cambio.

## 4. Integracion y cierre

- [x] 4.1 Crear PR del repositorio coordinador con cambios de contexto OpenSpec.
- [x] 4.2 Confirmar aprobacion/merge y registrar enlace final en `sync.md`.
- [x] 4.4 Ejecutar validacion OpenSpec final y preparar archive.
