# SCRUM-68 Integracion Frontend

## Endpoint

- Metodo: `GET`
- Ruta: `/api/workflow/ruta-trabajo/solicita-estructura-ruta`
- Claim requerido: `defaulalias`

## Respuesta

La respuesta usa `AppResponses<List<SolicitaEstructuraRutaWorkflowDto>?>`.

Campos del DTO:

- `id_Ruta`: identificador de la ruta workflow.
- `Nombre_Ruta`: nombre tecnico de la ruta.
- `Descripcion_Ruta`: descripcion funcional de la ruta.
- `Fecha_Creacion`: fecha de creacion registrada.
- `Estado_Ruta`: estado de la ruta; para esta consulta solo se devuelven rutas con valor `1`.
- `Archivo_Plantilla`: contenido binario de plantilla asociado.
- `Ruta_Archivo_Server`: ruta del archivo en servidor.
- `Archivo_Plantilla_Mindifucion`: referencia adicional de plantilla.

## Parametros requeridos para invocacion

- El cliente no envia parametros de query.
- El backend toma `defaultDbAlias` desde el claim `defaulalias`.

## Ejemplo de respuesta con datos

```json
{
  "success": true,
  "message": "YES",
  "data": [
    {
      "id_Ruta": 1,
      "Nombre_Ruta": "ENTRADA",
      "Descripcion_Ruta": "Ruta de entrada",
      "Fecha_Creacion": "2026-03-16T00:00:00",
      "Estado_Ruta": 1,
      "Archivo_Plantilla": "AQI=",
      "Ruta_Archivo_Server": "/tmp/ruta",
      "Archivo_Plantilla_Mindifucion": "mind"
    }
  ],
  "errors": []
}
```

## Ejemplo de respuesta sin resultados

```json
{
  "success": true,
  "message": "Sin resultados",
  "data": null,
  "errors": []
}
```
