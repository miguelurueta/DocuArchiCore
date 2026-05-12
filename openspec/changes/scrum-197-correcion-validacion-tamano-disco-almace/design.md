## Context

- Jira issue key: SCRUM-197
- Jira summary: CORRECION-VALIDACION-TAMANO-DISCO-ALMACENAMIENTO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-197

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE PROMPT 23E — Corrección de cuota de disco legacy sin dependencia obligatoria de ESTADO_DISCO (FASE CRÍTICA — BLOQUEANTE FUNCIONAL) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actúa como Arquitecto Master Backend .NET experto en: DocuArchi legacy reglas de almacenamiento por disco system1 y disco_detalle control de capacidad y concurrencia transacciones MySQL con FOR UPDATE Dapper / QueryOptions DapperCrudEngine consistencia transaccional migración VB → C# con paridad funcional ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Corregir la validación de capacidad de disco del Storage Engine para eliminar la dependencia obligatoria de: ESTADO_DISCO y replicar exactamente la lógica legacy VB basada en: TAMDISC + NUMERO_IMAGENES Debe garantizar: ✔ Compatibilidad con producción actual ✔ Paridad exacta con VB ✔ Compatibilidad con ambientes que sí tengan ESTADO_DISCO ✔ Bloqueo antes del commit ✔ Consistencia bajo concurrencia ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REFERENCIA LEGACY OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Ruta del código legacy: D:\imagenesda\GestorDocumental\promp\CORE-API\Almacenamiento\funcion-almacena-consolidad.txt :contentReference[oaicite:0]{index=0} FUNCIÓN LEGACY DE REFERENCIA Function Numero_Imagenes(...) REGLA LEGACY CONFIRMADA - Consultar NUMERO_IMAGENES desde disco_detalle por disco + gabinete.
- Si no existe registro → error.
- Si NUMERO_IMAGENES es NULL → error:
  "El disco {disc} no esta sincronizado para alamcenar contacte a su administrador estado null"

- Si NUMERO_IMAGENES = 0 → error:
  "El disco {disc} no esta sincronizado para alamcenar contacte a su administrador"

- Si TAMDISC > 572523149 y NUMERO_IMAGENES > 80000:
    EstadoDisco = "SL"

- Si TAMDISC < 572523149 y NUMERO_IMAGENES > 7500:
    EstadoDisco = "SL"

- Si TAMDISC == 572523149:
    NO activar SL (paridad exacta legacy) MENSAJE LEGACY OBLIGATORIO "Disco {disc} Sobrepaso el limite de capacidad" ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTEXTO DEL PROBLEMA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actualmente el flujo falla con: Unknown column 'ESTADO_DISCO' in 'field list' porque el código C# asume que existe: disco_detalle.ESTADO_DISCO pero producción NO tiene esa columna. La lógica VB: NO leía ESTADO_DISCO.
La calculaba dinámicamente. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ALCANCE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Implementar validación de capacidad: 1. Leer system1:
   - disco
   - tamdisc
   - proxid
   - numcarp
   - NUMPAG_CARP

2. Leer disco_detalle:
   - NUMERO_IMAGENES
   - NUMPAG_CARP

3. Calcular estado legacy:
   - SL
   - OK

4. Bloquear operación si aplica.

