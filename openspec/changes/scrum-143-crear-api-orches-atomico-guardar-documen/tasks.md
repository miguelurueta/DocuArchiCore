## 1. Preparación

- [ ] 1.1 Validar que `IGuardaEditorDocumentRepository` y `ISincronizaEditorDocumentImagesRepository` existen y están accesibles.
- [ ] 1.2 Confirmar si `IDapperCrudEngine` soporta transacciones explícitas o si se requiere inyectar `IDbConnection`.

## 2. Implementación - MiApp.DTOs

- [ ] 2.1 Crear `FullSaveEditorDocumentRequestDto.cs` en `MiApp.DTOs/DTOs/GestorDocumental/Editor/`.

## 3. Implementación - MiApp.Repository (Actualizaciones)

- [ ] 3.1 Actualizar `IGuardaEditorDocumentRepository` para aceptar `IDbTransaction? transaction = null`.
- [ ] 3.2 Actualizar `ISincronizaEditorDocumentImagesRepository` para aceptar `IDbTransaction? transaction = null`.

## 4. Implementación - MiApp.Services

- [ ] 4.1 Crear `IServiceFullSaveEditorDocument.cs` e implementación `ServiceFullSaveEditorDocument.cs`.
- [ ] 4.2 Implementar lógica de orquestación con `using var transaction = ...` y bloques try/catch.

## 5. Implementación - DocuArchi.Api

- [ ] 5.1 Crear `FullSaveEditorDocumentController.cs` en `Controllers/GestorDocumental/Editor/`.
- [ ] 5.2 Registrar el nuevo servicio en `Program.cs`.

## 6. Verificación y Pruebas

- [ ] 6.1 Crear Pruebas Unitarias para el Servicio validando Rollback en caso de fallo parcial.
- [ ] 6.2 Crear Prueba de Integración validando persistencia atómica en base de datos real.
- [ ] 6.3 Ejecutar `dotnet test`.
- [ ] 6.4 Validar contrato final con `openspec.cmd validate`.
