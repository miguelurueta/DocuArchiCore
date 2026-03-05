# Contexto Ampliado de Migracion Legacy -> React + API

Fecha: 2026-03-04  
Estado: Borrador tecnico operativo (base para tickets Jira por etapas)

## 1) Objetivo

Consolidar el contexto completo de migracion desde ASP.NET WebForms/VB.NET monolitico hacia arquitectura moderna:

- Frontend: React (`D:\imagenesda\GestorDocumental\DocuArchiCore.react`)
- Backend: API .NET con patron `Controller -> Service -> Repository -> DTO/Model/Mapping`
- Contrato de respuesta estandar: `AppResponses`

Este documento amplia el contexto existente y define checklist, matriz de mapeo, riesgos, mitigaciones, pruebas y decisiones obligatorias para ejecutar migraciones sin omisiones.

## 1.1) Regla de control previa a tickets

Antes de generar tickets Jira o especificaciones OpenSpec de implementacion, se debe presentar primero un `PLAN DE MIGRACION PROPUESTO` completo y esperar aprobacion explicita del usuario.

Orden obligatorio:
1. Plan de migracion (fases 1-8).
2. Revision/aprobacion del usuario.
3. Solo despues: descomposicion en tickets Jira/OpenSpec por etapas.

## 1.2) Estructura minima del Plan de Migracion (fases 1-8)

1. Inventario de funciones legacy.
   Regla obligatoria: inventariar el 100% de funciones relacionadas (principal, auxiliares, hermanas, disparadores de evento y dependencias directas del flujo). Sin inventario completo no se generan tickets.
2. Reconstruccion del flujo funcional extremo a extremo.
3. Conversion de parametros legacy a DTO de API.
4. Mapeo de consultas SQL legacy a repositorios target.
5. Mapeo Legacy -> Arquitectura Target.
6. Endpoints API propuestos (controller/service/repository/request/response).
7. Dependencias legacy detectadas y reemplazo target.
8. Riesgos tecnicos detectados y mitigaciones.

## 2) Fuentes Analizadas (evidencia directa)

### Origen legacy (ASP.NET clasico / VB.NET)

- `D:\imagenesda\GestorDocumental\Desarrollo\GestionDocumental-Docuarchi.net\GestionDocumental-Docuarchi.net`
- `Web.config`
- `GestionDocumental-Docuarchi.net.vbproj`
- `packages.config`
- `radicador/WebFormRadicacionEntrante.aspx`
- `radicador/WebFormRadicacionEntrante.aspx.vb`
- `radicador/ClassRadicador.vb`
- `Defaul/conect.vb`
- `js/radicacion/WebFormRadicacionEntrante.js`

### Destino backend (.NET)

- `D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api`
- `D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Services`
- `D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Repository`
- `D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.DTOs`
- `DocuArchi.Api/Program.cs`
- `DocuArchi.Api/appsettings.Development.json`
- `DocuArchi.Api/Controllers/Radicacion/Tramite/TramiteController.cs`
- `DocuArchi.Api/Controllers/Radicacion/ConsultaRadicacion/ConsultaRadicacionController.cs`
- `DocuArchi.Api/Controllers/Radicacion/PlantillaRadicado/PlantillaRadicacionController.cs`
- `MiApp.Services/Service/Radicacion/Tramite/*.cs`
- `MiApp.Services/Service/Radicacion/PlantillaRadicado/PlantillaRadicacionL.cs`
- `MiApp.Repository/Repositorio/Radicador/*/*.cs`
- `MiApp.Repository/Repositorio/DataAccess/DapperCrudEngine.cs`
- `MiApp.Repository/Repositorio/DataAccess/DbConnectionFactory.cs`
- `MiApp.DTOs/DTOs/Utilidades/AppResponses.cs`
- `MiApp.DTOs/DTOs/Errors/AppError.cs`

### Destino frontend (React)

- `D:\imagenesda\GestorDocumental\DocuArchiCore.react`
- `package.json`
- `src/api/ApiResponse.ts`
- `src/api/Clienteaxios.ts`
- `src/modules/radicacion/**/*`
- `src/shared/hooks/useAxiosErrorNotifier.ts`

## 3) Inventario Estructural

### Legacy

