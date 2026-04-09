# PROMPT ARQUITECTONICO - Ticket 08 SEC

# Sanear configuracion y secretos para el modelo API-first

Rol esperado:

Arquitecto de software senior backend y seguridad (.NET, configuracion por ambiente, secretos, IIS, despliegue).

## OBJETIVO

Retirar secretos reales de archivos versionados y formalizar el uso de configuracion por ambiente para `DocuArchi.Api` como host principal.

La solucion debe:

- Eliminar secretos reales de `appsettings.json`.
- Mantener placeholders claros para desarrollo.
- Documentar variables requeridas en despliegue.
- Mantener compatibilidad con IIS y `web.config`.

## CONTEXTO EXISTENTE

Archivos sensibles observados:

- `DocuArchi.Api/appsettings.json`
- `DocuArchiCore/appsettings.json`

Tipos de secretos expuestos:

- cadenas de conexion
- `Jwt:Key`
- secretos de pruebas/permission test

## UBICACION ESPERADA

Modificar unicamente si aplica:

- `DocuArchi.Api/appsettings.json`
- `DocuArchiCore/appsettings.json`
- documentacion de despliegue

## RESTRICCIONES OBLIGATORIAS

- No romper la aplicacion en ambientes productivos.
- No dejar valores vacios sin documentacion de reemplazo.
- No hardcodear secretos nuevos en codigo ni docs.

## REGLAS DE IMPLEMENTACION

1. Reemplazar secretos versionados por placeholders.
2. Documentar variables de entorno requeridas.
3. Mantener compatibilidad con el esquema actual de `web.config` / IIS.
4. Verificar que `DocuArchi.Api` pueda levantar con configuracion externa.

## REGLAS DE SEGURIDAD

- Ningun secreto real debe quedar en archivos versionados.
- Rotar claves comprometidas fuera del alcance tecnico si ya fueron expuestas.
- Documentar claramente el origen de configuracion por ambiente.

## REGLAS DE CONSISTENCIA

- El host principal debe seguir un modelo uniforme de configuracion.
- Los placeholders deben ser consistentes entre ambientes y docs.

## PRUEBAS UNITARIAS OBLIGATORIAS

- No aplica prueba unitaria directa salvo validaciones de configuracion si ya existen.

## PRUEBAS DE INTEGRACION / CALIDAD

- Arranque de `DocuArchi.Api` con variables externas.
- Verificacion de lectura de connection strings.
- Verificacion de lectura de `Jwt`.

## CRITERIOS DE ACEPTACION

- No hay secretos reales versionados en `appsettings.json`.
- La configuracion requerida para despliegue queda documentada.
- `DocuArchi.Api` sigue siendo desplegable con configuracion externa.

## NO OBJETIVOS

- No migrar a un vault especifico si no existe decision organizacional.
- No rediseñar todo el pipeline de despliegue en este ticket.
