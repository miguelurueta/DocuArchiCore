# Ticket Tecnico: Evaluacion y Extraccion de `opsxj:orchestrate:new`

## Titulo

Evaluar, modularizar y definir extraccion de `opsxj:orchestrate:new` como orquestador formal

## Tipo

Technical Story

## Contexto

`opsxj:orchestrate:new` ya no tiene el perfil de un script utilitario menor. Por el alcance que cubre y por los sistemas que coordina, debe evaluarse como un activo de plataforma interna con riesgo arquitectonico y operativo.

Actualmente el comando concentra responsabilidades sobre:

- Jira
- OpenSpec
- Git
- ramas y PRs
- trazabilidad
- coordinacion multi-repo
- ejecucion no interactiva

Esto sugiere mezcla de responsabilidades, acoplamiento alto al repo coordinador y riesgo de estados parciales.

## Problema

El activo tiene valor, pero su ubicacion y su forma actual no son coherentes con su peso operativo.

Riesgos principales:

- crecimiento informal de alcance
- baja separacion de responsabilidades
- no-interactividad no garantizada
- side effects distribuidos sin rollback claro
- dificultad para escalarlo y mantenerlo

## Objetivo

Definir una base tecnica para tratar `opsxj:orchestrate:new` como un orquestador formal, reduciendo acoplamiento, aclarando responsabilidades y preparando una estrategia de extraccion.

## Alcance

Incluye:

- inventario de responsabilidades reales
- identificacion de side effects
- identificacion de dependencias externas
- clasificacion de modulos funcionales
- evaluacion real del modo `-NonInteractive`
- propuesta de modularizacion
- propuesta de extraccion

No incluye:

- reemplazo inmediato del comando
- reescritura completa en esta fase
- cambio funcional amplio del flujo sin diagnostico previo

## Criterios de Aceptacion

1. Existe un inventario claro de responsabilidades actuales del comando.
2. Existe un mapa de side effects y puntos de fallo parcial.
3. Existe una lista de dependencias externas y acoplamientos estructurales.
4. Existe una evaluacion explicita del modo `-NonInteractive`.
5. Existe una propuesta de modularizacion con separacion minima por capacidades.
6. Existe una recomendacion tecnica documentada sobre mantener, extraer o reemplazar.
7. Existe una propuesta de plan de transicion por fases.

## Propuesta de Modulos

La evaluacion debe validar si la separacion minima correcta es:

- `JiraModule`
- `OpenSpecModule`
- `RepoImpactModule`
- `PRModule`
- `ValidationModule`
- `LoggingModule`

## Riesgos

- asumir que el valor actual del comando justifica su forma actual
- seguir agregando reglas sin modularizacion
- tratar `-NonInteractive` como automatizacion robusta sin prueba formal
- posponer la extraccion hasta que el costo de cambio sea mas alto

## Plan de Trabajo Sugerido

### Fase 1. Auditoria estructural

- levantar responsabilidades
- levantar side effects
- levantar dependencias
- levantar escenarios de fallo parcial

### Fase 2. Evaluacion operativa

- revisar preflight
- revisar manejo de errores
- revisar reruns
- revisar trazabilidad real

### Fase 3. Propuesta de diseño

- proponer modulos
- proponer limites
- proponer estrategia de extraccion
- proponer orden de implementacion

## Resultado Esperado

Al finalizar este trabajo debe quedar una decision tecnica defendible:

```text
seguir evolucionando el activo ya modularizado,
o iniciar su extraccion formal como componente de plataforma.
```

## Referencias

- [01-Auditoria-arquitectonica-opsxj-orchestrate-new.md](D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore/Docs/Architecture/orquestador/01-Auditoria-arquitectonica-opsxj-orchestrate-new.md)
- [02-Resumen-ejecutivo-opsxj-orchestrate-new.md](D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore/Docs/Architecture/orquestador/02-Resumen-ejecutivo-opsxj-orchestrate-new.md)
