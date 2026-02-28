# Table Schema Result - `rea_001_feriados`

Captured from local MySQL `docuarchi`:

## Columns

| Column | Type | Nullable | Default | Key | Extra |
|---|---|---|---|---|---|
| `ID_FERIADO` | `int(10) unsigned` | `NO` | `NULL` | `PRI` | `auto_increment` |
| `DIA_FERIADO` | `varchar(120)` | `NO` | `NULL` | `MUL` |  |
| `DESCRIPCION_FERIADO` | `varchar(250)` | `NO` | `NULL` |  |  |
| `FECHA_FERIADO` | `date` | `NO` | `NULL` |  |  |
| `AÑO_FERIADO` | `int(10)` | `NO` | `NULL` |  |  |
| `PAIS_FERIADO` | `varchar(250)` | `NO` | `NULL` |  |  |
| `ESTADO_DIA` | `int(11)` | `NO` | `1` |  |  |

## DDL

```sql
CREATE TABLE `rea_001_feriados` (
  `ID_FERIADO` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `DIA_FERIADO` varchar(120) NOT NULL,
  `DESCRIPCION_FERIADO` varchar(250) NOT NULL,
  `FECHA_FERIADO` date NOT NULL,
  `AÑO_FERIADO` int(10) NOT NULL,
  `PAIS_FERIADO` varchar(250) NOT NULL,
  `ESTADO_DIA` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`ID_FERIADO`),
  UNIQUE KEY `FER_UNO` (`DIA_FERIADO`,`DESCRIPCION_FERIADO`,`FECHA_FERIADO`,`AÑO_FERIADO`,`PAIS_FERIADO`)
) ENGINE=InnoDB AUTO_INCREMENT=496 DEFAULT CHARSET=utf8;
```
