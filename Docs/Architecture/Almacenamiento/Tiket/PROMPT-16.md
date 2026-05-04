# PROMPT ARQUITECTONICO - ORCHESTRATOR / ENGINE
# PROMPT 16 - Conteo Real de Paginas PDF (Paridad ITextSharp)
# (FASE CRITICA - INTEGRIDAD ARCHIVISTICA)

## FUNCIONES LEGACY REFERENCIA
- Retorna_numero_paginas_documentos_unificados
- Retorna_numero_paginas_documentos_pdf

## OBJETIVO
Calcular paginas reales PDF y evitar depender solo de NumeroPaginasDeclaradas.

## INSTRUCCION TECNICA
- Crear IDocumentPageCounter.
- Integrar en ResolveNumeroPaginasDocumento y reserva de identidad.
- Definir politica de fallback.
