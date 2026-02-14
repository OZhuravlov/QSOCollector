CREATE TABLE raw_qsodata (
    id                INTEGER      PRIMARY KEY AUTOINCREMENT,
    source_name       VARCHAR(50),
    orig_format       VARCHAR(20)  NOT NULL,
    orig_qsodata      TEXT         NOT NULL,
    created_at        DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_raw_qsodata_created_at ON raw_qsodata(created_at);
CREATE INDEX idx_raw_qsodata_source ON qsodata(mode);
