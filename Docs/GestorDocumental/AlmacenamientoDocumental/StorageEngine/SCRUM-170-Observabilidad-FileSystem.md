# SCRUM-170 Observabilidad FileSystem

## Eventos
- `inicio fase fisica`
- `archivo copiado`
- `xml generado`
- `compensation completada`
- `error fase fisica`

## Campos mínimos
- `requestId`
- `idAlmacen`
- `fase`
- `source/destination` (solo nombres sanitizados)

## Seguridad de logging
- no registrar SQL.
- no registrar contenido completo sensible de indexación.
- no registrar rutas absolutas de usuario no saneadas.
