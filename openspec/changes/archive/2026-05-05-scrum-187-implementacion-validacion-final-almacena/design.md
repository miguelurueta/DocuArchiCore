## Context

- Jira issue key: SCRUM-187
- Jira summary: IMPLEMENTACION-VALIDACION-FINAL-ALMACENAMIENTO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-187

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE PROMPT 21 — Pruebas de Paridad VB Legacy vs C# (FASE CRÍTICA — VALIDACIÓN FINAL DE EQUIVALENCIA FUNCIONAL) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actúa como Arquitecto Master Backend .NET experto en: QA técnico enterprise pruebas de regresión legacy pruebas de paridad funcional VB → C# sistemas documentales críticos automatización de pruebas Testcontainers / MySQL / Docker validación de consistencia DB + FileSystem + XML auditoría técnica de migraciones observabilidad y trazabilidad ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Construir una suite de pruebas de paridad funcional entre: Función legacy VB: Almacenamiento
Nueva implementación C#: Storage Engine El objetivo es demostrar con evidencia técnica que el nuevo Storage Engine produce resultados equivalentes o superiores al flujo legacy. Debe validar: - Base de datos
- Consecutivos
- Gabinete dinámico
- Inventario documental
- Expediente / unidad
- Índice electrónico
- Logdocuarchi
- FileSystem
- XML FXL
- XML índice expediente
- Naming DIG / FXL
- Rutas físicas
- Metadata física
- Preindex
- Validaciones obligatorias ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTEXTO Ya existen o deben existir los prompts correctivos: PROMPT 11 — Metadata Gabinete + Campos Obligatorios
PROMPT 12 — Preindex
PROMPT 13 — Naming DIG/FXL
PROMPT 14 — Ruta física SYSTEM1RUT
PROMPT 15 — Opciones legacy system1
PROMPT 16 — Tamaño + páginas reales
PROMPT 17 — Inventario documental completo
PROMPT 18 — Expediente/unidad legacy-compatible
PROMPT 19 — XML índice expediente
PROMPT 20 — Logdocuarchi workflow Este prompt valida el cierre funcional de todos ellos. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UBICACIÓN ESPERADA Proyecto de pruebas MiApp.Tests/StorageEngine/Parity/ Archivos esperados: StorageEngineParityTestSuite.cs
StorageEngineParityFixture.cs
StorageEngineLegacyExpectedBuilder.cs
StorageEngineActualResultReader.cs
StorageEngineParityAssert.cs
StorageEngineTestDataSeeder.cs
StorageEngineFileSystemFixture.cs
StorageEngineXmlAssert.cs
StorageEngineDatabaseAssert.cs Documentación Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/ Archivos esperados: SCRUM-[ID]-Pruebas-Paridad-VB-CSharp.md
SCRUM-[ID]-Matriz-Paridad-Funcional.md
SCRUM-[ID]-Evidencia-Paridad.md
SCRUM-[ID]-Brechas-Residuales.md
SCRUM-[ID]-Runbook-Pruebas-Paridad.md
SCRUM-[ID]-Metadata.md ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ESTRATEGIA DE PRUEBAS Usar estrategia de comparación: Given mismo input funcional
When se ejecuta Storage Engine C#
Then la salida debe ser equivalente al comportamiento esperado legacy VB No es obligatorio ejecutar VB en runtime si no está disponible. Sí es obligatorio construir expected legacy snapshots basados en: - función VB documentada
- matriz comparativa
- datos reales legacy
- fixtures controlados ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ TIPOS DE PRUEBA OBLIGATORIOS 1. Unitarias Validan componentes individuales: - metadata gabinete
- preindex
- naming
- ruta física
- opciones system1
- tamaño/páginas
- inventario builder
- expediente/unidad
- XML índice
- logdocuarchi 2. Integración Validan: - MySQL real
- Testcontainers
- transacciones
- rollback
- commit
- consultas reales
- inserts reales 3. E2E Validan flujo completo: Controller → UseCase → Orchestrator → DB → FS → XML 4. Concurrencia Validan: - dos almacenamientos simultáneos
- no duplicidad de proxid
- no colisión de carpeta
- no colisión de orden índice
- locks FOR UPDATE 5. Regresión Legacy Comparan resultado C# contra expected VB. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ESCENARIOS FUNCIONALES OBLIGATORIOS Escenario 1 — Almacenamiento simple sin inventario/TRD/unidad Debe validar: - system1.proxid
- system1.NUMCARP
- system1.NUMPAG_CARP
- tabla gabinete
- archivo DIG
- XML FXL Escenario 2 — Batch con preindex TXT Debe validar: - lectura preindex
- valores integrados a campos
- orden de campos
- campos obligatorios
- inserción gabinete con valores preindex Escenario 3 — Batch con preindex XMLS Debe validar: - lectura XMLS
- normalización de valores
- integración en pipeline
- persistencia correcta Escenario 4 — Inventario documental activo Debe validar: - registro_producion_documental
- columnas legacy
- IdRegistroProduccionDocumental
- Tamano
- Formato
- NumeroFolios
- SegundoNombreDocumento Escenario 5 — TRD activa Debe validar: - ID_AREA_DEPARTAMENTO
- ID_SERIE_DOCUMENTO
- SERIE_DOCUMENTO
- ID_SUBSERIE_DOCUMENTO
- SUBSERIE_DOCUMENTO
- ID_TIPO_DOCUMENTO
- DESCRIPCION_TIPO_DOCUMENTO Escenario 6 — Expediente electrónico Debe validar: - expediente_archivo.NUMERO_ELECTRONICO_CONTENIDO
- ra_cert_indice_expediente
- orden documento
- página inicial
- página final
- XML índice expediente Escenario 7 — Unidad de conservación digitalizada Debe validar: - unidad_conservacion.NUMERO_DIGITALIZADO_CONTENIDO
- ID_TIPO_UNIDAD_DOCUMENTAL = 1 Escenario 8 — Unidad de conservación electrónica Debe validar: - unidad_conservacion.NUMERO_ELECTRONICO_CONTENIDO
- ID_TIPO_UNIDAD_DOCUMENTAL = 1 Escenario 9 — Workflow activo Debe validar: - logdocuarchi
- IP_TRANS
- RUT_DOCU
- CAMPOS
- ID_TAREA_WF
- ID_RUTA_WF
- TIPOLOGIA_DOCUMENTAL Escenario 10 — Fallo FileSystem post-commit Debe validar: - compensación ejecutada
- archivos temporales eliminados
- estado de inconsistencia registrado
- DB no queda corrupta Escenario 11 — Fallo XML índice expediente Debe validar: - falla controlada
- no corrupción del XML original
- `.tmp` eliminado o controlado
- incidencia registrada Escenario 12 — Concurrencia Ejecutar al menos: 2 tareas simultáneas
5 tareas simultáneas si el ambiente lo permite Validar: - IDs únicos
- system1 consistente
- no deadlocks no controlados
- rollback correcto si una falla ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ MATRIZ DE PARIDAD OBLIGATORIA Crear matriz con columnas: Comportamiento Legacy
Input de prueba
Expected Legacy
Resultado C#
Estado
Diferencia
Prompt que cubre
Evidencia Estados válidos: CUMPLE
CUMPLE CON MEJORA
NO CUMPLE
NO APLICA
PENDIENTE DECISIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ASSERTS DE BASE DE DATOS Validar tablas: system1
disco_detalle
tabla dinámica gabinete
registro_producion_documental
expediente_archivo
unidad_conservacion
ra_cert_indice_expediente
logdocuarchi Validar especialmente: proxid
numcarp
NUMPAG_CARP
ID
DISC
PAG
DBT
IDEX
USER
DATE1
TIME1
ID_INVENTARIO_DOCUMENTAL
ID_USUARIO_GESTION
ID_EXPEDIENTE
ID_UNIDAD_CONSERVACION
ID_TIPO_UNIDAD_DOCUMENTAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ASSERTS DE FILESYSTEM Validar: ruta legacy real
carpeta gabinete + disco
carpeta legacy numcarp
archivo DIG
archivo FXL
no archivos temporales huérfanos
no sobrescritura accidental Ejemplo: {RutaAlmacenamiento}/{Gabinete}{Disco}/{Carpeta}/DIG00001.pdf
{RutaAlmacenamiento}/{Gabinete}{Disco}/{Carpeta}/FXL00001.xml ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ASSERTS XML FXL Validar atributos: ID
DISC
PAG
DBT
IDEX
USER
DATE1
TIME1
campos dinámicos
ID_AREA
ID_SERIE
ID_SUB_SERIE
ID_TIPODOCUMENTO
ID_USUARIO_GESTION
ID_EXPEDIENTE
ID_TIPO_EXPEDIENTE
ID_UNIDAD_CONSERVACION
ID_TIPO_UNIDAD_CONSERVACION
ID_TIPO_UNIDAD_DOCUMENTAL
ID_CLASE_DOCUMENTO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ASSERTS XML ÍNDICE EXPEDIENTE Validar nodo: DocumentoIndizado Campos esperados: Id
NombreDocumento
TipologiaDocumental
FechaDeclaracionDocumento
FechaIncorporacionDocumento
ValorHuella
FuncionResumen
OrdenDocumentoExpediente
PaginaInicial
PaginaFinal
Formato
Tamano
RutaDocumento
NumeroFolios
SegundoNombre ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ASSERTS DE INVENTARIO Validar columnas legacy: ID_USUARIO_GESTION
FECHA_DOCUMENTO
ID_AREA_DEPARTAMENTO
ID_SERIE_DOCUMENTO
SERIE_DOCUMENTO
ID_SUBSERIE_DOCUMENTO
SUBSERIE_DOCUMENTO
ID_TIPO_DOCUMENTO
DESCRIPCION_TIPO_DOCUMENTO
FULTEXT_DOCUMENTO
ID_DOCUMENTO_DOCUARCHI_ALMACEN
ESTADO_DOCUMENTO_ARCHIVO
NOMBRE_GABINETE
NUMERO_FOLIOS
EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE
EXPEDIENTE
ID_TIPO_EXPEDIENTE
ID_TIPO_UNIDAD_CONSERVACION
ID_UNIDAD_CONSERVACION
ID_CLASE_DOCUMENTO
CLASEDOCUMENTO
FECHA_ELABORACION
UNIDADCONSERVA
NOMBRE_AREA_DEPARTAMENTO
ID_TIPO_UNIDAD_DOCUMENTAL
ID_EMPRESA_DOCUMENTO
RADICADO_DOCUMENTO
SEGUNDO_NOMBRE_DOCUMENTO
DOCUMENTO_PRODUCION_DOCUMENTAL
TAMANO
FORMATO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ASSERTS DE LOGDOCUARCHI Validar columnas: id_tran
desc_op
USER_OPER
DATE_TRANS
RUT_DOCU
MODULO_REGISTRO
GABINETE
CAMPOS
IP_TRANS
HORA_REGISTRO
RADICADO
ID_TAREA_WF
ID_RUTA_WF
USER_PROPIETARIO
TIPOLOGIA_DOCUMENTAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ TEST FIXTURES OBLIGATORIOS Crear fixtures para: Gabinete sin opciones
Gabinete con inventario
Gabinete con TRD
Gabinete con unidad
Gabinete con expediente electrónico
Gabinete workflow
Preindex TXT
Preindex XMLS
PDF multipágina
Imagen simple
TIFF si aplica ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN TÉCNICA Usar: xUnit
FluentAssertions
Moq/NSubstitute
Testcontainers
MySQL Docker
TemporaryDirectory fixture No usar: Jest
pruebas manuales únicamente
base de producción
rutas reales no aisladas ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ FEATURE FLAG Las pruebas E2E deben poder ejecutarse con: StorageEngineV2 = ON Y opcionalmente comparar contra adapter legacy si existe: StorageEngineLegacyAdapter = ON/OFF ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ EVIDENCIA DE PRUEBAS Generar evidencia: Test run summary
Matriz de paridad
Capturas o dumps controlados DB
Listado de archivos generados
XML antes/después
Logs con requestId No incluir: datos sensibles
documentos reales de producción
rutas productivas completas ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBSERVABILIDAD EN PRUEBAS Cada prueba E2E debe capturar: requestId
idAlmacen
idRegistroProduccion
gabinete
estado final
duración
fase fallida si aplica ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN Existe suite de paridad. Existen fixtures controlados. Pruebas unitarias completas. Pruebas de integración completas. Pruebas E2E completas. Pruebas de concurrencia. Matriz de paridad actualizada. Todos los comportamientos bloqueantes están en CUMPLE. No quedan placeholders. No quedan diferencias no justificadas. Toda diferencia tiene decisión arquitectónica. Feature flag validado. Evidencia documentada. Resultado listo para recomendación Go/No-Go. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RESTRICCIONES No usar base productiva. No usar documentos reales sensibles. No modificar función legacy. No ocultar diferencias. No marcar como CUMPLE sin evidencia. No ejecutar FileSystem en rutas productivas. No omitir escenarios de rollback. No omitir escenarios de concurrencia. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN ENTERPRISE Ruta: Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/ Crear: SCRUM-[ID]-Pruebas-Paridad-VB-CSharp.md
SCRUM-[ID]-Matriz-Paridad-Funcional.md
SCRUM-[ID]-Evidencia-Paridad.md
SCRUM-[ID]-Brechas-Residuales.md
SCRUM-[ID]-Runbook-Pruebas-Paridad.md
SCRUM-[ID]-Go-NoGo-Produccion.md
SCRUM-[ID]-Metadata.md Debe incluir: - metodología de comparación
- fixtures usados
- escenarios ejecutados
- resultados por escenario
- diferencias encontradas
- decisiones arquitectónicas
- riesgos residuales
- recomendación Go/No-Go ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INSTRUCCIÓN FINAL Implementar: StorageEngineParityTestSuite
StorageEngineParityFixture
StorageEngineLegacyExpectedBuilder
StorageEngineActualResultReader
StorageEngineParityAssert
StorageEngineTestDataSeeder
StorageEngineFileSystemFixture
StorageEngineXmlAssert
StorageEngineDatabaseAssert
Pruebas unitarias
Pruebas integración
Pruebas E2E
Pruebas concurrencia
Documentación de evidencia
Matriz Go/No-Go El resultado debe permitir tomar una decisión formal: Storage Engine V2 puede reemplazar función VB Almacenamiento: SÍ / NO

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-187-implementacion-validacion-final-almacena.