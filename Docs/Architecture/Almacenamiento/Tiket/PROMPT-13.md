# PROMPT ARQUITECTONICO - ORCHESTRATOR / ENGINE
# PROMPT 13 - Paridad Legacy de Opciones system1 (Inventario/TRD/Unidad)
# (FASE CRITICA - BLOQUEANTE FUNCIONAL)

## FUNCIONES LEGACY REFERENCIA
- VerificaOpcionAplicarInventarioDocumental
- VerificaOpcionAplicarTablaRetencion
- Verfica_opcion_seleccion_unidad

## OBJETIVO
Eliminar placeholder de StorageOptionsResolver y consultar opciones reales de system1.

## INSTRUCCION TECNICA
- Crear IStorageOptionsRepository + StorageOptionsRepository.
- Resolver opciones reales (INVENTARIO_DOCUMENTAL, APLICA_TRD, ASIGNA_UNIDAD).
- Ajustar validadores dependientes.

## CRITERIOS
- Sin hardcode ni defaults vacios.
- Reglas del pipeline dependen de DB real.
