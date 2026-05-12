# SCRUM-198 - Pruebas Correccion DbNull StorageEngine

## Objetivo

Validar que el flujo transaccional de almacenamiento no envía `DBNull.Value` como parámetro a Dapper para campos opcionales.

## Evidencia ejecutada

Comando:

```powershell
dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj --filter "InventarioDocumentalRepositoryTests|GabineteStorageRepositoryTests|IndiceElectronicoRepositoryTests|WorkflowStorageLogRepositoryTests|StorageTransactionCoordinatorTests" -nologo
```

Resultado:
- Total: 18
- Exitosas: 18
- Fallidas: 0

## Cobertura específica agregada

Archivo:
- `tests/TramiteDiasVencimiento.Tests/InventarioDocumentalRepositoryTests.cs`

Validaciones añadidas:
1. `SERIE_DOCUMENTO` se envía como `null`.
2. `SUBSERIE_DOCUMENTO` se envía como `null`.
3. `ID_SERIE_DOCUMENTO` se envía como `null`.
4. `ID_SUBSERIE_DOCUMENTO` se envía como `null`.
5. El diccionario de parámetros no contiene instancias `DBNull`.

## Riesgos residuales

- Existen warnings históricos de nulabilidad fuera del alcance del ticket.
- No se detectaron regresiones en la ruta de almacenamiento cubierta por las pruebas focalizadas.
