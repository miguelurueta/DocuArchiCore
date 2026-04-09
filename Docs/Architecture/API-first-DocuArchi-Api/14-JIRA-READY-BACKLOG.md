# JIRA READY BACKLOG - API-first DocuArchi.Api

## Epica

### Titulo

API-first DocuArchi.Api

### Tipo

Epic

### Resumen

Converger progresivamente la solucion actual hacia un modelo API-first donde `DocuArchi.Api` sea el host principal, `DocuArchiCore` quede como host legado transitorio y las capas internas recuperen boundaries consistentes.

### Resultado esperado

- `DocuArchi.Api` como backend principal
- `DocuArchiCore` sin crecimiento funcional nuevo
- wiring modularizado
- controllers consumiendo solo servicios de aplicacion
- dependencias entre proyectos coherentes
- configuracion y secretos saneados
- plan controlado para reducir el host legado

---

## Sprint 1 recomendado

### Ticket 01

**Summary**  
Definir convergencia API-first con DocuArchi.Api como host principal

**Issue Type**  
Story

**Priority**  
Highest

**Depends On**  
Ninguno

**Description**  
Formalizar `DocuArchi.Api` como host principal y `DocuArchiCore` como host legado transitorio. Definir criterio para nuevas funcionalidades y roadmap por dominios.

**Acceptance Criteria**

- `DocuArchi.Api` queda definido como host principal
- `DocuArchiCore` queda definido como host legado transitorio
- existe roadmap por dominios
- la decision queda documentada para backlog y futuras implementaciones

**Documento fuente**  
`01-ARQ-Convergencia-api-first-docuarchi-api.md`

### Ticket 08

**Summary**  
Sanear configuracion y secretos para el modelo API-first

**Issue Type**  
Story

**Priority**  
Highest

**Depends On**  
01

**Description**  
Retirar secretos reales de `appsettings.json`, mantener placeholders claros y documentar configuracion externa requerida para `DocuArchi.Api`.

**Acceptance Criteria**

- no hay secretos reales versionados en `appsettings.json`
- la configuracion requerida para despliegue queda documentada
- `DocuArchi.Api` sigue siendo desplegable con configuracion externa

**Documento fuente**  
`08-SEC-Sanar-configuracion-y-secretos-api-first.md`

### Ticket 09

**Summary**  
Pruebas de regresion para saneamiento de configuracion y secretos en API-first

**Issue Type**  
Sub-task o Story QA

**Priority**  
Highest

**Depends On**  
08

**Description**  
Validar que el saneamiento de configuracion y secretos no rompa arranque, autenticacion ni conexiones del host principal.

**Acceptance Criteria**

- el host principal arranca con configuracion externa
- las variables criticas se leen correctamente
- existe validacion de fallo controlado cuando falta configuracion obligatoria

**Documento fuente**  
`09-QA-Pruebas-regresion-configuracion-y-secretos-api-first.md`

### Ticket 02

**Summary**  
Modularizar el composition root de DocuArchi.Api por dominios

**Issue Type**  
Story

**Priority**  
High

**Depends On**  
01

**Description**  
Extraer el wiring de `Program.cs` en modulos por dominio e infraestructura sin cambiar comportamiento funcional.

**Acceptance Criteria**

- `Program.cs` queda sustancialmente mas pequeno
- el wiring queda modularizado por dominios
- la aplicacion compila y arranca
- no aparecen errores de resolucion DI en runtime

**Documento fuente**  
`02-BE-Modularizar-composition-root-docuarchi-api.md`

### Ticket 03

**Summary**  
Pruebas de regresion para modularizacion del composition root de DocuArchi.Api

**Issue Type**  
Sub-task o Story QA

**Priority**  
High

**Depends On**  
02

**Description**  
Agregar smoke tests de DI, pipeline y arranque para proteger la modularizacion del host principal.

**Acceptance Criteria**

- el host arranca sin errores de DI
- las dependencias criticas se resuelven
- no hay regresion de pipeline

**Documento fuente**  
`03-QA-Pruebas-regresion-modularizacion-composition-root-docuarchi-api.md`

---

## Sprint 2 recomendado

### Ticket 04

**Summary**  
Eliminar dependencia inversa entre MiApp.Repository y codigo fuente de MiApp.Services

**Issue Type**  
Story

**Priority**  
High

**Depends On**  
01

**Description**  
Eliminar `Compile Include` cruzados desde `MiApp.Repository` hacia `MiApp.Services` y reubicar builders/policies en la capa correcta.

**Acceptance Criteria**

- `MiApp.Repository.csproj` no incluye archivos desde `MiApp.Services`
- la solucion compila
- no hay regresion funcional en radicacion asociada a builders/policies movidos

