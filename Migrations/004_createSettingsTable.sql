CREATE TABLE settings (
    key VARCHAR(100) NOT NULL,
    value VARCHAR(250)
);

CREATE UNIQUE INDEX idx_settings_key ON settings(key);
