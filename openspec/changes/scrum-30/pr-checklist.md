# SCRUM-30 - PR Checklist by Repository

## 1) MiApp.Models

- [x] Crear modelo `raradestadosmoduloradicacion` en `/Models/Radicacion/Tramite/`.
- [x] Aplicar DataAnnotations y XML docs.
- [ ] Commit sugerido: `SCRUM-30: add raradestadosmoduloradicacion model`
- [ ] Abrir PR y pegar URL en `sync.md`.

## 2) MiApp.DTOs

- [x] Crear `ListaRadicadosPendientesDto` en `/DTOs/Radicacion/Tramite/`.
- [x] Validar contrato para tabla MUI (`id_estado_radicado`, `consecutivo_radicado`, `remitente`, `fecha_registro`, `opciones`).
- [ ] Commit sugerido: `SCRUM-30: add ListaRadicadosPendientesDto`
- [ ] Abrir PR y pegar URL en `sync.md`.

## 3) MiApp.Repository

- [x] Implementar `SolicitaListaRadicadosPendientes` + interfaz en mismo archivo.
- [x] Parametrizar consulta (`id_usuario_radicado`, `system_plantilla_radicado_id_plantilla`, `estado=1`).
- [x] Configurar `QueryOptions.DefaultAlias = defaultDbAlias`.
- [x] Manejar contratos de `Sin resultados` y `Error`.
- [ ] Commit sugerido: `SCRUM-30: implement pending radicados repository query`
- [ ] Abrir PR y pegar URL en `sync.md`.

## 4) MiApp.Services

- [x] Implementar `ServiceListaRadicadosPendientes` + interfaz en mismo archivo.
- [x] Orquestar dependencias: usuario gestion + plantilla + lista pendientes.
- [x] Validar `Relacion_Id_Usuario_Radicacion > 0` y retornar error de validacion si no cumple.
- [x] Mapear DTO con AutoMapper en `/Service/Mapping/Radicacion/Tramite`.
- [ ] Commit sugerido: `SCRUM-30: implement ServiceListaRadicadosPendientes`
- [ ] Abrir PR y pegar URL en `sync.md`.

## 5) DocuArchi.Api

- [x] Crear endpoint `ApListaRadicadosPendientes` en `/Controllers/Radicacion/Tramite`.
- [x] Obtener `defaultDbAlias` por claim `defaulalias` (sin query param).
- [x] Envolver en `try/catch` y retornar `AppResponses`.
- [x] Registrar interfaces en `Program.cs` (`Services (L)` / `Repositories (R)`).
- [ ] Commit sugerido: `SCRUM-30: add ApListaRadicadosPendientes controller endpoint`
- [ ] Abrir PR y pegar URL en `sync.md`.

## 6) Validation

- [x] Unit tests en servicios/repositorios.
- [ ] Integration tests con MySQL Testcontainers (o `skipped` justificado).
- [x] `dotnet test` verde en repos impactados.
- [x] Contract validation OpenSpec actualizada.

## 7) Archive Gate

- [ ] Todas las PRs de repos impactados en estado `merged`.
- [ ] `sync.md` actualizado con URLs y estado final.
- [ ] Ejecutar `opsxj:archive SCRUM-30` por repo impactado.
- [ ] Ejecutar `opsxj:archive SCRUM-30` en repo coordinador.
