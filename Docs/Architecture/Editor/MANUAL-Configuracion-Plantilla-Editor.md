# Manual — Configuración de plantilla del Editor (Template + Tokens + Contexto)

Este documento describe el paso a paso para **configurar una plantilla del Editor Tiptap** para que el endpoint de contenido inicial (Initial Content) pueda hidratar el HTML sin errores de “Faltan tokens…”.

> Alcance: configuración **en base de datos** (tablas del módulo Editor) + checklist de validación.

## 1) Pre-requisitos (convenciones)

1. **Normalización de códigos**
   - `ContextCode` se trabaja como `TRIM + UPPER`.
   - MVP actual: `TemplateCode == ContextCode` (el servicio usa el ContextCode como templateCode).
2. **Tokens en el HTML**
   - Los placeholders deben estar en el HTML como `{{TokenCode}}` (sensible a espacios; el `TokenCode` debe coincidir).
3. **Fuente de token (`source_type`)**
   - `respuesta_api`: se obtiene de la estructura de negocio (`SolicitaEstructuraRespuestaIdTareaAsync`), usando `source_key` como nombre de propiedad.
   - `system`: valores del sistema (ej: `FechaActual`).
   - `static`: toma `default_value`.
   - `editable`: se inicializa con `default_value` (o vacío) y el usuario lo edita.

## 2) Tablas que se deben registrar

### 2.1 `ra_editor_context_definitions` (Catálogo de contextos)
Define qué `ContextCode` existe y está activo para el Editor.

Campos típicos (ver diseño/implementación del ticket de catálogo):
- `context_definition_id` (PK)
- `context_code` (UNIQUE)
- `entity_name`, `relation_type`
- banderas de reglas (según implementación)
- `is_active`

### 2.2 `ra_editor_template_definitions` (Definición de plantilla)
Define la plantilla (por `template_code`).

Campos:
- `template_definition_id` (PK)
- `template_code` (UNIQUE)
- `template_name`, `module_name`, `description`
- `is_active`
- `created_by`, `created_at`

### 2.3 `ra_editor_template_versions` (Versiones de plantilla)
Define las versiones y el `template_html` a hidratar.

Campos:
- `template_version_id` (PK)
- `template_definition_id` (FK)
- `version_number` (UNIQUE por definición)
- `template_html`
- `is_active`, `is_published`
- `created_by`, `created_at`

### 2.4 `ra_editor_template_tokens` (Tokens de la plantilla)
Catálogo de tokens permitidos (y sus fuentes).

Campos (según tu tabla):
- `template_token_id` (PK)
- `template_definition_id` (FK)
- `token_code` (debe coincidir con `{{token_code}}` en el HTML)
- `token_label`
- `source_type` (`respuesta_api|system|static|editable`)
- `source_key` (ej: nombre de propiedad en `respuesta_api`, o `FechaActual`)
- `is_required` (1/0)
- `is_editable` (1/0)
- `default_value`
- `created_by`, `created_at`

## 3) Paso a paso (SQL)

> SQL orientado a MySQL (por uso de `LAST_INSERT_ID()` en repos). Ajusta si tu motor difiere.

## 3A) Registrar una **nueva plantilla** para un **contexto existente** (recomendado)

Este es el caso más común cuando ya existe un `ContextCode` estable (por ejemplo `RAD_GESTION_RESPUESTA`) y quieres agregar una nueva plantilla versionada (por ejemplo `RAD_GESTION_RESPUESTA_V2`) sin crear un nuevo contexto.

### Paso A1 — Confirmar el contexto y obtener `context_definition_id`

```sql
SELECT context_definition_id, context_code, is_active
FROM ra_editor_context_definitions
WHERE context_code = UPPER(TRIM('RAD_GESTION_RESPUESTA'))
LIMIT 1;
```

Si `is_active != 1`, primero debes activar el contexto o corregir el catálogo.

### Paso A2 — Crear la nueva `template_definition` (nuevo `template_code`)

Regla: `template_code` debe ser **único**. Para versionar, usa sufijos tipo `_V1`, `_V2`, etc.

```sql
INSERT INTO ra_editor_template_definitions
(
  template_code,
  template_name,
  module_name,
  description,
  is_active,
  created_by,
  created_at
)
VALUES
(
  UPPER(TRIM('RAD_GESTION_RESPUESTA_V2')),
  'Plantilla Respuesta Gestión Correspondencia (V2)',
  'GestionCorrespondencia',
  'Nueva plantilla HTML para respuestas (V2)',
  1,
  'system',
  NOW()
);
```

Obtener el `template_definition_id` recién creado:

```sql
SELECT template_definition_id, template_code, is_active
FROM ra_editor_template_definitions
WHERE template_code = UPPER(TRIM('RAD_GESTION_RESPUESTA_V2'))
LIMIT 1;
```

