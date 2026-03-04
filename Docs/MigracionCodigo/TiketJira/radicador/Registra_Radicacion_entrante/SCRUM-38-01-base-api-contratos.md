# Ticket 1 - Base API y contratos (Registra_Radicacion_entrante)

## Contexto

- Repositorio fuente: `D:\imagenesda\GestorDocumental\Desarrollo\Visual-2019\GestionDocumental-Docuarchi.net\GestionDocumental-Docuarchi.net`
- Archivo fuente: `/radicador/ClassRadicador.vb`
- Función fuente: `Registra_Radicacion_entrante`
- Repositorio destino: `D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore`

## Objetivo

Crear la base de migración para exponer el caso de uso de radicación entrante en arquitectura por capas (`Controller -> Service -> Repository`) con contratos estables y respuesta estándar (`AppResponses`).

## Reglas de migración aplicadas

1. No replicar errores del legacy; documentar y aplicar corrección.
2. Mantener consultas parametrizadas y validación de datos.
3. Mapeo de conexión legacy:
   - `conect.Dbase_Conction_Mysql` -> `workflowtconta` -> alias `WF`
   - `conect.Dbase_Conction_Mysql_RA` -> `docuarchi` -> alias `DA`
   - `conect.Dbase_Conction_Mysql_DA` -> `docuarchi` -> alias `DA`

## Alcance funcional

1. Definir endpoint base para registrar radicación entrante.
2. Definir DTO de entrada y DTO de salida mínimos.
3. Definir interfaz de servicio principal de migración.
4. Estandarizar contrato de respuesta `AppResponses`.
5. Manejo controlado de errores en Controller y Service (`try/catch`).

## Correcciones incluidas

1. Evitar ambigüedad de tipo/valor en parámetros de flujo (sin mezclar `"null"` y `0`).
2. Garantizar mensajes de error coherentes por etapa de validación.
3. Preparar diseño para separar validación, autorización y persistencia (sin método monolítico).

## Requerimientos técnicos obligatorios

1. Mantener SoC, bajo acoplamiento y alta cohesión.
2. Cumplir principios SOLID.
3. Comentarios en funciones con:
   - fecha de creación,
   - descripción de parámetros,
   - descripción de retorno.
4. Este ticket no ejecuta lógica SQL aún; prepara contratos para tickets siguientes.

## Criterios de aceptación

1. Existe endpoint base documentado y callable.
2. Request/Response DTO definidos con campos mínimos del caso de uso.
3. Service interface definida y conectada al controller.
4. Todas las respuestas salen envueltas en `AppResponses`.
5. Casos cubiertos: éxito, validación fallida y excepción controlada.

## Pruebas requeridas

1. Prueba unitaria de validación de request inválido.
2. Prueba unitaria de flujo exitoso simulado.
3. Prueba unitaria de excepción controlada.

## Evidencia esperada en PR

1. Código de Controller/Service/DTO base.
2. Tests unitarios en verde.
3. Documento técnico actualizado en `/Docs/MigracionCodigo`.

## Reglas obligatorias del entorno de migracion (actualizado)

1. Mantener SoC, bajo acoplamiento y alta cohesion.
2. Cumplir principios SOLID.
3. Agregar diagramas de casos de uso, clases, secuencia y estado en `/Docs/Radicacion/Tramite`.
4. Incluir documentacion tecnica para frontend: descripcion del DTO, parametros de envio y direccion de la API.
5. Agregar comentarios en todas las funciones con:
   - fecha de creacion,
   - descripcion de parametros,
   - descripcion de retorno.
