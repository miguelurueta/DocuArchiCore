# SCRUM-206 Observabilidad Permisos Visor PDF

## 1. Logs minimos por operacion

### 1.1 Consulta de permisos
- Nivel: `Information`
- Evento: `PermisosVisorPdf.Resolve.Start`
- Campos: `requestId`, `usuarioid`, `alias`, `codigoImpl`

- Nivel: `Information`
- Evento: `PermisosVisorPdf.Resolve.End`
- Campos: `requestId`, `codigoImpl`, `idUsuarioObjetivo`, `permisosTotales`, `duracionMs`

### 1.2 Upsert override
- Nivel: `Information`
- Evento: `PermisosVisorPdf.Override.Upsert`
- Campos: `requestId`, `codigoImpl`, `idUsuarioObjetivo`, `procesados`

### 1.3 Delete override
- Nivel: `Information`
- Evento: `PermisosVisorPdf.Override.Delete`
- Campos: `requestId`, `codigoImpl`, `idUsuarioObjetivo`, `codigoPermiso`

### 1.4 Validaciones y seguridad
- Nivel: `Warning`
- Evento: `PermisosVisorPdf.Validation`
- Campos: `requestId`, `motivo`, `usuarioid`, `codigoImpl`

### 1.5 Excepciones
- Nivel: `Error`
- Evento: `PermisosVisorPdf.Exception`
- Campos: `requestId`, `codigoImpl`, `idUsuarioObjetivo`, `exceptionType`

## 2. Metricas sugeridas
- Latencia p95 por endpoint.
- Tasa de error por endpoint.
- Total de permisos resueltos por consulta.
- Volumen de mutaciones de override por dia.

## 3. Troubleshooting rapido
1. `success=false` por claims: validar JWT (`defaulalias`, `usuarioid`).
2. Permisos todos en `false`: revisar seed/default y estado de permisos.
3. Override no impacta: validar existencia de `codigoPermiso` y estado override.
4. Usuario sin perfil: verificar vigencia/estado en `ra_vis_per_usuario_perfil`.

