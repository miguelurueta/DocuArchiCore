## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-35 y confirmar alcance.
- [x] 1.2 Confirmar repos impactados y rutas destino antes de documentar.
- [x] 1.3 Confirmar que no se requiere nuevo modelo ni cambios de runtime para este ticket documental.

## 2. Specs

- [x] 2.1 Completar specs/jira-scrum-35/spec.md con requisitos finales de documentación.
- [x] 2.2 Incluir referencia explicita a openspec/context/OPSXJ_BACKEND_RULES.md.
- [x] 2.3 Verificar escenarios testables por requisito.

## 3. Documentation Implementation

- [x] 3.1 Mover `SCRUM-34-Diagramas.md` y `SCRUM-34-Integracion-Frontend.md` a `Docs/UI/MuiTable`.
- [x] 3.2 Documentar funciones publicas de `DynamicUiTableBuilder` con formato XML-style (proposito, parametros, retorno).
- [x] 3.3 Documentar consumo interno del servicio (`TableId`, `DefaultDbAlias`, claims, retorno).
- [x] 3.4 Incluir ejemplo end-to-end con request/response realista basado en `ra_rad_estados_modulo_radicacion`.
- [x] 3.5 Verificar y completar guia frontend (DTO, API, parametros, ejemplo React/MUI).

## 4. Test

- [x] 4.1 Validar consistencia de enlaces/rutas y estructura de documentos.
- [x] 4.2 Ejecutar `openspec validate scrum-35-documentacion-componente-dynamicuitable` y registrar evidencia.
- [ ] 4.3 Validar y archivar con OpenSpec.
