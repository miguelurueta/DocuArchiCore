## 1. Discovery y Paridad Legacy

- [x] 1.1 Revisar issue Jira SCRUM-190 y confirmar que el objetivo es cierre runtime del orquestador.
- [x] 1.2 Revisar fuente legacy obligatoria: `D:\imagenesda\GestorDocumental\promp\CORE-API\Almacenamiento\funcion-almacena-consolidad.txt`.
- [x] 1.3 Identificar brecha inicial: orquestador con stub (`Task.CompletedTask`, `IdAlmacen=0`, `Pending`).
- [x] 1.4 Construir matriz de paridad de flujo (orden de fases, condiciones de corte, errores) VB vs C#.

## 2. Spec y Diseño

- [x] 2.1 Definir en `design.md` la referencia legacy como fuente de verdad funcional.
- [x] 2.2 Definir flujo obligatorio: Validation -> Metadata -> Transaction -> Physical -> Resultado.
- [x] 2.3 Incorporar en `spec.md` requisitos verificables de paridad contra legacy (orden y cortes).
- [x] 2.4 Corregir trazabilidad documental del ticket (SCRUM-190) en artefactos donde aplique.

## 3. Implementación Runtime

- [x] 3.1 Eliminar stub en `DocumentStorageOrchestrator`.
- [x] 3.2 Conectar dependencias reales de pipeline, metadata, transacción y fase física.
- [x] 3.3 Implementar manejo centralizado de excepciones (`StorageException` y no tipadas).
- [x] 3.4 Garantizar resultado final con `IdAlmacen > 0` y estado no `Pending`.

## 4. Validación Técnica

- [x] 4.1 Compilar `MiApp.Services` en Release sin errores.
- [x] 4.2 Ejecutar pruebas unitarias focales del orquestador (o registrar brecha de pruebas automatizadas).
- [x] 4.3 Validar en evidencia que, si validation falla, no avanza a transaction ni physical.

## 5. Documentación Técnica y Arquitectura

- [x] 5.1 Crear documentos técnicos SCRUM-190 en `Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/`.
- [x] 5.2 Añadir sección explícita "Comparación contra legacy fuente" con hallazgos de cumplimiento y brechas.
- [x] 5.3 Publicar evidencia de compilación/pruebas y decisión de cierre.

## 6. Cierre de Flujo OpenSpec

- [x] 6.1 Commit/push en repos impactados (`MiApp.Services`, `DocuArchiCore`).
- [x] 6.2 Ejecutar `opsxj:orchestrate:publish -- SCRUM-190 -NonInteractive`.
- [x] 6.3 Verificar estado final en Jira y trazabilidad multi-repo.
