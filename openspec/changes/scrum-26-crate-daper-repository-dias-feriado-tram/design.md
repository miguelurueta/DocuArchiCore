## Context

- Jira issue key: SCRUM-26
- Jira summary: CRATE-DAPER-REPOSITORY-DIAS-FERIADO-TRAMITE
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-26

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

En el proyecto multirepo necesito crear una función de consulta llamada SolicitaListaDiasFeriados. La función debe ubicarse en el repositorio MiApp.Repository en la ruta /Repositorio/Radicador/Tramite. Debe utilizar la clase DapperCrudEngine a través de su interfaz IDapperCrudEngine y realizar una consulta sobre la tabla rea_001_feriados con las siguientes condiciones: ESTADO_DIA = 1 La consulta debe devolver la estructura de las fecha, envuelto en la clase AppResponses. La función debe devolver el formato de las fecha yyyy-mm-dd La función debe estar envuelta en un bloque try/catch para manejo controlado de errores. Requerimientos adicionales: La función debe implementarse siguiendo el patrón Controller → Service → Repository. Crear una interfaz en el mismo archivo que la clase para facilitar pruebas unitarias. Ejemplo: public interface ITferiadosRepository { Task<AppResponse<int>> SolicitaListaDiasFeriados(); } Registrar la interfaz y la clase en Program.cs del repositorio DocuArchi.Api bajo la sección de repositorios. La consulta debe ser parametrizada para evitar SQL Injection. Documentar la función con comentarios XML indicando propósito, parámetros y retorno. Validar que si no existen registros coincidentes, se retorne success=true con data=0 y message="Sin resultados". En caso de excepción, retornar success=false con erros=Nem[] { new AppError{Fields="IdTipoTramite", Mensage=data.Mensage}} y data vacío. Incluir pruebas unitarias: Caso success con datos válidos. Caso sin resultados. Caso excepción simulada. Incluir pruebas de integración con MySQL usando Testcontainers/Docker: Crear schema.sql y seed.sql mínimos para la tabla rea_001_feriados. Validar que la consulta devuelve el número correcto de días vencidos. Mantener separación de responsabilidades (SoC), bajo acoplamiento y alta cohesión. Cumplir principios SOLID en la implementación.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-26-crate-daper-repository-dias-feriado-tram.