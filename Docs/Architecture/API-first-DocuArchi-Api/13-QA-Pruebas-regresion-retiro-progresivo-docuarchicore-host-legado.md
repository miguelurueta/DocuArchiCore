# PROMPT ARQUITECTONICO - Ticket 13 QA

# Pruebas de regresion para el retiro progresivo de DocuArchiCore como host legado

Rol esperado:

Arquitecto de software senior backend QA (.NET, smoke tests, inventario funcional, migracion gradual).

## OBJETIVO

Definir y ejecutar las pruebas de regresion necesarias para acompañar cada reduccion de superficie del host legado `DocuArchiCore` sin afectar flujos aun activos.

## CONTEXTO EXISTENTE

Cambio relacionado:

- Ticket `12-ARQ-Retiro-progresivo-docuarchicore-host-legado`

## UBICACION ESPERADA

Actualizar o crear pruebas/checklists en:

- `DocuArchiCore/tests/TramiteDiasVencimiento.Tests`
- `Docs/Architecture/API-first-DocuArchi-Api`

## RESTRICCIONES OBLIGATORIAS

- No retirar rutas legacy sin smoke tests minimos.
- No asumir equivalencia funcional sin evidencia.

## REGLAS DE IMPLEMENTACION

1. Mantener checklist de flujos legacy aun soportados.
2. Definir smoke tests por cada flujo migrado o desactivado.
3. Verificar que el host principal cubre el reemplazo esperado cuando corresponda.

## PRUEBAS UNITARIAS OBLIGATORIAS

- Las asociadas a cada flujo que migre.

## PRUEBAS DE INTEGRACION / CALIDAD

- Smoke tests del host legado mientras siga vivo.
- Smoke tests del reemplazo en `DocuArchi.Api`.
- Validacion de no regresion por cada reduccion de superficie.

## CRITERIOS DE ACEPTACION

- Cada cambio de retiro del host legado tiene su prueba o checklist de regresion.
- No se desactiva funcionalidad sin evidencia de cobertura.
- La migracion a API-first queda controlada.

## NO OBJETIVOS

- No convertir este ticket en migracion total.
