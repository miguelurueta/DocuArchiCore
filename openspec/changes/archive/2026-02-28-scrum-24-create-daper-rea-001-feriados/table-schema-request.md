# Table Schema Request - `rea_001_feriados`

This change is blocked until table structure is confirmed.

## Required Output

Provide the following for `docuarchi.rea_001_feriados`:

1. Full DDL (`SHOW CREATE TABLE`)
2. Column list with:
   - `COLUMN_NAME`
   - `COLUMN_TYPE`
   - `IS_NULLABLE`
   - `COLUMN_DEFAULT`
   - `COLUMN_KEY`
   - `EXTRA`

## SQL Queries

```sql
SHOW CREATE TABLE docuarchi.rea_001_feriados;
```

```sql
SELECT
  COLUMN_NAME,
  COLUMN_TYPE,
  IS_NULLABLE,
  COLUMN_DEFAULT,
  COLUMN_KEY,
  EXTRA
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'docuarchi'
  AND TABLE_NAME = 'rea_001_feriados'
ORDER BY ORDINAL_POSITION;
```

## Acceptance to Unblock

- Primary key identified.
- Nullability and max lengths confirmed.
- Date/time and numeric precision confirmed.
- Any enum/set values confirmed.
