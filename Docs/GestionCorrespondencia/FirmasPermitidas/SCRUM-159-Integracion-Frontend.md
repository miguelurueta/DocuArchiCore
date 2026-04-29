# SCRUM-159 - Integración Frontend

## Descripción de API
API para consultar firmantes permitidos de una solicitud de aprobación y poblar controles de selección (dropdown/listado).

## Método HTTP
- `GET`

## Ruta oficial
- `/api/gestion-correspondencia/firmas/permitidas-por-solicitud`

## Query params
- `idSolicitudAprobacion` (long, obligatorio, > 0)

## Origen esperado de idSolicitudAprobacion
- Id de la solicitud de aprobación activa en el flujo de Gestión de Correspondencia, provisto por el contexto de pantalla o resultado de consulta previa de solicitudes.

## Claims requeridos
- `defaulalias`
- `usuarioid`

## Response AppResponses<List<ResponseDropdownDto>>
Estructura:
- `success`: bool
- `message`: string
- `data`: `List<ResponseDropdownDto>`
- `meta.status`: string (`success|empty|error`)
- `errors`: listado de errores tipados

## Significado de campos
- `success`: indica si la operación de negocio fue válida.
- `message`: mensaje funcional/técnico resumido.
- `data`: lista de firmas para UI.
- `meta.status`:
  - `success`: hay datos
  - `empty`: no hay datos
  - `error`: error de validación o ejecución
- `errors`: detalles de validación/exception.

## Ejemplo request válido
```http
GET /api/gestion-correspondencia/firmas/permitidas-por-solicitud?idSolicitudAprobacion=123
Authorization: Bearer <token>
```

## Ejemplo response success
```json
{
  "success": true,
  "message": "YES",
  "data": [
    { "id": 7, "descripcion": "Ana Perez - Analista" },
    { "id": 9, "descripcion": "Carlos Ruiz - Coordinador" }
  ],
  "meta": { "status": "success" },
  "errors": []
}
```

## Ejemplo response empty
```json
{
  "success": true,
  "message": "Sin resultados",
  "data": [],
  "meta": { "status": "empty" },
  "errors": []
}
```

## Ejemplo response unauthorized/error
```json
{
  "success": false,
  "message": "Claim invalido: usuarioid",
  "data": [],
  "meta": { "status": "error" },
  "errors": [
    { "type": "Validation", "field": "usuarioid", "message": "Claim invalido: usuarioid" }
  ]
}
```

## Reglas de consumo frontend
- No asumir que siempre retorna datos.
- Tratar `success=true + meta.status=empty` como caso funcional válido.
- Tratar `success=false` como error de negocio/autorización.

## Cómo poblar dropdown/listado de firmas
- Bind `data[].id` como `value`.
- Bind `data[].descripcion` como `label`.
- Orden de respuesta ya viene ascendente por nombre.

## UX para lista vacía
- Mostrar estado vacío no bloqueante: `"No hay firmantes autorizados para esta solicitud"`.
- Mantener dropdown deshabilitado o con placeholder.

## UX para error
- Mostrar mensaje de error genérico amigable y registrar detalle técnico en consola/telemetría cliente.

## UX para no autorizado
- Si falla por claims/token: notificar sesión inválida y redirigir a flujo de autenticación.

## Consideraciones de seguridad
- No loguear token en cliente.
- No persistir respuesta en almacenamiento inseguro.
- Validar que el usuario tenga contexto correcto de solicitud antes de invocar API.

## Regla clave de resiliencia
- No asumir que siempre hay firmantes; backend puede retornar `empty` por reglas de autorización de firma.
