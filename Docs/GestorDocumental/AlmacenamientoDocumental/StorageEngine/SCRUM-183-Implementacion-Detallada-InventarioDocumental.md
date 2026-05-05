# SCRUM-183 - Implementacion Detallada Inventario Documental

## Repositorios impactados
- `MiApp.DTOs`
- `MiApp.Models`
- `MiApp.Repository`
- `MiApp.Services`
- `DocuArchi.Api`
- `DocuArchiCore` (tests + docs)

## Cambios principales

### DTOs
- `InventarioDocumentalDto`:
  - agrega `FechaElaboracion`
  - agrega `SegundoNombreDocumento`
- `ExpedienteStorageDto`:
  - agrega `IdTipoUnidadConservacion`
  - agrega `NombreExpediente`, `ClaseDocumento`, `NombreUnidadConservacion`

### Models
- `Inventario/InventarioDocumentalInsertModel`
- `Inventario/InventarioDocumentalBuildResult`

### Services
- `Inventario/IInventarioDocumentalBuilder`
- `Inventario/InventarioDocumentalBuilder`
- `StorageTransactionCoordinator`:
  - integra `TryInsertInventarioAsync`
  - usa `IStorageNamingService` para `SegundoNombreDocumento`
  - retorna `IdRegistroProduccionDocumental` en `StorageTransactionResult`

### Repository
- `InventarioDocumentalRepository`:
  - cambia modelo a `InventarioDocumentalInsertModel`
  - inserta set ampliado de columnas legacy
  - valida `IdUsuarioGestion`, `IdEmpresaDocumento`, `IdDocumentoDocuarchiAlmacen`, `NumeroFolios`, `Tamano`, `Formato`, `NombreGabinete`

### API DI
- `Program.cs` registra `IInventarioDocumentalBuilder`.

## Columnas legacy cubiertas
- remit_dest_interno_idremit_dest_interno
- ID_USUARIO_GESTION
- FECHA_DOCUMENTO
- ID_AREA_DEPARTAMENTO
- ID_SERIE_DOCUMENTO
- SERIE_DOCUMENTO
- ID_SUBSERIE_DOCUMENTO
- SUBSERIE_DOCUMENTO
- ID_TIPO_DOCUMENTO
- DESCRIPCION_TIPO_DOCUMENTO
- FULTEXT_DOCUMENTO
- ID_DOCUMENTO_DOCUARCHI_ALMACEN
- ESTADO_DOCUMENTO_ARCHIVO
- NOMBRE_GABINETE
- NUMERO_FOLIOS
- EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE
- EXPEDIENTE
- ID_TIPO_EXPEDIENTE
- ID_TIPO_UNIDAD_CONSERVACION
- ID_UNIDAD_CONSERVACION
- ID_CLASE_DOCUMENTO
- CLASEDOCUMENTO
- FECHA_ELABORACION
- UNIDADCONSERVA
- NOMBRE_AREA_DEPARTAMENTO
- ID_TIPO_UNIDAD_DOCUMENTAL
- ID_EMPRESA_DOCUMENTO
- RADICADO_DOCUMENTO
- SEGUNDO_NOMBRE_DOCUMENTO
- DOCUMENTO_PRODUCION_DOCUMENTAL
- TAMANO
- FORMATO
