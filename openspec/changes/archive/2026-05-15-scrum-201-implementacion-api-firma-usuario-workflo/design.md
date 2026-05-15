## Context

- Jira issue key: SCRUM-201
- Jira summary: IMPLEMENTACION-API-FIRMA-USUARIO-WORKFLOW
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-201

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

Se requiere implementar una API que consulte la firma en `usuario_workflow.Firma_Usuario` y retorne un recurso temporal para frontend, bajo seguridad por claims workflow y sin exponer blob ni rutas sensibles.

## Functional Scope

- Endpoint de lectura de firma temporal workflow.
- Alias de BD exclusivamente desde claim `defaulaliaswf`.
- Identidad funcional exclusivamente desde claim `IdUsuarioWorkflow`.
- Consulta de firma por `idU_suario` en `usuario_workflow`.
- Persistencia temporal y retorno de metadata de consumo.

## Out of Scope

- Carga/actualizacion de firma.
- Modificaciones de autenticacion/token.
- Exposicion del blob en payload JSON.

## Architecture

- Patron obligatorio: `Controller -> Service -> Repository`.
- Reuso de repositorio existente: `UsuarioWorkflowR`.
- Uso de infraestructura de datos del proyecto (`DapperCrudEngine`/`QueryOptions`) para lectura.
- Respuesta estandar `AppResponses<T>`.

## API Contract

- Ruta objetivo: `GET /api/workflow/usuarios/firma-temporal`.
- Request funcional: sin parametros obligatorios.
- Si se decide aceptar `idUsuarioWorkflow` por query, solo como validacion de consistencia y debe coincidir con claim.

### Response DTO

- `IdUsuarioWorkflow`
- `FileName`
- `ContentType`
- `RelativePath` (o `UrlTemporal`)
- `ExpiresAt`

## Security Rules

- Claims requeridos:
  - `defaulaliaswf`
  - `IdUsuarioWorkflow`
- Nunca confiar en ID enviado por frontend para la consulta base.
- No exponer blob ni path fisico absoluto.
- Sanitizacion de nombre de archivo y bloqueo de path traversal.

## Temporary File Strategy

- Reutilizar patron de almacenamiento temporal ya existente en StorageEngine.
- Subcarpeta dedicada para firmas (ej: `signatures`).
- TTL configurable (recomendado: 15 minutos).
- Crear carpeta si no existe.
- Nombres unicos.

## Image Handling

- Detectar formato real desde bytes (PNG/JPG/BMP).
- Entregar content-type consistente con archivo temporal generado.
- Si el frontend requiere transparencia para estampado, normalizar a PNG.

## Error Semantics

- `success=true` cuando la firma temporal se genera correctamente.
- `success=false`, estado `validation` para:
  - claims faltantes/invalidos
  - firma inexistente o vacia
- `success=false`, estado `error` para excepciones inesperadas.

## Observability

- Log de inicio/fin con `requestId`, alias workflow e id workflow.
- Warnings para firma inexistente.
- Error con excepcion controlada sin datos sensibles.

## Approach

1. Ajustar contrato y DTOs.
2. Extender repositorio workflow para leer firma por `idU_suario`.
3. Implementar servicio de orquestacion temporal con validaciones y TTL.
4. Exponer controller con validacion de claims.
5. Cubrir pruebas y documentacion.

## Validation Guardrails

- Aplicar restricciones de repositorio, arquitectura y pruebas de `OPSXJ_BACKEND_RULES`.
- Mantener cambios acotados a alcance funcional del ticket.
- Validar OpenSpec antes de publicar.
