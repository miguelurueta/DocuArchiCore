# PROMPT ARQUITECTÓNICO — HARDENING POST-PARIDAD

# PROMPT 24 — Corrección de acoplamiento Services/Repository e infraestructura en contratos

# (DEUDA TÉCNICA CONTROLADA — NO BLOQUEANTE PARA PARIDAD)



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## ESTADO DEL PROMPT

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



Este prompt NO debe ejecutarse antes del cierre de paridad funcional del Storage Engine.



Debe registrarse como:



```txt

Deuda técnica controlada

Post-paridad

Post-Go/No-Go

```



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## ROL ESPERADO

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



Actúa como Arquitecto Master Backend .NET experto en:



- Clean Architecture

- inversión de dependencias

- puertos y adaptadores

- arquitectura hexagonal

- desacoplamiento progresivo

- orquestación transaccional

- Dapper / DapperCrudEngine

- migraciones legacy VB → C#

- hardening arquitectónico sin regresiones



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## OBJETIVO

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



Reducir deuda estructural del Storage Engine sin alterar comportamiento funcional.



Objetivos específicos:



```txt

1. Eliminar dependencia directa MiApp.Services → MiApp.Repository.

2. Retirar IDbConnection / IDbTransaction de contratos de aplicación.

3. Encapsular infraestructura transaccional en una capa adecuada.

4. Mantener compatibilidad transaccional existente.

5. No afectar paridad legacy ya validada.

```



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## JUSTIFICACIÓN COMO DEUDA TÉCNICA

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



Actualmente existe deuda estructural porque:



```txt

- MiApp.Services referencia directamente MiApp.Repository.

- Algunos contratos de servicios exponen tipos ADO.NET.

- Parte de la coordinación transaccional está mezclada con lógica de aplicación.

```



Sin embargo:



```txt

NO debe corregirse antes de cerrar paridad funcional.

```



Riesgo de ejecutarlo antes:



```txt

- romper FOR UPDATE

- romper transacciones manuales

- romper compensación

- romper pruebas E2E

- alterar comportamiento legacy

```



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## ORDEN RECOMENDADO DE EJECUCIÓN

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



Ejecutar SOLO después de:



```txt

PROMPT 23A — Runtime real

PROMPT 23B — Paridad física/XML

PROMPT 23D — Compensación DB post-fallo físico

PROMPT 23E — Capacidad disco

PROMPT 23C — Auditoría Integral + Go/No-Go

```



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## ALCANCE

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



Refactor incremental:



```txt

1. Introducir puertos de aplicación en capa neutral.

2. Crear adaptadores en Repository.

3. Migrar contratos que hoy exponen IDbConnection / IDbTransaction.

4. Mantener adapter puente para StorageTransactionCoordinator.

5. Eliminar ProjectReference directo Services → Repository.

6. Corregir auditoría hardcodeada.

```



Contratos a revisar:



```txt

IStorageIdentityAllocator

IExpedienteUnidadLegacyService

IWorkflowStorageLogService

IStorageDbCompensationService

IStorageDiskCapacityPolicy

```



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## ESTRATEGIA POR FASES

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



### Fase 1 — Diagnóstico



Inventariar:



```txt

- referencias Services → Repository

- contratos con IDbConnection

- contratos con IDbTransaction

- servicios que usan Dapper directo

- servicios que deberían consumir puertos

```



---



### Fase 2 — Puertos



Crear interfaces neutrales en capa adecuada:



```txt

MiApp.Application.Abstractions

```



o carpeta equivalente aprobada.



Ejemplos:



```txt

IStorageTransactionPort

IStorageIdentityPort

IStorageDiskPort

IStorageGabinetePort

IStorageInventarioPort

IStorageWorkflowPort

```



---



### Fase 3 — Adaptadores



Implementar adaptadores en Repository:



```txt

MiApp.Repository.Adapters

```



Los adaptadores pueden usar:



```txt

DapperCrudEngine por defecto

Dapper directo cuando exista necesidad transaccional crítica

```



---



### Fase 4 — Contratos limpios



Remover de contratos de aplicación:



```txt

IDbConnection

IDbTransaction

MySqlConnection

MySqlTransaction

```



Los detalles transaccionales deben quedar encapsulados.



---



### Fase 5 — Validación



Ejecutar:



```txt

- unitarias

- integración

- E2E

- regresión legacy

```



antes de eliminar referencias.



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## REGLA DE ACCESO A DATOS

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



```txt

DapperCrudEngine es obligatorio por defecto.

```



Excepción permitida:



```txt

Dapper directo solo cuando exista:

- FOR UPDATE

- transacción explícita

- orden crítico de ejecución

- compensación idempotente

- control fino de concurrencia

```



Toda excepción debe documentarse.



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## REGLAS CRÍTICAS

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



```txt

- NO cambiar comportamiento funcional observable.

- NO alterar paridad VB validada.

- NO perder atomicidad.

- NO romper FOR UPDATE.

- NO romper compensación.

- NO romper pruebas existentes.

- NO eliminar referencia Services → Repository hasta tener adaptadores completos.

- NO ejecutar como parte del cierre de paridad.

```



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## AUDITORÍA HARDCODEADA

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



Corregir valores hardcodeados como:



```txt

Miguel

system

usuario técnico fijo

```



Regla:



```txt

usuario debe venir de StorageContext / claims / contexto autenticado

```



Si no existe contexto:



```txt

usar usuario técnico configurable

```



nunca literal hardcodeado.



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## PRUEBAS OBLIGATORIAS

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



### Compilación



```txt

MiApp.Services.csproj sin ProjectReference directo a MiApp.Repository

```



### Unitarias



```txt

- Orchestrator

- TransactionCoordinator

- adapters

- puertos

```



### Integración



```txt

- transacciones reales

- FOR UPDATE

- rollback

- compensation

```



### Regresión



```txt

- flujo simple

- inventario

- expediente

- workflow

- físico/XML

```



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## CRITERIOS DE ACEPTACIÓN

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



```txt

✔ MiApp.Services sin referencia directa a MiApp.Repository

✔ contratos de aplicación sin IDbConnection / IDbTransaction

✔ transacciones siguen funcionando

✔ FOR UPDATE validado

✔ compensación sigue funcionando

✔ pruebas E2E en verde

✔ paridad legacy intacta

✔ documentación actualizada

```



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## DOCUMENTACIÓN OBLIGATORIA

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



Registrar como deuda técnica en:



```txt

D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\GestorDocumental\AlmacenamientoDocumental\Arquitectura-Final\

```



Archivo sugerido:



```txt

SCRUM-189-Deuda-Tecnica-Hardening-Clean-Architecture.md

```



Debe incluir:



```txt

- descripción de la deuda

- riesgo actual

- por qué no bloquea paridad

- cuándo ejecutarla

- estrategia por fases

- riesgos de regresión

- criterios de salida

```



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## RESTRICCIONES

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



```txt

- NO ejecutar antes del Go/No-Go.

- NO incluir dentro de 23A, 23B, 23C, 23D o 23E.

- NO mezclar con correcciones funcionales.

- NO hacer big-bang refactor.

```



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## INSTRUCCIÓN FINAL

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



Registrar este refactor como deuda técnica controlada post-paridad.



No implementarlo hasta que el Storage Engine tenga paridad funcional validada, pruebas E2E en verde y decisión Go/No-Go emitida.
