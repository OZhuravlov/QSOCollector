using Microsoft.Data.Sqlite;
using QSOCollector.Models;
using Serilog;
using System.Data;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Band = QSOCollector.Models.Band;

namespace QSOCollector.Data
{
    public class DbRepository : IDbRepository
    {
        private readonly ILogger log = Log.ForContext<DbRepository>();
        private readonly int maxMinuteIntervalToReplaceQso = 5;

        private const string getTableColumnsSql = "SELECT UPPER(p.name) name, UPPER(p.type) type FROM sqlite_master m JOIN pragma_table_info(m.name) p WHERE lower(m.name) = lower(@tablename) and not p.pk";
        private const string selectSettingsSql = "SELECT key, value FROM settings";
        private const string insertSettingsSql = "INSERT OR REPLACE INTO settings (key, value) VALUES (@key, @value)";
        private const string getListenerConfigsSql = "SELECT name as Name, id as Id, qso_port as QsoPort, forward_port as ForwardPort, acknowledge_port as AcknowledgePort, message_format as MessageFormat, is_active IsActive FROM listeners WHERE is_active = true";
        private const string getBandsSql = "SELECT id Id, name BandName, alt_name AltBandName, name || '(' || alt_name || ')' FullBandName, n1mm_name N1mmBandName, freq_mhz_from FreqFrom, freq_mhz_to FreqTo, designator Designator, is_active IsActive FROM bands";
        private const string getExportSchedulerHoursSql = "SELECT hour FROM qso_export_scheduler order by hour";
        private const string insertListenerConfigsSql = "INSERT INTO listeners (name, qso_port, forward_port, acknowledge_port, message_format, is_active) " +
                    " VALUES (@Name, @QsoPort, @ForwardPort, @AcknowledgePort, @MessageFormat, @IsActive)";
        private const string getServerQsoAmountsSql = "SELECT q.mode QsoAmountMode, COUNT(CASE WHEN q.qso_time >= current_date THEN 1 END) TodayQsoAmount, count(*) TotalQsoAmount, COUNT(e.id) ExportedQsoAmount, MAX(q.qso_time) LastQsoTime, MAX(e.end_time) LastExportedQsoTime FROM qsodata q LEFT JOIN adif_export e ON q.export_id = e.id AND e.is_confirmed = true WHERE q.is_temporary = false GROUP BY q.mode UNION ALL SELECT 'Total', COUNT(CASE WHEN q.qso_time >= current_date THEN 1 END), COUNT(*), COUNT(e.id), MAX(q.qso_time), MAX(e.end_time) FROM qsodata q LEFT JOIN adif_export e ON q.export_id = e.id AND e.is_confirmed = true WHERE q.is_temporary = false";
        private const string getQsoAmountsForExportSql = "SELECT COALESCE(q.source_name, '<UNKNOWN>') SourceName, e.id IS NOT NULL IsExported, DATE(q.qso_time) QsoDate, q.mode_group ModeGroup, q.mode Mode, q.band Band, COALESCE(q.operator, '<UNKNOWN>') Operator, COALESCE(q.source_ip_address, '<UNKNOWN>') SourceIp, count(*) Count FROM qsodata q LEFT JOIN adif_export e ON q.export_id = e.id AND e.is_confirmed = true WHERE q.is_temporary = false GROUP BY COALESCE(q.source_name, '<UNKNOWN>'), e.id IS NOT NULL, DATE(q.qso_time), q.mode_group, q.mode, q.band, COALESCE(q.operator, '<UNKNOWN>'), COALESCE(q.source_ip_address, '<UNKNOWN>')";
        private const string insertRawQsoSql = "INSERT INTO raw_qsodata (source_name, orig_format, orig_qsodata, is_replace) VALUES (@Source, @OriginalFormat, @OriginalQsoData, @Replace)";
        private const string insertQsoSql = "INSERT INTO qsodata (is_temporary, source_name, source_ip_address, external_id, import_id, qso_time, programid, station_callsign, qso_date, qso_date_off, call, time_on, time_off, band, freq, freq_rx, mode, mode_group, contest_id, rst_sent, rst_rcvd, exch_sent, exch_rcvd, operator, my_gridsquare, gridsquare, distance, comment, pfx, dxcc_pref, cqz, ituz, cont, qslmsg, dxcc, orig_format, orig_qsodata, adif_qsodata, is_replace, band_rx, prop_mode, sat_name, sat_mode)" +
                    " VALUES (@is_temporary, @source_name, @source_ip_address, @external_id, @import_id, @qso_time, @programid, @station_callsign, @qso_date, @qso_date_off, @call, @time_on, @time_off, @band, @freq, @freq_rx, @mode, @mode_group, @contest_id, @rst_sent, @rst_rcvd, @exch_sent, @exch_rcvd, @operator, @my_gridsquare, @gridsquare, @distance, @comment, @pfx, @dxcc_pref, @cqz, @ituz, @cont, @qslmsg, @dxcc, @orig_format, @orig_qsodata, @adif_qsodata, @is_replace, @band_rx, @prop_mode, @sat_name, @sat_mode)";
        private const string getTemporaryQsoSql = "SELECT id, source_name, orig_format, orig_qsodata, adif_qsodata, is_replace " +
            "  FROM qsodata " +
            " WHERE is_temporary = 1 AND orig_format IS NOT NULL AND orig_qsodata IS NOT NULL " +
            " ORDER BY id " +
            " LIMIT 100";
        private const string deleteQsoQsl = "DELETE FROM qsodata WHERE id = @id";
        private const string selectQsoToReplaceQsl = "SELECT id, export_id FROM qsodata WHERE external_id = @externalId AND qso_time BETWEEN @minTime AND @maxTime AND is_temporary = @isTemporary ORDER BY id DESC LIMIT 1";
        private const string selectSatRulesSql = "SELECT id Id, name Name, orig_freq_mhz_from SourceFreqFrom, orig_freq_mhz_to SourceFreqTo, propagation_mode PropagationMode, sat_name SatName, sat_mode SatMode, band_id BandTxId, band_rx_id BandRxId, freq_mhz FreqTx, freq_mhz_rx FreqRx, is_active IsActive FROM sat_rules";

