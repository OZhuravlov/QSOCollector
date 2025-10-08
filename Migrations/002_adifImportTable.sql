CREATE TABLE adif_import (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    start_time  DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    end_time    DATETIME,
    folder      VARCHAR(500) NOT NULL,
    file_name   VARCHAR(500) NOT NULL,
    qso_amount  INTEGER NOT NULL
);
