# JIRA Ticket - SCRUM-38

En el proyecto multirepo necesito crear una especificación OpenSpec para migrar la lógica de radicación entrante del sistema legacy (`Registra_Radicacion_entrante`) al entorno actual, lista para implementación por capas `Controller -> Service -> Repository`.

Ubicación:

- Repositorio origen (legacy): GestionDocumental-Docuarchi.net
- Ruta origen: /radicador/ClassRadicador.vb
- Función origen: Registra_Radicacion_entrante
- Repositorio destino: DocuArchiCore
- OpenSpec destino: /openspec/changes/ y /openspec/specs/jira-scrum-38/

Objetivo:

Desarrollar un ticket OpenSpec que capture el comportamiento funcional actual de radicación entrante y lo traduzca a artefactos implementables en el entorno actual, sin modificar código legacy.

Dependencias:

1. Flujo actual de validaciones, permisos, plantilla y asignación contenido en Registra_Radicacion_entrante.
2. Estructura del entorno destino en DocuArchiCore para Controller, Service, Repository, DTO, Mapping y Docs.
3. Convenciones OpenSpec (proposal.md, design.md, tasks.md, spec.md).



---



Controller: API de Radicación Entrante (Destino)

Ubicación:

- Repositorio: DocuArchiCore (API)
- Ruta sugerida: /Controllers/Radicacion/Tramite

Parámetros:

- defaultDbAlias
- Datos de solicitud de radicación entrante (según spec)

Requerimientos funcionales del Controller:

1. Exponer endpoint para registrar radicación entrante en el nuevo entorno.
2. Retornar respuesta envuelta en AppResponses.
3. Manejar errores con bloque try/catch.
4. No contener lógica de negocio; delegar a Service.



---



Servicio: Migración de lógica de Registra_Radicacion_entrante

Ubicación:

- Repositorio: DocuArchiCore (Service)
- Ruta sugerida: /Services/Radicacion/Tramite

Parámetros:

- defaultDbAlias
- DTO de entrada de radicación

Dependencias del Servicio:

1. Repositorio de usuario/gestión para datos de radicador.
2. Repositorio de plantilla/configuración de radicación.
3. Repositorio de persistencia de estado/radicado y consultas auxiliares.
4. Mapper DTO <-> entidades.

Requerimientos funcionales del Servicio:

1. Replicar el comportamiento funcional de la función legacy como casos de uso del nuevo entorno.
2. Separar validaciones por etapas (controles requeridos, plantilla, flujo, datos obligatorios, fechas).
3. Estandarizar contratos de salida con AppResponses.
4. Envolver el servicio en try/catch.
5. Definir respuestas para éxito, validación fallida, sin resultados y excepción.



---



Repositorio: Operaciones de Radicación Entrante

Ubicación:

- Repositorio: DocuArchiCore (Repository)
- Ruta sugerida: /Repositorio/Radicador/Tramite

Parámetros:

- Identificadores de plantilla/usuario/flujo requeridos por el servicio.
- defaultDbAlias

Dependencias:

- IDapperCrudEngine / engine de acceso a datos existente.
- Configuración de conexión por alias.

Requerimientos funcionales del Repositorio:

1. Implementar consultas parametrizadas (sin SQL Injection).
2. Retornar datos envueltos en AppResponses.
3. Incluir DefaultAlias = defaultDbAlias en QueryOptions.
4. Crear interfaz en el mismo archivo de cada repositorio nuevo para facilitar pruebas.
5. Registrar interfaz e implementación en Program.cs.
6. Manejar casos sin resultados con Success=true, Data=null, Message="Sin resultados".
7. Manejar excepciones con Success=false, ErrorMessage=ex.Message, Data vacío.
8. Documentar funciones con comentarios XML.



---



Modelo y DTO

Ubicación:

- Modelos: /Models/Radicacion/Tramite/
- DTOs: /DTOs/Radicacion/Tramite/
- Mapping: /Services/Mapping/Radicacion/Tramite/

Requerimientos:

1. Crear modelos/DTOs necesarios para representar entrada/salida del flujo migrado.
2. Usar DataAnnotations cuando aplique para mapeo y validación.
3. Crear perfil AutoMapper para transformaciones.
4. Documentar propiedades y contratos con comentarios XML.
5. Mantener separación de responsabilidades (modelo persistencia vs DTO de transporte).



---



OpenSpec (obligatorio en este ticket)

Ubicación:

- Cambio: /openspec/changes/<change-name>/
- Spec: /openspec/specs/jira-scrum-38/spec.md

Requerimientos OpenSpec:

1. Generar proposal.md con alcance y motivación de migración.
2. Generar design.md con arquitectura y decisiones técnicas.
3. Generar tasks.md con tareas implementables por capas.
4. Generar spec.md con reglas funcionales y criterios verificables.
5. Incluir matriz de trazabilidad: regla legacy -> requisito nuevo -> tarea técnica.
6. Dejar el ticket listo para ejecución posterior con /opsx:apply.



---



Requerimientos adicionales generales:

1. Seguir patrón Controller -> Service -> Repository.
2. Cumplir SoC, SOLID, bajo acoplamiento y alta cohesión.
3. Incluir pruebas unitarias por capa.
4. Incluir pruebas de integración con MySQL usando Testcontainers/Docker.
5. Cubrir escenarios de éxito, validación fallida, sin resultados y excepción.
6. Documentar API para frontend: endpoint, payload, DTO respuesta y códigos esperados.
7. Incluir diagramas en /Docs/Radicacion/Tramite: casos de uso, clases, secuencia y estado.

Criterio final del ticket:

1. El ticket debe producir especificación OpenSpec completa para migración funcional, no corrección directa del código legacy.
