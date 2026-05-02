# SCRUM-166 Observabilidad IdentityAllocator

## Logs recomendados en flujo de reserva
- Inicio de reserva (`requestId`, `gabinete`, `usuarioId`, `alias`).
- `system1` bloqueado.
- Identidad calculada (`idAlmacen`, `previousProxId`, `newProxId`, `folder`).
- `disco_detalle` bloqueado y estado validado.
- `system1` actualizado.
- Error de concurrencia o validacion legacy.

## Campos a capturar
- `requestId`
- `usuarioId`
- `alias`
- `nombreGabinete`
- `disco`
- `idAlmacen`
- `previousProxId`
- `newProxId`
- `previousFolder`
- `newFolder`
- `previousFolderPages`
- `newFolderPages`
- `tamdisc`
- `estadoDisco`
- `duracionMs`

## Eventos de warning/error
- `tamdisc` no permitido.
- `EstadoDisco = SL`.
- `system1` no encontrado.
- `disco_detalle` no encontrado.
- update de reserva sin filas afectadas.

## Datos que no deben loguearse
- Contenido documental.
- Fulltext.
- Payloads sensibles de indexacion.
