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
- [ ] 4.2 Implementar consulta de añadidos por `DBT` si no existe repositorio reusable directo.
- [ ] 4.3 Aplicar sanitización de `nombreGabinete` y validación anti path traversal.

## 5. Consolidación TIF a PDF

- [x] 5.1 Detectar correctamente flujo TIF/multiimagen.
- [x] 5.2 Generar PDF temporal consolidado bajo root temporal controlado.
- [x] 5.3 Retornar `contentType`, `fileName`, `urlTemporal`, `expiresAt`, `origen`.
- [ ] 5.4 Definir estrategia de cleanup para temporales generados.

## 6. DTOs y DI

- [x] 6.1 Crear/ajustar DTOs request/response en `MiApp.DTOs`.
- [x] 6.2 Registrar interfaces/implementaciones en `Program.cs` (Api).

## 7. Pruebas

- [ ] 7.1 Unit test: PDF original => URL temporal origen `ORIGINAL`.
- [ ] 7.2 Unit test: TIF/multiimagen => PDF temporal origen `TIF_CONSOLIDADO`.
- [ ] 7.3 Unit test: archivo inexistente y validaciones de entrada.
- [ ] 7.4 Unit/integration test: token expirado en download.
- [ ] 7.5 Ejecutar `dotnet test` en suites impactadas y registrar evidencia.

## 8. Documentación

- [x] 8.1 Documentar arquitectura en `Docs/GestorDocumental/Documentos/VisualizacionDocumento/`.
- [x] 8.2 Documentar contrato frontend completo con ejemplos reales.
- [ ] 8.3 Documentar reglas de consolidación TIF->PDF temporal y errores.

## 9. OpenSpec Flow

- [x] 9.1 Ejecutar `openspec.cmd validate scrum-204-implementacion-api-solicita-documento`.
- [ ] 9.2 Publicar flujo orquestado (`orchestrate:publish`) con tasks completos.
- [ ] 9.3 Cerrar con `orchestrate:archive` tras merge multi-repo.
