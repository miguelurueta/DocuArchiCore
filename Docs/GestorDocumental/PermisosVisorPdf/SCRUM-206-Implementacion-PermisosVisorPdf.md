# SCRUM-206 Implementacion Tecnica Permisos Visor PDF

## 1. Resumen de Implementación
Se implementó una API dedicada para resolver y administrar permisos del visor PDF por implementación/usuario, con contrato uniforme y precedencia efectiva en repositorio.

## 2. Endpoints
- `GET /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/mis-permisos`
- `GET /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/usuarios/{idUsuario}/permisos`
- `PUT /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/usuarios/{idUsuario}/overrides`
- `DELETE /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/usuarios/{idUsuario}/overrides/{codigoPermiso}`

## 3. Artefactos Implementados
### 3.1 API
- `DocuArchi.Api/Controllers/GestorDocumental/PermisosVisorPdf/PermisosVisorPdfController.cs`

### 3.2 Services
- `MiApp.Services/Service/GestorDocumental/PermisosVisorPdf/IPermisosVisorPdfService.cs`

### 3.3 DTOs
- `MiApp.DTOs/DTOs/GestorDocumental/PermisosVisorPdf/PermisosVisorPdfDtos.cs`

### 3.4 Repository
- `MiApp.Repository/Repositorio/GestorDocumental/PermisosVisorPdf/IPermisosVisorPdfRepository.cs`

### 3.5 DI
- Registro en `DocuArchi.Api/Program.cs` de:
  - `IPermisosVisorPdfService`
  - `IPermisosVisorPdfRepository`

## 4. Reglas funcionales implementadas
- Resolución por precedencia:
  - override usuario
  - perfil activo
  - default implementación
  - deny por fallback
- Contrato por usuario: `codiperfil` no viaja en request.
- `Sources` informa el origen de cada permiso.
- Validación de códigos por regex segura `^[A-Za-z0-9_\.]+$`.
- Mutaciones `PUT/DELETE overrides` con operación transaccional y rollback en error.

## 5. Manejo de Excepciones
- `try/catch` en controller/service/repository nuevos.
- `AppResponses<T>` para éxito, validación y error técnico.
- Endpoints admin retornan `Unauthorized` cuando no cumplen `IsAdmin()`.

## 6. Casos de error cubiertos
- Claim `defaulalias` ausente/invalid.
- Claim `usuarioid` ausente/invalid para `mis-permisos`.
- `codigoImpl` inválido/no encontrado.
- `idUsuario <= 0` en endpoints de tercero.
- `overrides` vacío en `PUT`.
- `codigoPermiso` inválido/no encontrado.

## 7. Resultado esperado
- Frontend consume un único mapa `Permissions` por implementación.
- La lógica de precedencia queda centralizada en backend.
- La administración puntual de permisos por usuario se realiza con `PUT/DELETE overrides`.

## 8. Validación técnica ejecutada
- Pruebas de contrato/controller ejecutadas:
  - `dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj --filter "FullyQualifiedName~PermisosVisorPdfControllerTests" --no-restore`
- Resultado:
  - `Total: 5, Superado: 5, Con error: 0, Omitido: 0`.
- Cobertura validada en esta corrida:
  - `GET .../mis-permisos` (claims y shape `AppResponses<VisorPdfPermissionsResponseDto>`).
  - `GET .../usuarios/{idUsuario}/permisos` (restricción admin).
  - `PUT .../overrides` y `DELETE .../overrides/{codigoPermiso}` (shape `AppResponses<SimpleOperationResultDto>`).

- Pruebas de integración repository ejecutadas:
  - `dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj --filter "FullyQualifiedName~PermisosVisorPdfRepositoryIntegrationTests" --no-restore`
- Resultado:
  - `Total: 5, Superado: 5, Con error: 0, Omitido: 0`.
- Cobertura validada:
  - Precedencia `default -> perfil -> override`.
  - Persistencia de `PUT/DELETE` sobre `ra_vis_per_usuario_override`.
  - Fallback `deny` cuando un permiso no existe en matrices.
