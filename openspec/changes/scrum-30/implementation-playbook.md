# SCRUM-30 - Multi-Repo Implementation Playbook

## Objective

Cerrar las tareas pendientes de implementacion (`3.1`, `3.2`, `4.1`, `4.3`) fuera del repo coordinador `DocuArchiCore`.

## Repo Execution Order

1. `MiApp.Models`
2. `MiApp.DTOs`
3. `MiApp.Repository`
4. `MiApp.Services`
5. `DocuArchi.Api`
6. `DocuArchiCore` (coordinador: evidencia final + archive)

## Required Deliverables by Repository

### 1) MiApp.Models

- Crear `raradestadosmoduloradicacion` en `/Models/Radicacion/Tramite/`.
- Aplicar DataAnnotations (`Key`, `Column`, `Required`, `StringLength` segun estructura real).
- Documentar propiedades con XML comments.

### 2) MiApp.DTOs

- Crear DTO `ListaRadicadosPendientesDto` en `/DTOs/Radicacion/Tramite/`.
- Campos contrato MUI:
  - `id_estado_radicado` (oculto)
  - `consecutivo_radicado` (`Radicado`)
  - `remitente` (`Remitente`)
  - `fecha_registro` (`Fecha`)
  - `opciones`

### 3) MiApp.Repository

- Implementar `SolicitaListaRadicadosPendientes`.
- Incluir interfaz en el mismo archivo que la clase.
- Query parametrizada (sin concatenacion) sobre `ra_rad_estados_modulo_radicacion`.
- Filtros: `id_usuario_radicado`, `system_plantilla_radicado_id_plantilla`, `estado = 1`.
- `QueryOptions.DefaultAlias = defaultDbAlias`.
- Contratos:
  - sin registros: `Success=true`, `Data=null`, `Message="Sin resultados"`
  - excepcion: `Success=false`, `ErrorMessage=ex.Message`, `Data` vacio

### 4) MiApp.Services

- Implementar `ServiceListaRadicadosPendientes` en `/Service/Radicacion/Tramite`.
- Orquestar:
  - `SolicitaEstructuraIdUsuarioGestion(defaultDbAlias)`
  - `SolicitaEstructuraPlantillaRadicacionDefault(defaultDbAlias)`
  - `SolicitaListaRadicadosPendientes(idPlantilla, idUsuarioRadicacion, defaultDbAlias)`
- Validar `Relacion_Id_Usuario_Radicacion > 0`; si no, retornar error de validacion y no continuar.
- Incluir interfaz en mismo archivo.
- `try/catch` + salida `AppResponses`.
- Configurar mapping en `/Service/Mapping/Radicacion/Tramite` y `AutoMapperProfile`.

### 5) DocuArchi.Api

- Crear endpoint `ApListaRadicadosPendientes` en `/Controllers/Radicacion/Tramite`.
- Obtener `defaultDbAlias` desde claim `defaulalias` y validar claim `usuarioid`.
- `try/catch` + retorno `AppResponses`.
- Registrar DI en `Program.cs`:
  - servicios bajo `// Services (L)`
  - repositorios bajo `// Repositories (R)`

## Tests (Task 4.1)

- Unit tests:
  - exito con datos
  - sin resultados
  - excepcion controlada
- Integration tests (MySQL Testcontainers/Docker):
  - validar consulta y shape de salida
- Contract tests OpenSpec:
  - validar shape `AppResponses`

Si Docker no esta disponible, marcar integration tests como `skipped` con mensaje explicito.

## Validation & Evidence

- `dotnet test`
- `openspec validate scrum-30` en repo coordinador
- Evidencia minima por repo:
  - URL PR
  - resultado pruebas
  - ruta archivos clave

## Archive Readiness (Task 4.3)

Cuando todos los PRs impactados esten mergeados:

1. Actualizar `sync.md` con URLs y estado `merged`.
2. Ejecutar `opsxj:archive SCRUM-30` en cada repo impactado.
3. Ejecutar `opsxj:archive SCRUM-30` en repo coordinador.
