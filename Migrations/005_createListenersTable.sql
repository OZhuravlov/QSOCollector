CREATE TABLE listeners (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    protocol VARCHAR(10) NOT NULL,
    qso_port INTEGER NOT NULL,
    acknowledge_port INTEGER,
    message_format VARCHAR(10) NOT NULL,
    description VARCHAR(100),
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE UNIQUE INDEX idx_listeners_qso_port ON listeners(qso_port);
CREATE UNIQUE INDEX idx_listeners_acknowledge_port ON listeners(acknowledge_port);
