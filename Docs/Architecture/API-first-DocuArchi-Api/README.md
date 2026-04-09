# API-first DocuArchi.Api

Esta carpeta contiene el backlog arquitectonico para converger la solucion actual hacia un modelo API-first con `DocuArchi.Api` como host principal.

## Archivo principal

- `00-EPIC-API-first-DocuArchi-Api-roadmap.md`

## Estructura

- `ARQ` -> decisiones y tareas de arquitectura
- `BE` -> cambios backend
- `SEC` -> seguridad y configuracion
- `QA` -> pruebas de regresion asociadas a cada cambio principal

## Regla de uso

No ejecutar tickets de regresion como opcionales. Cada cambio principal tiene su ticket espejo QA y ambos deben entrar al plan de trabajo.

## Secuencia sugerida

1. Alineacion API-first
2. Saneamiento de secretos
3. Modularizacion del host principal
4. Correccion de dependencias entre capas
5. Boundary controller -> service
6. Normalizacion de ownership y namespaces
7. Retiro progresivo del host legado
