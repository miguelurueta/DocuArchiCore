## Context

- Jira issue key: SCRUM-50
- Jira summary: ACTUALIZACION-MENSAJE-VALIDACION-CAMPOS-RADICACION
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-50

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

En el proyecto multirepo necesito actualizar la validación de la API radicacion_entrante. Ubicación: Repositorio Api: DocuArchi.Api Ruta: /Controllers/Radicacion/Tramite Objetivo: Agregar alias para los campos constantes o dinámicos de radicación validados. Los alias se usarán para enviar mensajes de error al usuario con un nombre descriptivo dentro del mensaje. Dependencias: Servicio RegistrarRadicacionEntranteAsync en MiApp.Service /Service/Radicacion/Tramite Servicio ValidaCamposRadicacion Repositorios asociados a radicación (Usuario, Plantilla, Radicados) Requerimientos funcionales: Para los campos constantes agregar alias descriptivos: Tipo de Radicado → TipoRadicado : IdtipoRadicado Anexos del Radicado → Anexos_Cor Fecha Límite Respuesta → FECHALIMITERESPUESTA Asunto → Asunto Flujo trámite → id_tipo_flujo_workflow Solicitante → Remitente_Cor : Remit_Dest_Interno_id_Remit_Dest_Int Responsable del trámite → Destinatario_Cor : Destinatario_Externo_id_Dest_Ext Número Folios → Numero_Folios Para los campos dinámicos agregar el valor del campo Aleas_Campo proveniente de la configuración de la plantilla. Los mensajes de error deben usar el alias en lugar del nombre técnico del campo. Ejemplo: "Error en campo 'Fecha Límite Respuesta': valor inválido." Retornar la estructura envuelta en AppResponses con Success, Message, Data y Errors. Requerimientos adicionales: Mantener el patrón Controller → Service → Repository. Documentar todas las funciones con comentarios XML indicando propósito, parámetros y retorno. Validar que si no existen registros coincidentes, se retorne Success=true con Data=null y Message="Sin resultados". En caso de excepción, retornar Success=false con: y Data vacío. 5. Incluir pruebas unitarias: Caso Success con datos válidos. Caso sin resultados. Caso excepción simulada. Caso validación con alias (ejemplo: error en "Asunto"). Incluir pruebas de integración con MySQL usando Testcontainers/Docker. Mantener separación de responsabilidades (SoC), bajo acoplamiento y alta cohesión. Cumplir principios SOLID en la implementación. Actualizar diagramas de casos de uso, clases, secuencia y estado en /Docs/Radicacion/Tramite si es necesario. Documentar pruebas funcionales en /Docs/Radicacion/Tramite. Actualizar documentación técnica de implementación para el frontend: Descripción del DTO actualizado con alias. Parámetros de envío a la API. Ejemplo de respuesta con radicado registrado y mensajes de error con alias.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-50-actualizacion-mensaje-validacion-campos.