# SCRUM-43 - Paso de ParametrosBackend a RegistrarRadicacionEntranteRepository

## Objetivo
Permitir que `RegistrarRadicacionEntranteRepository.RegistrarRadicacionEntranteAsync` reciba parámetros backend ya resueltos y no dependa de datos del front para campos derivados.

## Contrato agregado
- DTO nuevo:
  - `MiApp.DTOs/DTOs/Radicacion/Tramite/ParametrosBackendRadicacionDto.cs`
- Campos:
  - `IdAreaRemitDestInterno`
  - `NombreAreaRemitDestInterno`
  - `IdTipoDocEntrante`
  - `DescripcionTipoDocEntrante`
  - `IdSede`
  - `NombreSede`

## Flujo integrado
1. `RegistrarRadicacionEntranteService` llama `SolicitaParametrosRadicados`.
2. Mapea respuesta a `ParametrosBackendRadicacionDto`.
3. Envía ese DTO al repositorio:
   - `RegistrarRadicacionEntranteAsync(..., camposDinamicosPlantilla, parametrosBackend)`

## Cambios en repositorio
- Archivo:
  - `MiApp.Repository/Repositorio/Radicador/Tramite/RegistrarRadicacionEntranteRepository.cs`
- Firma actualizada (interfaz + implementación):
  - recibe `ParametrosBackendRadicacionDto parametrosBackend`

Uso de `parametrosBackend`:
- Q03 (`ExecuteQ03InsertPlantillaAsync`)
  - `Codigo_Sede` <- `IdSede`
  - `Id_area_remit_dest_interno` <- `IdAreaRemitDestInterno`
  - `Area_remit_dest_interno` <- `NombreAreaRemitDestInterno` (si no viene desde front)
  - `Descripcion_Documento` <- `DescripcionTipoDocEntrante` (fallback)
  - `tipo_doc_entrante_id_tipo_doc_entrante` <- `IdTipoDocEntrante` (fallback)
- Q06 (`ExecuteQ06InsertSolicitudRespuestaAsync`)
  - `ID_AREA` <- `IdAreaRemitDestInterno`
- Q08 (`ExecuteQ08InsertEstadoInicialAsync`)
  - `tipo_doc_entrante_id_Tipo_Doc_Entrante` <- `IdTipoDocEntrante`

## Pruebas ejecutadas
- Suite filtrada:
  - `RegistrarRadicacionEntranteServiceTests`
  - `RegistrarRadicacionEntranteRepositoryTransactionTests`
  - `SolicitaParametrosRadicadosServiceTests`
- Resultado:
  - `Superado: 10`
  - `Error: 0`
