# SCRUM-208 Pruebas

## Unit tests agregados
- `FirmaElectronicaDocumentoControllerTests`
- `FirmaElectronicaDocumentoServiceTests`
- `FirmaElectronicaDocumentoRepositoryTests`

## Cobertura funcional
- Claim faltante (`defaulalias`).
- Validaciones `idArchivo`/`nombreGabinete`.
- Caso firmado y no firmado.
- QueryOptions parametrizado (`filters/order/limit`).
- Error controlado cuando falla repository.

## Ejecutable
`dotnet test tests/TramiteDiasVencimiento.Tests --filter "FullyQualifiedName~FirmaElectronicaDocumento"`
