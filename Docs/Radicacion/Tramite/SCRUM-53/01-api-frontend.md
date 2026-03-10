# API Frontend - Mensaje de Validación por Tipo

## Endpoint

- `POST /api/radicacion/registrar-entrante`

## Regla de mensajes

El backend construye el mensaje según tipo de validación:

- `Required` -> `Campo <Alias>: requerido.`
- `Unique` -> `Campo <Alias>: valor existente.`
- `InvalidType` y `MaxLength` -> `Campo <Alias>: formato no compatible.`

## Ejemplos

```json
{
  "field": "FECHALIMITERESPUESTA",
  "type": "Required",
  "message": "Campo Fecha Límite Respuesta: requerido."
}
```

```json
{
  "field": "CampoIdentificador",
  "type": "Unique",
  "message": "Campo NIT del Solicitante: valor existente."
}
```

```json
{
  "field": "CampoNumero",
  "type": "InvalidType",
  "message": "Campo Número de Oficio: formato no compatible."
}
```
