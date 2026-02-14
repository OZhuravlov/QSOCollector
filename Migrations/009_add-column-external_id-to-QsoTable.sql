ALTER TABLE qsodata ADD external_id VARCHAR(64);
CREATE INDEX idx_qsodata_external_id ON qsodata(external_id);