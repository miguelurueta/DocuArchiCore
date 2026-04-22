# SCRUM-141 — Integración Frontend — Guardar imagen (Tiptap)

## API

- Método: `POST`
- Ruta: `/api/gestor-documental/editor/guardar-imagen`
- Content-Type: `multipart/form-data`
- Form field: `File`
- Claim requerido: `defaulalias`

## Objetivo

Subir imágenes desde el editor (Tiptap) y obtener un `imageUid`/`imageId` para referenciar la imagen en el contenido del documento.

## Request

### Body (multipart/form-data)

- Campo: `File` (archivo, requerido)
- Tipos permitidos (validación backend): `image/png`, `image/jpeg`, `image/jpg`, `image/webp`
- Tamaño máximo (validación backend): 5MB

## Contrato de respuesta

Tipo: `AppResponses<GuardaEditorImageResponseDto?>`

### Wrapper: `AppResponses<T>`

Definición (DTO real): `..\MiApp.DTOs\DTOs\Utilidades\AppResponses.cs`

- `success` (bool)
- `message` (string): en éxito se retorna `"YES"`
- `data` (T|null)
- `meta` (AppMeta|null)
- `errors` (object[]|null)

### DTO: `GuardaEditorImageResponseDto`

Definición (DTO real): `..\MiApp.DTOs\DTOs\GestorDocumental\Editor\GuardaEditorImageResponseDto.cs`

- `imageId` (long): ID interno de la imagen guardada
- `imageUid` (string): UID estable para referenciar la imagen en el editor
- `fileName` (string)
- `contentType` (string)
- `fileSize` (long): tamaño en bytes
- `storageTypeCode` (string): actualmente `"db"`
- `publicUrl` (string|null): `null` si el storage no expone endpoint público

## Ejemplos

### 1) Ejemplo `curl`

```bash
curl -X POST "https://{host}/api/gestor-documental/editor/guardar-imagen" \
  -H "Authorization: Bearer {token}" \
  -F "File=@C:/tmp/imagen.png"
```

### 2) Ejemplo en frontend (Axios)

```ts
const formData = new FormData();
formData.append("File", file); // IMPORTANTE: el nombre del campo es "File"

const res = await axios.post(
  "/api/gestor-documental/editor/guardar-imagen",
  formData,
  { headers: { "Content-Type": "multipart/form-data" } }
);

// res.data: AppResponses<GuardaEditorImageResponseDto | null>
if (res.data?.success && res.data.data) {
  const { imageUid, publicUrl } = res.data.data;
  const src = publicUrl ?? imageUid; // si no hay publicUrl, el front debe decidir cómo resolver el UID
  // Insertar <img src="..."> en el editor según la estrategia del proyecto
}
```

### 3) Respuesta exitosa (ejemplo)

```json
{
  "success": true,
  "message": "YES",
  "data": {
    "imageId": 123,
    "imageUid": "b0c1f720b7af4c7c9c4f73f9b4f1b0aa",
    "fileName": "imagen.png",
    "contentType": "image/png",
    "fileSize": 53210,
    "storageTypeCode": "db",
    "publicUrl": null
  },
  "meta": null,
  "errors": []
}
```

### 4) Error de validación (archivo faltante)

```json
{
  "success": false,
  "message": "Archivo requerido",
  "data": null,
  "meta": null,
  "errors": [
    {
      "type": "Validation",
      "field": "file",
      "message": "Archivo requerido"
    }
  ]
}
```

### 5) Error de validación (Content-Type no permitido)

```json
{
  "success": false,
  "message": "ContentType no permitido",
  "data": null,
  "meta": null,
  "errors": [
    {
      "type": "Validation",
      "field": "contentType",
      "message": "ContentType no permitido"
    }
  ]
}
```

## Recomendaciones UX

- Antes de enviar: validar tipo y tamaño del archivo (max 5MB).
- En error: mostrar `message` al usuario y loguear `errors` para diagnóstico.

