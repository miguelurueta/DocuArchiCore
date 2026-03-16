# SCRUM-66 Integracion Backend

## Objetivo
- Construir en memoria los valores de `DatoCampoPlantilla` para una lista de `RelacionCamposRutaWorklflow`.
- La fuente de datos es `RegistrarRadicacionEntranteRequestDto`.
- No usa controller, repository, Dapper ni consultas a base de datos.

## Servicio
- `IAsingacionValoresDatosRadicadoRutaWorklflow`
- Implementacion: `AsingacionValoresDatosRadicadoRutaWorklflow`
- Ubicacion: `MiApp.Services/Service/Radicacion/RelacionCamposRutaWorklflow`

## Contrato
```csharp
Task<AppResponses<List<RelacionCamposRutaWorklflow>>> AsignaDatosRadicacionAsync(
    IReadOnlyCollection<RelacionCamposRutaWorklflow>? relaciones,
    RegistrarRadicacionEntranteRequestDto? request)
```

## Reglas de transformacion
- Recorre la lista `relaciones`.
- Busca cada `NombreCampoPlantilla` en un mapa construido desde el request.
- Si existe valor, lo asigna a `DatoCampoPlantilla`.
- Si el campo no existe en el request, deja `DatoCampoPlantilla` vacio.
- Si no hay relaciones, retorna `success=true`, `message="Sin resultados"` y lista vacia.
- Si el request es nulo, retorna `success=false` con error de validacion.

## Campos fuente soportados
- Campos dinamicos de `request.Campos`.
- Fijos derivados del request:
  - `Asunto` / `ASUNTO`
  - `Anexos_Cor` / `ANEXOS_COR`
  - `Destinatario_Cor`
  - `Remitente_Cor`
  - `Numero_Folios`
  - `Descripcion_Documento`
  - `Descripcion`
  - `FECHALIMITERESPUESTA`
  - `Tipo_radicado_plantilla`
  - `TipoRadicado`
  - `IdtipoRadicado`
  - `IdTipoRadicado`
  - `System_Plantilla_Radicado_id_Plantilla`
  - `Destinatario_Externo_id_Dest_Ext`
  - `Remit_Dest_Interno_id_Remit_Dest_Int`
  - `Expediente`
  - `id_Expediente`
  - `tipo_doc_entrante_id_tipo_doc_entrante`
  - `id_tipo_flujo_workflow`

## Registro DI
- `DocuArchi.Api/Program.cs`

## Ejemplo de consumo interno
```csharp
var result = await _asingacionValoresDatosRadicadoRutaWorklflow.AsignaDatosRadicacionAsync(
    relaciones,
    registrarRadicacionEntranteRequestDto);
```