### Paso A3 — Registrar la versión vigente (`template_html`)

```sql
INSERT INTO ra_editor_template_versions
(
  template_definition_id,
  version_number,
  template_html,
  is_active,
  is_published,
  created_by,
  created_at
)
VALUES
(
  /* template_definition_id */ 2,
  1,
  '<p>Radicado: {{Radicado}}</p><p>Fecha: {{Fecha}}</p><p>{{TextoRespuesta}}</p>',
  1,
  1,
  'system',
  NOW()
);
```

> Recomendación: dejar solo una versión activa/publicada como vigente por cada `template_definition_id`.

### Paso A4 — Registrar tokens para la nueva definición (`ra_editor_template_tokens`)

Regla: cada `{{TokenCode}}` que exista en `template_html` debe existir como `token_code` en `ra_editor_template_tokens` para ese `template_definition_id`.

Ejemplo (ajusta a tu catálogo):
```sql
INSERT INTO ra_editor_template_tokens
(
  template_definition_id,
  token_code,
  token_label,
  source_type,
  source_key,
  is_required,
  is_editable,
  default_value,
  created_by,
  created_at
)
VALUES
(2,'Radicado','Radicado','respuesta_api','Radicado',1,0,NULL,'system',NOW()),
(2,'Fecha','Fecha','system','FechaActual',0,0,NULL,'system',NOW()),
(2,'Destinatario','Destinatario','respuesta_api','Destinatario',1,0,NULL,'system',NOW()),
(2,'DireccionDestinatario','Dirección','respuesta_api','DireccionDestinatario',0,0,NULL,'system',NOW()),
(2,'Ciudad','Ciudad','static','Ciudad',0,0,'Villavicencio','system',NOW()),
(2,'Asunto','Asunto','respuesta_api','Asunto',0,0,NULL,'system',NOW()),
(2,'TextoRespuesta','Texto Respuesta','editable','TextoRespuesta',0,1,'','system',NOW());
```

### Paso A5 — Asociar la plantilla al contexto (`ra_editor_template_context_rules`)

Este paso es el que habilita que el backend seleccione la plantilla para un `context_definition_id` dado.

Reglas:
- Se elige la regla activa con menor `priority_order`.
- Si quieres que la nueva plantilla sea la predeterminada del contexto, pon `priority_order = 1` y baja/desactiva la anterior.

**Opción 1 (recomendada): dejar la nueva como default**
```sql
-- (opcional) desactiva reglas previas del contexto para evitar ambigüedad operativa
UPDATE ra_editor_template_context_rules
SET is_active = 0
WHERE context_definition_id = 1;

-- activa la nueva regla como prioridad 1
INSERT INTO ra_editor_template_context_rules
(
  context_definition_id,
  template_definition_id,
  priority_order,
  is_active,
  created_by,
  created_at
)
VALUES
(
  1,
  2,
  1,
  1,
  'system',
  NOW()
);
```

**Opción 2: mantener varias plantillas y controlar prioridad**
```sql
INSERT INTO ra_editor_template_context_rules
(context_definition_id, template_definition_id, priority_order, is_active, created_by, created_at)
VALUES (1, 2, 2, 1, 'system', NOW());
```

### Paso A6 — Validación rápida

1) Verifica que el contexto apunte a la nueva plantilla:
```sql
SELECT r.context_definition_id, r.template_definition_id, r.priority_order, r.is_active, d.template_code
FROM ra_editor_template_context_rules r
JOIN ra_editor_template_definitions d ON d.template_definition_id = r.template_definition_id
WHERE r.context_definition_id = 1
ORDER BY r.priority_order ASC, r.template_context_rule_id ASC;
```

2) Verifica tokens vs HTML:
- extrae tokens del HTML (`{{...}}`)
- confirma que cada uno exista en `ra_editor_template_tokens` de esa definición

3) Prueba el endpoint:
- modo normal (por contexto): `contextCode=RAD_GESTION_RESPUESTA`
- si necesitas forzar: `templateDefinitionId=<id-nuevo>`

### Paso 1 — Crear/activar el `ContextCode`

1) Verificar si existe:
```sql
SELECT context_definition_id, context_code, is_active
FROM ra_editor_context_definitions
WHERE context_code = UPPER(TRIM('RAD_GESTION_RESPUESTA'));
```

2) Si no existe, insertarlo (ejemplo — ajusta columnas obligatorias reales):
```sql
INSERT INTO ra_editor_context_definitions
(
  context_code,
  entity_name,
  relation_type,
  is_active,
  created_by,
  created_at
)
VALUES
(
  UPPER(TRIM('RAD_GESTION_RESPUESTA')),
  'rad_gestion',
  'documento_respuesta',
  1,
  'system',
  NOW()
);
```

