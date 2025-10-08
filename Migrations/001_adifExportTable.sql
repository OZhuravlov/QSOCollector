CREATE TABLE adif_export (
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    start_time   DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    end_time     DATETIME,
    folder       VARCHAR(500) NOT NULL,
    file_name    VARCHAR(500) NOT NULL,
    filter       VARCHAR(2000) NOT NULL,
    qso_amount   INTEGER NOT NULL,
    is_confirmed BOOLEAN
);

