# SCRUM-42 - Documentacion Tecnica de Tramite

## Alcance

Migracion del flujo legacy `Registra_Radicacion_entrante` a API en arquitectura por capas.

## Artefactos de este paquete

- `01-api-frontend.md`: guia tecnica para frontend (DTOs, parametros y endpoints).
- `02-diagrama-casos-uso.md`: diagrama de casos de uso.
- `03-diagrama-clases.md`: diagrama de clases.
- `04-diagrama-secuencia.md`: diagrama de secuencia.
- `05-diagrama-estado.md`: diagrama de estado.

## Reglas tecnicas relevantes

- Atomicidad obligatoria:
  - Q01-Q08 dentro de una sola transaccion.
  - rollback total ante cualquier fallo en Q01-Q08.
  - Q09 condicional segun tipo de radicacion.
- Propagacion obligatoria de `defaultDbAlias` hacia repositorio.
- Contrato uniforme de respuesta: `AppResponses<T>`.
