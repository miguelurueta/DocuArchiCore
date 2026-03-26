# SCRUM-101 - Integracion Backend

## Resumen

`RegistrarRadicacionEntranteAsync` ahora expone `IdEstadoRadicado` dentro de `ReturnRegistraRadicacion`.

## Cambios aplicados

- `MiApp.Repository/Repositorio/Radicador/Tramite/RegistrarRadicacionEntranteRepository.cs`
  - `ExecuteQ08InsertEstadoInicialAsync` retorna `LAST_INSERT_ID()` de `ra_rad_estados_modulo_radicacion`.
  - `RegistrarRadicacionEntranteAsync` agrega `IdEstadoRadicado` al DTO de retorno y a `MetadataOperativa`.
- `MiApp.DTOs/DTOs/Radicacion/Tramite/RegistrarRadicacionEntranteDtos.cs`
  - `ReturnRegistraRadicacionDto` agrega `IdEstadoRadicado`.
- `MiApp.Services/Service/Radicacion/Tramite/RegistrarRadicacionEntranteService.cs`
  - normaliza `IdEstadoRadicado` desde `MetadataOperativa` para compatibilidad legacy.

## Validacion

- prueba transaccional del repositorio valida que Q08 devuelva `IdEstadoRadicado`
- prueba del servicio valida normalizacion legacy desde `MetadataOperativa`
