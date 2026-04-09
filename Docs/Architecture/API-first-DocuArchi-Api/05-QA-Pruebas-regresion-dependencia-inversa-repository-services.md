# PROMPT ARQUITECTONICO - Ticket 05 QA

# Pruebas de regresion por eliminacion de dependencia inversa entre Repository y Services

Rol esperado:

Arquitecto de software senior backend QA (.NET, build graph, regresion funcional, pruebas de radicacion).

## OBJETIVO

Proteger la eliminacion de la dependencia inversa entre `MiApp.Repository` y `MiApp.Services` mediante pruebas de build, wiring y regresion funcional.

## CONTEXTO EXISTENTE

Cambio relacionado:

- Ticket `04-BE-Eliminar-dependencia-inversa-repository-services`

Area sensible:

- flujo de radicacion
- builders de registro
- policies asociadas

## UBICACION ESPERADA

Actualizar o crear pruebas unicamente si aplica en:

- `DocuArchiCore/tests/TramiteDiasVencimiento.Tests`

## RESTRICCIONES OBLIGATORIAS

- No introducir mocks irreales que oculten el error de ensamblados.
- No validar solo compilacion; debe existir cobertura funcional minima.

## REGLAS DE IMPLEMENTACION

1. Agregar cobertura de build y wiring sobre clases reubicadas.
2. Asegurar que los flujos de radicacion siguen resolviendo los builders correctos.
3. Asegurar que no quedan referencias obsoletas a rutas antiguas.

## PRUEBAS UNITARIAS OBLIGATORIAS

- Pruebas sobre servicios/clases que usan los builders movidos.
- Validacion de resolucion DI si aplica.

## PRUEBAS DE INTEGRACION / CALIDAD

- Build de `MiApp.Repository`.
- Build de `MiApp.Services`.
- Build de `DocuArchi.Api`.
- Ejecucion de las suites de radicacion afectadas.

## CRITERIOS DE ACEPTACION

- El cambio queda protegido por pruebas.
- No se rompen flujos de radicacion asociados.
- No reaparece dependencia inversa por rutas de compilacion cruzadas.

## NO OBJETIVOS

- No cubrir todos los casos de negocio del sistema.
