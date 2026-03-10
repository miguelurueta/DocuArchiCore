## Context

- Jira issue key: SCRUM-46
- Jira summary: VALIDACION-CAMPOS-UNICOS-RADICACION
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-46

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

En el proyecto multirepo necesito crear un servicio llamado ValidaCamposDinamicosUnicosRadicacion. Ubicación: Repositorio: MiApp.Service Ruta: /Service/Radicacion/Tramite Objetivo: El servicio debe retornar List<ValidationError> envuelto en AppResponses si existe el valor de un campo en la plantilla retorna errores con ValidationHelper. debe tomar los campos que vienen del front tantos solo campo dinamicos y determinar si el valor del campo ya se encuetra registrado Parámetros: RegistrarRadicacionEntranteRequestDto. defaultDbAlias → parámetro enviado. Dependencias: Requerimientos funcionales: la funcion debe determinar los campos dinamicos de la plantilla de radicacion que el valor contenido en los campos no se encuetren registrados en la tabla. Retornar la estructura envuelta en la clase AppResponses. El servicio debe estar envuelto en bloque try/catch para manejo controlado de errores. Requerimientos adicionales: Implementar siguiendo el patrón  Service → Repository. Crear una interfaz en el mismo archivo que la clase para facilitar pruebas unitarias: Registrar la interfaz y la clase en Program.cs del repositorio DocuArchi.Api bajo la sección de servicios. Todas las consultas deben ser parametrizadas para evitar SQL Injection. Documentar la función con comentarios XML indicando propósito, parámetros y retorno. Validar que si no existen registros coincidentes, se retorne Success=true con Data=null y Message="Sin resultados". En caso de excepción, retornar Success=false con errors = new[] { new AppError { Fields="IdTipoTramite", Message=ex.Message } } y Data vacío. Incluir pruebas unitarias: Caso Success con datos válidos. Caso sin resultados. Caso excepción simulada. Incluir pruebas de integración con MySQL usando Testcontainers/Docker: Mantener separación de responsabilidades (SoC), bajo acoplamiento y alta cohesión. Cumplir principios SOLID en la implementación. Agregar en la documentación los diagramas de casos de uso, diagrama de clases, diagrama de secuencia y diagrama de estado en el repositorio DocuArchiCore /Docs/Radicacion/Tramite. Agregar documentación técnica de implementación para el frontend: Descripción del DTO.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-46-validacion-campos-unicos-radicacion.