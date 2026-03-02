## Why

SCRUM-30 requiere implementar una API para listar radicados pendientes por usuario de radicacion con contrato compatible con tablas MUI React. El cambio necesita trazabilidad entre Jira y OpenSpec para coordinar implementacion multi-repo y pruebas.

## What Changes

- Definir comportamiento funcional para `ApListaRadicadosPendientes` en el flujo Controller -> Service -> Repository.
- Estandarizar el contrato de respuesta con `AppResponses` para casos de exito, sin resultados y excepciones.
- Especificar dependencias tecnicas requeridas (DTO, AutoMapper, consultas parametrizadas, pruebas unitarias/integracion, documentacion tecnica).
- Mantener trazabilidad directa con Jira SCRUM-30 para coordinacion entre repositorios impactados.

## Capabilities

### New Capabilities
- `jira-scrum-30`: Requisitos de trazabilidad y reglas base para la implementacion de SCRUM-30.

### Modified Capabilities
- None.

## Impact

- Jira issue: https://contasoftcompany.atlassian.net/browse/SCRUM-30
- OpenSpec change path: openspec/changes/scrum-30/
- Backend rules reference: openspec/context/OPSXJ_BACKEND_RULES.md
- Multi-repo context reference: openspec/context/multi-repo-context.md
- Affected repositories:
  - DocuArchi.Api (Controller en `/Controllers/Radicacion/Tramite`)
  - MiApp.Service (Service en `/Service/Radicacion/Tramite`)
  - MiApp.Repository (Repository en `/Repositorio/Radicador/Tramite` y dependencia en `/Repositorio/GestorDocumental/Usuario`)
  - MiApp.Models (modelo en `/Models/Radicacion/Tramite`)
  - MiApp.DTOs (DTO en `/DTOs/Radicacion/Tramite`)
