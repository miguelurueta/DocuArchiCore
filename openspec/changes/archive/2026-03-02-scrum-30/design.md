## Context

- Jira issue key: SCRUM-30
- Jira summary: CREA-API-LISTA-RADICADOS-PEDIENTES
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-30
- Scope funcional confirmado desde Jira:
  - Controller `ApListaRadicadosPendientes` sin parametro de query para alias.
  - Service `ServiceListaRadicadosPendientes` para orquestar id usuario, plantilla y lista pendiente.
  - Repository `SolicitaListaRadicadosPendientes` con consulta parametrizada.
  - Modelo `raradestadosmoduloradicacion` + DTO/AutoMapper + pruebas + documentacion.

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Goals / Non-Goals

**Goals:**
- Definir contrato y comportamiento de la API de radicados pendientes.
- Alinear arquitectura y pruebas a las reglas backend corporativas.
- Mantener trazabilidad y coordinacion multi-repo por capacidad.

**Non-Goals:**
- Cambiar comportamientos fuera del flujo de radicacion/tramite.
- Introducir endpoints o entidades no solicitadas por SCRUM-30.

## Decisions

- Se usa `jira-scrum-30` como capacidad de contrato para concentrar requisitos funcionales y no funcionales del ticket.
- Se adopta `openspec/context/OPSXJ_BACKEND_RULES.md` como baseline de arquitectura:
  - confirmar repos/rutas impactadas antes de codificar,
  - mantener patron Controller -> Service -> Repository,
  - usar interfaces y registro DI en `Program.cs` cuando aplique,
  - envolver respuestas con `AppResponses` y manejo de errores controlado,
  - exigir cobertura de pruebas unitarias/integracion.
- Decisiones funcionales clave del ticket:
  - Controller y Service devuelven `AppResponses` y encapsulan errores con `try/catch`.
  - Controller valida claims `defaulalias` y `usuarioid`.
  - Service depende de tres consultas: usuario gestion, plantilla default y lista de pendientes.
  - Si `Relacion_Id_Usuario_Radicacion <= 0`, el service retorna error de validacion y corta flujo (sin fallback a `Id_Remit_Dest_Int`).
  - Repository usa `DefaultAlias = defaultDbAlias` en `QueryOptions`.
  - Sin registros: `Success=true`, `Data=null`, `Message=\"Sin resultados\"`.
  - Con excepcion: `Success=false`, `ErrorMessage=ex.Message`, `Data` vacio.

## Risks / Trade-offs

- [Codigo en repos externos] -> Implementar cambios de aplicacion en repos objetivo (DocuArchi.Api, MiApp.*) y usar este repo como coordinador OpenSpec.
- [Dependencia de Docker para integracion] -> Marcar pruebas de integracion como skipped con mensaje explicito cuando Docker no este disponible.
- [Drift entre contrato backend y frontend] -> Documentar DTO de salida, parametros y endpoint en `Docs/Radicacion/Tramite`.
