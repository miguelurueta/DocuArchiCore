## 1. OpenSpec Refinement

- [x] 1.1 Refinar `proposal.md` con problema real (`Unknown column ESTADO_DISCO`) y alcance tecnico.
- [x] 1.2 Refinar `design.md` con regla legacy de capacidad y matriz con/sin `ESTADO_DISCO`.
- [x] 1.3 Refinar `specs/jira-scrum-197/spec.md` con requisitos y escenarios testables.

## 2. MiApp.Repository

- [ ] 2.1 Ajustar `StorageDiskQuotaRepository.LockDiskStatusAsync` para no depender obligatoriamente de `ESTADO_DISCO`.
- [ ] 2.2 Mantener lock pesimista (`FOR UPDATE`) y consultas parametrizadas con `DapperCrudEngine`.
- [ ] 2.3 Si aplica, resolver lectura opcional de `ESTADO_DISCO` solo cuando exista la columna.

## 3. MiApp.Services

- [ ] 3.1 Ajustar politica de cuota para usar regla legacy basada en `tamdisc + numero_imagenes` como fuente primaria.
- [ ] 3.2 Conservar mensaje legacy para null/0/sobrecupo.
- [ ] 3.3 Mantener compatibilidad opcional con `EstadoDisco == "SL"` como refuerzo.
- [ ] 3.4 Verificar integracion en `StorageIdentityAllocator`/`StorageTransactionCoordinator` antes de commit.

## 4. MiApp.Models

- [ ] 4.1 Ajustar modelos de estado de cuota para soportar `EstadoDisco` opcional.
- [ ] 4.2 Validar compatibilidad con contratos internos existentes.

## 5. DocuArchi.Api / Contracts

- [ ] 5.1 Confirmar que no hay cambios de contrato HTTP ni DTO publico.
- [ ] 5.2 Confirmar que manejo de errores mantiene `AppResponses` consistente.

## 6. Tests

- [ ] 6.1 Unit tests de umbrales legacy (`80000`, `7500`, borde `572523149`).
- [ ] 6.2 Unit tests para `numero_imagenes` null y `0`.
- [ ] 6.3 Unit tests de compatibilidad opcional con `EstadoDisco == "SL"`.
- [ ] 6.4 Integration tests: schema sin `ESTADO_DISCO` y schema con `ESTADO_DISCO`.
- [ ] 6.5 Ejecutar `dotnet test` y registrar evidencia.

## 7. Documentation and Closure

- [ ] 7.1 Actualizar documentacion tecnica en `Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/`.
- [ ] 7.2 Incluir matriz de decisiones (con/sin `ESTADO_DISCO`) y decision de borde `tamdisc == 572523149`.
- [ ] 7.3 Ejecutar `openspec validate scrum-197-correcion-validacion-tamano-disco-almace`.
- [ ] 7.4 Ejecutar flujo `opsxj:orchestrate:publish` y `opsxj:orchestrate:archive` al completar checks.
