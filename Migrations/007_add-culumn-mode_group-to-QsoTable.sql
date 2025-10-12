ALTER TABLE qsodata ADD mode_group VARCHAR(10);

UPDATE qsodata SET mode_group = 
	CASE 
	    WHEN UPPER(mode) = 'CW' THEN 'CW'
		WHEN UPPER(mode) IN ('SSB', 'FM', 'AM', 'LSB', 'USB') THEN 'PHONE'
		WHEN UPPER(mode) IN ('RTTY', 'PSK31', 'PSK63', 'FT8', 'FT4', 'JT65', 'JT9') THEN 'DATA'
		WHEN UPPER(mode) = 'SAT' THEN 'SAT'
		ELSE 'OTHER'
	END;

CREATE INDEX idx_qsodata_source_name ON qsodata(mode_group);
