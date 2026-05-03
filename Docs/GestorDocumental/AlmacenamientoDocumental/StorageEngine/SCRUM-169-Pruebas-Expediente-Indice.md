# SCRUM-169 Pruebas Expediente Indice

## Unitarias implementadas
- `IndiceElectronicoCalculatorTests`
- `IndiceElectronicoBuilderTests`
- `ExpedienteRepositoryTests`
- `UnidadConservacionRepositoryTests`
- `IndiceElectronicoRepositoryTests`
- `StorageTransactionCoordinatorTests` (caso fase expediente-indice)

## Cobertura validada
- Calculo de orden/paginas y validaciones de folios.
- Hash SHA256 deterministico.
- Construccion de `QueryOptions` para lock/update/insert.
- Orquestacion de coordinator para commit transaccional en fase expediente-indice.

## Ejecucion
- `dotnet test` ejecutado sobre `TramiteDiasVencimiento.Tests`.
- Estado actual del entorno: bloqueo de MSBuild (`_GetProjectReferenceTargetFrameworkProperties`) sin errores C# detallados.
- Evidencia registrada en `.tmp/test-169.log`.

## Pendiente
- Re-ejecutar suite completa cuando se resuelva el bloqueo de build del entorno.
