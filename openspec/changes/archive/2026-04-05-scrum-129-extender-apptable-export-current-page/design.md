## Context

- Jira issue key: SCRUM-129
- Jira summary: EXTENDER-APPTABLE-EXPORT-CURRENT-PAGE
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-129

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

# PROMPT ARQUITECTÓNICO
Extender `/api/AppTable/export` para soportar `currentPage` en formatos ejecutivos

## Rol esperado

Arquitecto de software senior y desarrollador backend/frontend
(arquitectura enterprise + contratos API + exportacion documental)

## Resumen Jira sugerido

`[APPTABLE_EXPORT_22] Extender api AppTable export para currentPage en formatos ejecutivos`

## Objetivo

Extender la API backend de exportacion para que pueda generar archivos `xlsx` y `pdf` de la pagina actual visible, sin depender de exportacion local en frontend.

## Problema actual

Hoy la ruta `POST /api/AppTable/export` soporta la estrategia server-side real, pero valida `ExportMode = "allMatching"`.

Eso impide reutilizar el backend para:

- `currentPage` en `xlsx`
- `currentPage` en `pdf`

Como consecuencia:

- `csv` puede resolverse localmente
- `xlsx` y `pdf` quedan deshabilitados para `currentPage`

## Objetivo funcional

Permitir que el backend genere archivos ejecutivos para el subconjunto exacto de la pagina visible en la tabla actual.

La semantica esperada de `currentPage` debe respetar:

- `search`
- `searchType`
- `structuredFilters`
- `sortField`
- `sortDir`
- `page`
- `pageSize`

## Alcance

- extender el contrato backend de exportacion para aceptar `ExportMode = "currentPage"`
- mantener compatibilidad con `allMatching`
- generar `xlsx` y `pdf` reales para la pagina actual
- asegurar que la pagina exportada coincide exactamente con la visible
- preservar encabezado ejecutivo, metadata y branding corporativo

## No alcance

- no redefinir la exportacion local `csv`
- no mover al frontend la logica documental de `xlsx` o `pdf`
- no cambiar la semantica ya aprobada para `allMatching`
- no introducir iteracion de paginas desde navegador

## Reglas funcionales

- `ExportMode = "currentPage"` debe exportar solo el subconjunto de la pagina actual
- el subconjunto exportado debe respetar la consulta activa completa
- `currentPage` no debe confundirse con:
  - `allLoaded`
  - `allMatching`
- `xlsx` y `pdf` deben generarse desde backend con el mismo encabezado ejecutivo ya soportado para exportacion total
- si el backend soporta `csv` para `currentPage`, debe mantenerse consistente; pero el objetivo prioritario de este ticket son `xlsx` y `pdf`

## Reglas tecnicas

- el request de `POST /api/AppTable/export` debe aceptar explicitamente:
  - `ExportMode = "currentPage"`
  - `ExportMode = "allMatching"`
- el backend no debe inferir `currentPage` desde ausencia de datos
- la rama `currentPage` debe reutilizar la misma logica de consulta base, pero respetando `Page` y `PageSize` del request
- la rama `allMatching` debe seguir ignorando el page visible cuando aplique exportacion total
- la validacion del request no debe romper consumidores existentes de `allMatching`
- la respuesta debe seguir siendo archivo final listo para descarga
- no crear una ruta nueva si la extension del endpoint actual mantiene coherencia del contrato

## Contrato sugerido

Request:

```json
{
  "ColumnMode": 2,
  "EstadoTramite": "",
  "SearchType": 1,
  "Search": "radicado",
  "SortField": "fecha_inicio",
  "SortDir": "DESC",
  "Page": 2,
  "PageSize": 10,
  "Format": "xlsx",
  "ExportMode": "currentPage",
  "ReportTitle": "Bandeja de gestion correspondencia",
  "StructuredFilters": []
}
```

Semantica:

- `ExportMode = "currentPage"`
  - usa la misma consulta activa
  - respeta `Page` y `PageSize`

- `ExportMode = "allMatching"`
  - usa la misma consulta activa
  - ignora el page visible para exportar el universo filtrado

## Archivos esperados

- backend del endpoint `POST /api/AppTable/export`
- servicios internos de exportacion de workflow inbox
- pruebas backend de controller y service
- documentacion actualizada del contrato si el repo la genera

## Riesgos a evitar

- que `currentPage` exporte mas filas que la pagina visible
- que `currentPage` exporte menos filas por recalculo inconsistente
- duplicar endpoint para una variacion que cabe en el contrato existente
- romper `allMatching` al introducir la nueva rama
- usar una semantica distinta entre tabla visible y archivo descargado

## Pruebas obligatorias

- `ExportMode = "currentPage"` con formato `xlsx`
- `ExportMode = "currentPage"` con formato `pdf`
- consistencia exacta entre pagina visible y filas exportadas
- exportacion con filtros estructurados
- exportacion con busqueda simple
- exportacion con ordenamiento
- no regresion de `ExportMode = "allMatching"`
- validacion de `Format` invalido
- validacion de `ExportMode` invalido

## Criterios de aceptación

- `/api/AppTable/export` acepta `currentPage` sin romper `allMatching`
- `xlsx` y `pdf` de `currentPage` se generan correctamente en backend
- el archivo respeta la misma consulta activa y el mismo slice paginado visible
- el contrato backend queda suficientemente claro para conectar frontend reusable

## Conclusión

Sin esta extension backend, `currentPage` en formatos ejecutivos queda bloqueado o forzado a una implementacion documental en frontend que no es la direccion arquitectonica deseada.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-129-extender-apptable-export-current-page.