### Paso 2 — Crear la definición de plantilla

1) Verificar si ya existe por `template_code`:
```sql
SELECT template_definition_id, template_code, template_name, is_active
FROM ra_editor_template_definitions
WHERE template_code = UPPER(TRIM('RAD_GESTION_RESPUESTA'));
```

2) Insertar si no existe:
```sql
INSERT INTO ra_editor_template_definitions
(
  template_code,
  template_name,
  module_name,
  description,
  is_active,
  created_by,
  created_at
)
VALUES
(
  UPPER(TRIM('RAD_GESTION_RESPUESTA')),
  'Respuesta — Gestión',
  'GestorDocumental',
  'Plantilla inicial para respuesta (Tiptap)',
  1,
  'system',
  NOW()
);
```

3) Obtener el `template_definition_id`:
```sql
SELECT template_definition_id
FROM ra_editor_template_definitions
WHERE template_code = UPPER(TRIM('RAD_GESTION_RESPUESTA'))
LIMIT 1;
```

### Paso 3 — Registrar la versión activa (HTML)

1) Insertar versión:
```sql
INSERT INTO ra_editor_template_versions
(
  template_definition_id,
  version_number,
  template_html,
  is_active,
  is_published,
  created_by,
  created_at
)
VALUES
(
  /* template_definition_id */ 1,
  1,
  '<p>Radicado: {{Radicado}}</p><p>Fecha: {{Fecha}}</p><p>{{TextoRespuesta}}</p>',
  1,
  1,
  'system',
  NOW()
);
```

2) Validar cuál versión quedó vigente:
```sql
SELECT template_version_id, version_number, is_active, is_published, created_at
FROM ra_editor_template_versions
WHERE template_definition_id = 1
ORDER BY is_published DESC, version_number DESC, template_version_id DESC;
```

> Recomendación operativa: dejar **una** versión “vigente” (activa y publicada) por definición.

### Paso 4 — Registrar tokens de la definición

1) Ver tokens actuales:
```sql
SELECT template_token_id, token_code, source_type, source_key, is_required, default_value
FROM ra_editor_template_tokens
WHERE template_definition_id = 1
ORDER BY template_token_id;
```

2) Insertar tokens (ejemplos según tu configuración):
```sql
INSERT INTO ra_editor_template_tokens
(
  template_definition_id,
  token_code,
  token_label,
  source_type,
  source_key,
  is_required,
  is_editable,
  default_value,
  created_by,
  created_at
)
VALUES
(1,'Radicado','Radicado','respuesta_api','Radicado',1,0,NULL,'system',NOW()),
(1,'Fecha','Fecha','system','FechaActual',0,0,NULL,'system',NOW()),
(1,'Destinatario','Destinatario','respuesta_api','Destinatario',1,0,NULL,'system',NOW()),
(1,'DireccionDestinatario','Dirección','respuesta_api','DireccionDestinatario',0,0,NULL,'system',NOW()),
(1,'Ciudad','Ciudad','static','Ciudad',0,0,'Villavicencio','system',NOW()),
(1,'Asunto','Asunto','respuesta_api','Asunto',0,0,NULL,'system',NOW()),
(1,'TextoRespuesta','Texto Respuesta','editable','TextoRespuesta',0,1,'','system',NOW());
```

## 4) Validación rápida (antes de probar en UI)

### 4.1 Validar que el HTML solo use tokens configurados

1) Copiar el `template_html` de la versión vigente.
2) Listar los `{{TokenCode}}` del HTML.
3) Confirmar que **cada `TokenCode`** exista en `ra_editor_template_tokens` para ese `template_definition_id`.

### 4.2 Validar `is_required`

Si un token está en el HTML y tiene `is_required=1`, su resolución debe dar valor:
- `respuesta_api`: debe existir la propiedad en la estructura (o mapear correctamente con `source_key`).
- `system/static/editable`: debe resolver por regla o `default_value`.

### 4.3 Validar la estructura de negocio

Para tokens `respuesta_api`, confirmar que la respuesta real de `SolicitaEstructuraRespuestaIdTareaAsync` tenga propiedades con los nombres:
- `source_key` (preferido), o
- `token_code` (fallback, si aplica).

## 5) Troubleshooting: “Faltan tokens requeridos…”

Checklist:
1) ¿El HTML tiene `{{Token}}` con espacios o typo? (`{{ Fecha }}` ≠ `{{Fecha}}`)
2) ¿El `token_code` del catálogo coincide exactamente con el placeholder?
3) ¿El token está en la definición correcta (`template_definition_id` correcto)?
4) Si es `respuesta_api`: ¿la propiedad existe en la estructura y `source_key` coincide?
5) Si no debe bloquear, confirmar `is_required=0`.
