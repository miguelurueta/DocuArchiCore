## Context

- Jira issue key: SCRUM-191
- Jira summary: IMPLEMENTACION-PARIDAD-RUTA-ARCHIVOS
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-191

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE PROMPT 23B — Cierre Paridad Física/XML Legacy (FASE CRÍTICA — CONSISTENCIA DOCUMENTAL LEGACY) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actúa como Arquitecto Master Backend .NET experto en: almacenamiento documental legacy DocuArchi convenciones de naming legacy DIG / FXL / adjuntos escritura segura en FileSystem generación XML legacy-compatible índices de expediente electrónico compensación post-commit path hardening Clean Architecture migración VB → C# con paridad funcional ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Cerrar completamente las brechas de paridad física/XML del Storage Engine, garantizando interoperabilidad total con el ecosistema legacy DocuArchi. Debe quedar completamente alineado en: ✔ Ruta física final legacy real (SYSTEM1RUT) ✔ Naming DIG / FXL / adjuntos ✔ XML documental legacy (Gabinetes/Gabinete) ✔ XML índice expediente (DocumentoIndizado) ✔ Consistencia entre DB, FileSystem y XML ✔ Compensación robusta ante fallos ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REFERENCIA LEGACY OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ El código legacy consolidado es la fuente de verdad funcional . Ruta: D:\imagenesda\GestorDocumental\promp\CORE-API\Almacenamiento\funcion-almacena-consolidad.txt Archivo usado en este análisis: funcion-almacena-consolidad(2).txt :contentReference[oaicite:0]{index=0} REGLA DE PARIDAD Si existe discrepancia entre:
- prompts
- documentación
- interpretación
- implementación C#

DEBE prevalecer el comportamiento del código legacy. FUNCIONALIDADES LEGACY OBLIGATORIAS Debe validarse contra este archivo: - Consulta_Ruta_Almacenamiento (SYSTEM1RUT)
- Construcción DIG + ceros
- Adjuntos .0000, .0001
- Generando_Archivo_Dat_Xml (FXL)
- Estructura XML Gabinetes/Gabinete
- Actualiza_archivo_xml_indice_expediente
- Ruta física final
- Copia_Imagen_Almacenada_local Toda desviación debe documentarse como: Cumple con desviación aprobada ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTEXTO DE BRECHA ACTUAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Brechas confirmadas: uso de Path.GetTempPath() como destino final ❌ naming de adjuntos _1 , _2 ❌ XML StorageDocumento ❌ XML índice expediente incompleto ❌ rutas físicas inconsistentes ❌ posible incompatibilidad con visor legacy ❌ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UBICACIÓN ESPERADA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Services MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/ Builders/StoragePlanBuilder.cs Physical/StoragePathResolver.cs Physical/StoragePhysicalPathService.cs Physical/StorageFileWriter.cs Physical/StorageXmlWriter.cs Physical/StoragePhysicalPhaseExecutor.cs Physical/StorageCompensationManager.cs ExpedienteXml/ExpedienteIndiceXmlService.cs ExpedienteXml/ExpedienteIndiceXmlWriter.cs Naming/StorageNamingService.cs Repository MiApp.Repository/Repositorio/GestorDocumental/AlmacenamientoDocumental/ StorageRoute/StorageRouteRepository.cs ExpedienteXml/ExpedienteIndiceXmlRepository.cs API DocuArchi.Api/Program.cs ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN REQUERIDA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 1. Ruta física legacy Debe construirse desde: SYSTEM1RUT.ruta_gabi
+ gabinete
+ disco
+ carpeta legacy Reglas: eliminar %TEMP% no hardcodear rutas validar root permitido prevenir path traversal 2. Separación obligatoria NamingService → nombres
PhysicalPathService → rutas
FileWriter → copia
XmlWriter → XML
CompensationManager → rollback físico 3. Naming principal DIG########.ext Obligatorio: prefijo DIG padding legacy extensión desde DB determinístico 4. Naming adjuntos DIG########.0000.ext
DIG########.0001.ext Reglas: mantener extensión original no sobrescribir orden determinístico 5. XML documental FXL Debe replicar: <Gabinetes>
  <Gabinete ... />
</Gabinetes> Reglas: atributos exactos legacy casing exacto no omitir campos legacy 6. XML índice expediente Debe: insertar DocumentoIndizado preservar estructura usar .tmp → reemplazo seguro no corromper XML 7. Ruta persistida vs física Ruta física → absoluta
Ruta persistida → relativa o formato legacy Debe ser consistente y derivable. 8. Orden de ejecución 1. DB commit
2. FileSystem
3. XML FXL
4. XML índice expediente
5. Compensation si falla 9. Política de colisión Si archivo existe:
→ error controlado
→ no sobrescribir 10. Manejo IO Capturar: IOException
UnauthorizedAccessException
PathTooLongException Con: logging requestId compensación ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS CRÍTICAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ NO usar rutas temporales finales NO sobrescribir archivos NO cambiar naming legacy NO cambiar XML sin aprobación NO aceptar rutas externas NO duplicar lógica de rutas NO usar valores hardcode ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Unitarias ruta desde SYSTEM1RUT bloqueo path traversal naming DIG naming adjuntos XML correcto error archivo existente Integración copia real XML FXL real XML índice real rollback/compensación Regresión Comparar contra legacy: Ruta_Alamce_Image
DIG########
FXL########
XML Gabinetes/Gabinete
Adjuntos .0000
XML índice expediente ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBSERVABILIDAD ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Logs: requestId
idAlmacen
gabinete
disco
carpeta
fase
xml generado
archivos copiados
compensación
duración No loguear: contenido
fulltext
datos sensibles ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actualizar: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\GestorDocumental\AlmacenamientoDocumental\ Ticket: SCRUM-189 Archivos: SCRUM-189-Arquitectura-Cierre-Paridad-Fisica-Xml.md
SCRUM-189-Implementacion-Detallada-Paridad-Fisica-Xml.md
SCRUM-189-Pruebas-Paridad-Fisica-Xml.md
SCRUM-189-Observabilidad-Paridad-Fisica-Xml.md
SCRUM-189-Regresion-Legacy-Paridad-Fisica-Xml.md
SCRUM-189-Metadata.md ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ✔ Ruta legacy real ✔ Naming DIG correcto ✔ Adjuntos legacy ✔ XML FXL correcto ✔ XML índice correcto ✔ Sin sobrescritura ✔ Path seguro ✔ Compensación funcional ✔ Pruebas en verde ✔ Documentación SCRUM-189 actualizada ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RESTRICCIONES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ No modificar lógica de negocio No cambiar API No introducir placeholders No usar rutas temporales ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INSTRUCCIÓN FINAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Implementar la paridad física/XML completa usando como fuente de verdad el código legacy. Actualizar la documentación de arquitectura en SCRUM-189 en la ruta indicada. Dejar el sistema listo para validación final y decisión Go/No-Go.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-191-implementacion-paridad-ruta-archivos.