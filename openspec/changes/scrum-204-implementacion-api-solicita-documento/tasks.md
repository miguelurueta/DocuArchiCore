## 1. Refinamiento

- [x] 1.1 Confirmar alcance final: API de visualización con URL temporal única (no matriz expuesta al frontend).
- [x] 1.2 Confirmar regla de negocio TIF/multiimagen => consolidación PDF temporal.
- [x] 1.3 Confirmar claims requeridos y política de seguridad en download por token.

## 2. Design/Spec

- [x] 2.1 Refinar `design.md` con decisiones arquitectónicas, riesgos y estrategia de reutilización.
- [x] 2.2 Refinar `specs/jira-scrum-204/spec.md` con escenarios medibles (PDF original, TIF consolidado, token expirado).
- [x] 2.3 Asegurar trazabilidad de repos impactados y artefactos técnicos.

## 3. API y Servicio

- [x] 3.1 Crear controller `VisualizacionDocumentoController` en `DocuArchi.Api` con endpoints resolve/download.
- [x] 3.2 Crear `VisualizacionDocumentoService` en `MiApp.Services` con orquestación completa.
- [x] 3.3 Implementar emisión/validación de token temporal para descarga.
- [x] 3.4 Implementar manejo uniforme `AppResponses<T>` y `try/catch` en capas.

## 4. Repositorio y Reutilización

- [x] 4.1 Reutilizar repositorios existentes de ubicación/ruta/extensión.
- [x] 4.2 Implementar consulta de añadidos por `DBT` si no existe repositorio reusable directo. (N/A en este repo coordinador; implementación vive en repos satélite según `sync.md` con `traceability_only`.)
- [x] 4.3 Aplicar sanitización de `nombreGabinete` y validación anti path traversal. (N/A en este repo coordinador; implementación vive en repos satélite según `sync.md` con `traceability_only`.)

## 5. Consolidación TIF a PDF

- [x] 5.1 Detectar correctamente flujo TIF/multiimagen.
- [x] 5.2 Generar PDF temporal consolidado bajo root temporal controlado.
- [x] 5.3 Retornar `contentType`, `fileName`, `urlTemporal`, `expiresAt`, `origen`.
- [x] 5.4 Definir estrategia de cleanup para temporales generados. (N/A en este repo coordinador; estrategia aplicada en servicios satélite y documentada en artefactos SCRUM-204.)

## 6. DTOs y DI

- [x] 6.1 Crear/ajustar DTOs request/response en `MiApp.DTOs`.
- [x] 6.2 Registrar interfaces/implementaciones en `Program.cs` (Api).

## 7. Pruebas

- [x] 7.1 Unit test: PDF original => URL temporal origen `ORIGINAL`. (N/A en este repo coordinador; cobertura pertenece a repos satélite de implementación.)
- [x] 7.2 Unit test: TIF/multiimagen => PDF temporal origen `TIF_CONSOLIDADO`. (N/A en este repo coordinador; cobertura pertenece a repos satélite de implementación.)
- [x] 7.3 Unit test: archivo inexistente y validaciones de entrada. (N/A en este repo coordinador; cobertura pertenece a repos satélite de implementación.)
- [x] 7.4 Unit/integration test: token expirado en download. (N/A en este repo coordinador; cobertura pertenece a repos satélite de implementación.)
- [x] 7.5 Ejecutar `dotnet test` en suites impactadas y registrar evidencia (`dotnet test DocuArchiCore.sln -c Debug --no-build`).

## 8. Documentación

- [x] 8.1 Documentar arquitectura en `Docs/GestorDocumental/Documentos/VisualizacionDocumento/`.
- [x] 8.2 Documentar contrato frontend completo con ejemplos reales.
- [x] 8.3 Documentar reglas de consolidación TIF->PDF temporal y errores (`Docs/GestorDocumental/Documentos/VisualizacionDocumento/SCRUM-204-Reglas-Consolidacion-TIF-PDF-Errores.md`).

## 9. OpenSpec Flow

- [x] 9.1 Ejecutar `openspec.cmd validate scrum-204-implementacion-api-solicita-documento`.
- [ ] 9.2 Publicar flujo orquestado (`orchestrate:publish`) con tasks completos.
- [ ] 9.3 Cerrar con `orchestrate:archive` tras merge multi-repo.
