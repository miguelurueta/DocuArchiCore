# SCRUM-206 Arquitectura Final Permisos Visor PDF

## 1. Objetivo
Definir la arquitectura final para exponer permisos efectivos del visor PDF por implementación y usuario, reutilizable en módulos como `workflow` y `gestion_correspondencia`.

## 2. Alcance
- Resolver permisos efectivos por usuario para un `codigoImpl`.
- Soportar consulta propia (`mis-permisos`) y consulta/mutación administrativa de terceros.
- Aplicar precedencia determinística:
  - `usuario_override`
  - `perfil_activo`
  - `default_implementacion`
  - `fallback_deny`
- Exponer contrato uniforme `AppResponses<T>`.

Fuera de alcance:
- UI administrativa completa de perfiles/catálogo.
- Motor IAM externo.
- Gestión masiva de implementaciones/perfiles.

## 3. Contexto Arquitectónico
`Controller -> Service (UseCase) -> Repository -> DapperCrudEngine + QueryOptions -> tablas ra_vis_per_*`

### 3.1 Separación por módulos
- API: `DocuArchi.Api/Controllers/GestorDocumental/PermisosVisorPdf/PermisosVisorPdfController.cs`
- Servicio: `MiApp.Services/Service/GestorDocumental/PermisosVisorPdf/IPermisosVisorPdfService.cs`
- Repositorio: `MiApp.Repository/Repositorio/GestorDocumental/PermisosVisorPdf/IPermisosVisorPdfRepository.cs`
- DTOs: `MiApp.DTOs/DTOs/GestorDocumental/PermisosVisorPdf/PermisosVisorPdfDtos.cs`

## 4. Contrato API
- Base: `/api/gestor-documental/permisos-visorpdf`
- Endpoints:
  - `GET /implementaciones/{codigoImpl}/mis-permisos`
  - `GET /implementaciones/{codigoImpl}/usuarios/{idUsuario}/permisos`
  - `PUT /implementaciones/{codigoImpl}/usuarios/{idUsuario}/overrides`
  - `DELETE /implementaciones/{codigoImpl}/usuarios/{idUsuario}/overrides/{codigoPermiso}`
- Seguridad: JWT Bearer.
- Claims requeridos:
  - `defaulalias`
  - `usuarioid` (obligatorio en `mis-permisos`)

### 4.1 Contrato frontend completo por endpoint
El detalle de requests/responses y checklist operativo está en:
- `Docs/Seguridad/PermisosVisorPdf/SCRUM-206-Integracion-Frontend-PermisosVisorPdf.md`

## 5. Flujo End-to-End
1. Controller valida claims y entradas de ruta/body.
2. Controller autoriza endpoints admin por `IsAdmin()`.
3. Service valida reglas base y orquesta caso de uso.
4. Repository valida `codigoImpl`/`codigoPermiso` con regex segura.
5. Repository resuelve `id_impl`, catálogo activo y fuentes de permiso.
6. Repository consolida mapa final `Permissions` + `Sources`.
7. Service retorna `AppResponses<T>` al controller.

## 6. Validaciones
### 6.1 Seguridad
- `defaulalias` obligatorio.
- `usuarioid` obligatorio y numérico para `mis-permisos`.
- Endpoints admin requieren `IsAdmin()`.

### 6.2 Funcionales
- `codigoImpl` requerido y válido.
- `idUsuario` > 0 en endpoints de tercero.
- `overrides` no vacío en `PUT`.
- `permitido` sólo `0|1`.

### 6.3 Integridad de datos
- Regex segura: `^[A-Za-z0-9_\.]+$` para códigos.
- Acceso vía `DapperCrudEngine + QueryOptions`.
- Sin SQL manual concatenado en esta capacidad.

## 7. Manejo de Errores
- `400 Validation`: claims/entrada inválida o implementación/permiso no encontrado.
- `401 Unauthorized`: endpoint admin sin autorización.
- `500`: error no controlado.
- Componentes nuevos con `try/catch` y envelope uniforme.

## 8. Observabilidad
Logs mínimos en Service:
- Inicio/fin por operación (`GetMyPermissions`, `GetUserPermissions`, `Upsert`, `Delete`).
- Campos: `usuarioId`, `alias`, `codigoImpl`, `success`, `total/procesados`.
- Error técnico por excepción.

## 9. Riesgos y Mitigaciones
- Riesgo: reglas inconsistentes por módulo frontend.
  - Mitigación: backend centraliza precedencia y contrato único.
- Riesgo: override parcial en cargas grandes.
  - Mitigación: control por item; pendiente transacción de lote.
- Riesgo: sobreexposición de administración.
  - Mitigación: autorización explícita `IsAdmin()` y claims/permisos.

## 10. Depuración Guiada
1. Verificar JWT (`defaulalias`, `usuarioid`).
2. Validar `codigoImpl` real activo.
3. Probar `mis-permisos` y revisar `Sources`.
4. Probar endpoint admin con/ sin rol admin.
5. Validar persistencia override (`PUT`/`DELETE`) y recalcular `mis-permisos`.
