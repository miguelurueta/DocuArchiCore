# SCRUM-180 — Metadata

- Fecha: 2026-05-04
- Issue: `SCRUM-180`
- Tema: `IMPLEMENTACION-RUTA-FISICA-ALMACENAMIENTO`
- Tipo: Arquitectura + Implementación backend + Seguridad de paths
- Estado técnico: Implementado en código y documentado

## Repositorios impactados
- `MiApp.Models`
- `MiApp.Repository`
- `MiApp.Services`
- `DocuArchi.Api`
- `DocuArchiCore` (documentación y pruebas)

## Artefactos principales
- Modelos físicos legacy (`StorageRouteModel`, `StorageFolderResult`, `StoragePhysicalPathModel`)
- Repositorio de ruta `SYSTEM1RUT`
- Política de carpeta legacy
- Servicio de resolución física legacy
- Hardening de resolución de ruta final
- Pruebas unitarias nuevas SCRUM-180

## Dependencias funcionales
- Prompt 9: integración de ruta final segura en plan físico.
- Prompt 13: separación Naming vs PathService preservada.
