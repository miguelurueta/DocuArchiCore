# PROMPT ARQUITECTONICO - Ticket 12 ARQ

# Definir retiro progresivo de DocuArchiCore como host legado

Rol esperado:

Arquitecto de software senior backend (.NET, migracion de hosts, legado, compatibilidad operativa).

## OBJETIVO

Definir el plan tecnico para retirar progresivamente `DocuArchiCore` como backend activo y dejarlo como shell legado transitorio o candidato a desactivacion futura.

La solucion debe:

- Inventariar flujos que siguen viviendo en `DocuArchiCore`.
- Determinar cuales deben migrar a `DocuArchi.Api`.
- Definir el criterio de fin de vida del host legado.

## CONTEXTO EXISTENTE

Host legado:

- `DocuArchiCore/Program.cs`
- `DocuArchiCore/Controllers/**`

Host principal:

- `DocuArchi.Api/Program.cs`
- `DocuArchi.Api/Controllers/**`

## UBICACION ESPERADA

Documentar en:

- `Docs/Architecture/API-first-DocuArchi-Api`

## RESTRICCIONES OBLIGATORIAS

- No apagar `DocuArchiCore` sin inventario de dependencias.
- No asumir que todas las vistas MVC ya tienen reemplazo API/cliente.

## REGLAS DE IMPLEMENTACION

1. Inventariar rutas, controladores y dependencias activas del host legado.
2. Clasificar:
   - migrar pronto
   - mantener temporalmente
   - retirar
3. Definir prerequisitos para apagado o reduccion del host legado.

## REGLAS DE SEGURIDAD

- No dejar rutas legacy expuestas sin ownership.
- Mantener controles de acceso durante la transicion.

## REGLAS DE CONSISTENCIA

- Toda capacidad backend nueva pertenece al host principal.
- El host legado debe reducir superficie con cada iteracion.

## PRUEBAS UNITARIAS OBLIGATORIAS

- No aplica prueba unitaria directa al documento de retiro.

## PRUEBAS DE INTEGRACION / CALIDAD

- Checklist de flujos criticos aun dependientes del host legado.
- Evidencia de equivalencia o plan de migracion por flujo.

## CRITERIOS DE ACEPTACION

- Existe inventario del host legado.
- Existe plan de retiro progresivo.
- Existe criterio tecnico para saber cuando `DocuArchiCore` puede quedar minimizado o retirado.

## NO OBJETIVOS

- No retirar el host legado en este ticket.
- No reescribir frontend ni vistas MVC en este ticket.
