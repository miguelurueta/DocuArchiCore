## 1. Preparation

- [ ] 1.1 Validar que `IGuardaEditorDocumentRepository` y `ISincronizaEditorDocumentImagesRepository` existen y están accesibles.
- [ ] 1.2 Confirmar si `IDapperCrudEngine` soporta transacciones explícitas o si se requiere inyectar `IDbConnection`.

## 2. Implementation - MiApp.DTOs

- [ ] 2.1 Crear `FullSaveEditorDocumentRequestDto.cs` en `MiApp.DTOs/DTOs/GestorDocumental/Editor/`.

## 3. Implementation - MiApp.Repository (Updates)

- [ ] 3.1 Actualizar `IGuardaEditorDocumentRepository` para aceptar `IDbTransaction? transaction = null`.
- [ ] 3.2 Actualizar `ISincronizaEditorDocumentImagesRepository` para aceptar `IDbTransaction? transaction = null`.

## 4. Implementation - MiApp.Services

- [ ] 4.1 Crear `IServiceFullSaveEditorDocument.cs` e implementación `ServiceFullSaveEditorDocument.cs`.
- [ ] 4.2 Implementar lógica de orquestación con `using var transaction = ...` y bloques try/catch.

## 5. Implementation - DocuArchi.Api

- [ ] 5.1 Crear `FullSaveEditorDocumentController.cs` en `Controllers/GestorDocumental/Editor/`.
- [ ] 5.2 Registrar el nuevo servicio en `Program.cs`.

## 6. Verification & Tests

- [ ] 6.1 Crear Unit Tests para el Servicio validando Rollback en caso de fallo parcial.
- [ ] 6.2 Crear Integration Test validando persistencia atómica en base de datos real.
- [ ] 6.3 Ejecutar `dotnet test`.
- [ ] 6.4 Validar contrato final con `openspec.cmd validate`.
