## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-42 y confirmar alcance.
- [x] 1.2 Confirmar repos impactados y rutas destino antes de crear Controllers/DTOs/Models/funciones.
- [x] 1.3 Definir `DocuArchiCore` como repositorio orquestador del flujo `opsxj:new`.

## 2. Specs

- [x] 2.1 Completar specs/jira-scrum-42/spec.md con requisitos finales.
- [x] 2.2 Incluir referencia explicita a openspec/context/OPSXJ_BACKEND_RULES.md.
- [x] 2.3 Agregar requisitos de endpoints, atomicidad y alias DB con escenarios testables.

## 3. Application

- [ ] 3.1 Implementar `POST /api/radicacion/registrar-entrante` (Controller + Service + Repository).
- [ ] 3.2 Implementar `POST /api/radicacion/validar-entrante`.
- [ ] 3.3 Implementar `GET /api/radicacion/flujo-inicial`.
- [ ] 3.4 Implementar persistencia Q01-Q09 con:
  - Q01-Q08 en una sola transaccion.
  - rollback total si falla cualquier paso.
  - Q09 condicional segun rama funcional.
- [ ] 3.5 Registrar interfaces en Program.cs (Services/Repositories/Mapping).
- [ ] 3.6 Verificar compilacion local por repo impactado.

## 4. Test

- [ ] 4.1 Implementar pruebas unitarias:
  - validaciones de request
  - reglas de negocio
  - propagacion de `defaultDbAlias`.
- [ ] 4.2 Implementar pruebas de integracion transaccional:
  - commit exitoso
  - rollback total ante fallo Q01-Q08.
- [ ] 4.3 Implementar pruebas de contrato para endpoints API.
- [ ] 4.4 Ejecutar `dotnet test` (o dejar evidencia de skip justificado).
- [ ] 4.5 Validar cambio con `openspec.cmd validate scrum-42-crea-api-radicacion`.
- [ ] 4.6 Archivar con OpenSpec solo despues de merge de PRs impactados.
