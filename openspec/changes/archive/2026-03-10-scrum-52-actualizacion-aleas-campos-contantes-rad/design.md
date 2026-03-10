## Context

- Jira issue key: SCRUM-52
- Jira summary: ACTUALIZACION-ALEAS-CAMPOS-CONTANTES-RADICACION
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-52

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

En el proyecto multirepo necesito actualizar la validación de la API radicacion_entrante. Ubicación: Repositorio Api: DocuArchi.Api Ruta: /Controllers/Radicacion/Tramite Objetivo: Modificar los aleas de algunos campos constantes. Modificar el mensaje de error. Dependencias: Servicio RegistrarRadicacionEntranteAsync en MiApp.Service /Service/Radicacion/Tramite Servicio ValidaCamposRadicacion Repositorios asociados a radicación (Usuario, Plantilla, Radicados) Requerimientos funcionales: Para los campos constantes agregar alias descriptivos: Tipo de Radicado → TipoRadicado Tipo de Radicado →IdtipoRadicado Solicitante → Remitente_Cor Solicitante →Remit_Dest_Interno_id_Remit_Dest_Int Responsable del trámite → Destinatario_Cor Responsable del trámite →Destinatario_Externo_id_Dest_Ext Los mensajes de error deben usar el alias en lugar del nombre técnico del campo. Ejemplo: "Fecha Límite Respuesta: valor inválido." Retornar la estructura envuelta en AppResponses con Success, Message, Data y Errors. Evitar describir capos asi: Tipo_tramite.Descripcion. Requerimientos adicionales: Mantener el patrón Controller → Service → Repository. Documentar todas las funciones con comentarios XML indicando propósito, parámetros y retorno. Validar que si no existen registros coincidentes, se retorne Success=true con Data=null y Message="Sin resultados". En caso de excepción, retornar Success=false con:

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-52-actualizacion-aleas-campos-contantes-rad.