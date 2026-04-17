# Diseño - SCRUM-143 CREAR-API-ORCHES-ATOMICO-GUARDAR-DOCUMENTO

## Referencia de Contexto
- `openspec/context/multi-repo-context.md`
- `openspec/context/OPSXJ_BACKEND_RULES.md`

## Declaración del Problema
La implementación actual para guardar documentos y sincronizar imágenes en el editor Tiptap está dividida en APIs separadas. Esta falta de atomicidad genera inconsistencia de datos si el documento se guarda pero la sincronización de imágenes falla. Necesitamos un único endpoint orquestado que garantice la consistencia mediante una transacción de base de datos.

## Solución Propuesta
Implementar una nueva API orquestada `full-save` que:
1. Reciba el HTML del documento y la lista de UIDs de imágenes asociadas.
2. Inicie una transacción de base de datos.
3. Llame a la lógica existente de guardado de documentos.
4. Llame a la lógica existente de sincronización de imágenes usando la misma transacción.
5. Haga commit si ambos tienen éxito, de lo contrario, realice un rollback.

## Patrón Arquitectónico
`Controller -> Service (Orquestador) -> Repositorios (Existentes)`

### Repositorios Afectados
- `DocuArchi.Api`: Nuevo Controller.
- `MiApp.Services`: Nuevo Service e Interfaz.
- `MiApp.DTOs`: Nuevos DTOs.
- `MiApp.Repository`: Actualizar repositorios existentes para soportar transacciones.
- `MiApp.Models`: Referencia a modelos existentes.

## Detalles de Implementación

### 1. DTOs (MiApp.DTOs)
**Archivo:** `MiApp.DTOs/DTOs/GestorDocumental/Editor/FullSaveEditorDocumentRequestDto.cs`
```csharp
public class FullSaveEditorDocumentRequestDto
{
    public long? DocumentId { get; set; }
    public long? TemplateId { get; set; }
    public int? TemplateVersion { get; set; }
    public string? DocumentTitle { get; set; }
    public string DocumentHtml { get; set; } = null!;
    public string? StatusCode { get; set; }
    public List<string> ImageUids { get; set; } = new();
}
```

### 2. Service (MiApp.Services)
**Archivo:** `MiApp.Services/Service/GestorDocumental/Editor/ServiceFullSaveEditorDocument.cs`
**Interfaz:**
```csharp
public interface IServiceFullSaveEditorDocument
{
    Task<AppResponses<RaEditorDocument>> FullSaveAsync(FullSaveEditorDocumentRequestDto request, string defaultDbAlias);
}
```
**Lógica:**
- Validar `defaultDbAlias` y `request.DocumentHtml`.
- Inicializar transacción en `IDapperCrudEngine`.
- Llamar a `IGuardaEditorDocumentRepository.GuardaEditorDocumentAsync` (pasando la transacción).
- Llamar a `ISincronizaEditorDocumentImagesRepository.SincronizaEditorDocumentImagesAsync` (pasando la transacción).
- Commit/Rollback según el resultado.

### 3. Controller (DocuArchi.Api)
**Archivo:** `DocuArchi.Api/Controllers/GestorDocumental/Editor/FullSaveEditorDocumentController.cs`
**Ruta:** `POST /api/gestor-documental/editor/document/full-save`
- Validar claim `defaulalias`.
- Llamar al Service.
- Retornar `AppResponses`.

### 4. Actualizaciones de Repositorio (MiApp.Repository)
Los repositorios existentes `GuardaEditorDocumentRepository` y `SincronizaEditorDocumentImagesRepository` deben ser actualizados para aceptar un `IDbTransaction` opcional en sus métodos.

## Estrategia de Pruebas
### Pruebas Unitarias (Service)
- Éxito en creación/actualización.
- Fallo en guardado -> Rollback.
- Fallo en sincronización -> Rollback.
- Fallo en validación de claims (Controller).

### Pruebas de Integración
- Prueba de transacción real con MySQL.
- Verificar que si la sincronización falla, el documento NO se guarda.

## Restricciones (OPSXJ_BACKEND_RULES)
- Registrar en `Program.cs`.
- Usar `AppResponses`.
- Envolver en `try/catch`.
- Mockear interfaces, no Dapper.