- Tipo: WebForms + VB.NET (.NET Framework 4.6.1)
- Conteo aproximado: `510` archivos `.aspx`, `503` archivos `.vb`, `8` `.Master`
- Arquitectura: monolitica por carpetas funcionales (`radicador`, `workflow`, `Gestion`, etc.)
- Acoplamiento fuerte UI-servidor:
  - `Page.FindControl(...)`
  - `HiddenField`
  - `UpdatePanel`
  - `IsPostBack`
  - `Session.Item(...)`

### Target backend

- Repositorios especializados:
  - `DocuArchi.Api` (controllers + DI + auth + CORS)
  - `MiApp.Services` (logica y orquestacion)
  - `MiApp.Repository` (acceso datos Dapper + factory conexiones)
  - `MiApp.DTOs` (contratos)
  - `MiApp.Models` (modelos DB)
- Patron de inyeccion/registro en `Program.cs`:
  - `// Repositories (R)`
  - `// Services (L)`

### Target frontend

- Tipo: React + TypeScript + Vite
- Modulos: `api`, `app`, `modules`, `shared`
- Radicacion ya modularizada en `src/modules/radicacion`
- Transporte: `axios` + JWT interceptor + React Query

## 4) Dependencias y Frameworks

### Legacy

- WebForms + AjaxControlToolkit + jQuery
- Conectores MySQL propios (`conect.vb`)
- Sesion FormsAuth (`Web.config`: auth Forms + Session InProc)
- Librerias antiguas (ej. Newtonsoft 5.x, WebApi 4.x en `packages.config`)

### Target

- Backend: ASP.NET Core + Dapper + AutoMapper + JWT + Session + CORS
- Frontend: React 19 + TS + axios + react-query + antd + MUI
- Testing:
  - Backend unit/integration (xUnit + Testcontainers MySQL)
  - Frontend unit/component (`vitest`, `@testing-library/react`)

## 5) Convenciones Detectadas

### Naming

- Legacy: mezcla de `PascalCase`, `snake_like`, mayusculas historicas, nombres de control UI embebidos.
- Target backend:
  - clases/interfaces: `PascalCase`
  - carpetas por dominio/capa
  - interfaces junto a implementacion en varios casos
- Target frontend:
  - componentes/hooks: `PascalCase` y `useX`
  - contratos TS tipados por modulo

### Contrato de respuesta

- Backend: `AppResponses<T>` con `success`, `message`, `data`, `meta`, `errors`
- Frontend: `ApiResponse<T>` equivalente (`success`, `message`, `data`, `meta?`, `errors?`)

### Manejo de errores

- Legacy: string `"YES"` para exito, string de error para falla.
- Target:
  - validacion y negocio via `AppResponses` + `errors[]`
  - fallback a `500` con estructura controlada
  - frontend notifica segun `status` y `errors`

## 6) Reconstruccion de Flujo Funcional Real (Registra_Radicacion_entrante)

### Flujo legacy reconstruido (evidencia)

1. UI WebForm:
   - `WebFormRadicacionEntrante.aspx` carga scripts, tabs, hidden fields y eventos.
2. Evento servidor:
   - `Buttonradicar_entrante_Click` en `WebFormRadicacionEntrante.aspx.vb`.
3. Llamada de negocio:
   - invoca `ClassRadicador.Registra_Radicacion_entrante(...)`.
4. Validaciones acopladas a UI:
   - `FindControl` de checkbox/dropdown/hidden/updatepanel.
5. Reglas negocio:
   - autorizacion dias/horas
   - validacion flujo inicial workflow
   - campos obligatorios y formato fecha
   - reglas de expediente/radicado relacionado
6. Persistencia y operaciones auxiliares:
   - queries y updates en varias tablas
   - asignaciones y estados
   - trazabilidad y componentes auxiliares
7. Salida:
   - `"YES"` o texto de error funcional.

### Flujo target equivalente esperado

1. React recoge datos en DTO request (sin `FindControl`/`Hidden` server-side).
2. API valida claims (`defaulalias`, `usuarioid`) y request.
3. Service centraliza reglas de negocio y orquesta repositorios.
4. Repository ejecuta SQL parametrizado con `QueryOptions.DefaultAlias`.
5. Response uniforme `AppResponses<T>`.
6. Frontend interpreta `success/message/errors` y actualiza UI.

