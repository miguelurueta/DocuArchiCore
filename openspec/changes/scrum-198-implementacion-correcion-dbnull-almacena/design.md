## Context

- Jira issue key: SCRUM-198
- Jira summary: IMPLEMENTACION-CORRECION-DBNULL-ALMACENAMIENTO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-198
- Error observado: `InsertAsync error: The member SERIE_DOCUMENTO of type System.DBNull cannot be used as a parameter value`

## Problem

El flujo transaccional de almacenamiento falla al insertar inventario porque se están enviando valores `DBNull.Value` en estructuras que Dapper mapea como parámetros de objeto.  
Para el flujo actual, los campos opcionales deben viajar como `null` (C#) y no como `DBNull.Value` en capas de aplicación/repositorio.

## Scope

En alcance:
- Corregir mapeo de campos opcionales en `InventarioDocumentalRepository` para usar `null`.
- Revisar `InsertBeginTrandAsync` en `DapperCrudEngine` para asegurar manejo consistente de `null` y mensajes de error claros.
- Ejecutar búsqueda dirigida de usos de `DBNull` en Storage Engine y documentar hallazgos.
- Actualizar tests y documentación técnica del módulo.

Fuera de alcance:
- Cambios de contrato HTTP.
- Nuevas reglas de negocio de validación.
- Refactor amplio del engine de persistencia.

## Architectural Decisions

1. Regla de capas:
- Dominio, servicios y repositorios trabajan con `null` semántico.
- `DBNull` solo es aceptable en frontera ADO.NET explícita y controlada.

2. Separación de responsabilidades:
- `ValidationPipeline` mantiene reglas funcionales.
- `Repository` solo traduce y persiste.

3. Corrección quirúrgica:
- Se reemplazan únicamente asignaciones que hoy inyectan `DBNull.Value` en payload de insert.
- No se alteran decisiones de negocio ni flujo transaccional.

## Technical Plan

1. Inventario:
- Ajustar diccionario/reglas del insert transaccional para campos opcionales (`SERIE_DOCUMENTO`, `ID_SERIE_DOCUMENTO`, `SUBSERIE_DOCUMENTO`, `ID_SUBSERIE_DOCUMENTO`, y equivalentes) usando `null`.

2. Dapper engine:
- Verificar que `InsertBeginTrandAsync` no transforme `null` a `DBNull` de forma manual.
- Normalizar mensaje de error para identificar operación transaccional y tabla afectada.

3. Búsqueda global:
- Buscar `DBNull.Value`, `System.DBNull`, `Convert.DBNull` en `MiApp.Repository`, `MiApp.Services`, `MiApp.Models` dentro de Storage Engine.
- Registrar qué usos quedan justificados y cuáles se corrigen.

4. Pruebas:
- Unitarias de repositorio para inserts con campos opcionales `null`.
- Prueba de regresión del flujo transaccional de almacenamiento.

## Risks and Mitigations

- Riesgo: cambiar `DBNull` por `null` en un campo realmente obligatorio en DB.
  Mitigación: cubrir con pruebas de inserción y revisar columnas NOT NULL involucradas.

- Riesgo: mensajes de error poco trazables en transacción.
  Mitigación: ajustar mensaje en `DapperCrudEngine` para identificar método/tabla.

## Validation

- Ejecutar `openspec validate scrum-198-implementacion-correcion-dbnull-almacena`.
- Ejecutar pruebas de almacenamiento afectadas por la inserción transaccional.
