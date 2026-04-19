## 1. Discovery

- [ ] 1.1 Confirmar contrato y semántica (`AppResponses`).
- [ ] 1.2 Revisar SQL actual y mapeo `RaRespuestaRadicado`.
- [ ] 1.3 Identificar campos usados por tokens/plantillas.

## 2. Specs

- [ ] 2.1 Especificar alias obligatorios (`AS IdTareaWf`, etc.).
- [ ] 2.2 Definir comportamiento “Sin resultados” sin cambios.
- [ ] 2.3 Definir prueba anti-regresión de mapeo.

## 3. Application

- [ ] 3.1 Reemplazar mapeo automático por SQL con alias explícitos.
- [ ] 3.2 Mantener service/controller sin cambios funcionales.
- [ ] 3.3 Documentar SQL y decisión técnica.

## 4. Test

- [ ] 4.1 Agregar test anti-regresión (mapeo `IdTareaWf`).
- [ ] 4.2 Ejecutar pruebas del repo (o skip justificado).
- [ ] 4.3 Validar change con OpenSpec.
