CREATE TABLE qso_export_scheduler (
    hour CHAR(2)            NOT NULL,
    saved_at   DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE UNIQUE INDEX idx_qso_export_scheduler_hour ON qso_export_scheduler(hour);
