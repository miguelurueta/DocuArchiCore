# SCRUM-173 - Arquitectura

## Objetivo
Exponer un catalogo de tipos de respuesta para frontend mediante `GET /api/gestion-correspondencia/tipos-respuesta`.

## Flujo
1. `SolicitaListaTiposRespuestaController` valida claim `defaulalias`.
2. `ServiceSolicitaListaTiposRespuesta` ejecuta estrategia cache-first.
3. `SolicitaListaTiposRespuestaRepository` consulta `ra_tipo_respuesta` con `DapperCrudEngine` + `QueryOptions`.
4. Service construye `AppResponses<List<ResponseDropdownDto>>`.

## Capas
- Controller: HTTP + seguridad de claims.
- Service: reglas de respuesta (`success`, `empty`, `error`) y cache.
- Repository: acceso a datos, sin SQL manual.

## Regla de datos
- Tabla: `ra_tipo_respuesta`
- Filtro: `estado = 1`
- Orden: `orden ASC`
- Límite defensivo: `500`
- Campos expuestos: `Id`, `Descripcion`

## DI
- `ISolicitaListaTiposRespuestaRepository -> SolicitaListaTiposRespuestaRepository`
- `IServiceSolicitaListaTiposRespuesta -> ServiceSolicitaListaTiposRespuesta`
