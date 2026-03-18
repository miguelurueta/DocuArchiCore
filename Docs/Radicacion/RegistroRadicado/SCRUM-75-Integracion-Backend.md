# SCRUM-75 - Integracion Backend

## Endpoint actualizado

- Metodo: `POST`
- URL: `/api/radicacion/registrar-entrante`
- Query param nuevo: `tipoModuloRadicacion`
- Valor por defecto: `1`

## Contrato actualizado

El body `RegistrarRadicacionEntranteRequestDto` ahora admite tambien el campo:

```json
{
  "idPlantilla": 100,
  "tipoModuloRadicacion": 1
}
```

El controller expone el parametro operativo asi:

```http
POST /api/radicacion/registrar-entrante?tipoModuloRadicacion=1
```

## Flujo funcional

1. `RadicacionController` toma `tipoModuloRadicacion` desde query string.
2. `RegistrarRadicacionEntranteService` normaliza el valor; si llega `<= 0`, usa `1`.
3. El valor se copia al request canonico y se propaga al repository.
4. Si `tipoModuloRadicacion == 2`, se ejecuta `IValidaPreRegistroWorkflowService` antes del registro.
5. `RegistrarRadicacionEntranteRepository` usa `tipoModuloRadicacion` en la policy para decidir `Q08`.
6. La respuesta exitosa deja trazabilidad en `MetadataOperativa.tipoModuloRadicacion`.

## Ejemplos

### Request

```http
POST /api/radicacion/registrar-entrante?tipoModuloRadicacion=2
Authorization: Bearer <token>
Content-Type: application/json
```

```json
{
  "idPlantilla": 100,
  "tipoModuloRadicacion": 2,
  "asunto": "Solicitud de prueba",
  "remitente": {
    "nombre": "Usuario externo",
    "id_Dest_Ext": 123
  },
  "destinatario": {
    "destinatario": "Area Interna",
    "id_Remit_Dest_Int": 33
  },
  "tipo_tramite": {
    "descripcion": "TRAMITE",
    "tipo_doc_entrante": 302
  }
}
```

### Response exitosa

```json
{
  "success": true,
  "message": "OK",
  "errors": [],
  "data": {
    "consecutivoRadicado": "26000100001",
    "estadoAsignacion": "Registrado",
    "alertas": [],
    "metadataOperativa": {
      "dbAlias": "DA",
      "idUsuarioRadicador": 55,
      "idUsuarioGestion": 10,
      "tipoModuloRadicacion": 2,
      "moduloRegistro": "RADICACION"
    }
  }
}
```

## Consideraciones

- No se agregaron services ni repositories nuevos.
- El parametro nuevo no rompe compatibilidad porque el endpoint usa `tipoModuloRadicacion=1` por defecto.
- El flujo workflow queda controlado por el parametro y no solo por `util_tipo_modulo_envio`.
