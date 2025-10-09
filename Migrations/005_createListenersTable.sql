CREATE TABLE listeners (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name VARCHAR(50) NOT NULL,
    qso_port INTEGER NOT NULL,
    forward_port INTEGER,
    acknowledge_port INTEGER,
    message_format VARCHAR(10) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE UNIQUE INDEX idx_listeners_qso_port ON listeners(qso_port);
CREATE UNIQUE INDEX idx_listeners_acknowledge_port ON listeners(acknowledge_port);
CREATE UNIQUE INDEX idx_listeners_forward_port ON listeners(forward_port);
