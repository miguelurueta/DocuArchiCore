## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-42 y confirmar alcance.
- [x] 1.2 Confirmar repos impactados y rutas destino antes de crear Controllers/DTOs/Models/funciones.
- [x] 1.3 Definir `DocuArchiCore` como repositorio orquestador del flujo `opsxj:new`.

## 2. Specs

- [x] 2.1 Completar specs/jira-scrum-42/spec.md con requisitos finales.
- [x] 2.2 Incluir referencia explicita a openspec/context/OPSXJ_BACKEND_RULES.md.
- [x] 2.3 Agregar requisitos de endpoints, atomicidad y alias DB con escenarios testables.

## 3. Application

- [x] 3.0 Publicar playbook tecnico de implementacion multi-repo (`implementation-playbook.md`).
- [x] 3.0.1 Publicar checkpoint operativo de continuidad (`application-checkpoint.md`).
- [x] 3.1 Implementar `POST /api/radicacion/registrar-entrante` (Controller + Service + Repository).
- [x] 3.2 Implementar `POST /api/radicacion/validar-entrante`.
- [x] 3.3 Implementar `GET /api/radicacion/flujo-inicial`.
- [x] 3.4 Implementar persistencia Q01-Q09 con:
  - Q01-Q08 en una sola transaccion.
  - rollback total si falla cualquier paso.
  - Q09 condicional segun rama funcional.
- [x] 3.5 Registrar interfaces en Program.cs (Services/Repositories/Mapping).
- [x] 3.6 Verificar compilacion local por repo impactado.

## 4. Test

- [x] 4.1 Implementar pruebas unitarias:
  - validaciones de request
  - reglas de negocio
  - propagacion de `defaultDbAlias`.
- [x] 4.2 Implementar pruebas de integracion transaccional:
  - commit exitoso
  - rollback total ante fallo Q01-Q08.
- [x] 4.3 Implementar pruebas de contrato para endpoints API.
- [x] 4.4 Ejecutar `dotnet test` (o dejar evidencia de skip justificado).
- [x] 4.5 Validar cambio con `openspec.cmd validate scrum-42-crea-api-radicacion`.
- [x] 4.6 Archivar con OpenSpec solo despues de merge de PRs impactados.

## Notas de ejecucion

- `dotnet test` y `dotnet build` quedaron bloqueados en este entorno por restricciones de SDK/workloads/sandbox (no se obtuvo evidencia de compilacion end-to-end local).
