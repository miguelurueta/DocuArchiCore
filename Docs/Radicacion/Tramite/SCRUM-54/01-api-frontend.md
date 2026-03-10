# API Frontend - Mensaje MaxLength

## Endpoint

- `POST /api/radicacion/registrar-entrante`

## Cambio funcional

Para errores de validación `MaxLength`, el mensaje ahora debe ser:

- `Campo <Alias>: supera la longitud máxima permitida.`

## Ejemplo

```json
{
  "field": "FECHALIMITERESPUESTA",
  "type": "MaxLength",
  "message": "Campo Fecha Límite Respuesta: supera la longitud máxima permitida."
}
```

## Nota

Este ajuste solo aplica a `MaxLength`; otros tipos de error conservan su formato actual.
