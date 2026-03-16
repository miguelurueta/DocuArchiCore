## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-67 y confirmar alcance.
- [x] 1.2 Confirmar repos impactados y rutas destino antes de crear Controllers/DTOs/Models/funciones.
- [x] 1.3 Solicitar estructura de tabla si se requiere nuevo modelo.

## 2. Specs

- [x] 2.1 Completar specs/jira-scrum-67/spec.md con requisitos finales.
- [x] 2.2 Incluir referencia explicita a openspec/context/OPSXJ_BACKEND_RULES.md.
- [x] 2.3 Verificar escenarios testables por requisito.

## 3. Application

- [x] 3.1 Aplicar patron interno minimo con Service + AppResponses + try/catch, sin Controller ni Repository nuevos.
- [x] 3.2 Registrar interfaces en Program.cs (Services L / Repositories R).
- [x] 3.3 Integrar cambios de aplicacion y verificar compilacion local.

## 4. Test

- [x] 4.1 Implementar Unit tests y documentar evidencia.
- [x] 4.2 Ejecutar dotnet test (o skipped explicito si Docker no disponible).
- [x] 4.3 Validar cambio con OpenSpec.

## 5. Operacion

- [ ] 5.1 Preparar commits/PRs multi-repo en MiApp.Services, DocuArchi.Api y DocuArchiCore.
- [ ] 5.2 Archivar desde DocuArchiCore como repositorio coordinador despues del merge de PRs.
