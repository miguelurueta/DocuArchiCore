# PROMPT ARQUITECTONICO - Ticket 02 BE

# Modularizar el composition root de DocuArchi.Api por dominios

Rol esperado:

Arquitecto de software senior backend (.NET, DI, composition root, modularizacion por dominios).

## OBJETIVO

Reducir el acoplamiento del composition root actual de `DocuArchi.Api` extrayendo el registro de dependencias por modulos de dominio sin cambiar comportamiento funcional.

La solucion debe:

- Reducir el tamaño y complejidad de `Program.cs`.
- Agrupar el wiring por dominios y preocupaciones transversales.
- Evitar drift respecto de los registros ya existentes.
- Mantener el mismo comportamiento runtime.

## CONTEXTO EXISTENTE

Componente afectado:

`DocuArchi.Api/Program.cs`

Problema actual:

- `Program.cs` registra manualmente un volumen alto de dependencias.
- Mezcla infraestructura, seguridad, repositorios, servicios, session y CORS.
- El host principal conoce demasiado detalle interno de cada dominio.

## UBICACION ESPERADA

Modificar unicamente si aplica:

- `DocuArchi.Api/Program.cs`
- `DocuArchi.Api/**/DependencyInjection/*.cs`
- `DocuArchi.Api/**/Extensions/*.cs`

## RESTRICCIONES OBLIGATORIAS

- No cambiar contratos publicos.
- No eliminar registros existentes sin validar reemplazo equivalente.
- No introducir auto-discovery o reflection scanning si el equipo no lo usa hoy.
- No mover logica de negocio a `Program.cs`.

## REGLAS DE IMPLEMENTACION

1. Separar el wiring en modulos explicitamente nombrados.
2. Minimo esperado:
   - `AddAccountModule(...)`
   - `AddRadicacionModule(...)`
   - `AddWorkflowModule(...)`
   - `AddInfrastructureModule(...)`
   - `AddSecurityModule(...)`
3. Mantener el orden correcto del pipeline HTTP.
4. Mantener registro consistente de:
   - repositorios
   - servicios
   - current user
   - claim validation
   - session
   - JWT
   - CORS
5. `Program.cs` debe quedar como composition root legible y no como inventario manual de todo el sistema.

## REGLAS DE SEGURIDAD

- No alterar el comportamiento de JWT.
- No alterar el comportamiento de Session.
- No alterar el comportamiento de CORS.
- No perder el wiring de `ICurrentUserService` ni `IClaimValidationService`.

## REGLAS DE CONSISTENCIA

- Cada modulo debe registrar componentes de una unica responsabilidad coherente.
- Evitar registros duplicados entre modulos.
- Mantener compatibilidad con el host principal `DocuArchi.Api`.

## PRUEBAS UNITARIAS OBLIGATORIAS

- Si se crean extensiones de DI, agregar pruebas de wiring o smoke tests si el proyecto ya tiene ese patron.
- Verificar que dependencias clave pueden resolverse:
  - `ITokenService`
  - `ICurrentUserService`
  - `IClaimValidationService`
  - `IWorkflowInboxService`
  - `IPlantillaRadicacionL`

## PRUEBAS DE INTEGRACION / CALIDAD

- Compilar `DocuArchi.Api`.
- Verificar que Swagger levanta.
- Verificar que endpoints autenticados siguen respondiendo segun el estado esperado.
- Verificar que no hay servicios faltantes en runtime por DI.

## CRITERIOS DE ACEPTACION

- `Program.cs` queda sustancialmente mas pequeno.
- El wiring queda modularizado por dominios.
- No hay cambio funcional observable.
- La aplicacion compila y arranca.
- No aparecen errores de resolucion DI en runtime.

## NO OBJETIVOS

- No cambiar logica de negocio.
- No reescribir controladores.
- No rediseñar contratos ni DTOs.
