## 1. Refinamiento OpenSpec

- [x] 1.1 Refinar `proposal.md` con problema real (`GAB_FIELDS_NOT_FOUND`) y alcance tecnico.
- [x] 1.2 Refinar `design.md` con arquitectura, reglas de validacion, cache y riesgos.
- [x] 1.3 Refinar `specs/jira-scrum-196/spec.md` con requisitos y escenarios testables.
- [x] 1.4 Ejecutar `openspec validate scrum-196-implementacion-validacion-metada-data-ga`.

## 2. MiApp.Models

- [ ] 2.1 Ampliar modelo de metadata de campo para incluir tipo declarado, longitud y obligatoriedad backend.
- [ ] 2.2 Mantener compatibilidad con `StorageContext.GabineteFieldsMetadata`.
- [ ] 2.3 Definir constantes/enums para codigos de error de validacion metadata.

## 3. MiApp.Repository

- [ ] 3.1 Crear repositorio para consultar metadata de `detalle_gabienete` por `gabinete` (case-insensitive).
- [ ] 3.2 Implementar SQL parametrizado con `DapperCrudEngine` (sin concatenacion dinamica).
- [ ] 3.3 Implementar consulta opcional a `INFORMATION_SCHEMA.COLUMNS` para consistencia fisica.
- [ ] 3.4 Cubrir errores operativos con respuesta controlada y logging tecnico.

## 4. MiApp.Services

- [ ] 4.1 Reemplazar placeholder de `StorageGabineteMetadataProvider` por provider real con repositorio.
- [ ] 4.2 Implementar parser de tipos legacy (`VARCHAR`, `CHAR`, `INT`, `INTEGER`, `DATE`, `DATETIME`, `TEXT`, `LONGTEXT`).
- [ ] 4.3 Implementar validacion de longitud por metadata declarada.
- [ ] 4.4 Implementar validacion de compatibilidad de tipo por valor recibido.
- [ ] 4.5 Integrar validaciones en pipeline sin romper contrato `StorageValidationResult`.
- [ ] 4.6 Agregar cache de metadata por `alias + gabinete` con TTL configurable.

## 5. DocuArchi.Api

- [ ] 5.1 Registrar nuevas interfaces en `Program.cs` bajo secciones Services (L) y Repositories (R).
- [ ] 5.2 Revisar manejo de excepciones para conservar `AppResponses` consistente.
- [ ] 5.3 Verificar que endpoint de almacenamiento preserve comportamiento actual salvo nuevas validaciones.

## 6. MiApp.DTOs

- [ ] 6.1 Ajustar DTOs solo si se requiere exponer nuevos detalles de error de validacion.
- [ ] 6.2 Mantener compatibilidad hacia atras de contrato JSON.

## 7. Pruebas

- [ ] 7.1 Unit tests parser de tipos (casing, espacios, longitud y tipos no soportados).
- [ ] 7.2 Unit tests validacion de longitud (`VARCHAR/CHAR`) y exclusiones (`TEXT/LONGTEXT`).
- [ ] 7.3 Unit tests validacion de tipo (`INT`, `DATE`, `DATETIME`) y errores esperados.
- [ ] 7.4 Unit tests validacion de existencia/obligatoriedad de campo.
- [ ] 7.5 Unit tests cache (`miss`, `hit`, expiracion).
- [ ] 7.6 Integration tests lectura real de metadata (`detalle_gabienete`) y chequeo fisico opcional.
- [ ] 7.7 Contract tests para forma de errores de validacion en endpoint almacenamiento.

## 8. Documentacion y cierre

- [ ] 8.1 Actualizar documentos tecnicos de arquitectura/implementacion/pruebas para metadata de gabinete.
- [ ] 8.2 Adjuntar evidencia de pruebas ejecutadas y resultados.
- [ ] 8.3 Ejecutar flujo de publicacion/archivo OpenSpec cuando todos los checks esten en verde.
