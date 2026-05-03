# SCRUM-169 Implementacion Detallada Expediente Indice

## Repos impactados
- `MiApp.Models`
- `MiApp.Repository`
- `MiApp.Services`
- `DocuArchi.Api`
- `DocuArchiCore` (OpenSpec, pruebas, docs)

## Archivos principales
### MiApp.Models
- `ExpedienteInfoModel.cs`
- `UnidadConservacionInfoModel.cs`
- `IndiceElectronicoCalculationResult.cs`
- `IndiceElectronicoInsertModel.cs`

### MiApp.Repository
- `Expediente/IExpedienteRepository.cs`
- `UnidadConservacion/IUnidadConservacionRepository.cs`
- `IndiceElectronico/IIndiceElectronicoRepository.cs`

### MiApp.Services
- `Expediente/IIndiceElectronicoCalculator.cs`
- `Expediente/IndiceElectronicoCalculator.cs`
- `Expediente/IIndiceElectronicoBuilder.cs`
- `Expediente/IndiceElectronicoBuilder.cs`
- `Transaction/StorageTransactionCoordinator.cs` (integracion de fase)

### DocuArchi.Api
- `Program.cs` (registro DI de repositorios y servicios nuevos)

## Detalle tecnico
- Locks de expediente/unidad implementados con `QueryOptions.LockMode = ForUpdate`.
- Updates de expediente/unidad con filtros optimistas de valores previos.
- Insercion de indice con `InsertBeginTrandAsync` y retorno de identidad.
- Hash documental calculado con SHA256 deterministico sobre datos del documento/indice.
- Fase archivistica ejecutada dentro de la misma transaccion del flujo storage.
