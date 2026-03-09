## 1. Discovery

- [ ] 1.1 Confirmar definición de `ValidationError` y `ValidationHelper` disponible.
- [ ] 1.2 Confirmar fuente de `DetallePlantillaRadicado` para la validación.
- [ ] 1.3 Confirmar política de salida para `Success=true` con `Data=null` y `Message="Sin resultados"`.

## 2. Specs

- [ ] 2.1 Completar spec con obligatoriedad de campos fijos y dinámicos.
- [ ] 2.2 Formalizar contrato `AppResponses<List<ValidationError>>`.
- [ ] 2.3 Formalizar reglas de excepción y caso sin resultados.

## 3. Service/Repository

- [ ] 3.1 Crear `ValidaCamposObligatoriosService` + interfaz en mismo archivo.
- [ ] 3.2 Centralizar constante de campos fijos obligatorios reusable.
- [ ] 3.3 Implementar método que evalúe request + dinámicos (`DetallePlantillaRadicado`) sin leer tabla de plantilla.
- [ ] 3.4 Integrar servicio en flujo de radicación entrante.
- [ ] 3.5 Garantizar consultas parametrizadas en repositorio.

## 4. API/DI

- [ ] 4.1 Registrar interfaz e implementación en `Program.cs` de `DocuArchi.Api`.
- [ ] 4.2 Documentar con comentarios XML propósito/parámetros/retorno.

## 5. Test

- [ ] 5.1 Unit test: caso success con datos válidos.
- [ ] 5.2 Unit test: caso sin resultados.
- [ ] 5.3 Unit test: caso excepción simulada.
- [ ] 5.4 Integration tests MySQL con Testcontainers/Docker.
- [ ] 5.5 Ejecutar y anexar evidencia `dotnet build` y `dotnet test`.

## 6. Documentation

- [ ] 6.1 Actualizar docs técnicas en `DocuArchiCore/Docs/Radicacion/Tramite`.
- [ ] 6.2 Incluir diagramas: casos de uso, clases, secuencia y estado.
- [ ] 6.3 Incluir documentación frontend del DTO y respuestas.

## 7. File Map (target)

- [ ] 7.1 `MiApp.Services/Service/Radicacion/Tramite/ValidaCamposObligatoriosService.cs` (interfaz + clase + XML docs).
- [ ] 7.2 `MiApp.Services/Service/Radicacion/Tramite/RegistrarRadicacionEntranteService.cs` (invocación del nuevo servicio).
- [ ] 7.3 `MiApp.Repository/Repositorio/Radicador/PlantillaRadicado/DetallePlantillaRadicadoR.cs` (solo si se requiere ajuste de retorno/sin resultados).
- [ ] 7.4 `DocuArchi.Api/Program.cs` (registro DI de interfaz/implementación).
- [ ] 7.5 `DocuArchiCore/tests/TramiteDiasVencimiento.Tests/ValidaCamposObligatoriosServiceTests.cs` (unit tests).
- [ ] 7.6 `DocuArchiCore/tests/TramiteDiasVencimiento.Tests/ValidaCamposObligatoriosServiceIntegrationTests.cs` (integration tests con Docker/Testcontainers).
- [ ] 7.7 `DocuArchiCore/Docs/Radicacion/Tramite/SCRUM-44/*` (API frontend + diagramas).
