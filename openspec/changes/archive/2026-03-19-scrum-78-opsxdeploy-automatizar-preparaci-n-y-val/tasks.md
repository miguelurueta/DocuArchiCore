## 1. Discovery

- [x] 1.1 Confirmar que el alcance real de `SCRUM-78` es una herramienta nueva en `Tools/` para preparar publicaciones orientadas a IIS.
- [x] 1.2 Delimitar el MVP para no hacer obligatorio el despliegue directo a IIS.

## 2. Specs

- [x] 2.1 Reemplazar la spec generica por requisitos concretos para `doctor`, `prepare`, `publish-package` y manual obligatorio.
- [x] 2.2 Alinear `tasks.md` con el alcance real del MVP.

## 3. Application

- [x] 3.1 Crear `Tools/iis-deploy` con `package.json`, `opsxdeploy.ps1` y README.
- [x] 3.2 Implementar `doctor` para validar publish, bloquear artifacts de desarrollo y detectar secretos evidentes.
- [x] 3.3 Implementar `prepare` para crear carpeta del sitio y storage paths operativos.
- [x] 3.4 Implementar `publish-package` para generar una carpeta limpia lista para IIS sin desplegar al sitio.

## 4. Verification

- [x] 4.1 Agregar prueba automatizada del flujo MVP.
- [x] 4.2 Ejecutar la prueba del tool y documentar resultado.
- [x] 4.3 Validar el change con `openspec.cmd validate`.

## 5. Documentation

- [x] 5.1 Agregar manual operativo completo desde Visual Studio hasta IIS.
- [x] 5.2 Documentar el uso del tool y aclarar que el despliegue directo a IIS no es obligatorio.
