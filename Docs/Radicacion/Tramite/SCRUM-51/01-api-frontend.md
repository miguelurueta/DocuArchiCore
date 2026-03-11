# API Frontend - Validacion Tipo de Campos

## Endpoint relacionado

- `POST /api/radicacion/registrar-entrante`

## Cambio funcional

Se agrega validación de compatibilidad de tipo entre el valor enviado por frontend y el tipo de dato de la columna en la tabla de plantilla.

La validación aplica sobre:

- Campos fijos de radicación.
- Campos dinámicos contenidos en `Campos[]`.

## Componente backend

- Servicio: `ValidaTipoCamposService`
- Repositorio: `ValidaTipoCamposRepository`
- Orquestador: `ValidaCamposRadicacionService`

## Fuente de tipos

Los tipos se obtienen desde:

1. `system_plantilla_radicado` para resolver `Nombre_Plantilla_Radicado`.
2. `information_schema.columns` para recuperar `DATA_TYPE` de cada columna.

## Mensaje de error esperado

Formato:

- `Error en campo '<Alias>': tipo de dato incompatible con <tipo>.`

Ejemplo:

```json
{
  "success": false,
  "message": "Validacion fallida",
  "data": [
    {
      "field": "CampoNumero",
      "message": "Error en campo 'Número de Oficio': tipo de dato incompatible con int.",
      "type": "InvalidType",
      "attemptedValue": "ABC"
    }
  ],
  "errors": [
    {
      "field": "CampoNumero",
      "message": "Error en campo 'Número de Oficio': tipo de dato incompatible con int.",
      "type": "InvalidType",
      "attemptedValue": "ABC"
    }
  ]
}
```
