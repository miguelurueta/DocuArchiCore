# SCRUM-43 - Servicio SolicitaParametrosRadicados

## Objetivo
Centralizar en backend la consulta de parámetros que requiere `RegistrarRadicacionEntranteAsync` y que no vienen del front.

## Ubicación
- Servicio:
  - `MiApp.Services/Service/Radicacion/PlantillaRadicado/SolicitaParametrosRadicadosService.cs`
- DTO de retorno agregado:
  - `MiApp.Services/Service/Radicacion/PlantillaRadicado/ParametrosRadicadosDto.cs`
- Contrato:
  - `ISolicitaParametrosRadicadosService`
- Registro DI:
  - `DocuArchi.Api/Program.cs`
  - `AddScoped<ISolicitaParametrosRadicadosService, SolicitaParametrosRadicadosService>()`

## Firma
```csharp
Task<AppResponses<ParametrosRadicadosDto>> SolicitaParametrosRadicados(
    int idUsuarioDestinatario,
    int idTipoDocEntrante,
    int idUsuarioRadicador,
    string defaultDbAlias);
```

## Retorno
- DTO agregado: `ParametrosRadicadosDto`
  - `NombreAreaRemitdestDto NombreAreaRemitdest`
  - `TipoDocEntrante TipoDocEntrante`
  - `IdSedeNombreDto IdSedeNombre`
- Envoltorio: `AppResponses<ParametrosRadicadosDto>`

## Flujo Interno
1. Consulta área y nombre área del destinatario:
   - `IRemitDestInternoR.SolicitaIdAreaNombreAreaDestinatario`
2. Consulta estructura de tipo documento entrante:
   - `ITipoDocEntranteR.SolicitaEstructuraTipoDoEntrante`
3. Consulta sede del usuario radicador:
   - `ISedeEmpresaR.RetornaIdNombreSedeEmpresa`
4. Construye respuesta consolidada para consumo de registro.

## Validaciones
- `idUsuarioDestinatario > 0`
- `idTipoDocEntrante > 0`
- `idUsuarioRadicador > 0`
- `defaultDbAlias` no vacío

## Pruebas
- Archivo:
  - `DocuArchiCore/tests/TramiteDiasVencimiento.Tests/SolicitaParametrosRadicadosServiceTests.cs`
- Casos:
  - Retorno exitoso consolidado.
  - Error cuando falla consulta de área (sin ejecutar siguientes repositorios).
  - Validación por `idTipoDocEntrante` inválido.
