# PROMPT ARQUITECTONICO - Ticket BE-02

# Migrar GestorDocumental/Editor a DapperCrudEngine

## OBJETIVO
Estandarizar acceso a datos del modulo `GestorDocumental/Editor` con `DapperCrudEngine`, sin cambios funcionales observables.

## REPOSITORIO Y RUTAS/FUNCIONES A INTERVENIR
- `...\GuardaEditorDocumentContextRepository.cs`
  - `GuardaEditorDocumentContextAsync(...)`
  - `TryGetActiveByEntityAsync(...)`
  - `TryGetActiveByDocumentAndDefinitionAsync(...)`
- `...\GuardaEditorDocumentRepository.cs`
  - `GuardaEditorDocumentAsync(...)`
- `...\LimpiezaEditorImagesRepository.cs`
  - `SolicitaImagenesHuerfanasAsync(...)`
  - `MarcaImagenesComoEliminadasAsync(...)`
- `...\SincronizaEditorDocumentImagesRepository.cs`
  - `SincronizaAsync(...)`
- `...\SolicitaEditorContextDefinitionRepository.cs`
  - `SolicitaPorContextCodeAsync(...)`
- `...\SolicitaEditorDocumentByContextRepository.cs`
  - `SolicitaEditorDocumentByContextAsync(...)`
  - `CountActiveByContextAsync(...)`
- `...\SolicitaEditorDocumentByIdRepository.cs`
  - `SolicitaByIdAsync(...)`
- `...\TemplateContextRulesRepository.cs`
  - `TryGetTemplateDefinitionIdForContextAsync(...)`
- `...\TemplateDefinitionsRepository.cs`
  - `CreateDefinitionAsync(...)`
  - `CreateVersionAsync(...)`
  - `GetDefinitionByCodeAsync(...)`
  - `GetDefinitionByIdAsync(...)`
  - `GetVersionsByCodeAsync(...)`
  - `GetVersionsByDefinitionIdAsync(...)`
- `...\TemplateTokensRepository.cs`
  - `GetTokensByDefinitionIdAsync(...)`

## REGLAS
- Priorizar migracion incremental en sublotes (Document, Context, Template).
- Respetar transacciones existentes.
- Sin romper contratos/DTOs.

## VALIDACION DEL LOTE
- Build de proyectos impactados.
- Tests focalizados de Editor.
- Informe de riesgo por concurrencia/transacciones.

## ENTREGABLE FINAL
- funciones migradas
- funciones pendientes
- decisiones tecnicas
- plan de cierre
