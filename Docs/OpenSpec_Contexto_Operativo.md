# OpenSpec Contexto Operativo

Este archivo se actualiza automaticamente al ejecutar `Tools/Generate-OpenSpecMultiRepoContext.ps1`.

- Updated: 2026-02-28 07:21:24 -05:00
- OpenSpec root: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\openspec
- Context source: openspec/context/multi-repo-context.md
- Repositories found: 7

## Reglas Obligatorias

1. Antes de cambios cross-repo, regenerar el contexto multi-repo.
2. Proposals y design deben referenciar `openspec/context/multi-repo-context.md`.
3. Definir repos impactados en `sync.md` usando `openspec/context/REPO_IMPACT_TEMPLATE.md`.
4. Ejecutar `opsxj:new` solo en repos con `Impacta? = yes`.
5. Ejecutar `opsxj:archive` en cada repo solo despues del merge de su PR.

## Inventario de Repos

- DocuArchi.Api
  - Path: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api
  - Branch: master
  - Remote(origin): https://github.com/miguelurueta/DocuArchi.Api.git
- DocuArchiCore
  - Path: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore
  - Branch: opsxj-777-issue-opsxj-777
  - Remote(origin): https://github.com/miguelurueta/DocuArchiCore.git
- DocuArchiCore.Abstractions
  - Path: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore.Abstractions\DocuArchiCore.Abstractions
  - Branch: main
  - Remote(origin): https://github.com/miguelurueta/DocuArchiCore.Abstractions.git
- MiApp.DTOs
  - Path: D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.DTOs
  - Branch: main
  - Remote(origin): https://github.com/miguelurueta/MiApp.DTOs.git
- MiApp.Models
  - Path: D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Models
  - Branch: main
  - Remote(origin): https://github.com/miguelurueta/MiApp.Models.git
- MiApp.Repository
  - Path: D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Repository
  - Branch: main
  - Remote(origin): https://github.com/miguelurueta/MiApp.Repository.git
- MiApp.Services
  - Path: D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Services
  - Branch: main
  - Remote(origin): https://github.com/miguelurueta/MiApp.Services.git

## Referencias

- Context completo: `openspec/context/multi-repo-context.md`
- Checklist: `openspec/context/CROSS_REPO_CHECKLIST.md`
- Plantilla de impacto: `openspec/context/REPO_IMPACT_TEMPLATE.md`
- Guia operativa: `Docs/Guia_Opsxj_MultiRepo.md`