## 7) Mapeo de Conectores Legacy -> Alias Target (confirmado por codigo)

Evidencia: `Defaul/conect.vb` + `DocuArchi.Api/appsettings.Development.json` + `DbConnectionFactory.cs`.

- `conect.Dbase_Conction_Mysql` -> DB workflow (`DB_NAME_MODULO`) -> alias target `WF` (`MySqlConnection_WF`)
- `conect.Dbase_Conction_Mysql_RA` -> DB radicacion (`RA_DB_NAME_MODULO`) -> alias target `DA` (`MySqlConnection_DA`)
- `conect.Dbase_Conction_Mysql_DA` -> DB docuarchi (`DA_DB_NAME_MODULO`) -> alias target `DA` (`MySqlConnection_DA`)

## 8) Dependencias Implicitas Legacy y Reemplazo Objetivo

| Dependencia legacy | Evidencia | Reemplazo target | Estado |
|---|---|---|---|
| `Session.Item("RA_ID_USUARIO")`, `GA_IDUSUARIOGESTION`, etc. | `ClassRadicador.vb`, `WebForm*.vb` | claims JWT (`usuarioid`) + servicios de contexto usuario | Parcialmente resuelto |
| `Page.FindControl(...)` | `Registra_Radicacion_entrante` | DTO tipado de request desde React | Pendiente por endpoint final |
| `HiddenField` como transporte | `.aspx` + `.js` | payload JSON + estado React | Parcialmente resuelto |
| `IsPostBack` / `UpdatePanel` / postback event | WebForms | API stateless + React Query | Pendiente cierre total |
| `YES`/mensaje string | `ClassRadicador.vb` | `AppResponses<T>` con `errors[]` | Parcialmente resuelto |
| SQL string concatenado historico | clases VB | Dapper + filtros parametrizados | Parcialmente resuelto |
| formularios + validacion client/server mezclada | `.aspx` + `.vb` + `.js` | validacion separada (frontend + service) | Pendiente |
| estado workflow en sesion | multiples `.vb` | claims + consultas repositorio | Pendiente decisiones |

## 9) Gaps Tecnicos Legacy vs Target

| Categoria | Legacy | Target | Gap | Accion obligatoria |
|---|---|---|---|---|
| Contrato datos | strings/hiddens/postback | DTO JSON | alto | definir DTO final por funcion |
| Manejo errores | `"YES"`/texto libre | `AppResponses` | alto | normalizar codigos/mensajes |
| Nullables/tipos | conversiones implicitas y `Option Strict Off` | tipado fuerte C#/TS | alto | matriz de tipos por campo |
| Fechas/timezone | parse manual (`DATE`) | `DateTime`/ISO | medio-alto | definir timezone oficial |
| Transacciones | mezcla en flujo monolitico | repositorios aislados | alto | unidad transaccional por caso |
| Idempotencia | no explicita | necesaria en API | medio | estrategia de idempotency key |
| Autorizacion | forms/session + permisos indirectos | JWT/claims/policies | alto | mapear permisos legacy->claims |
| Serializacion | ViewState/Hidden | JSON | alto | versionar contrato request/response |
| Dependencias UI server | muy alta | nula | alto | eliminar `FindControl` y side effects |

## 10) Matriz de Mapeo Inicial (Legacy -> Target)

