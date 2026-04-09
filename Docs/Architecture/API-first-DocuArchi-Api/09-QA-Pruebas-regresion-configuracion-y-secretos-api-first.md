# PROMPT ARQUITECTONICO - Ticket 09 QA

# Pruebas de regresion para saneamiento de configuracion y secretos en API-first

Rol esperado:

Arquitecto de software senior backend QA (.NET, configuracion por ambiente, despliegue, smoke tests).

## OBJETIVO

Validar que el saneamiento de configuracion y secretos no rompa arranque, autenticacion ni conexiones del host principal `DocuArchi.Api`.

## CONTEXTO EXISTENTE

Cambio relacionado:

- Ticket `08-SEC-Sanar-configuracion-y-secretos-api-first`

## UBICACION ESPERADA

Actualizar o crear pruebas unicamente si aplica en:

- `DocuArchiCore/tests/TramiteDiasVencimiento.Tests`
- scripts o checklist de despliegue si aplica

## RESTRICCIONES OBLIGATORIAS

- No asumir valores por defecto inseguros.
- No validar con secretos hardcodeados en el repositorio.

## REGLAS DE IMPLEMENTACION

1. Agregar smoke checks de configuracion externa.
2. Verificar lectura correcta de `Jwt`.
3. Verificar lectura correcta de connection strings.
4. Verificar que la aplicacion falla de forma controlada si falta configuracion critica.

## PRUEBAS UNITARIAS OBLIGATORIAS

- Validaciones de configuracion si existen componentes testables asociados.

## PRUEBAS DE INTEGRACION / CALIDAD

- Arranque del host con variables de entorno.
- Smoke login o endpoint autenticado con configuracion correcta.
- Verificacion de startup failure controlado con configuracion critica ausente.

## CRITERIOS DE ACEPTACION

- El cambio queda protegido por pruebas de configuracion.
- El host principal sigue arrancando con configuracion externa.
- No hay regresion de despliegue por el retiro de secretos versionados.

## NO OBJETIVOS

- No automatizar un pipeline completo de DevSecOps en este ticket.
