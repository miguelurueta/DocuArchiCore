# EPICA - API-first DocuArchi.Api

## Objetivo de la epica

Converger progresivamente la arquitectura actual hacia un modelo API-first donde `DocuArchi.Api` sea el host principal, `DocuArchiCore` quede como host legado transitorio y las capas internas recuperen boundaries consistentes.

## Resultado esperado

- `DocuArchi.Api` como backend principal.
- `DocuArchiCore` sin crecimiento funcional nuevo.
- Wiring modularizado.
- Controllers consumiendo solo servicios de aplicacion.
- Dependencias entre proyectos coherentes.
- Configuracion y secretos saneados.
- Plan controlado para reducir el host legado.

## Tickets incluidos

### Arquitectura

- `01-ARQ-Convergencia-api-first-docuarchi-api.md`
- `10-ARQ-Normalizar-namespaces-y-ownership-de-capas.md`
- `12-ARQ-Retiro-progresivo-docuarchicore-host-legado.md`

### Backend

- `02-BE-Modularizar-composition-root-docuarchi-api.md`
- `04-BE-Eliminar-dependencia-inversa-repository-services.md`
- `06-BE-Enforce-controller-service-boundary-docuarchi-api.md`

### Seguridad / configuracion

- `08-SEC-Sanar-configuracion-y-secretos-api-first.md`

### Regresion / QA

- `03-QA-Pruebas-regresion-modularizacion-composition-root-docuarchi-api.md`
- `05-QA-Pruebas-regresion-dependencia-inversa-repository-services.md`
- `07-QA-Pruebas-regresion-controller-service-boundary-docuarchi-api.md`
- `09-QA-Pruebas-regresion-configuracion-y-secretos-api-first.md`
- `11-QA-Pruebas-regresion-namespaces-y-ownership-de-capas.md`
- `13-QA-Pruebas-regresion-retiro-progresivo-docuarchicore-host-legado.md`

## Orden recomendado

### Fase 1 - Alineacion y control del backlog

1. `01-ARQ-Convergencia-api-first-docuarchi-api`
2. `08-SEC-Sanar-configuracion-y-secretos-api-first`
3. `09-QA-Pruebas-regresion-configuracion-y-secretos-api-first`

Objetivo:

- Dejar decidido el norte.
- Bajar riesgo operativo inmediato por secretos versionados.

### Fase 2 - Estabilizar el host principal

4. `02-BE-Modularizar-composition-root-docuarchi-api`
5. `03-QA-Pruebas-regresion-modularizacion-composition-root-docuarchi-api`

Objetivo:

- Hacer mantenible el host principal antes de mover mas piezas.

### Fase 3 - Saneamiento de capas

6. `04-BE-Eliminar-dependencia-inversa-repository-services`
7. `05-QA-Pruebas-regresion-dependencia-inversa-repository-services`
8. `10-ARQ-Normalizar-namespaces-y-ownership-de-capas`
9. `11-QA-Pruebas-regresion-namespaces-y-ownership-de-capas`

Objetivo:

- Corregir el grafo de dependencias y ownership tecnico real.

### Fase 4 - Boundary de aplicacion

10. `06-BE-Enforce-controller-service-boundary-docuarchi-api`
11. `07-QA-Pruebas-regresion-controller-service-boundary-docuarchi-api`

Objetivo:

- Asegurar que la API principal tenga un boundary estable controller -> service.

### Fase 5 - Retiro controlado del legado

12. `12-ARQ-Retiro-progresivo-docuarchicore-host-legado`
13. `13-QA-Pruebas-regresion-retiro-progresivo-docuarchicore-host-legado`

Objetivo:

- Reducir progresivamente la superficie del host legado sin afectar negocio.

## Dependencias entre tickets

| Ticket | Depende de | Motivo |
| --- | --- | --- |
| 02 | 01 | El host principal debe estar formalmente definido |
| 03 | 02 | Pruebas de regresion del refactor de DI |
| 04 | 01 | La convergencia define la direccion de dependencias |
| 05 | 04 | Pruebas del saneamiento Repository/Services |
| 06 | 02, 04 | Conviene migrar controllers cuando el host y capas esten mas estables |
| 07 | 06 | Pruebas de regresion del boundary controller/service |
| 08 | 01 | La configuracion debe alinearse al host principal |
| 09 | 08 | Pruebas de regresion del saneamiento de secretos |
| 10 | 04 | Primero corregir ownership real, luego naming |
| 11 | 10 | Pruebas de regresion del rename / ownership |
| 12 | 01, 02, 06 | El retiro del legado depende de consolidar el host principal |
| 13 | 12 | Pruebas del retiro gradual |

## Priorizacion sugerida

### Alta

- `01`
- `02`
- `04`
- `06`
- `08`

### Media

- `10`
- `12`

### Obligatoria de soporte

- `03`
- `05`
- `07`
- `09`
- `11`
- `13`

## Criterio de exito de la epica

La epica se considera bien encaminada cuando:

- nuevas capacidades backend solo se implementan en `DocuArchi.Api`
- `Program.cs` del host principal deja de ser un inventario manual gigante
- `MiApp.Repository` ya no compila archivos desde `MiApp.Services`
- los controladores API principales ya no consumen repositorios directos
- los secretos ya no viven en `appsettings.json`
- existe inventario y plan verificable para reducir `DocuArchiCore`

## Riesgos si no se ejecuta

- crecimiento paralelo en dos hosts
- mayor drift de wiring y seguridad
- mas deuda de capas y namespaces
- mayor costo de mantenimiento por cada nueva historia
- mas riesgo operativo en despliegue y configuracion
