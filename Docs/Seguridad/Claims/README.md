# Documentación Claims

## Objetivo

Centralizar la documentación funcional y técnica del sistema de claims existente en `DocuArchiCore`.

## Índice de documentos

- [CLAIMS-Arquitectura.md](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore/Docs/Seguridad/Claims/CLAIMS-Arquitectura.md)
  - Describe la arquitectura general de seguridad basada en JWT y claims.
  - Explica generación del token, claims base, servicios principales y recomendaciones.

- [CLAIMS-UsoBackend.md](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore/Docs/Seguridad/Claims/CLAIMS-UsoBackend.md)
  - Explica cómo se consumen los claims en controllers, services y lógica funcional.
  - Incluye ejemplos reales de `ICurrentUserService` y `IClaimValidationService`.

- [CLAIMS-FlujoAutorizacion.md](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore/Docs/Seguridad/Claims/CLAIMS-FlujoAutorizacion.md)
  - Documenta el flujo login -> JWT -> middleware -> `HttpContext.User` -> controller/service.
  - Incluye diagramas textuales de autenticación y autorización.

- [CLAIMS-Ejemplos.md](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore/Docs/Seguridad/Claims/CLAIMS-Ejemplos.md)
  - Reúne ejemplos prácticos del sistema actual.
  - Incluye `defaulalias`, lectura de claims, agregado de claims de contexto y agregado de permisos.

- [CLAIMS-Ejemplos-Tickets-Jira.md](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore/Docs/Seguridad/Claims/CLAIMS-Ejemplos-Tickets-Jira.md)
  - Contiene ejemplos de redacción de tickets Jira para cambios relacionados con claims.
  - Sirve como plantilla para futuros requerimientos de autenticación, contexto o permisos.

- [CLAIMS-Matriz.md](/D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore/Docs/Seguridad/Claims/CLAIMS-Matriz.md)
  - Resume cada claim principal con su origen, ubicación de llenado, validación y uso.
  - Sirve como mapa rápido para soporte e implementación.

## Ruta base

Todos los documentos de este paquete viven en:

- `Docs/Seguridad/Claims`

## Uso recomendado

### Para entender el sistema actual

Leer en este orden:

1. `CLAIMS-Arquitectura.md`
2. `CLAIMS-FlujoAutorizacion.md`
3. `CLAIMS-UsoBackend.md`

### Para implementar o modificar claims

Leer en este orden:

1. `CLAIMS-Ejemplos.md`
2. `CLAIMS-Ejemplos-Tickets-Jira.md`
3. `CLAIMS-Matriz.md`

## Observaciones

- Esta documentación está basada en el código actual del proyecto.
- No describe comportamiento hipotético.
- Los ejemplos de tickets Jira están pensados como guía de redacción, no como contratos cerrados.
