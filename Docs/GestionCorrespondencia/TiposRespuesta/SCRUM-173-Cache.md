# SCRUM-173 - Cache

## Estrategia
`cache-first` en capa service:
1. Intentar lectura por clave `catalogo:ra_tipo_respuesta:{alias}`.
2. Si hay hit y no expiró, devolver desde caché.
3. Si miss/expirada, consultar repository y refrescar caché.

## Implementacion
- Tipo: memoria en proceso (`ConcurrentDictionary`) en `MiApp.Services`.
- TTL: `10` minutos.
- Alcance: por `defaultDbAlias`.

## Reglas
- No bloquear endpoint por fallas de cache.
- No cachear estados de error.
- `empty` se resuelve desde DB cuando no hay filas activas.

## Deuda tecnica
- Migrar a `IDistributedCache`/Redis cuando `MiApp.Services` incluya referencia de caching distribuido.
