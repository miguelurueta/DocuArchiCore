# Consulta Radicacion

Documentacion tecnica del flujo `ApListaCoinsidenciaRadicados` para SCRUM-37/SCRUM-38.

- API: `POST /api/tramite/consulta-radicacion/apListaCoinsidenciaRadicados`
- Request DTO: `ParametroCoinsidenciaRadicadoDTO`
- Response DTO: `AppResponses<DynamicUiTableDto>`
- Patrón aplicado: `Controller -> Service -> Repository`
- Cambio SCRUM-38: la columna `estado_validacion` se elimino de `BuildColumns` para todos los tipos de modulo.

## Contrato rapido

```json
{
  "TextoBuscado": "rad-2026",
  "TipoModuloDeConsulta": 2
}
```

Respuesta exitosa:

```json
{
  "success": true,
  "message": "OK",
  "data": {
    "TableId": "coinsidencia-radicados",
    "Columns": [],
    "Rows": []
  }
}
```
