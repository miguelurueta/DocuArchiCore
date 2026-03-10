# DTO frontend - Correccion campo Descripcion

## Cambio funcional

En `POST /api/radicacion/registrar-entrante`, la validacion toma `Tipo_tramite.Descripcion` como fuente de `Descripcion_Documento`.

## Request esperado

```json
{
  "Tipo_tramite": {
    "Descripcion": "DERECHOS DE PETECION",
    "tipo_doc_entrante": 1
  }
}
```

## Regla aplicada en backend

- Si `Campos["Descripcion_Documento"]` no viene informado:
  - se usa `Tipo_tramite.Descripcion`.
- La validacion de dimension tambien usa ese mapeo para verificar longitud maxima.

## Respuesta

- Si cumple validacion: `success=true`.
- Si excede longitud: `success=false`, `message="Validacion fallida"`, error `MaxLength` sobre `Descripcion_Documento`.
