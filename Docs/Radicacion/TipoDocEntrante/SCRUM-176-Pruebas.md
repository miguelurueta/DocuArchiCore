# SCRUM-176 - Pruebas

## Unitarias implementadas
- `SolicitaEstructuraTipoDocEntranteServiceTests`
  - Retorna `YES` con `TipoDocEntrante` completo y flags nuevos.
  - Retorna error por `idTipoDocEntrante` inválido.

## Cobertura funcional esperada
- Claim inválido/ausente en controller -> `BadRequest`.
- `idTipoDocEntrante` inválido -> error validación.
- Registro no encontrado -> error funcional controlado.

## Evidencia de ejecución
- Intento de `dotnet test` bloqueado por política local de PowerShell profile no firmado.
- Se deja pendiente validación en pipeline/ambiente con política permitida.
