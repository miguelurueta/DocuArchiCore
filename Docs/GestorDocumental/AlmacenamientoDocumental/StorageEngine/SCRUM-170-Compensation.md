# SCRUM-170 Compensation

## Definición
Compensación es limpieza de artefactos físicos cuando la fase FS/XML falla después del commit DB.

## Escenarios
1. DB commit OK + FS OK + XML OK -> `Completed`.
2. DB commit OK + FS FAIL -> compensación.
3. DB commit OK + FS OK + XML FAIL -> compensación.

## Qué limpia
- archivos finales creados.
- archivos temporales `.tmp`.
- XML final/temporal.
- directorios vacíos creados por la fase física.

## Límite
- no revierte inserciones DB ya comprometidas.
