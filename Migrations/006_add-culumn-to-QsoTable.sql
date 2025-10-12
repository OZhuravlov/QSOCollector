ALTER TABLE qsodata ADD source_name VARCHAR(50);

DROP INDEX idx_qsodata_programid;
CREATE INDEX idx_qsodata_source_name ON qsodata(source_name);
