# PROMPT ARQUITECTONICO - Ticket 04 BE

# Eliminar dependencia inversa entre MiApp.Repository y codigo fuente de MiApp.Services

Rol esperado:

Arquitecto de software senior backend (.NET, capas, dependencias de proyecto, build graph, arquitectura limpia).

## OBJETIVO

Eliminar la dependencia inversa actual donde `MiApp.Repository` compila archivos fuente ubicados en `MiApp.Services`.

La solucion debe:

- Restaurar la direccion correcta de dependencias.
- Evitar `Compile Include` cruzados desde `MiApp.Repository` hacia `MiApp.Services`.
- Reubicar builders y policies en la capa correcta.
- Mantener compatibilidad funcional.

## CONTEXTO EXISTENTE

Proyecto afectado:

`MiApp.Repository/MiApp.Repository.csproj`

Problema actual:

`MiApp.Repository.csproj` incluye archivos desde `MiApp.Services` para builders y policies de radicacion.

## UBICACION ESPERADA

Modificar unicamente si aplica:

- `MiApp.Repository/MiApp.Repository.csproj`
- `MiApp.Repository/**`
- `MiApp.Services/**`
- proyecto nuevo shared si se aprueba formalmente

## RESTRICCIONES OBLIGATORIAS

- No dejar tipos duplicados compilando en dos ensamblados.
- No romper contratos consumidos por `DocuArchi.Api`.
- No mover clases sin revisar ownership real.
- No mezclar esta tarea con refactor global de naming salvo lo minimo necesario.

## REGLAS DE IMPLEMENTACION

1. Eliminar todos los `Compile Include` cruzados desde `MiApp.Repository` hacia `MiApp.Services`.
2. Determinar para cada clase si pertenece a:
   - acceso a datos
   - aplicacion/orquestacion
   - shared/transversal
3. Reubicar cada clase al ensamblado correcto.
4. Ajustar referencias de proyecto y namespaces solo donde sea necesario para que la solucion compile.
5. Mantener el comportamiento funcional existente.

## REGLAS DE SEGURIDAD

- No alterar validaciones de negocio por mover archivos.
- No debilitar las reglas usadas durante registro de radicacion.

## REGLAS DE CONSISTENCIA

- `Repository` no debe depender de codigo fuente de `Services`.
- La direccion de dependencias debe ser coherente y estable.
- El build graph debe quedar explicable.

## PRUEBAS UNITARIAS OBLIGATORIAS

- Actualizar pruebas que cubran flujo de registro de radicacion y builders afectados.
- Verificar compilacion de cada proyecto impactado.

## PRUEBAS DE INTEGRACION / CALIDAD

- Build de:
  - `MiApp.Repository`
  - `MiApp.Services`
  - `DocuArchi.Api`
- Validar endpoints o servicios que usen los builders reubicados.

## CRITERIOS DE ACEPTACION

- `MiApp.Repository.csproj` no incluye archivos desde `MiApp.Services`.
- La solucion compila.
- No hay regresion funcional en radicacion asociada a builders/policies movidos.

## NO OBJETIVOS

- No rediseñar toda la capa de dominio.
- No convertir el sistema a arquitectura hexagonal completa en este ticket.