5. Mantener compatibilidad opcional con ESTADO_DISCO si existe en otros ambientes. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UBICACIÓN ESPERADA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Services MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Disk/ Archivos: IStorageDiskCapacityPolicy.cs
StorageDiskCapacityPolicy.cs Models MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/ DiskCapacityValidationResult.cs Repository Extender: MiApp.Repository/Repositorio/GestorDocumental/AlmacenamientoDocumental/ Archivos relacionados: SystemStorageRepository.cs
StorageDiskQuotaRepository.cs
StorageIdentityAllocator.cs ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ACCESO A DATOS (REGLA) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DapperCrudEngine es obligatorio por defecto. Excepción permitida: Dapper directo cuando exista:
- FOR UPDATE
- transacción explícita
- control de concurrencia No mezclar ambos enfoques en la misma operación. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ MODELO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ public sealed class DiskCapacityValidationResult
{
    public bool Allowed { get; init; }

    public string EstadoDisco { get; init; } = "";

    public string Message { get; init; } = "";

    public int Disco { get; init; }

    public long TamDisc { get; init; }

    public int NumeroImagenes { get; init; }
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — StorageDiskCapacityPolicy ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Responsabilidad Replicar exactamente: Numero_Imagenes(...) Reglas - usar system1 + disco_detalle
- NO depender obligatoriamente de ESTADO_DISCO
- NO inventar umbrales
- NO reinterpretar lógica Flujo esperado if (numeroImagenes == null)
{
    throw new StorageTransactionException(
        $"El disco {disc} no esta sincronizado para alamcenar contacte a su administrador estado null");
}

if (numeroImagenes == 0)
{
    throw new StorageTransactionException(
        $"El disco {disc} no esta sincronizado para alamcenar contacte a su administrador");
}

if (tamdisc > 572523149 && numeroImagenes > 80000)
{
    estadoDisco = "SL";
}

if (tamdisc < 572523149 && numeroImagenes > 7500)
{
    estadoDisco = "SL";
}

if (estadoDisco == "SL")
{
    throw new StorageTransactionException(
        $"Disco {disc} Sobrepaso el limite de capacidad");
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ COMPATIBILIDAD OPCIONAL ESTADO_DISCO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Si el ambiente SÍ tiene columna: ESTADO_DISCO puede usarse como refuerzo adicional: Si EstadoDisco == "SL" → bloquear Pero: ESTADO_DISCO NO es fuente primaria. La fuente principal siempre debe ser: TAMDISC + NUMERO_IMAGENES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTEGRACIÓN EN TRANSACTION COORDINATOR ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Orden obligatorio: 1. SELECT system1 FOR UPDATE
2. SELECT disco_detalle FOR UPDATE
3. StorageDiskCapacityPolicy
4. Update system1
5. Insert gabinete Si policy falla: rollback inmediato ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONCURRENCIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Obligatorio: SELECT ... FOR UPDATE Objetivos: - evitar race conditions
- evitar sobreasignación de disco
- garantizar consistencia concurrente ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS CRÍTICAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ - NO requerir ESTADO_DISCO
- NO romper producción actual
- NO inventar umbrales
- NO reinterpretar lógica VB
- SIEMPRE validar antes del commit
- SIEMPRE usar lock transaccional
- SIEMPRE devolver mensajes homologables a legacy ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Unitarias Validar: - tamdisc grande + numero_imagenes > 80000 → bloquea
- tamdisc pequeño + numero_imagenes > 7500 → bloquea
- tamdisc == 572523149 → NO bloquea por umbral
- numero_imagenes NULL → error
- numero_imagenes = 0 → error
- disco permitido → OK
- EstadoDisco == SL (si existe) → bloquea Integración Validar: - schema sin ESTADO_DISCO → funciona
- schema con ESTADO_DISCO → funciona
- SELECT FOR UPDATE ejecutado
- rollback ejecutado correctamente
- concurrencia controlada Regresión legacy Comparar contra: Numero_Imagenes(...) Validar: - mensajes equivalentes
- umbrales equivalentes
- comportamiento equivalente ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBSERVABILIDAD ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Logs obligatorios: requestId
gabinete
disco
tamdisc
numeroImagenes
estadoDisco
resultado (ALLOW/BLOCK)
duración NO loguear: datos sensibles
fulltext
contenido documental ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actualizar en: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\GestorDocumental\AlmacenamientoDocumental\StorageEngine\ y/o: Arquitectura-Final\ Crear/actualizar: SCRUM-189-Arquitectura-Capacidad-Disco.md
SCRUM-189-Implementacion-Capacidad-Disco.md
SCRUM-189-Pruebas-Capacidad-Disco.md
SCRUM-189-Observabilidad-Capacidad-Disco.md
SCRUM-189-Regresion-Legacy-Capacidad-Disco.md
SCRUM-189-Metadata.md Debe incluir: - brecha detectada
- regla legacy adoptada
- matriz con/sin ESTADO_DISCO
- decisión tamdisc == 572523149
- evidencia de pruebas
- diagramas de secuencia y validación ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ✔ no aparece más Unknown column ESTADO_DISCO
✔ comportamiento igual a VB
✔ validación antes del commit
✔ concurrencia controlada
✔ rollback correcto
✔ compatibilidad con/sin ESTADO_DISCO
✔ pruebas en verde
✔ documentación actualizada ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RESTRICCIONES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ - NO cambiar lógica legacy
- NO introducir migración obligatoria
- NO hardcodear reglas nuevas
- NO eliminar FOR UPDATE
- NO tocar contratos HTTP ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INSTRUCCIÓN FINAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Implementar la validación de capacidad de disco replicando exactamente el comportamiento legacy VB, eliminando la dependencia obligatoria de ESTADO_DISCO y garantizando consistencia transaccional bajo concurrencia.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-197-correcion-validacion-tamano-disco-almace.