| Funcion legacy | Punto entrada legacy | Endpoint target | DTO request/response | Service target | Repository target | Tablas principales | Estado |
|---|---|---|---|---|---|---|---|
| `Registra_Radicacion_entrante` | `WebFormRadicacionEntrante.aspx.vb` botón radicar | `PENDIENTE: /api/tramite/.../registrar-radicacion-entrante` | `PENDIENTE: request/response consolidado` | `PENDIENTE` | `PENDIENTE` | `detalle_plantilla_radicado`, `ra_rad_estados_modulo_radicacion` y relacionadas | No implementado |
| `Registra_Radicacion_entrante_interna` | `WebFormRadicacionEntranteInterna.aspx.vb` | `PENDIENTE` | `PENDIENTE` | `PENDIENTE` | `PENDIENTE` | mismas familias + asignacion interna | No implementado |
| `Registra_Radicacion_saliente` | `WebFormRadicacionEntrante.aspx.vb` (ruta saliente) | `PENDIENTE` | `PENDIENTE` | `PENDIENTE` | `PENDIENTE` | flujo saliente + trazabilidad | No implementado |
| consulta flujos | controles tramite/flujo | `/api/tramite/tramites/empsolicitaListaflujosRelacionadosTramite` | query param + `AppResponses` | `IFlujosRelacionadosTramiteService` | `IFlujosRelacionadosTramiteRepository` | tablas de relacion tramite/flujo | Implementado |
| restriccion destinatario | seleccion destinatario | `/api/tramite/tramites/solicitaEstructuraRelacionTipoRestriccion` | query param + `AppResponses` | `IRelacionTipoRestriccionService` | `IRaRestriRelacionTramiteR` | restricciones tramite/destinatario | Implementado |
| lista coincidencias | busqueda en tablas dinamicas | `/api/tramite/consulta-radicacion/apListaCoinsidenciaRadicados` | body DTO + `AppResponses<DynamicUiTableDto>` | `IListaCoinsidenciaRadicadosService` | `IConsultaCoinsidenciaRadicadosRepository` | plantilla + tabla dinamica | Implementado |

## 11) Checklist de Migracion por Funcion

Para cada funcion legacy:

1. Identificar entrada real (evento WebForms + JS asociado).
2. Enumerar todos los `FindControl` y `Hidden` usados.
3. Enumerar todas las `Session.Item` leidas/escritas.
4. Identificar conectores usados (`Mysql`, `Mysql_RA`, `Mysql_DA`).
5. Mapear cada consulta a tabla, filtros y alias target (`WF`/`DA`).
6. Definir DTO request final (sin dependencia UI server).
7. Definir DTO response final (`AppResponses<T>`).
8. Separar reglas de validacion:
   - request (sintactica)
   - negocio (funcional)
   - autorizacion/permisos
9. Diseñar estrategia transaccional (commit/rollback por caso).
10. Definir idempotencia de registro.
11. Implementar Controller/Service/Repository/Mapping.
12. Registrar DI en `Program.cs`.
13. Implementar pruebas unitarias de reglas.
14. Implementar pruebas integracion con MySQL Testcontainers (schema/seed minimos).
15. Ejecutar pruebas de paridad funcional con escenarios legacy.
16. Documentar diferencias funcionales justificadas.

## 12) Decisiones de Arquitectura Obligatorias

1. Prohibido usar `Session.Item`, `FindControl`, `ViewState` en codigo migrado.
2. Toda entrada debe ser DTO versionado de API.
3. Toda salida debe ser `AppResponses<T>`.
4. SQL siempre parametrizado.
5. Resolver alias por `defaultDbAlias` y `QueryOptions.DefaultAlias`.
6. Validaciones de negocio en Service, no en Controller ni Repository.
7. Repository sin logica de UI.
8. Errores controlados con `errors[]` tipado (`Validation`, `Business`, `Exception`).
9. Transacciones explicitas para operaciones multi-tabla.
10. Atomicidad obligatoria en operaciones multi-tabla: si falla cualquier registro del bloque transaccional, se ejecuta rollback completo y no se permiten persistencias parciales.
11. Cada comportamiento corregido respecto al legacy debe quedar documentado con evidencia.

## 13) Riesgos y Mitigaciones

| Riesgo | Impacto | Mitigacion |
|---|---|---|
| Dependencia oculta en `Session.Item` | alto | inventario por funcion + decision de origen de dato (claim/DTO/repo) |
| Contrato legacy difuso (hidden/postback) | alto | DTO canonico + pruebas de contrato |
| Divergencia funcional por refactor | alto | pruebas de paridad por escenario |
| SQL historico no parametrizado | alto | QueryOptions + filtros parametrizados |
| Diferencias de fecha/hora | medio-alto | estandarizar UTC/zonas y formato ISO |
| Errores ambiguos de negocio | medio | catalogo de errores por regla |
| Cobertura insuficiente | alto | gate de pruebas unit+integration+contract |

## 14) Estrategia de Pruebas

### Unitarias

- Service:
  - validaciones de entrada
  - reglas funcionales
  - errores controlados
