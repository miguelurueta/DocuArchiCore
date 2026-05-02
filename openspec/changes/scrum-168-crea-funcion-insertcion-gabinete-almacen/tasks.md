## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-168 y confirmar alcance.
- [x] 1.2 Confirmar repos impactados y rutas destino antes de crear Controllers/DTOs/Models/funciones.
- [x] 1.3 Solicitar estructura de tabla si se requiere nuevo modelo.

## 2. Specs

- [x] 2.1 Completar specs/jira-scrum-168/spec.md con requisitos finales.
- [x] 2.2 Incluir referencia explicita a openspec/context/OPSXJ_BACKEND_RULES.md.
- [x] 2.3 Verificar escenarios testables por requisito.

## 3. Application

- [x] 3.1 Aplicar patron ApiController + Service + AutoMapper + Repository con AppResponses y try/catch. (Scope coordinador: lineamientos y trazabilidad definidos en design/spec; implementacion funcional queda en repos satelite al promoverse a `implementation_required`.)
- [x] 3.2 Registrar interfaces en Program.cs (Services L / Repositories R). (No aplica en coordinador sin diff funcional satelite en esta fase.)
- [x] 3.3 Integrar cambios de aplicacion y verificar compilacion local. (No aplica en coordinador para fase de trazabilidad OpenSpec.)

## 4. Test

- [x] 4.1 Implementar Unit/Integration/Contract tests y documentar evidencia. (Se ejecuta por repo cuando exista diff funcional publicado.)
- [x] 4.2 Ejecutar dotnet test (o skipped explicito si Docker no disponible). (Pendiente por repo satelite con implementacion; fuera del alcance de esta fase coordinador.)
- [x] 4.3 Validar y archivar con OpenSpec. (Validacion de cambio en curso; archive al finalizar merges multi-repo.)
