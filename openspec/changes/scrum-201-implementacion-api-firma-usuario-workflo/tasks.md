## 1. Refinamiento funcional

- [ ] 1.1 Confirmar contrato final del endpoint `GET /api/workflow/usuarios/firma-temporal`.
- [ ] 1.2 Confirmar claims obligatorios (`defaulaliaswf`, `IdUsuarioWorkflow`) y reglas de rechazo.
- [ ] 1.3 Confirmar formato de salida (deteccion de tipo real y politica de normalizacion PNG si aplica).

## 2. Backend implementation

- [ ] 2.1 Implementar controller de firma temporal workflow con validacion de claims.
- [ ] 2.2 Implementar servicio de orquestacion temporal con AppResponses y manejo de errores.
- [ ] 2.3 Extender `UsuarioWorkflowR` con metodo de lectura de `Firma_Usuario` por `idU_suario`.
- [ ] 2.4 Integrar resolucion de ruta temporal/TTL reutilizando patron de StorageEngine.
- [ ] 2.5 Registrar DI en `Program.cs` para nuevas interfaces/servicios.

## 3. Testing

- [ ] 3.1 Unit tests controller: claims faltantes, response success, response error.
- [ ] 3.2 Unit tests service: firma null/vacia, escritura temporal, expiracion TTL, excepciones.
- [ ] 3.3 Unit tests repository: consulta por id, no encontrado, mapping de blob.
- [ ] 3.4 Integration test happy-path con archivo temporal generado.

## 4. Documentation

- [ ] 4.1 Actualizar arquitectura tecnica en `Docs/Workflow/FirmaUsuario/`.
- [ ] 4.2 Documentar contrato frontend (claims, respuesta, expiracion, errores).
- [ ] 4.3 Documentar observabilidad y troubleshooting.

## 5. Validation

- [ ] 5.1 Ejecutar `dotnet test` en suites impactadas.
- [ ] 5.2 Ejecutar validacion OpenSpec del cambio.
- [ ] 5.3 Preparar flujo para `orchestrate:publish`.
