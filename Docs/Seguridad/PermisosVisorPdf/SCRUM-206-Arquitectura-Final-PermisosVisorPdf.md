# SCRUM-206 Arquitectura Final Permisos Visor PDF

## 1. Objetivo
Definir una API de permisos efectivos del visor PDF reutilizable por multiples implementaciones frontend (`workflow`, `gestion_correspondencia`, futuras), con resolucion por usuario y precedencia deterministica.

## 2. Alcance
- Consulta de permisos efectivos del usuario autenticado.
- Consulta de permisos efectivos de un usuario objetivo (administrativo).
- Upsert de overrides por usuario/implementacion.
- Eliminacion de override por permiso.
- Salida de permisos en formato simple `{ "pdf.xxx": true|false }`.

## 3. No alcance
- UI administrativa de perfiles y permisos.
- Integracion con IAM externo.
- ABM completo de catalogos fuera de lo requerido para la API.

## 4. Arquitectura por capas
- `Controller`: validacion de claims/inputs, autorizacion, envelope `AppResponses<T>`.
- `Service`: reglas de negocio y precedencia de permisos.
- `Repository`: acceso a datos con `DapperCrudEngine + QueryOptions`.

## 5. Flujo logico de resolucion
1. Obtener catalogo activo de permisos (`ra_vis_per_permiso`).
2. Resolver override por usuario (`ra_vis_per_usuario_override`).
3. Resolver permiso por perfil activo (`ra_vis_per_usuario_perfil` + `ra_vis_per_perfil_permiso`).
4. Resolver default de implementacion (`ra_vis_per_impl_perm_default`).
5. Aplicar fallback denegar (`false`) para permisos sin fuente.

## 6. Regla de precedencia
1. `usuario_override`
2. `perfil_activo`
3. `default_implementacion`
4. `fallback_deny`

## 7. Decision de contrato (clave)
- El consumo es por usuario individual (`usuarioid` claim o `idUsuario` en ruta).
- `codiperfil` NO se solicita en request.
- Si hay perfil vigente, se resuelve internamente en backend.

## 8. Endpoints objetivo
- `GET /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/mis-permisos`
- `GET /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/usuarios/{idUsuario}/permisos`
- `PUT /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/usuarios/{idUsuario}/overrides`
- `DELETE /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/usuarios/{idUsuario}/overrides/{codigoPermiso}`

## 9. Requerimientos tecnicos obligatorios
- Claims: `defaulalias`, `usuarioid`.
- `try/catch` en controller/service/repository.
- Envelope uniforme `AppResponses<T>`.
- Registro DI en `Program.cs`:
  - Servicios bajo `// Services (L)`.
  - Repositorios bajo `// Repositories (R)`.

