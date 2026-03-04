## Context

- Jira issue key: SCRUM-38
- Jira summary: ACTUALIZA-SCRUM-37
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-38

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

En el proyecto multirepo, dentro de la implementación de la API ApListaCoinsidenciaRadicados, se debe ajustar la función BuildColumns del repositorio SolicitaEstructuraCamposConsultaCoinsidenciaRadicados. Objetivo: Eliminar la sentencia que agrega la columna "estado_validacion" cuando tipoModuloDeConsulta == 1. Cambio requerido: Eliminar el bloque de código: if (tipoModuloDeConsulta == 1) { data.Add(new CamposConsultaCoinsidenciaRadicadosDTO { Key = "estado_validacion", ColumnName = "estado_validacion", HeaderName = "Estado Validacion", Order = order, Visible = true }); } Requerimientos adicionales: Mantener intacta la lógica de inserción de los campos fijos (id_Radicado, Consecutivo_Rad, Consecutivo_CodBarra, etc.). Asegurar que el mapeo de DetallePlantillaRadicado → UiColumnDto vía AutoMapperProfile siga funcionando sin la columna estado_validacion. Validar que la eliminación no afecte la generación de UiActionDto ni las reglas de RowRuleDto. Documentar el cambio en comentarios XML dentro de la función BuildColumns indicando que la columna estado_validacion fue eliminada por decisión arquitectónica. Actualizar pruebas unitarias para reflejar que la columna estado_validacion ya no debe aparecer en el DTO de salida. Actualizar documentación técnica en /Docs/Radicacion/ConsultaRadicacion para reflejar la nueva estructura de columnas.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-38-actualiza-scrum-37.