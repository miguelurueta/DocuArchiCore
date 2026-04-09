# PROMPT ARQUITECTONICO - Ticket 06 BE

# Forzar el boundary controller -> service en DocuArchi.Api

Rol esperado:

Arquitecto de software senior backend (.NET, API design, application services, separacion de capas, testing).

## OBJETIVO

Eliminar el acceso directo de controladores de `DocuArchi.Api` a repositorios para que la API consuma unicamente servicios de aplicacion.

La solucion debe:

- Consolidar reglas de negocio en la capa de servicios.
- Reducir acoplamiento en controladores.
- Mejorar testabilidad.
- Mantener contratos HTTP existentes.

## CONTEXTO EXISTENTE

Controladores candidatos iniciales:

- `DocuArchi.Api/Controllers/Account/AccountController.cs`
- `DocuArchi.Api/Controllers/Radicacion/PlantillaRadicado/PlantillaRadicacionController.cs`

Problema actual:

- Los controladores mezclan `R` y `L`.
- Parte de la logica de caso de uso queda repartida entre controller y servicios.

## UBICACION ESPERADA

Modificar unicamente si aplica:

- `DocuArchi.Api/Controllers/**`
- `MiApp.Services/**`

## RESTRICCIONES OBLIGATORIAS

- No cambiar rutas ni contratos HTTP.
- No romper DTOs actuales.
- No mover logica de negocio del service al controller.
- No dejar repositorios inyectados en controladores ya migrados.

## REGLAS DE IMPLEMENTACION

1. Identificar controladores que consumen repositorios directos.
2. Crear o completar application services donde falten.
3. Mover validaciones y orquestacion de negocio desde controller hacia service.
4. Dejar el controller limitado a:
   - recibir request
   - delegar
   - traducir respuesta HTTP

## REGLAS DE SEGURIDAD

- Mantener `[Authorize]` y reglas de claims donde existan.
- No mover seguridad a repositorios.
- Mantener validacion de usuario actual y claims en la capa adecuada.

## REGLAS DE CONSISTENCIA

- Todo controller nuevo o migrado debe depender solo de servicios de aplicacion y componentes transversales permitidos.
- El acceso a datos debe quedar encapsulado en services/repositories.

## PRUEBAS UNITARIAS OBLIGATORIAS

- Actualizar pruebas de controladores migrados.
- Agregar pruebas de services nuevos o ampliados.

## PRUEBAS DE INTEGRACION / CALIDAD

- Verificar respuestas HTTP existentes.
- Verificar que endpoints no pierden comportamiento.
- Verificar que no queden repositorios directos inyectados en controladores migrados.

## CRITERIOS DE ACEPTACION

- Los controladores migrados ya no consumen repositorios directos.
- El comportamiento HTTP se conserva.
- La logica de caso de uso queda en services.
- Las pruebas actualizadas pasan.

## NO OBJETIVOS

- No migrar todos los controladores en una sola iteracion si el alcance inicial es parcial.
- No reescribir toda la API de una vez.
