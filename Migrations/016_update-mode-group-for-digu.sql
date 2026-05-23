UPDATE qsodata SET mode_group = 'DATA'
 WHERE UPPER(mode) = 'DIGU'
   AND mode_group != 'DATA';
