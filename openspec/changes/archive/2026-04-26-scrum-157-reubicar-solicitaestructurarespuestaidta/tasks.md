## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-157 y confirmar alcance.
- [x] 1.2 Confirmar rutas origen/destino (Controller/Service/Repository) y que NO hay cambios funcionales.
- [x] 1.3 Identificar consumidores (usings/DI/tests) del namespace actual.

## 2. Specs

- [x] 2.1 Completar specs/jira-scrum-157/spec.md con requisitos finales de reubicación (sin cambio funcional).
- [x] 2.2 Incluir referencia explicita a openspec/context/OPSXJ_BACKEND_RULES.md.
- [x] 2.3 Verificar escenarios testables: build compila + pruebas focales del flujo.

## 3. Application

- [x] 3.1 Mover Controller a `DocuArchi.Api/Controllers/GestionCorrespondencia/GestionRespuesta/` y actualizar `namespace` (mantener `[Route]` y `[HttpGet]`).
- [x] 3.2 Mover Service a `MiApp.Services/Service/GestionCorrespondencia/GestionRespuesta/` y actualizar `namespace` + `using` del repository.
- [x] 3.3 Mover Repository a `MiApp.Repository/Repositorio/GestionCorrespondencia/GestionRespuesta/` y actualizar `namespace`.
- [x] 3.4 Actualizar `using` en consumidores (controllers/tests/services) y en `Program.cs`/DI si aplica.
- [x] 3.5 Buscar y eliminar referencias al namespace anterior (no deben quedar `using` obsoletos).

## 4. Test

- [x] 4.1 Ejecutar `dotnet build` y adjuntar evidencia (salida/resultado) en el PR.
- [x] 4.2 Ejecutar `dotnet test` (o al menos pruebas focales del flujo) y adjuntar evidencia.
- [x] 4.3 Validar artefactos OpenSpec del change (si existe comando/proceso en el repo) y archivar cuando corresponda.
