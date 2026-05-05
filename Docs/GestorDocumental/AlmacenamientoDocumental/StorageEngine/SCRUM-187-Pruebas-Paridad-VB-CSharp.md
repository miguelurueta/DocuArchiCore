# SCRUM-187 — Pruebas de Paridad VB Legacy vs C#

## Objetivo
Validar equivalencia funcional del Storage Engine V2 frente a la función legacy `Almacenamiento` con evidencia técnica reproducible.

## Suite implementada
Ruta:
- `tests/TramiteDiasVencimiento.Tests/StorageEngine/Parity/`

Componentes:
- `StorageEngineParityTestSuite.cs`
- `StorageEngineParityFixture.cs`
- `StorageEngineLegacyExpectedBuilder.cs`
- `StorageEngineActualResultReader.cs`
- `StorageEngineParityAssert.cs`
- `StorageEngineTestDataSeeder.cs`
- `StorageEngineFileSystemFixture.cs`
- `StorageEngineXmlAssert.cs`
- `StorageEngineDatabaseAssert.cs`

## Cobertura actual
- Paridad core de identidad/transacción (`ID`, `DISC`, `PAG`, naming DIG/FXL).
- Paridad workflow (`logdocuarchi` columnas críticas).
- Validación XML FXL (atributos base legacy).
- Validación de incremento mínimo `system1` (`proxid`, `numpag_carp`).
- Escenario de concurrencia base (identidades únicas).

## Cobertura diferida por ambiente
- Integración real Docker/Testcontainers con dataset legacy completo.
- E2E completo Controller → UseCase → Orchestrator → DB/FS/XML.
- Concurrencia real 2/5 tareas con `FOR UPDATE` en MySQL real.
