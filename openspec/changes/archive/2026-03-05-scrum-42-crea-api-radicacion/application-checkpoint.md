# SCRUM-42 Application Checkpoint

## Current Status

1. Change scaffolding refined (`proposal`, `design`, `tasks`, `spec`, `sync`).
2. Application stage is prepared in planning artifacts.
3. Real code implementation for `3.1` depends on sibling repos referenced by `DocuArchiCore.csproj`:
   - `..\DocuArchi.Api`
   - `..\MiApp.DTOs`
   - `..\MiApp.Services`
   - `..\MiApp.Repository`
   - `..\MiApp.Models`

## Immediate Execution Plan (3.1)

1. `MiApp.DTOs`
   - Add DTOs for `RegistrarRadicacionRequest/Response`, `ValidarRadicacionRequest/Response`, `FlujoInicialDto`.
2. `MiApp.Repository`
   - Add repository interfaces/implementations for Q01-Q09.
   - Ensure `QueryOptions.DefaultAlias = defaultDbAlias` in all queries.
3. `MiApp.Services`
   - Add `IRadicacionRegistroService`, `IRadicacionValidacionService`, `IFlujoTrabajoService`.
   - Enforce atomic transaction Q01-Q08 with full rollback on failure.
4. `DocuArchi.Api`
   - Add controller endpoints:
     - `POST /api/radicacion/registrar-entrante`
     - `POST /api/radicacion/validar-entrante`
     - `GET /api/radicacion/flujo-inicial`
   - Register DI in `Program.cs`.
5. `MiApp.Models`
   - Add/adjust models required by new persistence flow.

## Guardrails

1. No SQL concatenation.
2. `AppResponses<T>` in all API responses.
3. Rollback total if any Q01-Q08 step fails.
4. Preserve existing local work in repos with active changes (no destructive cleanup).

## Validation Gates

1. `dotnet build` on impacted repos.
2. `dotnet test` with unit/integration/contract scope.
3. `openspec.cmd validate scrum-42-crea-api-radicacion`.
