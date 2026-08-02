CREATE TABLE bands (
    id                 INTEGER     PRIMARY KEY AUTOINCREMENT,
    name               VARCHAR(10) NOT NULL UNIQUE,
    alt_name           VARCHAR(10) NOT NULL UNIQUE,
    n1mm_name          VARCHAR(10) NOT NULL UNIQUE,
    freq_mhz_from      DOUBLE      NOT NULL,
    freq_mhz_to        DOUBLE      NOT NULL,
    designator		   VARCHAR(2)  UNIQUE,
    created_time       DATETIME    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    is_active          BOOLEAN     NOT NULL DEFAULT TRUE
);

INSERT INTO bands (name, alt_name, n1mm_name, freq_mhz_from, freq_mhz_to, designator, is_active)
VALUES ('160m', '1.8 mHz', '1.8', 1.800, 2.000, null, 1),
       ('80m', '3.5 mHz', '3.5', 3.500, 4.000, null, 1),
       ('60m', '5 mHz', '5', 5.06, 5.45, null, 1),
       ('40m', '7 mHz', '7', 7.000, 7.300, null, 1),
       ('30m', '10 mHz', '10', 10.100, 10.150, null, 1),
       ('20m', '14 mHz', '14', 14.000, 14.350, null, 1),
       ('17m', '18 mHz', '18', 18.068, 18.168, null, 1),
       ('15m', '21 mHz', '21', 21.000, 21.450, 'H', 1),
       ('12m', '24 mHz', '24', 24.890, 24.990, null, 1),
       ('10m', '28 mHz', '28', 28.000, 29.700, 'A', 1),
       ('6m', '50 mHz', '50', 50.000, 54.000, null, 1),
       ('4m', '70 mHz', '70', 69.900, 70.500, null, 0),
       ('2m', '144 mHz', '144', 144.000, 148.000, 'V', 1),
       ('70cm', '432 mHz', '432', 420.000, 450.000, 'U', 1),
       ('33cm', '902 mHz', '902', 902.000, 928.000, null, 0),
       ('23cm', '1.2 GHz', '1.2GHz', 1240.000, 1300.000, 'L', 0),
       ('13cm', '2.4 GHz', '2.4GHz', 2400.000, 2450.000, 'S', 1),
       ('9cm', '3.4 GHz', '3.4GHz', 3300.000, 3500.000, 'S2', 0),
       ('5cm', '5.7 GHz', '5.7GHz', 5650.000, 5925.000, 'C', 0),
       ('3cm', '10 GHz', '10GHz', 10000.000, 10500.000, 'X', 1),
       ('1.2cm', '24 GHz', '24GHz', 24000.000, 24250.000, 'K', 0),
       ('6mm', '47 GHz', '47GHz', 47000.000, 47200.000, 'R', 0);

CREATE TABLE sat_rules (
    id                 INTEGER     PRIMARY KEY AUTOINCREMENT,
    name               VARCHAR(50) NOT NULL UNIQUE,
    orig_freq_mhz_from DOUBLE      NOT NULL,
    orig_freq_mhz_to   DOUBLE      NOT NULL,
    propagation_mode   VARCHAR(50) NOT NULL,
    sat_name 		   VARCHAR(50) NOT NULL,
    sat_mode 		   VARCHAR(10),
    band_id            INTEGER,
    band_rx_id         INTEGER,
    freq_mhz           DOUBLE,
    freq_mhz_rx        DOUBLE,
    created_time       DATETIME    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    is_active          BOOLEAN     NOT NULL DEFAULT TRUE,
    FOREIGN KEY (band_id)          REFERENCES bands(id),
    FOREIGN KEY (band_rx_id)       REFERENCES bands(id)
);

CREATE INDEX idx_sat_rules_band_id ON sat_rules (band_id);
CREATE INDEX idx_sat_rules_band_rx_id ON sat_rules (band_rx_id);

INSERT INTO sat_rules (name, orig_freq_mhz_from, orig_freq_mhz_to, propagation_mode, sat_name, sat_mode, band_id, band_rx_id, freq_mhz_rx) 
VALUES ('QO100 2.4/10', 2400, 2400.50, 'SAT', 'QO-100', 'SX', 
         (SELECT id FROM bands WHERE name = '13cm'), 
         (SELECT id FROM bands WHERE name = '3cm'), 
         10489.0);

CREATE TABLE sat_modes (
    id                 INTEGER     PRIMARY KEY AUTOINCREMENT,
    name               VARCHAR(10) NOT NULL UNIQUE,
    uplink_band_id     INTEGER     NOT NULL,
    downlink_band_id   INTEGER     NOT NULL,
    created_time       DATETIME    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    is_active          BOOLEAN     NOT NULL DEFAULT TRUE,
    FOREIGN KEY (uplink_band_id)   REFERENCES bands(id),
    FOREIGN KEY (downlink_band_id) REFERENCES bands(id)
);

CREATE INDEX idx_sat_modes_uplink_band_id ON sat_modes (uplink_band_id);
CREATE INDEX idx_sat_modes_downlink_band_id ON sat_modes (downlink_band_id);

INSERT INTO sat_modes (name, uplink_band_id, downlink_band_id)
VALUES ('SX', (SELECT id FROM bands WHERE name = '13cm'), (SELECT id FROM bands WHERE name = '3cm')),
       ('UV', (SELECT id FROM bands WHERE name = '70cm'), (SELECT id FROM bands WHERE name = '2m')),
       ('VU', (SELECT id FROM bands WHERE name = '2m'), (SELECT id FROM bands WHERE name = '70cm')),
       ('US', (SELECT id FROM bands WHERE name = '70cm'), (SELECT id FROM bands WHERE name = '13cm')),
       ('LS', (SELECT id FROM bands WHERE name = '23cm'), (SELECT id FROM bands WHERE name = '13cm')),
       ('LU', (SELECT id FROM bands WHERE name = '23cm'), (SELECT id FROM bands WHERE name = '70cm'));

CREATE TABLE listener_sat_rules (
    id                 INTEGER  PRIMARY KEY AUTOINCREMENT,
    listener_id        INTEGER  NOT NULL,
    rule_id            INTEGER  NOT NULL,
    created_time       DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (listener_id)   REFERENCES  listeners(id),
    FOREIGN KEY (rule_id)       REFERENCES sat_rules(id),
    UNIQUE (listener_id, rule_id)
);

ALTER TABLE qsodata ADD band_rx VARCHAR(6);
ALTER TABLE qsodata ADD prop_mode VARCHAR(10);
ALTER TABLE qsodata ADD sat_name VARCHAR(50);
ALTER TABLE qsodata ADD sat_mode VARCHAR(10);
