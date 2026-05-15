# SCRUM-201 - Observabilidad y Troubleshooting

## 1. Qué monitorear
- Tasa de `400` por claims faltantes.
- Tasa de `400` por firma no configurada.
- Tasa de `404` en descarga por expiración.
- Latencia de generación de temporal.

## 2. Eventos clave del flujo
1. Inicio de solicitud de metadata.
2. Resultado de consulta repositorio (`firma encontrada/no encontrada`).
3. Archivo temporal generado.
4. Token temporal registrado en cache.
5. Resultado de descarga (`ok/notfound`).

## 3. Fallas comunes y diagnóstico

## 3.1 `400` Claim inválido
- Verificar JWT y claims:
  - `defaulaliaswf`
  - `IdUsuarioWorkflow`

## 3.2 `400` Usuario workflow no tiene firma configurada
- Verificar en DB:
  - tabla `usuario_workflow`
  - `Firma_Usuario` no null / no vacío

## 3.3 `404` en download
- Token expirado (TTL 15 minutos).
- Token inexistente en cache (reinicio de API limpia cache).
- Archivo temporal borrado manualmente.

## 3.4 `500` inesperado
- Revisar permisos de escritura en root temporal.
- Revisar path resolver y disponibilidad de disco.
