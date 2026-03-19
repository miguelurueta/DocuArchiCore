## Context

`opsxdeploy` hoy asume que el operador ya generó un publish limpio y luego solo valida/prepara/empaqueta. El ejercicio real mostró que ese supuesto no se cumple: el operador intenta usar el tool desde el repositorio y el publish generado por `DocuArchi.Api` puede contener `appsettings.Development.json`, `Tools/**` y valores sensibles en `appsettings.json`, por lo que `doctor` bloquea el flujo antes de que el paquete final pueda ser saneado.

## Context Reference

- `openspec/context/multi-repo-context.md`: el flujo cruza `DocuArchiCore` como repositorio del tool y `DocuArchi.Api` como proyecto fuente del publish.
- `openspec/context/OPSXJ_BACKEND_RULES.md`: se usa como baseline de disciplina de implementacion/pruebas, aunque este cambio no crea Controllers/DTOs/Repositories.
- Dependencias cross-repo:
  - `DocuArchiCore/Tools/iis-deploy` ejecuta el flujo de publicacion y empaquetado
  - `DocuArchi.Api` aporta el `.csproj`, `appsettings*.json`, `web.config` y el contenido real del publish

## Goals / Non-Goals

**Goals:**
- Permitir que `opsxdeploy publish-package` trabaje desde un `.csproj` sin requerir que el operador haga `dotnet publish` manualmente.
- Separar el concepto de "publish intermedio" del "paquete final listo para IIS".
- Sanitizar automaticamente el paquete final para excluir artefactos no productivos y mitigar configuracion sensible conocida.
- Conservar el modo actual basado en `-PublishPath` para no romper automatizaciones existentes.

**Non-Goals:**
- No desplegar directamente a IIS ni crear App Pools/sitios desde este cambio.
- No modificar archivos fuente dentro de `DocuArchi.Api`.
- No inventar secretos ni valores productivos; solo placeholders o blanking controlado en la salida final.

## Decisions

1. Extender `publish-package` con doble modo de entrada.
   - Decision: aceptar `-ProjectPath` como alternativa a `-PublishPath`.
   - Rationale: evita agregar un comando nuevo y mantiene un punto unico para generar el paquete final.
   - Alternative considered: crear `publish-from-project`; se descarta por duplicar semantica y documentacion.

2. Introducir staging temporal interno.
   - Decision: cuando se use `-ProjectPath`, el tool ejecuta `dotnet publish` hacia una carpeta temporal y despues copia/sanitiza hacia `OutputPath`.
   - Rationale: el publish bruto puede contener archivos validos para desarrollo pero no para entrega final; separar staging y output permite limpiar sin tocar el source repo ni depender de un publish manual perfecto.

3. Cambiar la validacion a dos fases.
   - Decision: mantener una validacion estricta del paquete final, pero permitir que el staging tenga hallazgos corregibles por sanitizacion automatica.
   - Rationale: hoy `doctor` falla demasiado pronto para casos donde el paquete final si puede quedar limpio.
   - Alternative considered: relajar `doctor` globalmente; se descarta porque reduciria seguridad en el modo `-PublishPath`.

4. Sanitizar `appsettings.json` solo en la salida final.
   - Decision: detectar campos sensibles conocidos (`Jwt.Key`, `Secret`, connection strings con password/root`) y reemplazarlos por placeholders o valores vacios en el paquete final.
   - Rationale: el objetivo del paquete IIS es no transportar secretos reales, pero tampoco se debe editar la configuracion fuente del proyecto.
   - Alternative considered: fallar siempre si hay secretos; se descarta para el modo `-ProjectPath` porque impediria automatizar exactamente el caso que motivó este cambio.

## Risks / Trade-offs

- [Riesgo] Ejecutar `dotnet publish` desde el tool aumenta tiempo y complejidad del comando. -> Mitigacion: habilitarlo solo cuando se use `-ProjectPath` y conservar el modo actual.
- [Riesgo] Sanitizar `appsettings.json` por patrones puede dejar campos sensibles no contemplados. -> Mitigacion: limitar el alcance a claves conocidas, documentar la politica y seguir fallando si aparecen patrones inseguros fuera del conjunto soportado.
- [Riesgo] El staging temporal puede dejar residuos si el proceso falla. -> Mitigacion: usar una ruta temporal controlada y cleanup en `finally` cuando sea posible.
- [Riesgo] Cambiar semantica de `publish-package` puede romper expectativas actuales. -> Mitigacion: mantener compatibilidad con `-PublishPath` y documentar claramente los dos modos.

## Migration Plan

1. Implementar parsing de parametros `-ProjectPath` y carpeta de staging temporal.
2. Ejecutar `dotnet publish` interno para `DocuArchi.Api` u otro `.csproj` compatible.
3. Aplicar sanitizacion del paquete final antes de la validacion estricta final.
4. Ajustar pruebas automatizadas para ambos modos (`-PublishPath` y `-ProjectPath`).
5. Actualizar README/manual operativo con ejemplos reales de uso.

## Open Questions

- Si el proyecto publica `appsettings.json` con otras claves sensibles no estandarizadas, ¿debemos introducir una lista configurable externa?
- ¿Conviene exponer un flag explicito para conservar secretos en escenarios internos controlados o mantener la sanitizacion obligatoria en modo `-ProjectPath`?
