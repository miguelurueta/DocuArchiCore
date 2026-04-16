# SCRUM-139 — Integración Frontend — Configuración Upload

## API

- Método: `GET`
- Ruta: `/api/gestor-documental/configuracion-upload`
- Querystring requerido: `nameProceso`
- Claim requerido: `defaulalias` (usado como `defaultDbAlias`)

## Request

Ejemplo:

`GET /api/gestor-documental/configuracion-upload?nameProceso=WF_RADICACION`

## Response (AppResponses<List<RaConfiguracionUploadModel>>)

Campos:

- `success`: `true | false`
- `message`: texto de estado
- `data`: lista de `RaConfiguracionUploadModel`
- `errors`: lista de errores (si aplica)

### Con resultados

- `success = true`
- `data` contiene elementos

### Sin resultados

- `success = true`
- `data = []`
- `message = "Sin resultados"`

### Error / validación

- `success = false`
- `data = []`
- `errors` contiene `AppError`

## UX recomendaciones

- Empty state: mostrar “Sin resultados” y permitir reintentar/ajustar `nameProceso`.
- Error state: mostrar `message` y, si existe, detalle de `errors`.