        private readonly string connectionString;
        private Dictionary<string, string>? qsodataColumns = null;

        public DbRepository(string dbConnectionString)
        {
            this.connectionString = dbConnectionString;
            log.Debug("Initialized DbRepository with connection string: {connectionString}", dbConnectionString);
        }

        public string GetConnectionString() { 
            return connectionString;
        }

        public Dictionary<string, string?> LoadSettings()
        {
            var settings = new Dictionary<string, string?>();
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = selectSettingsSql;
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                string key = reader.GetString(0);
                string? value = reader.GetString(1);
                settings[key] = value;
            }
            return settings;
        }

        public void SaveSetting(string key, string? value)
        {
            log.Debug("Saving setting to database: {key} = {value}", key, value);
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = insertSettingsSql;
                command.Parameters.Add(new SqliteParameter("@key", key));
                if (value != null) {
                    command.Parameters.Add(new SqliteParameter("@value", value));
                } else {
                    command.Parameters.Add(new SqliteParameter("@value", DBNull.Value));
                }
                command.ExecuteNonQuery();
                transaction.Commit();
            }
            catch (SqliteException)
            {
                transaction.Rollback();
                throw;
            }
        }

        public List<ListenerConfig> GetListenerConfigs()
        {
            log.Verbose("Loading listener configurations from database");
            return GetData(getListenerConfigsSql, reader => Map<ListenerConfig>(reader));
        }

        public void ReplaceListenerConfigs(List<ListenerConfig> configs)
        {
            log.Debug("Replacing listener configurations in database with {count} configs", configs.Count);
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            CleanupListenerConfigs(connection, configs);
            SaveListenerConfigs(connection, configs);
            transaction.Commit();
        }

        public string GetListenerConcatRuleNames(int listenerId) {
            log.Verbose("Get listener concatenated rule names from database");
            string sql = "select group_concat(sr.name, '; ') rules from listener_sat_rules lr join sat_rules sr on lr.rule_id = sr.id where lr.listener_id = @listenerId";
            return GetData(sql, reader => MapField<string>(reader, "rules"), [new SqliteParameter("@listenerId", listenerId)])[0];
        }

        public Band GetBand(int id)
        {
            log.Verbose("Get band with id {id} from database", id);
            string sql = getBandsSql + " WHERE id = @id";
            var bands = GetData(sql, reader => Map<Band>(reader), [new SqliteParameter("@id", id)]);
            return bands.FirstOrDefault() ?? throw new InvalidOperationException("Band not found");
        }

        public List<Band> GetBands(bool isActiveOnly = false)
        {
            log.Verbose("Get bands from database, isActiveOnly: {isActiveOnly}", isActiveOnly);
            string sql = getBandsSql;
            if (isActiveOnly)
            {
                sql += " WHERE is_active = 1";
            }
            return GetData(sql, reader => Map<Band>(reader));
        }

        public List<SatRule> GetSatRules()
        {
            log.Verbose("Get satellite rules from database");
            List<SatRule> satRules = GetData(selectSatRulesSql, reader => Map<SatRule>(reader, true));
            satRules.ForEach(EnrichSatRule);
            return satRules;
        }

        public SatRule GetSatRule(int id)
        {
            log.Verbose("Get satellite rule with id {id} from database", id);
            string sql = selectSatRulesSql + " WHERE id = @id";
            var rules = GetData(sql, reader => Map<SatRule>(reader, true), [new SqliteParameter("@id", id)]);
            SatRule rule = rules.FirstOrDefault() ?? throw new InvalidOperationException("Rule not found");
            EnrichSatRule(rule);
            return rule;
        }

        public void SaveSatRule(SatRule rule)
        {
            log.Information("Saving satellite rule to database: {rule}", rule);
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            using var command = connection.CreateCommand();
            if (rule.Id.HasValue)
            {
                command.CommandText = "UPDATE sat_rules SET name = @name, orig_freq_mhz_from = @orig_freq_mhz_from, orig_freq_mhz_to = @orig_freq_mhz_to, propagation_mode = @propagation_mode, sat_name = @sat_name, sat_mode = @sat_mode, band_id = @band_id, band_rx_id = @band_rx_id, freq_mhz = @freq_mhz, freq_mhz_rx = @freq_mhz_rx, is_active = @is_active WHERE id = @id";
                command.Parameters.Add(new SqliteParameter("@id", rule.Id.Value));
            }
            else
            {
                command.CommandText = "INSERT INTO sat_rules (name, orig_freq_mhz_from, orig_freq_mhz_to, propagation_mode, sat_name, sat_mode, band_id, band_rx_id, freq_mhz, freq_mhz_rx, is_active) VALUES (@name, @orig_freq_mhz_from, @orig_freq_mhz_to, @propagation_mode, @sat_name, @sat_mode, @band_id, @band_rx_id, @freq_mhz, @freq_mhz_rx, @is_active)";
            }
            command.Parameters.Add(new SqliteParameter("@name", rule.Name));
            command.Parameters.Add(new SqliteParameter("@orig_freq_mhz_from", rule.SourceFreqFrom));
            command.Parameters.Add(new SqliteParameter("@orig_freq_mhz_to", rule.SourceFreqTo));
            command.Parameters.Add(new SqliteParameter("@propagation_mode", rule.PropagationMode));
            command.Parameters.Add(new SqliteParameter("@sat_name", rule.SatName));
            command.Parameters.Add(new SqliteParameter("@sat_mode", rule.SatMode != null ? rule.SatMode : DBNull.Value));
            command.Parameters.Add(new SqliteParameter("@band_id", rule.BandTxId.HasValue ? rule.BandTxId.Value : DBNull.Value));
            command.Parameters.Add(new SqliteParameter("@band_rx_id", rule.BandRxId.HasValue ? rule.BandRxId.Value : DBNull.Value));
            command.Parameters.Add(new SqliteParameter("@freq_mhz", rule.FreqTx.HasValue ? rule.FreqTx.Value : DBNull.Value));
            command.Parameters.Add(new SqliteParameter("@freq_mhz_rx", rule.FreqRx.HasValue ? rule.FreqRx.Value : DBNull.Value));
            command.Parameters.Add(new SqliteParameter("@is_active", rule.IsActive ? 1 : 0));
            command.ExecuteNonQuery();
            transaction.Commit();
        }

        public void DeleteSatRule(int id)
        {
            log.Information("Deleting satellite rule from database: {id}", id);
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM sat_rules WHERE id = @id";
            command.Parameters.Add(new SqliteParameter("@id", id));
            command.ExecuteNonQuery();
            transaction.Commit();
        }

        public void DeactivateSatRule(int id)
        {
            log.Information("Deactivating satellite rule in database: {id}", id);
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE sat_rules SET is_active = 0 WHERE id = @id and is_active = 1";
            command.Parameters.Add(new SqliteParameter("@id", id));
            command.ExecuteNonQuery();
            transaction.Commit();
        }

        public void ActivateSatRule(int id)
        {
            log.Information("Activating satellite rule in database: {id}", id);
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE sat_rules SET is_active = 1 WHERE id = @id and is_active = 0";
            command.Parameters.Add(new SqliteParameter("@id", id));
            command.ExecuteNonQuery();
            transaction.Commit();
        }

        public void RemoveRulesFromListener(int listenerId, List<int> selectedRuleIds)
        {
            log.Information("Removing satellite rules from listener: {listenerId}", listenerId);
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            using var command = connection.CreateCommand();
            command.Parameters.Add(new SqliteParameter("@listenerId", listenerId));

            string[] paramArray = [.. selectedRuleIds.Select((id, i) => "@ruleId" + i)];
            string paramList = string.Join(",", paramArray);
            string sql = "DELETE FROM listener_sat_rules WHERE listener_id = @listenerId AND rule_id IN ({0})";
            command.CommandText = string.Format(sql, paramList);
            for (int i = 0; i < selectedRuleIds.Count; ++i)
            {
                command.Parameters.Add(new SqliteParameter("@ruleId" + i, selectedRuleIds[i]));
            }

            command.ExecuteNonQuery();
            transaction.Commit();
        }

        public void AssignRuleToListener(int listenerId, int ruleId) { 
            log.Information("Assigning satellite rule to listener: {listenerId}, {ruleId}", listenerId, ruleId);
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO listener_sat_rules (listener_id, rule_id) VALUES (@listenerId, @ruleId)";
            command.Parameters.Add(new SqliteParameter("@listenerId", listenerId));
            command.Parameters.Add(new SqliteParameter("@ruleId", ruleId));
            command.ExecuteNonQuery();
        }

        public List<SatRule> GetListenerSatRules(int listenerId) {
            log.Verbose("Getting satellite rules for listener: {listenerId}", listenerId);
            string sql = "SELECT sr.id Id, sr.name Name, sr.orig_freq_mhz_from SourceFreqFrom, sr.orig_freq_mhz_to SourceFreqTo, sr.propagation_mode PropagationMode, sr.sat_name SatName, sr.sat_mode SatMode, sr.band_id BandTxId, sr.band_rx_id BandRxId, sr.freq_mhz FreqTx, sr.freq_mhz_rx FreqRx, sr.is_active IsActive FROM listener_sat_rules lr JOIN sat_rules sr ON lr.rule_id = sr.id WHERE lr.listener_id = @listenerId";
            List<SatRule> satRules = GetData(sql, reader => Map<SatRule>(reader, true), [new SqliteParameter("@listenerId", listenerId)]);
            satRules.ForEach(EnrichSatRule);
            return satRules;
        }


        public List<string> GetSatModes()
        {
            log.Verbose("Getting satellite modes from database");
            return GetData("SELECT name FROM sat_modes", reader => MapField<string>(reader, "name"));
        }

        public void CleanClientQsos()
        {
            log.Debug("Cleaning temporary QSO data from database");
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM qsodata WHERE is_temporary = 1";
            command.ExecuteNonQuery();
            transaction.Commit();
        }

        public void CleanupServerQsoData()
        {
            log.Warning("Cleaning server QSO data from database. This will delete all non-temporary QSOs and all import and export records. This action cannot be undone.");
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            using var deleteQsoCommand = connection.CreateCommand();
            deleteQsoCommand.CommandText = "DELETE FROM qsodata WHERE is_temporary = 0";
            deleteQsoCommand.ExecuteNonQuery();
            using var deleteImports = connection.CreateCommand();
            deleteImports.CommandText = "DELETE FROM adif_import";
            deleteImports.ExecuteNonQuery();
            using var deleteExports = connection.CreateCommand();
            deleteExports.CommandText = "DELETE FROM adif_export";
            deleteExports.ExecuteNonQuery();
            transaction.Commit();
        }

        public void CleanRawQsoData()
        {
            log.Warning("Cleaning client raw QSO data from database");
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM raw_qsodata";
            command.ExecuteNonQuery();
            transaction.Commit();
        }

        private void CleanupListenerConfigs(SqliteConnection connection, List<ListenerConfig> configs)
        {
            log.Warning("Cleaning listener configurations from database");
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM listeners";
            command.ExecuteNonQuery();
        }

        private void SaveListenerConfigs(SqliteConnection connection, List<ListenerConfig> configs)
        {
            log.Information("Saving listener configurations to database: {count} configs", configs.Count);
            configs.ForEach(config =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = insertListenerConfigsSql;
                AddSqlParameter(command, "Name", config.Name);
                AddSqlParameter(command, "QsoPort", config.QsoPort);
                AddSqlParameter(command, "ForwardPort", config.ForwardPort);
                AddSqlParameter(command, "AcknowledgePort", config.AcknowledgePort);
                AddSqlParameter(command, "MessageFormat", config.MessageFormat);
                AddSqlParameter(command, "IsActive", config.IsActive);
                command.ExecuteNonQuery();
            });
        }

        public List<ServerQsoAmount> GetServerQsoAmounts()
        {
            log.Verbose("Getting server QSO amounts from database");
            return GetData(getServerQsoAmountsSql, reader => Map<ServerQsoAmount>(reader));
        }

        public List<QsoExportExpectedAmounts> GetQsoAmountsForExport()
        {
            log.Verbose("Getting QSO amounts for export from database");
            return GetData(getQsoAmountsForExportSql, reader => Map<QsoExportExpectedAmounts>(reader));
        }

        private Dictionary<string, string> GetTableColumns(string tablename)
        {
            log.Verbose("Getting table columns for {tablename} from database", tablename);
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = getTableColumnsSql;
            command.Parameters.Add(new SqliteParameter("@tablename", tablename));
            using var reader = command.ExecuteReader();
            Dictionary<string, string> columns = [];
            while (reader.Read())
            {
                columns.Add(reader.GetString(0), reader.GetString(1));
            }
            return columns;
        }

        public void ForceImportQsoRecords(List<Dictionary<string, string?>> dups, string folder, string fileName, Func<string, Task> progressUpdater)
        {
            log.Information("Force saving duplicate QSOs to database. This will delete all duplicates of the QSOs to be imported and then save the QSOs again. This action cannot be undone.");
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                progressUpdater.Invoke("Force saving duplicate QSOs to database ... prepare");
                using var commandImportId = connection.CreateCommand();
                commandImportId.CommandText = "SELECT MAX(id) id FROM adif_import WHERE end_time IS NOT NULL AND folder = @folder AND file_name = @fileName";
                commandImportId.Parameters.Add(new SqliteParameter("@folder", folder));
                commandImportId.Parameters.Add(new SqliteParameter("@fileName", fileName));
                SqliteDataReader reader = commandImportId.ExecuteReader();
                reader.Read();
                int importId = reader.GetInt32(0);
                reader.Close();

                int deleted = 0;
                dups.ForEach(dup =>
                {
                    using var commandImportId = connection.CreateCommand();
                    commandImportId.CommandText = "DELETE FROM qsodata WHERE qso_time = @qso_time AND station_callsign = @station_callsign AND band = @band AND mode = @mode AND is_temporary = false";
                    commandImportId.Parameters.Add(new SqliteParameter("@qso_time", dup["QSO_TIME"]));
                    commandImportId.Parameters.Add(new SqliteParameter("@station_callsign", dup["STATION_CALLSIGN"]));
                    commandImportId.Parameters.Add(new SqliteParameter("@band", dup["BAND"]));
                    commandImportId.Parameters.Add(new SqliteParameter("@mode", dup["MODE"]));
                    deleted += commandImportId.ExecuteNonQuery();
                    if (deleted % 10 == 0)
                    {
                        string logMessage = $"Deleted {deleted} duplicates of {dups.Count}";
                        log.Verbose("logMessage: {@dup}", dup);
                        progressUpdater.Invoke(logMessage);
                    }
                });
                progressUpdater.Invoke($"Deleted {deleted} duplicates of {dups.Count}");

                ExecuteSavingQsoRecords(connection, dups, importId, progressUpdater: progressUpdater);

                transaction.Commit();
            }
            catch (SqliteException ex)
            {
                log.Error(ex, "Error force saving duplicate QSOs to database. Rolling back transaction.");
                transaction.Rollback();
                throw;
            }
        }

        public List<Dictionary<string, string?>> ImportQsoRecords(List<Dictionary<string, string?>> qsoRecords, string folder, string fileName, Func<string, Task> progressUpdater)
        {
            log.Information("Saving QSOs to database. Returned a list of dups");
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            List<Dictionary<string, string?>> dups = [];
            try
            {
                progressUpdater.Invoke("Saving QSOs to database ... prepare");
                using var commandImport = connection.CreateCommand();
                commandImport.CommandText = "INSERT INTO adif_import (folder, file_name, qso_amount) VALUES (@folder, @fileName, @qsoAmount)";
                commandImport.Parameters.Add(new SqliteParameter("@folder", folder));
                commandImport.Parameters.Add(new SqliteParameter("@fileName", fileName));
                commandImport.Parameters.Add(new SqliteParameter("@qsoAmount", qsoRecords.Count));
                commandImport.ExecuteNonQuery();

                using var commandImportId = connection.CreateCommand();
                commandImportId.CommandText = "SELECT MAX(id) id FROM adif_import WHERE end_time IS NULL AND folder = @folder AND file_name = @fileName AND qso_amount = @qsoAmount";
                commandImportId.Parameters.Add(new SqliteParameter("@folder", folder));
                commandImportId.Parameters.Add(new SqliteParameter("@fileName", fileName));
                commandImportId.Parameters.Add(new SqliteParameter("@qsoAmount", qsoRecords.Count));
                SqliteDataReader reader = commandImportId.ExecuteReader();
                reader.Read();
                int importId = reader.GetInt32(0);
                reader.Close();

                dups = ExecuteSavingQsoRecords(connection, qsoRecords, importId, progressUpdater: progressUpdater);

                using var commandImportComplete = connection.CreateCommand();
                commandImportComplete.CommandText = "UPDATE adif_import SET end_time = CURRENT_TIMESTAMP WHERE id = @id";
                commandImportComplete.Parameters.Add(new SqliteParameter("@id", importId));
                commandImportComplete.ExecuteNonQuery();

                transaction.Commit();

                log.Information("Finished saving QSOs to database: saved - {successCount}, skipped {dupsCount} dups", qsoRecords.Count - dups.Count, dups.Count);
                return dups;
            }
            catch (SqliteException ex)
            {
                log.Error(ex, "Error saving QSOs to database. Rolling back transaction.");
                transaction.Rollback();
                throw;
            }
        }

        public void SaveQsoRecords(List<Dictionary<string, string?>> qsoRecords, int? importId = null, bool isTemporary = false)
        {
            log.Information("Saving QSOs to database. ImportId: {importId}, IsTemporary: {isTemporary}, QSO count: {qsoCount}", importId, isTemporary, qsoRecords.Count);
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                ExecuteSavingQsoRecords(connection, qsoRecords, importId, isTemporary: isTemporary);
                log.Debug("Finished saving QSOs to database. Successfully saved {successCount} QSOs.", qsoRecords.Count);
                log.Verbose("Saved QSOs: {@qsoRecords}", qsoRecords);
                transaction.Commit();
            }
            catch (SqliteException ex)
            {
                log.Error(ex, "Error saving QSOs to database. Rolling back transaction.");
                transaction.Rollback();
                throw;
            }
        }

        private List<Dictionary<string, string?>> ExecuteSavingQsoRecords(
            SqliteConnection connection,
            List<Dictionary<string, string?>> qsoRecords,
            int? importId,
            bool isTemporary = false,
            Func<string, Task>? progressUpdater = null)
        {
            int n = 0;
            int dupsCount = 0;
            int succCount = 0;
            List<Dictionary<string, string?>> dups = [];
            qsodataColumns ??= GetTableColumns("qsodata");
            foreach (var qsoRecord in qsoRecords)
            {
                if (log.IsEnabled(Serilog.Events.LogEventLevel.Verbose)) { 
                    string qsoRecordJson = JsonSerializer.Serialize(qsoRecord);
                    log.Verbose("Save QSO: {qsoRecord}", qsoRecordJson);
                }
                n++;
                using var command = connection.CreateCommand();
                command.CommandText = insertQsoSql;

                List<string> parameterKeys = GetParameterKeys(command.CommandText);
                List<string> addedParamKeys = [];
                foreach (var item in qsoRecord)
                {
                    qsodataColumns.TryGetValue(item.Key.ToUpper(), out string? columnType);
                    if (columnType == null) continue; // skip unknown columns
                    object columnValue = ConvertValueToColumnDatatype(columnType, item.Value);
                    string paramKey = item.Key.ToLower();
                    AddSqlParameter(command, paramKey, columnValue, addedParamKeys);
                }

                AddSqlParameter(command, "import_id", importId, addedParamKeys);
                AddSqlParameter(command, "is_temporary", isTemporary, addedParamKeys);

                parameterKeys.RemoveAll(key => addedParamKeys.Contains(key));

                foreach (var paramKey in parameterKeys)
                {
                    qsodataColumns.TryGetValue(paramKey.ToUpper(), out string? columnType);
                    if (columnType == null) continue; // skip unknown columns
                    object columnValue = columnType == "BOOLEAN" ? false : DBNull.Value;
                    AddSqlParameter(command, paramKey, columnValue, addedParamKeys);
                }

                parameterKeys.RemoveAll(key => addedParamKeys.Contains(key));

                HandlePosibleQsoReplacement(connection, qsoRecord, isTemporary);

                try
                {
                    command.ExecuteNonQuery();
                    succCount++;
                }
                catch (SqliteException ex)
                {
                    if (ex.SqliteExtendedErrorCode != IDbRepository.SQLITE_CONSTRAINT_UNIQUE) throw;
                    dups.Add(qsoRecord);
                    dupsCount++;
                }
                if (n % 10 == 0)
                {
                    progressUpdater?.Invoke($"Saved: {succCount} + dups: {dupsCount} = {n} of {qsoRecords.Count}");
                }
            }
            string logMessage = $"Saved: {succCount} + dups: {dupsCount} = {n} of {qsoRecords.Count}";
            log.Information(logMessage);
            progressUpdater?.Invoke(logMessage);
            return dups;
        }

        private void HandlePosibleQsoReplacement(SqliteConnection connection, Dictionary<string, string?> qsoRecord, bool isTemporary)
        {
            qsoRecord.TryGetValue("IS_REPLACE", out string? isReplace);
            if (string.IsNullOrEmpty(isReplace) || !bool.Parse(isReplace))
            {
                return;
            }

            qsoRecord.TryGetValue("EXTERNAL_ID", out string? externalId);
            if (string.IsNullOrEmpty(externalId))
            {
                log.Warning("No externalId provided in QSO replacement: {qsoRecord}", qsoRecord);
                return;
            }

            DateTime qsoTime = DateTime.Parse(qsoRecord["QSO_TIME"]);

            using var command = connection.CreateCommand();
            command.CommandText = selectQsoToReplaceQsl;
            AddSqlParameter(command, "externalId", externalId);
            AddSqlParameter(command, "minTime", qsoTime.AddMinutes(-maxMinuteIntervalToReplaceQso));
            AddSqlParameter(command, "maxTime", qsoTime.AddMinutes(maxMinuteIntervalToReplaceQso));
            AddSqlParameter(command, "isTemporary", isTemporary);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                int id = reader.GetInt32(0);
                int? exportId = reader.GetValue(1) == DBNull.Value ? null : reader.GetInt32(1);
                if (exportId != null) {
                    log.Warning("QSO with externalId [{externalId}] and Qso Time [{qsoTime}] has been already exported nd can't be replaced", externalId, qsoTime);
                    return;
                }
                log.Warning("Delete QSO id {id}, externalId [{externalId}], isTemporary [{isTemporary}] and Qso Time [{qsoTime}] to be replaced", id, externalId, isTemporary, qsoTime);
                DeleteQsoRecord(connection, id);
            }
            else
            {
                log.Warning("No QSO for replacent found with externalId [{externalId}] and whithin -+ 5 minute of Qso Time [{qsoTime}]", externalId, qsoTime);
            }
        }

        public void SaveRawQso(QsoMessage qsoMessage)
        {
            log.Debug("Saving raw QSO message to database: {qsoMessage}", qsoMessage);
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = insertRawQsoSql;
            AddSqlParameter(command, "Source", qsoMessage.Source);
            AddSqlParameter(command, "OriginalFormat", qsoMessage.OriginalFormat);
            AddSqlParameter(command, "OriginalQsoData", qsoMessage.OriginalQsoData);
            AddSqlParameter(command, "Replace", qsoMessage.Replace);
            command.ExecuteNonQuery();
        }

        public Dictionary<int, QsoMessage> GetTemporaryQsoMessages()
        {
            log.Verbose("Getting temporary QSO messages from database");
            var qsoMessages = new Dictionary<int, QsoMessage>();
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = getTemporaryQsoSql;
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                QsoMessage qsoMessage = new()
                {
                    Source = reader.GetValue(1) == DBNull.Value ? null : reader.GetString(1),
                    OriginalFormat = reader.GetString(2),
                    OriginalQsoData = reader.GetString(3),
                    AdifQsoData = reader.GetString(4),
                    Replace = reader.GetBoolean(5)
                };
                qsoMessages.Add(id, qsoMessage);
            }
            return qsoMessages;
        }

        public void DeleteQsoRecord(int id)
        {
            log.Debug("Deleting QSO record with id {id} from database", id);
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                DeleteQsoRecord(connection, id);
                transaction.Commit();
            }
            catch (SqliteException)
            {
                transaction.Rollback();
                throw;
            }
        }

        private void DeleteQsoRecord(SqliteConnection connection, int id)
        {
            log.Debug("Deleting QSO record with id {id} from database", id);
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = deleteQsoQsl;
                command.Parameters.Add(new SqliteParameter("@id", id));
                command.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                log.Error(ex, "Error deleting temporary QSO record with id {id} from database. Rolling back transaction.", id);
                throw;
            }
        }

        public Dictionary<int, string> GetAdif(QsoExportFilters exportFilters)
        {
            log.Verbose("Getting ADIF data from database with filters: {exportFilters}", exportFilters);
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            AddAdifSqlCommandTextAndParams(command, exportFilters);
            var adifEntries = new Dictionary<int, string>();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                adifEntries.Add(reader.GetInt32(0), reader.GetString(1));
            }
            return adifEntries;
        }

        public void SetQSOsExported(List<int> keys, string folder, string fileName, QsoExportFilters filter, bool isConfirmed)
        {
            log.Information("Marking QSOs as exported in database. Folder: {folder}, File Name: {fileName}, QSO count: {qsoCount}", folder, fileName, keys.Count);
            if (keys.Count == 0)
            {
                return;
            }

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            int startPos = 0;
            using var transaction = connection.BeginTransaction();
            try
            {
                using var commandExport = connection.CreateCommand();
                commandExport.CommandText = "INSERT INTO adif_export (folder, file_name, qso_amount, filter, is_confirmed) VALUES (@folder, @fileName, @qsoAmount, @filter, @isConfirmed)";
                commandExport.Parameters.Add(new SqliteParameter("@folder", folder));
                commandExport.Parameters.Add(new SqliteParameter("@fileName", fileName));
                commandExport.Parameters.Add(new SqliteParameter("@qsoAmount", keys.Count));
                commandExport.Parameters.Add(new SqliteParameter("@isConfirmed", isConfirmed));
                commandExport.Parameters.Add(new SqliteParameter("@filter", JsonSerializer.Serialize(filter)));
                commandExport.ExecuteNonQuery();

                using var commandGetExport = connection.CreateCommand();
                commandGetExport.CommandText = "SELECT MAX(id) id FROM adif_export WHERE end_time IS NULL and folder = @folder AND file_name = @fileName and qso_amount = @qsoAmount and is_confirmed = @isConfirmed";
                commandGetExport.Parameters.Add(new SqliteParameter("@folder", folder));
                commandGetExport.Parameters.Add(new SqliteParameter("@fileName", fileName));
                commandGetExport.Parameters.Add(new SqliteParameter("@qsoAmount", keys.Count));
                commandGetExport.Parameters.Add(new SqliteParameter("@isConfirmed", isConfirmed));
                using var reader = commandGetExport.ExecuteReader();
                reader.Read();
                int exportId = reader.GetInt32(0);
                reader.Close();
                log.Information("Marking {Count} QSOs as exported in database with exportId {exportId}", keys.Count, exportId);

                do
                {
                    int count = Math.Min(999, keys.Count - startPos);
                    keys.GetRange(startPos, Math.Min(999, keys.Count - startPos));
                    startPos += count + 1;
                    using var command = connection.CreateCommand();
                    StringBuilder sb = new("UPDATE qsodata SET export_id = @exportId WHERE id IN (");
                    command.Parameters.Add(new SqliteParameter("@exportId", exportId));
                    for (int i = 0; i < count; i++)
                    {
                        if (i > 0) sb.Append(", ");
                        sb.Append($"@id{i}");
                        command.Parameters.Add(new SqliteParameter($"@id{i}", keys[startPos - count - 1 + i]));

                    }
                    sb.Append(')');
                    command.CommandText = sb.ToString();
                    command.ExecuteNonQuery();
                } while (startPos < keys.Count);

                using var commandExportComplete = connection.CreateCommand();
                commandExportComplete.CommandText = "UPDATE adif_export SET end_time = CURRENT_TIMESTAMP WHERE id = @id";
                commandExportComplete.Parameters.Add(new SqliteParameter("@id", exportId));
                commandExportComplete.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (SqliteException)
            {
                transaction.Rollback();
                throw;
            }
        }

        public List<string> GetExportHours()
        {
            log.Debug("Loading export scheduler hours from database");
            return GetData(getExportSchedulerHoursSql, reader => MapField<string>(reader, "hour"));
        }

        public void SaveExportHours(List<string> hours)
        {
            log.Debug("Saving export scheduler hours to database");
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                using var commandDelete = connection.CreateCommand();
                commandDelete.CommandText = "DELETE FROM qso_export_scheduler";
                commandDelete.ExecuteNonQuery();

                hours.ForEach(hour =>
                {
                    using var commandInsert = connection.CreateCommand();
                    commandInsert.CommandText = "INSERT INTO qso_export_scheduler (hour) VALUES (@hour)";
                    commandInsert.Parameters.Add(new SqliteParameter("@hour", hour));
                    commandInsert.ExecuteNonQuery();
                });
                transaction.Commit();
            }
            catch (SqliteException)
            {
                transaction.Rollback();
                throw;
            }
        }

        public DateTime GetLatestExportTaskTime()
        {
            log.Verbose("Get latest export task timestamp");
            string sql = "select max(start_time) max_start_time from adif_export where is_auto = true";
            return GetData(sql, reader => MapField<DateTime>(reader, "max_start_time"))[0];
        }

        private static void AddAdifSqlCommandTextAndParams(SqliteCommand command, QsoExportFilters exportFilters)
        {
            StringBuilder sb = new("SELECT q.id, q.adif_qsodata FROM qsodata q LEFT JOIN adif_export e ON q.export_id = e.id AND e.is_confirmed = true WHERE q.is_temporary = false AND q.adif_qsodata IS NOT NULL ");

            if (exportFilters.IsNewOnly == true)
            {
                sb.Append(" AND e.id IS NULL");
            }

            if (exportFilters.DateFrom != null)
            {
                sb.Append(" AND q.qso_time >= @dateFrom");
                AddSqlParameter(command, "dateFrom", DateOnly.FromDateTime(exportFilters.DateFrom.Value));
            }

            if (exportFilters.DateTo != null)
            {
                sb.Append(" AND q.qso_time < @dateTo");
                DateTime dateTo = exportFilters.DateTo.Value;
                // if dateTo not in the Past but in Future then move to next day
                // this way the whole day is included
                if (dateTo <= DateTime.UtcNow)
                {
                    dateTo = dateTo.AddDays(1);
                }
                AddSqlParameter(command, "dateTo", DateOnly.FromDateTime(dateTo));
            }

            if (exportFilters.ModeGroup != null && exportFilters.Mode == null)
            {
                sb.Append(" AND q.mode_group = @mode_group");
                AddSqlParameter(command, "mode_group", exportFilters.ModeGroup);
            }

            if (exportFilters.Mode != null)
            {
                sb.Append(" AND q.mode = @mode");
                AddSqlParameter(command, "mode", exportFilters.Mode);
            }

            if (exportFilters.Band != null)
            {
                sb.Append(" AND q.band = @band");
                AddSqlParameter(command, "band", exportFilters.Band);
            }

            if (exportFilters.SourceName != null)
            {
                if (exportFilters.SourceName == "UNKNOWN")
                {
                    sb.Append(" AND q.source_name IS NULL");
                }
                else
                {
                    sb.Append(" AND q.source_name = @source_name");
                    AddSqlParameter(command, "source_name", exportFilters.SourceName);
                }
            }

            if (exportFilters.Operator != null)
            {
                if (exportFilters.Operator == "UNKNOWN")
                {
                    sb.Append(" AND q.operator IS NULL");
                }
                else
                {
                    sb.Append(" AND q.operator = @operator");
                    AddSqlParameter(command, "operator", exportFilters.Operator);
                }
            }

            if (exportFilters.SourceIp != null)
            {
                if (exportFilters.SourceIp == "UNKNOWN")
                {
                    sb.Append(" AND q.source_ip_address IS NULL");
                }
                else
                {
                    sb.Append(" AND q.source_ip_address = @source_ip_address");
                    AddSqlParameter(command, "source_ip_address", exportFilters.SourceIp);
                }
            }
            sb.Append(" ORDER BY q.qso_time");
            command.CommandText = sb.ToString();
        }

        private static void AddSqlParameter(SqliteCommand command, string paramKey, object? paramValue, List<string>? parameterKeys = null)
        {
            command.Parameters.Add(new SqliteParameter($"@{paramKey}", paramValue ?? DBNull.Value));
            parameterKeys?.Add(paramKey);
        }

        private static List<string> GetParameterKeys(string commandText)
        {
            List<string> parameterkeys = [];
            int index = 0;
            while (index < commandText.Length)
            {
                int atIndex = commandText.IndexOf('@', index);
                if (atIndex == -1) break;
                atIndex++; // move past '@'
                int endIndex = commandText.IndexOfAny([' ', ',', ')'], atIndex);
                if (endIndex == -1) endIndex = commandText.Length;
                string parameterKey = commandText[atIndex..endIndex];
                if (!parameterkeys.Contains(parameterKey))
                {
                    parameterkeys.Add(parameterKey);
                }
                index = endIndex;
            }
            return parameterkeys;
        }

        private static object ConvertValueToColumnDatatype(string columnType, string? value)
        {
            if (string.IsNullOrEmpty(value)) return DBNull.Value;

            try
            {
                return columnType.ToUpper() switch
                {
                    "INTEGER" => int.Parse(value),
                    "REAL" => double.Parse(value, System.Globalization.CultureInfo.InvariantCulture),
                    "DOUBLE" => double.Parse(value, System.Globalization.CultureInfo.InvariantCulture),
                    "BOOLEAN" => bool.Parse(value),
                    "DATE" => DateOnly.Parse(value),
                    "DATETIME" => DateTime.Parse(value),
                    "BLOB" => Encoding.UTF8.GetBytes(value),
                    _ => value,
                };
            }
            catch (FormatException ex)
            {
                throw new FormatException($"Error converting value '{value}' to type '{columnType}'", ex);
            }
        }

        public List<Dictionary<string, object?>> SearchQsosByCall(string callPattern, string? modeGroup = null, string? band = null, int maxResults = 200)
        {
            log.Debug("Searching QSOs by call pattern: {pattern}, modeGroup: {modeGroup}, band: {band}", callPattern, modeGroup, band);
            var results = new List<Dictionary<string, object?>>();
            List<SqliteParameter> parameters = [];
            var sb = new StringBuilder();
            sb.Append("SELECT call, qso_time, mode_group, mode, band, freq, operator, source_ip_address ");
            sb.Append("FROM qsodata ");
            sb.Append("WHERE call LIKE @callPattern ");
            sb.Append("AND is_temporary = false ");

            parameters.Add(new SqliteParameter("@callPattern", callPattern));
            if (!string.IsNullOrEmpty(modeGroup))
            {
                sb.Append("AND mode_group = @modeGroup ");
                parameters.Add(new SqliteParameter("@modeGroup", modeGroup));
            }

            if (!string.IsNullOrEmpty(band))
            {
                sb.Append("AND band = @band ");
                parameters.Add(new SqliteParameter("@band", band));
            }

            sb.Append("ORDER BY qso_time DESC ");
            sb.Append($"LIMIT {maxResults}");
            string sql = sb.ToString();

            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = sql;
                command.Parameters.AddRange(parameters);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var row = new Dictionary<string, object?>
                    {
                        { "call", reader["call"] },
                        { "qso_time", reader["qso_time"] },
                        { "mode_group", reader["mode_group"] },
                        { "mode", reader["mode"] },
                        { "band", reader["band"] },
                        { "freq", reader["freq"] },
                        { "operator", reader["operator"] },
                        { "source_ip_address", reader["source_ip_address"] }
                    };
                    results.Add(row);
                }
            }
            catch (SqliteException ex)
            {
                log.Error(ex, "Error searching QSOs by call");
                throw;
            }

            return results;
        }

        public List<string> GetDistinctModeGroups()
        {
            log.Debug("Getting distinct mode groups");
            string sqlQuery = "SELECT DISTINCT mode_group FROM qsodata WHERE is_temporary = false AND mode_group IS NOT NULL ORDER BY mode_group";
            return GetData(sqlQuery, reader => MapField<string>(reader, "mode_group"));
        }

        public List<string> GetDistinctBands()
        {
            log.Debug("Getting distinct bands");
            string sqlQuery = "SELECT DISTINCT band FROM qsodata WHERE is_temporary = false AND band IS NOT NULL ORDER BY band";
            return GetData(sqlQuery, reader => MapField<string>(reader, "band"));
        }

        private void EnrichSatRule(SatRule rule) {
            if (rule.BandTxId.HasValue)
            {
                rule.BandTx = GetBand(rule.BandTxId.Value);
            }
            if (rule.BandRxId.HasValue)
            {
                rule.BandRx = GetBand(rule.BandRxId.Value);
            }
        }

        private List<T> GetData<T>(string sqlQuery, Func<SqliteDataReader, T> mapRecord, List<SqliteParameter>? queryParams = null)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = sqlQuery;
            if (queryParams != null)
            {
                foreach (var param in queryParams)
                {
                    command.Parameters.Add(param);
                }
            }
            using var reader = command.ExecuteReader();
            var results = new List<T>();
            while (reader.Read())
            {
                results.Add(mapRecord(reader));
            }
            return results;
        }


        private static T MapField<T>(SqliteDataReader reader, string fieldName)
        {
            Type type = typeof(T);
            type = Nullable.GetUnderlyingType(type) ?? type;
            object value = reader[fieldName];
            if (value == DBNull.Value)
            {
                return default!;
            }
            T? item = (T?)(value == DBNull.Value ? null : Convert.ChangeType(value, type));
            return item ?? default!;
        }

        private T Map<T>(SqliteDataReader reader, bool skipUnknown = false)
        {
                Type type = typeof(T);
                T? item = (T?)Activator.CreateInstance(type);
                if (item == null)
                {
                    return default!;
                }
                foreach (PropertyInfo prop in type.GetProperties())
                {
                    object? value = null;
                    try
                    {
                        value = reader[prop.Name];
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        if (!skipUnknown)
                        {
                            throw new Exception($"Error mapping property '{prop.Name}' of type '{type.Name}'", ex);
                        }
                        else
                        {
                            log.Debug(ex, "Skipping unknown property '{propName}' of type '{typeName}'", prop.Name, type.Name);
                            continue;
                        }
                    }

                    if (value is DBNull) continue;
                    var propType = prop.PropertyType;
                    propType = Nullable.GetUnderlyingType(propType) ?? propType;
                    prop.SetValue(item, Convert.ChangeType(value, propType));
                }
            return item;
        }
    }
}