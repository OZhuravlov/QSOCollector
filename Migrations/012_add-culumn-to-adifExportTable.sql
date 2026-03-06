ALTER TABLE adif_export ADD is_auto BOOLEAN NOT NULL DEFAULT false;
CREATE INDEX idx_adif_export_name_is_auto ON adif_export(is_auto);
