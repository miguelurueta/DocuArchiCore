## Context

- Jira issue key: SCRUM-24
- Jira summary: CREATE-DAPER-rea_001_feriados
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-24

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

Con la conexión existente en el repositorio DocuArchi.Api (registrada en appsettings.Development.json) y usando Dapper, lee la estructura de la tabla rea_001_feriados de la base de datos docuarchi (motor MySQL). Con esa estructura crea una clase modelo en el repositorio MiApp.Models, dentro del directorio /Models/Radicacion/Feriados/. Requerimientos: La clase debe mapear directamente las columnas de la tabla rea_001_feriados usando atributos de DataAnnotations: [Key] para la columna primaria. [Column("NombreColumna")] para cada campo. [Required] y [StringLength] según corresponda. La clase debe seguir el patrón de nombres y namespaces del proyecto: namespace MiApp.Models.Models.Radicacion.Feriados La clase debe ser pública y serializable, preparada para ser usada con Dapper. Documentar cada propiedad con comentarios XML para indicar su propósito y origen en la tabla. Mantener consistencia con el ejemplo de estructura Dapper ya existente (ejemplo: CDeRelacionEstadoRetriccionD). Preguntar la ruta antes de crear nuevos modelos, pero en este caso ubicarlo en /Models/Radicacion/Feriados/. Solicitar la estructura de la tabla rea_001_feriados antes de generar el modelo, para asegurar que las columnas y tipos de datos sean correctos. El modelo debe ser reutilizable como DTO para servicios y repositorios. Incluir pruebas unitarias que validen el mapeo de columnas con Dapper (ej. lectura y escritura de registros de prueba). Implementar separación de responsabilidades: el modelo solo representa la tabla, sin lógica adicional.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-24-create-daper-rea-001-feriados.