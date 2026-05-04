# PROMPT ARQUITECTONICO - ORCHESTRATOR / ENGINE
# PROMPT 12 - Paridad Legacy de Preindex + Alineacion con Metadata
# (FASE CRITICA - BLOQUEANTE FUNCIONAL)

## OBJETIVO
Restaurar paridad funcional de:
- Contruye_Nombre_Archvio_Index
- Lee_Archivo_Txt_Index_Ceros_Txt
- Lectura de preindex .txt/.xmls
- Carga de valores en orden exacto
- Validacion contra metadata

## FUNCIONES LEGACY REFERENCIA
- Almacenamiento
- Contruye_Nombre_Archvio_Index
- Lee_Archivo_Txt_Index_Ceros_Txt
- Leer_Archivo_Preindex

## INSTRUCCION TECNICA
Implementar:
1. Constructor de nombre preindex legacy (DIG + zero-fill + correlativo).
2. Parser robusto para .txt y .xmls.
3. Mapeo posicional preindex -> CamposIndexacion.
4. Validacion estricta de cantidad/orden/nombre/obligatoriedad.
5. Integracion en pipeline antes de validacion de obligatorios.

## CRITERIOS
- Errores equivalentes: Documento sin preindexacion, mismatch de matrices, obligatorio vacio.
- Sin perdida de orden.