- Repository:
  - validaciones previas de parametros
  - shape de respuesta en exito/sin resultados/error

### Integracion

- MySQL Testcontainers (ya existe patron en `tests/TramiteDiasVencimiento.Tests`)
- incluir `schema.sql` y `seed.sql` minimos por caso
- validar consultas reales y contratos de salida

### Paridad funcional (legacy vs target)

- Dataset de escenarios legacy (feliz + validaciones + errores funcionales)
- esperado legacy vs esperado target (ajustado por correcciones intencionales)
- reporte de diferencias con justificacion

### Contract (API)

- validar `success/message/data/errors` en escenarios clave
- validar codigos HTTP coherentes (`200`, `400`, `500`)
- validar serializacion de campos criticos

## 15) Rutas Destino Definitivas por Capa

### Backend

- API: `DocuArchi.Api/Controllers/Radicacion/Tramite/`
- Service: `MiApp.Services/Service/Radicacion/Tramite/`
- Repository: `MiApp.Repository/Repositorio/Radicador/Tramite/`
- DTO: `MiApp.DTOs/DTOs/Radicacion/Tramite/`
- Model: `MiApp.Models/Models/Radicacion/Tramite/`
- Mapping: `MiApp.Services/Service/Mapping/Radicacion/Tramite/`
- Tests: `DocuArchiCore/tests/TramiteDiasVencimiento.Tests/`
- Docs: `DocuArchiCore/Docs/Radicacion/Tramite/`

### Frontend (adicional a existentes)

- Proyecto destino frontend: `D:\imagenesda\GestorDocumental\DocuArchiCore.react`
- Modulo radicacion: `src/modules/radicacion/`
- Contratos API compartidos: `src/api/`
- Manejo errores API: `src/shared/hooks/useAxiosErrorNotifier.ts`

## 16) Convencion de Nomenclatura por Capa

1. Controller:
   - verbo + caso de uso claro (`Ap...`, `Solicita...`, `Registrar...`)
2. Service:
   - `I<Nombre>Service` + `<Nombre>Service`
3. Repository:
   - `I<Nombre>Repository` + `<Nombre>Repository`
4. DTO:
   - `<Entidad><Accion>Dto` o `<Entidad>DTO` consistente por modulo
5. Model:
   - reflejar tabla/origen con nombres claros y mapeables
6. Frontend:
   - hooks `useX`, componentes `PascalCase`, serializer separado en `services/`

## 17) Documentacion Minima Requerida por Ticket Jira

Cada ticket de migracion debe incluir:

1. Contexto funcional reconstruido (UI legacy -> backend legacy -> target).
2. Inventario de dependencias implicitas (`Session`, `Hidden`, `FindControl`).
3. Matriz SQL:
   - tabla
   - filtros
   - alias (`WF`/`DA`)
   - tipo de operacion
4. Contrato request/response final.
5. Reglas de validacion y autorizacion.
6. Correcciones intencionales (si aplica) con evidencia.
7. Pruebas requeridas (unit/integration/paridad/contract).
8. Riesgos del ticket y mitigaciones.
9. Evidencia esperada para cierre.
10. Generacion obligatoria de tres artefactos por migracion: Plan de Migracion, Especificacion Consolidada y Ticket Jira consolidado basado en ambos documentos.

## 18) Decisiones Pendientes del Usuario (no asumir)

1. Confirmar ruta canonica del repo legacy fuente:
   - en algunos docs aparece `Desarrollo\Visual-2019\...`
   - en analisis actual se uso `Desarrollo\GestionDocumental-Docuarchi.net\...`
2. Confirmar endpoint final para `Registra_Radicacion_entrante` en API target.
3. Confirmar set minimo de campos request para registro inicial (MVP).
4. Confirmar politica de timezone oficial para fechas de radicacion.
5. Confirmar politica de idempotencia en registro (token externo, hash payload o llave de negocio).
6. Confirmar equivalencia exacta de permisos legacy -> claims/policies actuales.

## 19) Resultado Operativo

Este contexto ampliado ya permite:

1. generar tickets Jira por etapas sin perder dependencias implicitas,
2. mantener trazabilidad legacy->target con matriz funcional,
3. ejecutar migracion con criterios tecnicos verificables.