**Documento fuente**  
`04-BE-Eliminar-dependencia-inversa-repository-services.md`

### Ticket 05

**Summary**  
Pruebas de regresion por eliminacion de dependencia inversa entre Repository y Services

**Issue Type**  
Sub-task o Story QA

**Priority**  
High

**Depends On**  
04

**Description**  
Proteger el saneamiento de dependencias con build checks y regresion funcional de radicacion.

**Acceptance Criteria**

- el cambio queda protegido por pruebas
- no se rompen flujos de radicacion asociados
- no reaparece dependencia inversa por rutas cruzadas

**Documento fuente**  
`05-QA-Pruebas-regresion-dependencia-inversa-repository-services.md`

### Ticket 10

**Summary**  
Normalizar namespaces y ownership real de capas en la solucion API-first

**Issue Type**  
Story

**Priority**  
Medium

**Depends On**  
04

**Description**  
Corregir namespaces inconsistentes para que reflejen ensamblado, carpeta y ownership real de la capa.

**Acceptance Criteria**

- los namespaces inconsistentes priorizados quedan normalizados
- la solucion compila
- el host principal sigue resolviendo dependencias

**Documento fuente**  
`10-ARQ-Normalizar-namespaces-y-ownership-de-capas.md`

### Ticket 11

**Summary**  
Pruebas de regresion para normalizacion de namespaces y ownership de capas

**Issue Type**  
Sub-task o Story QA

**Priority**  
Medium

**Depends On**  
10

**Description**  
Validar que los cambios de namespace no rompan compilacion, wiring ni contratos del host principal.

**Acceptance Criteria**

- los cambios de namespace quedan cubiertos por pruebas
- no aparecen errores de tipo o DI por renames
- el host principal sigue funcionando

**Documento fuente**  
`11-QA-Pruebas-regresion-namespaces-y-ownership-de-capas.md`

---

## Sprint 3 recomendado

### Ticket 06

**Summary**  
Forzar el boundary controller -> service en DocuArchi.Api

**Issue Type**  
Story

**Priority**  
High

**Depends On**  
02, 04

**Description**  
Eliminar acceso directo de controladores a repositorios para que la API consuma unicamente servicios de aplicacion.

**Acceptance Criteria**

- los controladores migrados ya no consumen repositorios directos
- el comportamiento HTTP se conserva
- la logica de caso de uso queda en services
- las pruebas actualizadas pasan

**Documento fuente**  
`06-BE-Enforce-controller-service-boundary-docuarchi-api.md`

### Ticket 07

**Summary**  
Pruebas de regresion para migracion de controllers a application services en DocuArchi.Api

**Issue Type**  
Sub-task o Story QA

**Priority**  
High

**Depends On**  
06

**Description**  
Validar que la migracion de controllers a services no altere contratos, codigos HTTP ni comportamiento observable.

**Acceptance Criteria**

- los endpoints migrados conservan su contrato
- no aparecen regresiones funcionales
- la migracion queda protegida por pruebas de regresion

**Documento fuente**  
`07-QA-Pruebas-regresion-controller-service-boundary-docuarchi-api.md`

---

## Sprint 4 recomendado

### Ticket 12

**Summary**  
Definir retiro progresivo de DocuArchiCore como host legado

**Issue Type**  
Story

**Priority**  
Medium

**Depends On**  
01, 02, 06

**Description**  
Inventariar flujos aun activos en `DocuArchiCore`, clasificarlos y definir prerequisitos para su reduccion o retiro.

**Acceptance Criteria**

- existe inventario del host legado
- existe plan de retiro progresivo
- existe criterio tecnico para saber cuando `DocuArchiCore` puede quedar minimizado o retirado

**Documento fuente**  
`12-ARQ-Retiro-progresivo-docuarchicore-host-legado.md`

### Ticket 13

**Summary**  
Pruebas de regresion para el retiro progresivo de DocuArchiCore como host legado

**Issue Type**  
Sub-task o Story QA

**Priority**  
Medium

**Depends On**  
12

**Description**  
Definir smoke tests y checklists de regresion para cada reduccion de superficie del host legado.

**Acceptance Criteria**

- cada cambio de retiro del host legado tiene su prueba o checklist de regresion
- no se desactiva funcionalidad sin evidencia de cobertura
- la migracion a API-first queda controlada

**Documento fuente**  
`13-QA-Pruebas-regresion-retiro-progresivo-docuarchicore-host-legado.md`

---

## Sugerencia de etiquetas Jira

- `architecture`
- `api-first`
- `docuarchi-api`
- `legacy`
- `regression`
- `security`
- `tech-debt`

## Regla operativa

No cerrar ningun ticket BE/ARQ/SEC sin planificar y ejecutar su ticket QA espejo asociado.
