# SCRUM-86 - Integracion Backend

## Objetivo

Ajustar el contrato de `RegistrarRadicacionEntranteAsync` para que el repositorio y el servicio expongan un payload dedicado con `ConsecutivoRadicado` e `IdRadicado`.

## Cambios aplicados

- Se agrega `ReturnRegistraRadicacionDto` en `MiApp.DTOs`.
- `RegistrarRadicacionEntranteRepository` retorna el nuevo payload con el consecutivo persistido y `idRadicadoPlantilla`.
- `RegistrarRadicacionEntranteService` conserva compatibilidad con contratos previos y normaliza `ReturnRegistraRadicacion` cuando el repositorio solo devuelve campos legados.

## Comportamiento funcional

- En registro exitoso, la respuesta mantiene `ConsecutivoRadicado` y además expone `ReturnRegistraRadicacion`.
- `ReturnRegistraRadicacion.ConsecutivoRadicado` refleja el consecutivo persistido.
- `ReturnRegistraRadicacion.IdRadicado` refleja el identificador de radicado generado por la transacción.

## Evidencia tecnica

- `MiApp.DTOs/DTOs/Radicacion/Tramite/RegistrarRadicacionEntranteDtos.cs`
- `MiApp.Repository/Repositorio/Radicador/Tramite/RegistrarRadicacionEntranteRepository.cs`
- `MiApp.Services/Service/Radicacion/Tramite/RegistrarRadicacionEntranteService.cs`
- `tests/TramiteDiasVencimiento.Tests/RegistrarRadicacionEntranteServiceTests.cs`
