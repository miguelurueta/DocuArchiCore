# SCRUM-43 - Integracion RegistrarRadicacionEntrante con SolicitaParametrosRadicados

## Decision de arquitectura
Se integra `RegistrarRadicacionEntranteAsync` con `SolicitaParametrosRadicados` sin dependencia circular.

Flujo aplicado:
1. `RegistrarRadicacionEntranteService.RegistrarRadicacionEntranteAsync`
2. `ISolicitaParametrosRadicadosService.SolicitaParametrosRadicados`
3. `IRegistrarRadicacionEntranteRepository.RegistrarRadicacionEntranteAsync`

No se realiza:
- `RegistrarRadicacionEntranteAsync -> SolicitaParametrosRadicados -> RegistrarRadicacionEntranteAsync`

## Cambios
- Archivo:
  - `MiApp.Services/Service/Radicacion/Tramite/RegistrarRadicacionEntranteService.cs`
- Nueva dependencia inyectada:
  - `ISolicitaParametrosRadicadosService`
- Validaciones agregadas antes de consulta backend:
  - `Destinatario.id_Remit_Dest_Int` requerido
  - `Tipo_tramite.tipo_doc_entrante` requerido
- Enriquecimiento aplicado desde backend:
  - `requestCanonico.Tipo_tramite.tipo_doc_entrante`
  - `requestCanonico.Tipo_tramite.Descripcion` (si venía vacía)

## Comportamiento
- Si `SolicitaParametrosRadicados` falla, se retorna error y no se ejecuta inserción en repositorio.
- Si tiene éxito, continúa el flujo normal con estructura de plantilla + campos dinámicos + registro transaccional.

## Pruebas
- Archivo:
  - `DocuArchiCore/tests/TramiteDiasVencimiento.Tests/RegistrarRadicacionEntranteServiceTests.cs`
- Casos cubiertos:
  - Flujo válido con llamada a `SolicitaParametrosRadicados`.
  - Propagación de alias y usuario radicador.
  - Corte de flujo cuando falla `SolicitaParametrosRadicados`.
