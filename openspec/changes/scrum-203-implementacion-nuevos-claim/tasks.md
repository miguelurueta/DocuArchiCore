## 1. Refinamiento

- [x] 1.1 Confirmar alcance final del ticket: solo incorporación de claims JWT y contrato asociado.
- [x] 1.2 Confirmar mapeo canónico campo DB -> claim.
- [x] 1.3 Confirmar compatibilidad con login normal y 2FA (sin cambiar claims legacy).

## 2. Design/Spec

- [x] 2.1 Refinar `design.md` con decisiones arquitectónicas, riesgos y estrategia de validación.
- [x] 2.2 Refinar `specs/jira-scrum-203/spec.md` con requisitos funcionales medibles.
- [x] 2.3 Asegurar trazabilidad explícita de repos/archivos impactados.

## 3. Implementación Backend

- [x] 3.1 Actualizar `MiApp.DTOs/DTOs/Autenticacion/UsuarioAutenticadoDTO.cs` con:
  - `IdUsuarioWorkflow`
  - `IdUsuarioWorkflowExt`
  - `IdUsuarioRadicador`
  - `IdUsuarioDa`
- [x] 3.2 Actualizar mapping en `MiApp.Services/Service/Mapping/GestorDocumental/Usuario/RemitDestInternoMapping.cs`.
- [x] 3.3 Actualizar emisión de claims en `MiApp.Services/Service/Autenticacion/TokenService.cs`.
- [x] 3.4 Verificar continuidad en flujo 2FA:
  - `MiApp.Services/Service/Autenticacion/Providers/EmailSecondFactorProvider.cs`
  - `MiApp.Services/Service/Autenticacion/SecondFactor/SecondFactorService.cs`
- [ ] 3.5 (Opcional recomendado) centralizar nombres en `DocuArchiClaimTypes.cs`.

## 4. Contrato Frontend/Documentación

- [x] 4.1 Actualizar documentación técnica backend de claims JWT para frontend.
- [x] 4.2 Incluir tabla: `claim`, origen DB, tipo, obligatoriedad, fallback.
- [x] 4.3 Incluir ejemplo de payload JWT (normal + 2FA) con claims nuevos y legacy.

## 5. Pruebas

- [ ] 5.1 Unit tests de mapping y DTO.
- [ ] 5.2 Unit tests de `TokenService` validando 4 claims nuevos y claims legacy.
- [ ] 5.3 Prueba de integración login normal -> token contiene claims nuevos.
- [ ] 5.4 Prueba de integración 2FA -> token contiene claims nuevos.
- [ ] 5.5 Ejecutar `dotnet test` en suites impactadas y registrar evidencia.

## 6. OpenSpec Flow

- [x] 6.1 Ejecutar `openspec.cmd validate scrum-203-implementacion-nuevos-claim`.
- [ ] 6.2 Preparar `orchestrate:publish` con tasks completos.
- [ ] 6.3 Cerrar con `orchestrate:archive` tras merge multi-repo.
