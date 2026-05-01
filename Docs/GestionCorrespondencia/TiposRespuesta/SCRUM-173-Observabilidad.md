# SCRUM-173 - Observabilidad

## Logging esperado
- Controller:
  - `requestId`
  - `alias`
- Service:
  - `cache hit/miss`
  - `alias`
  - `requestId`
  - errores controlados con stacktrace en log interno
- Repository:
  - warning si query retorna error o `Data=null`

## Eventos clave
- Inicio de request de catalogo.
- Cache hit o cache miss.
- Error de consulta o de ejecucion.

## Troubleshooting
- `BadRequest` inmediato:
  - validar claim `defaulalias` en token.
- `success=false` con mensaje de error:
  - revisar conectividad DB del alias.
- `Sin resultados`:
  - verificar registros `estado = 1` en `ra_tipo_respuesta`.
