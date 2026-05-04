# PROMPT ARQUITECTONICO - ORCHESTRATOR / ENGINE
# PROMPT 14 - Paridad Legacy de Ruta Fisica + Nomenclatura DIG
# (FASE CRITICA - BLOQUEANTE OPERATIVO)

## FUNCIONES LEGACY REFERENCIA
- Consulta_Ruta_Almacenamiento
- Ceros_Imagen_Almacenada
- Ceros_Imagen_Alamacenada_ext
- Copia_Imagen_Almacenada_local
- RetornaExtensionTipoDocumento

## OBJETIVO
Recuperar ruta desde SYSTEM1RUT y convencion de nombres legacy.

## INSTRUCCION TECNICA
- Resolver ruta fisica por gabinete/disco desde DB.
- Aplicar naming DIG + zero-fill + id + extension.
- Anexos con .0000, .0001...
- Mantener hardening anti traversal.
