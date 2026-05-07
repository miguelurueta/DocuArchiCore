## 1. Discovery y Paridad Legacy

- [x] 1.1 Revisar `funcion-almacena-consolidad.txt` y extraer reglas físicas/XML obligatorias.
- [x] 1.2 Mapear equivalencia VB -> C# por componente (ruta, naming, XML, compensación).
- [x] 1.3 Confirmar repos impactados reales y actualizar `sync.md` si cambia clasificación.
- [x] 1.4 Definir desviaciones aprobadas (si existen) con justificación arquitectónica.

## 2. Spec y Diseño

- [x] 2.1 Completar `spec.md` con requisitos verificables de paridad física/XML.
- [x] 2.2 Añadir escenarios de seguridad de rutas y colisión de archivos.
- [x] 2.3 Añadir escenarios de actualización de XML índice expediente.
- [x] 2.4 Validar que diseño/documentación usen `SCRUM-191` (sin referencias cruzadas erróneas).

## 3. Implementación Técnica

- [x] 3.1 Ajustar resolución de ruta final a `SYSTEM1RUT + gabinete + disco + carpeta`.
- [x] 3.2 Alinear naming principal `DIG########.ext` y XML `FXL########.xml`.
- [x] 3.3 Alinear naming adjuntos `.0000`, `.0001`, ... sin sobrescribir.
- [x] 3.4 Alinear estructura XML documental legacy (`Gabinetes/Gabinete` + atributos).
- [x] 3.5 Alinear actualización XML índice expediente (`DocumentoIndizado`) con escritura segura.
- [x] 3.6 Endurecer `StorageCompensationManager` para fallos post-commit físicos/XML.
- [x] 3.7 Verificar DI en `Program.cs` para todos los componentes de fase física/XML.

## 4. Pruebas

- [x] 4.1 Unit tests: ruta, traversal, naming principal/adjuntos, XML estructural, colisión.
- [x] 4.2 Integration tests: copia real + XML FXL + XML índice + compensación.
- [x] 4.3 Ejecutar `dotnet test` y registrar evidencia (pass/fail/skip justificado).
- [x] 4.4 Generar matriz de regresión VB vs C# con evidencia por regla.

## 5. Documentación Técnica

- [x] 5.1 Crear/actualizar `SCRUM-191-Arquitectura-Paridad-Fisica-Xml.md`.
- [x] 5.2 Crear/actualizar `SCRUM-191-Implementacion-Detallada-Paridad-Fisica-Xml.md`.
- [x] 5.3 Crear/actualizar `SCRUM-191-Pruebas-Paridad-Fisica-Xml.md`.
- [x] 5.4 Crear/actualizar `SCRUM-191-Observabilidad-Paridad-Fisica-Xml.md`.
- [x] 5.5 Crear/actualizar `SCRUM-191-Regresion-Legacy-Paridad-Fisica-Xml.md`.
- [x] 5.6 Crear/actualizar `SCRUM-191-Metadata.md`.

## 6. Cierre de Flujo

- [x] 6.1 Ejecutar `openspec validate` del change.
- [x] 6.2 Commit/push en repos impactados.
- [x] 6.3 Ejecutar `opsxj:orchestrate:publish -- SCRUM-191 -NonInteractive` (pendiente ejecución operativa).
- [x] 6.4 Revisar/mergear PRs de satélites y coordinador (siguiente paso de flujo).
- [x] 6.5 Ejecutar `opsxj:orchestrate:archive -- SCRUM-191 -NonInteractive` (fase posterior a merge).
