# SCRUM-139 — Integración Frontend — Configuración Upload

## API

- Método: `GET`
- Ruta: `/api/gestor-documental/configuracion-upload`
- Querystring requerido: `nameProceso`
- Claim requerido: `defaulalias` (usado como `defaultDbAlias`)

## Request

Ejemplo:

`GET /api/gestor-documental/configuracion-upload?nameProceso=EDITOR`

## Contrato de respuesta

Tipo: `AppResponses<List<RaConfiguracionUploadModel>>`

### Wrapper: `AppResponses<T>`

Definición (DTO real): `..\MiApp.DTOs\DTOs\Utilidades\AppResponses.cs`

- `success` (bool): indica si la operación fue exitosa.
- `message` (string): mensaje de estado (`YES`, `Sin resultados`, mensaje de validación/error).
- `data` (T): para este endpoint, lista de `RaConfiguracionUploadModel`.
- `meta` (AppMeta|null): no se usa en este endpoint (puede venir `null`).
- `errors` (object[]|null): lista de errores. En validación/errores controlados suele contener `AppError`.

### DTO: `RaConfiguracionUploadModel`

Definición (modelo real): `..\MiApp.Models\Models\GestorDocumental\ConfiguracionUpload\RaConfiguracionUploadModel.cs`

- `idConfigUploadGestion` (int?)
- `extensionUpload` (string?) 
- `lengUpload` (long?)
- `nameProceso` (string?)
- `estadoProceso` (int?)

Campos:

- `success`: `true | false`
- `message`: texto de estado
- `data`: lista de `RaConfiguracionUploadModel`
- `errors`: lista de errores (si aplica)

## Ejemplos de respuesta

### 1) Con resultados (`message = "YES"`)

```json
{
  "success": true,
  "message": "YES",
  "data": [
    {
      "idConfigUploadGestion": 1,
      "extensionUpload": "PDF",
      "lengUpload": 10485760,
      "nameProceso": "WF_RADICACION",
      "estadoProceso": 1
    }
  ],
  "meta": null,
  "errors": []
}
```

- `success = true`
- `data` contiene elementos

### 2) Sin resultados (`message = "Sin resultados"`)

```json
{
  "success": true,
  "message": "Sin resultados",
  "data": [],
  "meta": null,
  "errors": []
}
```

- `success = true`
- `data = []`
- `message = "Sin resultados"`

### 3) Error / validación (ej: `nameProceso` vacío)

```json
{
  "success": false,
  "message": "NameProceso requerido",
  "data": [],
  "meta": null,
  "errors": [
    {
      "type": "Validation",
      "field": "nameProceso",
      "message": "NameProceso requerido"
    }
  ]
}
```

- `success = false`
- `data = []`
- `errors` contiene `AppError`

## UX recomendaciones

- Empty state: mostrar “Sin resultados” y permitir reintentar/ajustar `nameProceso`.
- Error state: mostrar `message` y, si existe, detalle de `errors`.

