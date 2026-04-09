# PROMPT ARQUITECTONICO - Ticket 10 ARQ

# Normalizar namespaces y ownership real de capas en la solucion API-first

Rol esperado:

Arquitecto de software senior backend (.NET, namespaces, ownership de ensamblados, mantenibilidad).

## OBJETIVO

Normalizar los namespaces para que coincidan con el ensamblado y la responsabilidad real de cada capa dentro del modelo API-first.

La solucion debe:

- Alinear namespace, carpeta y ensamblado.
- Eliminar casos donde clases de `Repository` viven bajo namespaces `Services`.
- Facilitar navegabilidad y ownership tecnico.

## CONTEXTO EXISTENTE

Ejemplos problematicos:

- `MiApp.Repository/Repositorio/GestorDocumental/usuario/RemitDestInternoR.cs`
- `MiApp.Repository/Repositorio/Account/GestorModuloR.cs`

Problema actual:

- Existen namespaces inconsistentes con la capa y el proyecto.
- Esto dificulta mantenimiento, DI, busqueda y ownership.

## UBICACION ESPERADA

Modificar unicamente si aplica:

- `MiApp.Repository/**`
- `MiApp.Services/**`
- `DocuArchi.Api/**`
- referencias afectadas en pruebas

## RESTRICCIONES OBLIGATORIAS

- No mezclar este ticket con cambios funcionales.
- No renombrar tipos publicos sin evaluar impacto.
- No dejar imports huérfanos o ambiguedades de namespace.

## REGLAS DE IMPLEMENTACION

1. Inventariar tipos con namespace inconsistente.
2. Priorizar los mas graves y usados por el host principal.
3. Ajustar namespaces y usings.
4. Mantener comportamiento funcional.

## REGLAS DE SEGURIDAD

- No alterar componentes de autenticacion o autorizacion mas alla del rename tecnico.

## REGLAS DE CONSISTENCIA

- `Repository` usa namespaces `MiApp.Repository.*`
- `Services` usa namespaces `MiApp.Services.*`
- El naming debe reflejar ownership real.

## PRUEBAS UNITARIAS OBLIGATORIAS

- Actualizar pruebas afectadas por los renames.

## PRUEBAS DE INTEGRACION / CALIDAD

- Build de proyectos impactados.
- Verificacion de wiring DI posterior al rename.

## CRITERIOS DE ACEPTACION

- Los namespaces inconsistentes priorizados quedan normalizados.
- La solucion compila.
- El host principal sigue resolviendo dependencias.

## NO OBJETIVOS

- No hacer una limpieza total de nombres historicos en una sola iteracion si el alcance lo vuelve riesgoso.
