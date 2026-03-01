## Context

- Jira issue key: SCRUM-29
- Jira summary: CREA-SERVICIO-API-SOLICITAFECHALIMITE-RESPUESTA
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-29

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

En el proyecto multirepo necesito crear un servicio llamado SolicitaFechaLimiteRespuesta. Ubicación: Repositorio: MiApp.Service Ruta: /Service/Radicacion/Tramite Objetivo: El servicio debe retornar la fecha límite de respuesta de un trámite, calculada en base a días de vencimiento, días efectivos y exclusión de sábados y domingos. Parámetros: IdTipoTramite → proviene del backend. defaultDbAlias → parámetro enviado por el controlador. Dependencias: SolicitaTotalDiasVencimientoTramite() en MiApp.Repository → obtiene número de días de vencimiento. SolicitaListaDiasFeriados() en MiApp.Repository → obtiene lista de días feriados. SolicitaEstructuraPlantillaRadicacionDefault() en MiApp.Repository → obtiene identificación de la plantilla de radicación. return data.id_Plantilla; Requerimientos funcionales: Calcular la fecha límite de respuesta sumando días de vencimiento y excluyendo sábados, domingos y días feriados. Retornar la fecha límite en formato yyyy-MM-dd . Retornar la estructura envuelta en la clase AppResponses. El servicio debe estar envuelto en bloque try/catch para manejo controlado de errores. Requerimientos adicionales: Implementar siguiendo el patrón Controller → Service → Repository. Crear un DTO para el retorno en el repositorio MiApp.DTOs en la ruta /DTOs/Radicacion/Tramite/. Crear un automapper entre el contrato de salida DTO y el retorno del servicio, usando la clase AutoMapperProfile en la ruta /Service/Mapping, con mapeo en /Service/Mapping/Radicacion/Tramite. Crear una interfaz en el mismo archivo que la clase para facilitar pruebas unitarias: Ejemplo: public interface IFechaLimiteRespuestaService {
    Task<AppResponses<FechaLimiteRespuestaDTO>> SolicitaFechaLimiteRespuesta(int IdTipoTramite, string defaultDbAlias);
}
public class FechaLimiteRespuestaService : IFechaLimiteRespuestaService { ... } Registrar la interfaz y la clase en Program.cs del repositorio DocuArchi.Api bajo la sección de servicios. Todas las consultas deben ser parametrizadas para evitar SQL Injection. Documentar la función con comentarios XML indicando propósito, parámetros y retorno. Validar que si no existen registros coincidentes, se retorne Success=true con Data=null y Message="Sin resultados". En caso de excepción, retornar Success=false con errors = new[] { new AppError { Fields="IdTipoTramite", Message=ex.Message } } y Data vacío. Incluir pruebas unitarias: Caso Success con datos válidos. Caso sin resultados. Caso excepción simulada. Incluir pruebas de integración con MySQL usando Testcontainers/Docker: Validar que la consulta devuelve la fecha límite de respuesta correcta. Mantener separación de responsabilidades (SoC), bajo acoplamiento y alta cohesión. Cumplir principios SOLID en la implementación. Agregar en la documentación los diagramas de casos de uso, diagrama de clases, diagrama de secuencia y diagrama de estado en el repositorio DocuArchiCore /Docs/Radicacion/Tramite. Agregar documentación técnica de implementación para el frontend: Descripción del DTO. Parámetros de envío a la API. Dirección de la API.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-29-crea-servicio-api-solicitafechalimite-re.