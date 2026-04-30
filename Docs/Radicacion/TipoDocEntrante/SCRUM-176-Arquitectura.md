# SCRUM-176 - Arquitectura

## Objetivo
Crear API para solicitar estructura de `TipoDocEntrante` por id.

## Flujo
1. Controller valida claim `defaulalias`.
2. Controller delega al service.
3. Service valida entrada y llama repository existente (`ITipoDocEntranteR`).
4. Service mapea a `TipoDocEntranteParametroDto`.
5. Respuesta en `AppResponses<T>`.

## Capas
- Controller: HTTP + claims.
- Service: reglas, mapeo, errores.
- Repository: acceso a datos con `DapperCrudEngine` + `QueryOptions` (reutilizado).

## Decisiones
- Reutilizar repository existente para evitar duplicación.
- Mantener contrato compatible con flujo actual de radicación.
- Incluir los 3 flags nuevos en la salida.
