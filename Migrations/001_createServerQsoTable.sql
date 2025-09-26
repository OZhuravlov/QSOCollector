CREATE TABLE qsodata (
    id                INTEGER      PRIMARY KEY AUTOINCREMENT,
    is_temporary      BOOLEAN      NOT NULL DEFAULT FALSE,
    source_ip_address VARCHAR(20),
    created_time      DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_time       DATETIME,
    is_imported       BOOLEAN      NOT NULL DEFAULT FALSE,
    exported_time     DATETIME,
    qso_time          DATETIME     NOT NULL,
    programid         VARCHAR(20),
    station_callsign  VARCHAR(20)  NOT NULL,
    qso_date          VARCHAR(8),
    qso_date_off      VARCHAR(8),
    call              VARCHAR(15)  NOT NULL,
    time_on           VARCHAR(8),
    time_off          VARCHAR(8),
    band              VARCHAR(6)   NOT NULL,
    freq              DOUBLE,
    freq_rx           DOUBLE,
    mode              VARCHAR(10)  NOT NULL,
    contest_id        VARCHAR(100),
    rst_sent          VARCHAR(3),
    rst_rcvd          VARCHAR(3),
    exch_sent         VARCHAR(20),
    exch_rcvd         VARCHAR(20),
    operator          VARCHAR(20),
    my_gridsquare     VARCHAR(8),
    gridsquare        VARCHAR(8),
    distance          INTEGER,
    comment           VARCHAR(100),
    pfx               VARCHAR(6),
    dxcc_pref         VARCHAR(6),
    cqz               INTEGER,
    ituz              INTEGER,
    cont              VARCHAR(2),
    qslmsg            VARCHAR(100),
    dxcc              INTEGER,
    orig_format       VARCHAR(20) NOT NULL,
    orig_qsodata      TEXT NOT NULL,
    adif_qsodata      TEXT NOT NULL,
    CHECK ((qso_date IS NOT NULL AND time_on IS NOT NULL) OR (qso_date_off IS NOT NULL AND time_off IS NOT NULL))
);

CREATE INDEX idx_qsodata_is_temporary ON qsodata(is_temporary);
CREATE UNIQUE INDEX idx_qsodata_uniq ON qsodata(qso_time, call, band, mode, is_temporary);
CREATE INDEX idx_qsodata_band ON qsodata(band);
CREATE INDEX idx_qsodata_mode ON qsodata(mode);
CREATE INDEX idx_qsodata_operator ON qsodata(operator);
