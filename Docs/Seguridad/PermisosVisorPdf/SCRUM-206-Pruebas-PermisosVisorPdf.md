# SCRUM-206 Pruebas Permisos Visor PDF

## 1. Objetivo de pruebas
Validar contrato API, reglas de precedencia, seguridad de endpoints y comportamiento de mutaciones de override.

## 2. Matriz minima por capa

### 2.1 Controller
- Claim `defaulalias` ausente -> error controlado.
- Claim `usuarioid` invalido -> error controlado.
- Endpoint admin sin autorizacion -> rechazo controlado.
- Respuesta success con envelope `AppResponses<T>`.

### 2.2 Service
- `override` sobreescribe `perfil/default`.
- Sin `override` usa `perfil`.
- Sin `perfil` usa `default`.
- Sin ninguna fuente aplica `deny=false`.
- Excepcion de repositorio -> `success=false`.

### 2.3 Repository
- Consulta efectiva respeta precedencia.
- `upsert` inserta/actualiza override.
- `delete` elimina override especifico.
- Filtros parametrizados sin SQL manual.

### 2.4 Contract/Regression
- Shape exacto: `success/message/data/errors`.
- `permissions` siempre presente para catalogo activo.
- Sin regresion de endpoints existentes no relacionados.

## 3. Datos de prueba recomendados
Basarse en:
- `ra_vis_per_schema.sql`
- `ra_vis_per_seed.sql`

Escenarios:
1. Usuario con default solamente.
2. Usuario con perfil activo.
3. Usuario con override activo.
4. Usuario sin asignaciones (fallback deny).

## 4. Criterios de aceptacion tecnicos
- Todos los permisos activos aparecen en `permissions`.
- No se solicita `codiperfil` en request.
- Endpoints admin exigen autorizacion.
- `AppResponses` en todos los caminos de error/success.

## 5. Comando de verificacion sugerido
```powershell
dotnet test tests\TramiteDiasVencimiento.Tests\TramiteDiasVencimiento.Tests.csproj --filter "FullyQualifiedName~PermisosVisorPdf"
```

Si Docker/Testcontainers no esta disponible:
- Marcar integration tests como `Skipped` con razon explicita.

