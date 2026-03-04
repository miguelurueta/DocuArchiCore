# Consulta Radicacion

Documentacion tecnica del flujo `ApListaCoinsidenciaRadicados` para SCRUM-37.

- API: `POST /api/tramite/consulta-radicacion/apListaCoinsidenciaRadicados`
- Request DTO: `ParametroCoinsidenciaRadicadoDTO`
- Response DTO: `AppResponses<DynamicUiTableDto>`
- Patrón aplicado: `Controller -> Service -> Repository`

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
