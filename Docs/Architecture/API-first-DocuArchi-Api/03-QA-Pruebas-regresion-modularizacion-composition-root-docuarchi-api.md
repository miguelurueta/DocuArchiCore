# PROMPT ARQUITECTONICO - Ticket 03 QA

# Pruebas de regresion para modularizacion del composition root de DocuArchi.Api

Rol esperado:

Arquitecto de software senior backend QA (.NET, DI, smoke tests, regresion de wiring y pipeline HTTP).

## OBJETIVO

Validar que la modularizacion del composition root de `DocuArchi.Api` no introduzca regresiones de arranque, DI, seguridad ni pipeline.

## CONTEXTO EXISTENTE

Cambio relacionado:

- Ticket `02-BE-Modularizar-composition-root-docuarchi-api`

Puntos criticos:

- Registro de dependencias
- Orden del pipeline
- JWT
- Session
- CORS
- Swagger

## UBICACION ESPERADA

Actualizar o crear pruebas unicamente si aplica en:

- `DocuArchiCore/tests/TramiteDiasVencimiento.Tests`

## RESTRICCIONES OBLIGATORIAS

- No cambiar comportamiento productivo para acomodar pruebas.
- No convertir esta suite en pruebas end-to-end pesadas si no es necesario.

## REGLAS DE IMPLEMENTACION

1. Agregar smoke tests de resolucion DI para servicios criticos.
2. Validar que el host construye correctamente.
3. Validar que el pipeline conserva autenticacion, autorizacion y session.
4. Validar que Swagger y endpoints minimos siguen disponibles.

## PRUEBAS UNITARIAS OBLIGATORIAS

- Resolucion DI de:
  - `ITokenService`
  - `ICurrentUserService`
  - `IClaimValidationService`
  - `IWorkflowInboxService`
  - `IPlantillaRadicacionL`

## PRUEBAS DE INTEGRACION / CALIDAD

- Build de `DocuArchi.Api`.
- Arranque del host.
- Smoke request a Swagger.
- Smoke request a endpoint protegido y endpoint no protegido.

## CRITERIOS DE ACEPTACION

- El host arranca sin errores de DI.
- Las dependencias criticas se resuelven.
- No hay regresion de pipeline.
- La modularizacion queda protegida por pruebas de humo.

## NO OBJETIVOS

- No validar logica de negocio profunda.
- No cubrir cada endpoint del sistema.
