# JIRA CREATION ORDER - API-first DocuArchi.Api

## Objetivo

Definir el orden exacto de creacion de la epica, stories y subtasks QA en Jira para el backlog `API-first DocuArchi.Api`.

## Regla de carga

- Crear primero la **Epic**
- Crear despues las **Stories**
- Crear inmediatamente la **Sub-task QA** de cada Story
- No dejar tickets QA sueltos sin Story padre

## Orden exacto de creacion

### 1. Epic

**Titulo**

`API-first DocuArchi.Api`

### 2. Story

**Titulo**

`Definir convergencia API-first con DocuArchi.Api como host principal`

### 3. Story

**Titulo**

`Sanear configuracion y secretos para el modelo API-first`

### 4. Sub-task QA

**Parent**

`Sanear configuracion y secretos para el modelo API-first`

**Titulo**

`Pruebas de regresion para saneamiento de configuracion y secretos en API-first`

### 5. Story

**Titulo**

`Modularizar el composition root de DocuArchi.Api por dominios`

### 6. Sub-task QA

**Parent**

`Modularizar el composition root de DocuArchi.Api por dominios`

**Titulo**

`Pruebas de regresion para modularizacion del composition root de DocuArchi.Api`

### 7. Story

**Titulo**

`Eliminar dependencia inversa entre MiApp.Repository y codigo fuente de MiApp.Services`

### 8. Sub-task QA

**Parent**

`Eliminar dependencia inversa entre MiApp.Repository y codigo fuente de MiApp.Services`

**Titulo**

`Pruebas de regresion por eliminacion de dependencia inversa entre Repository y Services`

### 9. Story

**Titulo**

`Normalizar namespaces y ownership real de capas en la solucion API-first`

### 10. Sub-task QA

**Parent**

`Normalizar namespaces y ownership real de capas en la solucion API-first`

**Titulo**

`Pruebas de regresion para normalizacion de namespaces y ownership de capas`

### 11. Story

**Titulo**

`Forzar el boundary controller -> service en DocuArchi.Api`

### 12. Sub-task QA

**Parent**

`Forzar el boundary controller -> service en DocuArchi.Api`

**Titulo**

`Pruebas de regresion para migracion de controllers a application services en DocuArchi.Api`

### 13. Story

**Titulo**

`Definir retiro progresivo de DocuArchiCore como host legado`

### 14. Sub-task QA

**Parent**

`Definir retiro progresivo de DocuArchiCore como host legado`

**Titulo**

`Pruebas de regresion para el retiro progresivo de DocuArchiCore como host legado`

## Asociaciones obligatorias

- Todas las **Stories** deben quedar asociadas a la Epic:
  - `API-first DocuArchi.Api`
- Cada ticket **QA** debe quedar como **Sub-task** de su Story tecnica correspondiente

## Fuente de detalle

Para descripcion, prioridad, dependencias y criterios de aceptacion usar:

- `14-JIRA-READY-BACKLOG.md`

Como respaldo detallado por ticket usar:

- `01-ARQ-Convergencia-api-first-docuarchi-api.md`
- `02-BE-Modularizar-composition-root-docuarchi-api.md`
- `03-QA-Pruebas-regresion-modularizacion-composition-root-docuarchi-api.md`
- `04-BE-Eliminar-dependencia-inversa-repository-services.md`
- `05-QA-Pruebas-regresion-dependencia-inversa-repository-services.md`
- `06-BE-Enforce-controller-service-boundary-docuarchi-api.md`
- `07-QA-Pruebas-regresion-controller-service-boundary-docuarchi-api.md`
- `08-SEC-Sanar-configuracion-y-secretos-api-first.md`
- `09-QA-Pruebas-regresion-configuracion-y-secretos-api-first.md`
- `10-ARQ-Normalizar-namespaces-y-ownership-de-capas.md`
- `11-QA-Pruebas-regresion-namespaces-y-ownership-de-capas.md`
- `12-ARQ-Retiro-progresivo-docuarchicore-host-legado.md`
- `13-QA-Pruebas-regresion-retiro-progresivo-docuarchicore-host-legado.md`

## Recomendacion operativa

Crear los tickets en este mismo orden para que:

- la Epic exista antes de asociar Stories
- las Stories tecnicas existan antes de crear subtasks QA
- el backlog quede alineado con la priorizacion sugerida en `14-JIRA-READY-BACKLOG.md`
