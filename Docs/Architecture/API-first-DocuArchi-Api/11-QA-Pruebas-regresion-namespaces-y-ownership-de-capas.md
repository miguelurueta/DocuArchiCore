# PROMPT ARQUITECTONICO - Ticket 11 QA

# Pruebas de regresion para normalizacion de namespaces y ownership de capas

Rol esperado:

Arquitecto de software senior backend QA (.NET, regresion de compilacion, wiring, referencias, tests contractuales).

## OBJETIVO

Validar que la normalizacion de namespaces y ownership de capas no introduzca regresiones de compilacion, wiring ni contratos en `DocuArchi.Api`.

## CONTEXTO EXISTENTE

Cambio relacionado:

- Ticket `10-ARQ-Normalizar-namespaces-y-ownership-de-capas`

## UBICACION ESPERADA

Actualizar o crear pruebas unicamente si aplica en:

- `DocuArchiCore/tests/TramiteDiasVencimiento.Tests`

## RESTRICCIONES OBLIGATORIAS

- No limitar la validacion a que “compila”.
- Debe validarse resolucion real de tipos y wiring principal.

## REGLAS DE IMPLEMENTACION

1. Validar compilacion de todos los proyectos impactados.
2. Validar resolucion DI del host principal.
3. Validar que controladores y services afectados siguen ejecutando.

## PRUEBAS UNITARIAS OBLIGATORIAS

- Pruebas afectadas por cambios de namespace.

## PRUEBAS DE INTEGRACION / CALIDAD

- Build de `MiApp.Repository`, `MiApp.Services` y `DocuArchi.Api`.
- Smoke de endpoints o services impactados por los renames.

## CRITERIOS DE ACEPTACION

- Los cambios de namespace quedan cubiertos por pruebas de regresion.
- No aparecen errores de tipo o DI por renames.
- El host principal sigue funcionando.

## NO OBJETIVOS

- No cubrir todos los modulos del sistema si el cambio fue acotado.
