# SCRUM-72 Integracion Backend Interna

## Alcance

Este ticket no expone un endpoint nuevo ni crea un service publico. El entregable funcional es un repository para consumo interno:

- `ISolicitaEstructuraConfiguracionListadoRutaRepository`
- `SolicitaEstructuraConfiguracionListadoRutaRepository`

## Metodo interno

- Firma:

```csharp
Task<AppResponses<List<ConfiguracionListadoRuta>?>> SolicitaEstructuraConfiguracionListadoRutaAsync(
    int idRuta,
    string defaultDbAlias)
```

## Parametros requeridos

- `idRuta`: identificador de la ruta workflow.
- `defaultDbAlias`: alias de base de datos del modulo workflow.

## Contrato de respuesta

- Tipo: `AppResponses<List<ConfiguracionListadoRuta>?>`
- Tabla consultada: `configuracion_listado_ruta`
- Filtro: `Rutas_Workflow_id_Ruta = idRuta`

## Respuesta con datos

```json
{
  "success": true,
  "message": "YES",
  "data": [
    {
      "Id_Configuracion": 1,
      "Rutas_Workflow_id_Ruta": 7,
      "id_Campo": 10,
      "Nombre_Campo": "Consecutivo",
      "Tipo_Campo": "text",
      "Lista_Tarea": 1,
      "Ordena_Tarea": 2,
      "Prioridad": 1,
      "campo_radicado": 1,
      "campo_tramite": 0,
      "campo_fecha_vence": 0,
      "Campo_beneficiario": 0,
      "Lista_gestion_tamite": 1,
      "Orden_lista_gestion_tamite": 2,
      "campo_codigo_barras": 0
    }
  ],
  "errors": []
}
```

## Respuesta sin resultados

```json
{
  "success": true,
  "message": "Sin resultados",
  "data": null,
  "errors": []
}
```

## Ejemplo de consumo interno

```csharp
var result = await _solicitaEstructuraConfiguracionListadoRutaRepository
    .SolicitaEstructuraConfiguracionListadoRutaAsync(idRuta, defaultDbAlias);
```

## Consideraciones

- No se implementa controller nuevo.
- No se implementa service nuevo.
- La dependencia queda registrada en `Program.cs` para consumo interno futuro.
