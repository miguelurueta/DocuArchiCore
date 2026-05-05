# SCRUM-183 - Pruebas Inventario Documental

## Pruebas ejecutadas
Comando:
`dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj --filter "InventarioDocumentalRepositoryTests|InventarioDocumentalBuilderTests|StorageTransactionCoordinatorTests|StorageValidationPipelineTests"`

Resultado:
- Total: 22
- Passed: 22
- Failed: 0

## Cobertura agregada
- `InventarioDocumentalBuilderTests`
  - omite insercion cuando opcion inventario esta apagada
  - error cuando inventario aplica y DTO falta
  - error cuando inventario aplica y falta metadata fisica
  - mapeo legacy (TRD/expediente/unidad/fulltext/segundo nombre/tamano/formato)
- `StorageTransactionCoordinatorTests`
  - inserta inventario cuando la opcion aplica
  - no inserta inventario cuando la opcion no aplica
- `InventarioDocumentalRepositoryTests`
  - valida requeridos
  - trunca fulltext largo
  - retorna id de insercion
