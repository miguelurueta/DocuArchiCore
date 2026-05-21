## 1. Refinamiento OpenSpec

- [x] 1.1 Refinar `design.md` con alcance/no alcance, decisiones de contrato, estrategia de datos y validaciones.
- [x] 1.2 Refinar `specs/jira-scrum-208/spec.md` con escenarios testeables por endpoint, validación y compatibilidad.
- [x] 1.3 Reestructurar `tasks.md` en fases implementables por capas y cierre orquestado.

## 2. Discovery técnico previo a código

- [x] 2.1 Confirmar estructura real de tabla `ra_cert_registro_certificado_archivo` (nombres de columnas y tipos efectivos).
- [x] 2.2 Confirmar ubicaciones finales en repos satélite para Controller/Service/Repository/DTO.
- [x] 2.3 Confirmar contrato de claims (`defaulalias` obligatorio, `usuarioid` trazabilidad).

## 3. Contratos y capa API

- [x] 3.1 Crear DTO `FirmaElectronicaDocumentoResponseDto` en ruta de `MiApp.DTOs` acordada.
- [x] 3.2 Implementar `FirmaElectronicaDocumentoController` con endpoint:
  - `GET /api/gestor-documental/documentos/{idArchivo}/firma-electronica`
- [x] 3.3 Validar claim `defaulalias` y entradas (`idArchivo`, `nombreGabinete`) con respuesta controlada `AppResponses`.

## 4. Capa Service

- [x] 4.1 Crear `IFirmaElectronicaDocumentoService` y `FirmaElectronicaDocumentoService`.
- [x] 4.2 Implementar orquestación con fallback funcional:
  - registro existe -> `FirmadoElectronico=true`, `IdCertificado>0`
  - registro no existe -> `FirmadoElectronico=false`, `IdCertificado=0`
- [x] 4.3 Aplicar `try/catch` y mapeo de errores controlados sin exponer detalles internos.

## 5. Capa Repository y migración legacy

- [x] 5.1 Crear `IFirmaElectronicaDocumentoRepository` y `FirmaElectronicaDocumentoRepository`.
- [x] 5.2 Implementar consulta con `DapperCrudEngine + QueryOptions`:
  - tabla `ra_cert_registro_certificado_archivo`
  - filtros `id_archivo` + `nombre_gabinete`
  - orden `id_registro_certificado_archivo DESC`
  - `Limit = 1`
- [x] 5.3 Verificar cumplimiento de política sin SQL manual ni concatenación.

## 6. DI, observabilidad y hardening

- [x] 6.1 Registrar Service/Repository en `Program.cs` (`Services (L)` / `Repositories (R)`).
- [x] 6.2 Incorporar logs mínimos (`Information`, `Warning`, `Error`) con campos operativos.
- [x] 6.3 Revisar consistencia de envelope `AppResponses` y códigos HTTP (`Ok`/`BadRequest`).

## 7. Pruebas

- [x] 7.1 Controller tests:
  - claim faltante
  - `idArchivo` inválido
  - `nombreGabinete` inválido
  - success `200`
- [x] 7.2 Service tests:
  - certificado encontrado
  - certificado no encontrado (fallback)
  - excepción de repository
- [x] 7.3 Repository tests:
  - `QueryOptions` correcto
  - prioridad del último registro
  - sin registros
- [ ] 7.4 Integration/contract tests y evidencia:
  - seed/schema caso firmado/no firmado
  - no regresión de contratos existentes (incl. SCRUM-206)
- [x] 7.5 Ejecutar `dotnet test` (o marcar integración `Skipped` con justificación explícita).

## 8. Documentación técnica

- [x] 8.1 Crear carpeta `Docs/GestorDocumental/Documentos/FirmaElectronica/`.
- [x] 8.2 Crear `SCRUM-208-Arquitectura.md`.
- [x] 8.3 Crear `SCRUM-208-Implementacion-Detallada.md`.
- [x] 8.4 Crear `SCRUM-208-Integracion-Frontend.md`.
- [x] 8.5 Crear `SCRUM-208-Pruebas.md`.
- [x] 8.6 Crear `SCRUM-208-Observabilidad.md`.
- [x] 8.7 Crear `SCRUM-208-Seguridad.md`.
- [x] 8.8 Crear `SCRUM-208-Metadata.md`.
- [x] 8.9 Verificar consistencia cruzada entre `design/spec/tasks` y docs.

## 9. Cierre OpenSpec

- [x] 9.1 Ejecutar `openspec.cmd validate --changes scrum-208-implementacion-api-valida-firma-digital`.
- [ ] 9.2 Ejecutar `opsxj:orchestrate:publish -- SCRUM-208 -NonInteractive` al completar implementación.
- [ ] 9.3 Ejecutar `opsxj:orchestrate:archive -- SCRUM-208 -NonInteractive` tras merge multi-repo.
