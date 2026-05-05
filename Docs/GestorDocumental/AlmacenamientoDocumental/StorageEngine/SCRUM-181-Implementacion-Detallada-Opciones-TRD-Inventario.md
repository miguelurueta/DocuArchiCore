# SCRUM-181 - Implementacion Detallada Opciones TRD/Inventario

## Alcance implementado
Se implementaron ajustes de compatibilidad y estabilizacion de pruebas para cerrar el flujo funcional de opciones legacy.

## Cambios de codigo

### MiApp.DTOs
- Archivo: `DTOs/GestorDocumental/AlmacenamientoDocumental/ExpedienteStorageDto.cs`
- Cambio: adicion de `IdClaseDocumento` (`int?`) para reglas expediente/unidad.

### MiApp.Services
- Archivo: `Service/GestorDocumental/AlmacenamientoDocumental/Physical/IStoragePathResolver.cs`
  - Se restauran:
    - `GetStorageRoot(string nombreGabinete)`
    - `GetFinalFolder(string nombreGabinete, int disco, int carpeta)`
- Archivo: `Service/GestorDocumental/AlmacenamientoDocumental/Physical/StoragePathResolver.cs`
  - Se implementan ambos metodos con validaciones de seguridad.

### DocuArchiCore (tests)
- Ajuste de pruebas de StorageEngine para contratos actuales:
  - `StorageValidationPipelineTests`
  - `StorageTransactionCoordinatorTests`
  - `StorageTransactionCoordinatorIntegrationTests`
  - `StorageEngineContractsTests`
  - `AlmacenarDocumentoUseCaseTests`
  - `WorkflowStorageLogBuilderTests`

## Resultado tecnico
- Se eliminan errores de compilacion por miembros faltantes (`CS1061`).
- Se normaliza compatibilidad entre DTO/servicios/tests.
- Se deja el modulo compilandose en `main`.

## PRs asociados
- `DocuArchiCore` #239
- `MiApp.DTOs` #66
- `MiApp.Services` #122
