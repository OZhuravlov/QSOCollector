CREATE TABLE premium_callsign (
    id                 INTEGER PRIMARY KEY AUTOINCREMENT,
    callsign           VARCHAR(20)        NOT NULL,
    club               VARCHAR(100)       NOT NULL DEFAULT 'N/A',
    donated_amount_usd DECIMAL(10, 2)     NOT NULL,
    comment			   VARCHAR(500)       NOT NULL DEFAULT '',
    last_update_time   DATETIME           NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE UNIQUE INDEX idx_premium_callsign_callsign_club ON premium_callsign(callsign, club);

CREATE TRIGGER tr_premium_callsign_au
AFTER UPDATE OF callsign, club, donated_amount_usd, comment 
   ON premium_callsign
  FOR EACH ROW 
BEGIN 
  UPDATE premium_callsign 
     SET last_update_time = DATETIME('NOW')
   WHERE rowid = new.rowid;
END;
