# PROMPT ARQUITECTONICO - Ticket 01 ARQ

# Convergencia API-first con DocuArchi.Api como host principal

Rol esperado:

Arquitecto de software senior backend (.NET, arquitectura por capas, migracion de hosts, seguridad, despliegue IIS).

## OBJETIVO

Definir y ejecutar la convergencia arquitectonica para que `DocuArchi.Api` sea el host principal del sistema y `DocuArchiCore` quede como host legado transitorio sin crecimiento funcional nuevo.

La solucion debe:

- Establecer `DocuArchi.Api` como backend principal.
- Congelar el crecimiento arquitectonico de `DocuArchiCore`.
- Identificar responsabilidades que permanecen en el host legado durante la transicion.
- Definir el orden tecnico de migracion por dominios.
- Reducir duplicacion de wiring, seguridad y configuracion entre hosts.

## CONTEXTO EXISTENTE

Hosts actuales:

- `DocuArchi.Api/Program.cs`
- `DocuArchiCore/Program.cs`
- `DocuArchiCore.Web/Program.cs`

Capas compartidas:

- `MiApp.Services`
- `MiApp.Repository`
- `MiApp.DTOs`
- `MiApp.Models`
- `DocuArchiCore.Abstractions`

## UBICACION ESPERADA

Analizar y modificar unicamente si aplica:

- `DocuArchi.Api`
- `DocuArchiCore`
- `DocuArchiCore.Web`
- `Docs/Architecture/API-first-DocuArchi-Api`

## RESTRICCIONES OBLIGATORIAS

- No romper endpoints actuales publicados en `DocuArchi.Api`.
- No retirar `DocuArchiCore` de forma abrupta.
- No mover funcionalidades de dominio sin plan de regresion.
- No mezclar este ticket con refactors de naming, seguridad o repositorios mas alla del impacto minimo de alineacion.

## REGLAS DE IMPLEMENTACION

1. Declarar formalmente `DocuArchi.Api` como host principal.
2. Declarar formalmente `DocuArchiCore` como host legado transitorio.
3. Definir criterio para nuevas funcionalidades:
   - nuevas capacidades solo en `DocuArchi.Api`
   - fixes de legado solo donde el flujo aun viva
4. Definir un roadmap tecnico por dominio:
   - Account
   - Radicacion
   - Workflow
   - Gestion documental
5. Identificar wiring, seguridad y configuracion duplicados entre hosts.
6. Documentar el estado objetivo y el estado transitorio.

## REGLAS DE SEGURIDAD

- Mantener compatibilidad de autenticacion durante la migracion.
- No deshabilitar autorizacion por simplificar la convergencia.
- No exponer rutas legacy sin control de acceso.

## REGLAS DE CONSISTENCIA

- El host principal debe ser el punto canonico para nuevos contratos backend.
- Los cambios posteriores deben alinearse con esta decision.
- La documentacion de arquitectura debe reflejar el modelo final.

## PRUEBAS UNITARIAS OBLIGATORIAS

- No aplica prueba unitaria directa para el documento arquitectonico.
- Si el ticket deriva cambios de wiring, esos cambios deben cubrirse en sus tickets tecnicos y de regresion asociados.

## PRUEBAS DE INTEGRACION / CALIDAD

- Verificar que el sistema documentado coincide con los hosts reales de la solucion.
- Verificar que nuevas historias tecnicas se asignen a `DocuArchi.Api`.
- Verificar que no se creen nuevas capacidades backend en `DocuArchiCore` fuera de excepciones aprobadas.

## CRITERIOS DE ACEPTACION

- `DocuArchi.Api` queda definido como host principal.
- `DocuArchiCore` queda definido como host legado transitorio.
- Existe roadmap por dominios para convergencia.
- La decision queda documentada para backlog y futuras implementaciones.

## NO OBJETIVOS

- No retirar el host legado en este ticket.
- No modificar endpoints ni contratos publicos en este ticket.
- No resolver por completo la deuda historica de capas en este ticket.
