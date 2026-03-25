# SCRUM-100 - Integracion Backend

## Resumen

- Se migra la funcion legacy `Actualiza_estado_registro_modulo_radicacion`.
- La migracion se implementa en `RaRadEstadosModuloRadicacionR` sin crear capas extra.
- El metodo nuevo usa `IDapperCrudEngine` + `QueryOptions` y retorna `AppResponses<bool>`.

## Impacto Real

- `MiApp.Repository`: agrega `ActualizaEstadoModuloRadicacio(...)` a `IRaRadEstadosModuloRadicacionR`.
- `DocuArchiCore`: agrega pruebas unitarias y trazabilidad OpenSpec.
- `MiApp.DTOs`, `MiApp.Models`, `MiApp.Services`, `DocuArchi.Api`: sin cambios funcionales adicionales.

## Flujo Migrado

1. Valida `defaultDbAlias` e `idRegistroEstado`.
2. Construye `QueryOptions` para `ra_rad_estados_modulo_radicacion`.
3. Actualiza el campo `estado` usando `UpdateDynamicWithValidationAsync`.
4. Retorna `YES` si la actualizacion fue efectiva.
5. Retorna `Sin resultados` si no hubo filas afectadas.
6. Retorna error controlado ante excepcion.

## Verificacion

- `dotnet test .tmp\\scrum95-harness\\TramiteDiasVencimiento.Tests.csproj --filter RaRadEstadosModuloRadicacionRepositoryTests -m:1`
