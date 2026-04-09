# PROMPT ARQUITECTONICO - Ticket 07 QA

# Pruebas de regresion para migracion de controllers a application services en DocuArchi.Api

Rol esperado:

Arquitecto de software senior backend QA (.NET, controllers, services, contratos HTTP, regresion funcional).

## OBJETIVO

Validar que la migracion de controladores para depender solo de application services no altere contratos, codigos HTTP ni comportamiento observable.

## CONTEXTO EXISTENTE

Cambio relacionado:

- Ticket `06-BE-Enforce-controller-service-boundary-docuarchi-api`

## UBICACION ESPERADA

Actualizar o crear pruebas unicamente si aplica en:

- `DocuArchiCore/tests/TramiteDiasVencimiento.Tests`

## RESTRICCIONES OBLIGATORIAS

- No mockear tanto que se pierda la verificacion del contrato.
- No reemplazar pruebas de controller por pruebas exclusivamente internas de service.

## REGLAS DE IMPLEMENTACION

1. Mantener o ampliar contract tests de controllers afectados.
2. Verificar codigos HTTP, payload y mensajes principales.
3. Verificar rutas protegidas y validaciones de claim.

## PRUEBAS UNITARIAS OBLIGATORIAS

- Tests de controllers migrados.
- Tests de services creados o completados.

## PRUEBAS DE INTEGRACION / CALIDAD

- Requests felices y fallidos de endpoints migrados.
- Verificacion de `BadRequest`, `NotFound`, `Ok` y `500` donde aplique.
- Verificacion de claims y usuario actual si forman parte del flujo.

## CRITERIOS DE ACEPTACION

- Los endpoints migrados conservan su contrato.
- No aparecen regresiones funcionales por mover logica fuera del controller.
- La migracion queda protegida por pruebas de regresion.

## NO OBJETIVOS

- No cubrir todos los endpoints del sistema en este ticket.
