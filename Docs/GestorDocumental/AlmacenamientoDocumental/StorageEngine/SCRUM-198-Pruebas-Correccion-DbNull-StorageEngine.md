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

## Evidencia de revisión global `DBNull`

Comandos ejecutados:

```powershell
rg -n "DBNull\.Value|System\.DBNull|Convert\.DBNull" -g "*.cs" D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Repository
rg -n "DBNull\.Value|System\.DBNull|Convert\.DBNull" -g "*.cs" D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Services
rg -n "DBNull\.Value|System\.DBNull|Convert\.DBNull" -g "*.cs" D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Models
```

Resultado:
1. `MiApp.Repository`: sin coincidencias activas en Storage Engine.
2. `MiApp.Models`: sin coincidencias.
3. `MiApp.Services`: 4 coincidencias fuera de Storage (`Workflow/BandejaCorrespondencia`).

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
