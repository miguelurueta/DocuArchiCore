# SCRUM-76 - Integracion Backend

## Objetivo

Actualizar `RegistrarRadicacionEntranteAsync` para que la plantilla de radicacion se resuelva en backend con `SolicitaEstructuraPlantillaRadicacionDefault(defaultDbAlias)` y el endpoint ya no dependa de `IdPlantilla` en el payload publico.

## Cambios aplicados

- `RegistrarRadicacionEntranteRequestDto` marca `IdPlantilla` con `JsonIgnore`.
- `RegistrarRadicacionEntranteService` deja de validar `IdPlantilla` de entrada.
- El service consulta la plantilla por defecto usando `ISystemPlantillaRadicadoR.SolicitaEstructuraPlantillaRadicacionDefault(defaultDbAlias)`.
- El id resuelto (`id_Plantilla`) se inyecta internamente en el request canonico para:
  - consultar `detalle_plantilla_radicado`
  - consultar configuracion de tipo de radicacion
  - registrar el radicado en repository

## Contrato esperado desde frontend

El `POST /api/radicacion/registrar-entrante` ya no debe enviar `IdPlantilla` en el body.

Ejemplo valido:

```json
{
  "tipoModuloRadicacion": 1,
  "ASUNTO": "Registro de prueba",
  "Remitente": {
    "Nombre": "EMSA",
    "id_Dest_Ext": 7
  },
  "Destinatario": {
    "Destinatario": "Carolina Cruz",
    "id_Remit_Dest_Int": 136
  },
  "Tipo_tramite": {
    "Descripcion": "DERECHOS DE PETICION",
    "tipo_doc_entrante": 1
  },
  "TipoRadicado": {
    "TipoRadicacion": "EXTERNA",
    "IdTipoRadicado": 1
  },
  "TipoPlantillaRadicado": {
    "TipoPlantillaRadicado": "",
    "IdTipoPlantillaRdicado": 1
  },
  "Campos": []
}
```

## Respuesta funcional

- Si existe plantilla por defecto: el flujo continua normalmente.
- Si no existe plantilla por defecto: retorna `success=false` con mensaje controlado y no registra.
- La respuesta sigue envuelta en `AppResponses<RegistrarRadicacionEntranteResponseDto>`.

## Evidencia tecnica

- Pruebas focalizadas:
  - `RegistrarRadicacionEntranteServiceTests`
  - `RadicacionControllerContractTests`
  - `RegistrarEntranteConcurrencyTests`
- Resultado:
  - `17` pruebas OK
