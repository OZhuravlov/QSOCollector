using Microsoft.Data.Sqlite;
using QSOCollector.Models;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace QSOCollector.Data
{
    public class DbRepository
    {
        private const string getTableColumnsSql = "SELECT UPPER(p.name) name, UPPER(p.type) type FROM sqlite_master m JOIN pragma_table_info(m.name) p WHERE lower(m.name) = lower(@tablename) and not p.pk";
        private const string selectSettingsSql = "SELECT key, value FROM settings";
        private const string insertSettingsSql = "INSERT OR REPLACE INTO settings (key, value) VALUES (@key, @value)";
        private const string getListenerConfigsSql = "SELECT name as Name, id as Id, qso_port as QsoPort, forward_port as ForwardPort, acknowledge_port as AcknowledgePort, message_format as MessageFormat, is_active IsActive FROM listeners WHERE is_active = true";
        private const string insertListenerConfigsSql = "INSERT INTO listeners (name, qso_port, forward_port, acknowledge_port, message_format, is_active) " +
                    " VALUES (@Name, @QsoPort, @ForwardPort, @AcknowledgePort, @MessageFormat, @IsActive)";
        private const string getServerQsoAmountsSql = "SELECT q.mode QsoAmountMode, COUNT(CASE WHEN q.qso_time >= current_date THEN 1 END) TodayQsoAmount, count(*) TotalQsoAmount, COUNT(e.id) ExportedQsoAmount, MAX(q.qso_time) LastQsoTime, MAX(e.end_time) LastExportedQsoTime FROM qsodata q LEFT JOIN adif_export e ON q.export_id = e.id AND e.is_confirmed = true WHERE q.is_temporary = false GROUP BY q.mode UNION ALL SELECT 'Total', COUNT(CASE WHEN q.qso_time >= current_date THEN 1 END), COUNT(*), COUNT(e.id), MAX(q.qso_time), MAX(e.end_time) FROM qsodata q LEFT JOIN adif_export e ON q.export_id = e.id AND e.is_confirmed = true WHERE q.is_temporary = false";
        private const string getQsoAmountsForExportSql = "SELECT COALESCE(q.source_name, '<UNKNOWN>') SourceName, e.id IS NOT NULL IsExported, DATE(q.qso_time) QsoDate, q.mode_group ModeGroup, q.mode Mode, q.band Band, COALESCE(q.operator, '<UNKNOWN>') Operator, COALESCE(q.source_ip_address, '<UNKNOWN>') SourceIp, count(*) Count FROM qsodata q LEFT JOIN adif_export e ON q.export_id = e.id AND e.is_confirmed = true WHERE q.is_temporary = false GROUP BY COALESCE(q.source_name, '<UNKNOWN>'), e.id IS NOT NULL, DATE(q.qso_time), q.mode_group, q.mode, q.band, COALESCE(q.operator, '<UNKNOWN>'), COALESCE(q.source_ip_address, '<UNKNOWN>')";
        private const string insertQsoSql = "INSERT INTO qsodata (is_temporary, source_name, source_ip_address, import_id, qso_time, programid, station_callsign, qso_date, qso_date_off, call, time_on, time_off, band, freq, freq_rx, mode, mode_group, contest_id, rst_sent, rst_rcvd, exch_sent, exch_rcvd, operator, my_gridsquare, gridsquare, distance, comment, pfx, dxcc_pref, cqz, ituz, cont, qslmsg, dxcc, orig_format, orig_qsodata, adif_qsodata)" +
                    " VALUES (@is_temporary, @source_name, @source_ip_address, @import_id, @qso_time, @programid, @station_callsign, @qso_date, @qso_date_off, @call, @time_on, @time_off, @band, @freq, @freq_rx, @mode, @mode_group, @contest_id, @rst_sent, @rst_rcvd, @exch_sent, @exch_rcvd, @operator, @my_gridsquare, @gridsquare, @distance, @comment, @pfx, @dxcc_pref, @cqz, @ituz, @cont, @qslmsg, @dxcc, @orig_format, @orig_qsodata, @adif_qsodata)";
        private const string getTemporaryQsoSql = "SELECT id, source_name, orig_format, orig_qsodata, adif_qsodata " +
            "  FROM qsodata " +
            " WHERE is_temporary = 1 AND orig_format IS NOT NULL AND orig_qsodata IS NOT NULL " +
            " ORDER BY id " +
            " LIMIT 100";
        private const string deleteTemporaryQsoQsl = "DELETE FROM qsodata WHERE is_temporary = 1 and id = @id";
        private const int SQLITE_CONSTRAINT_UNIQUE = 2067;

        private readonly string connectionString;
        private Dictionary<string, string>? qsodataColumns = null;

        public DbRepository(string dbConnectionString)
        {
            connectionString = dbConnectionString;
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

        public void SaveSetting(string key, string value)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = insertSettingsSql;
                command.Parameters.Add(new SqliteParameter("@key", key));
                command.Parameters.Add(new SqliteParameter("@value", value));
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
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = getListenerConfigsSql;
            using var reader = command.ExecuteReader();
            return GetData<ListenerConfig>(reader);
        }

        public void ReplaceListenerConfigs(List<ListenerConfig> configs)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            DeleteListenerConfigs(connection, configs);
            SaveListenerConfigs(connection, configs);
            transaction.Commit();
        }

        public void CleanTemporarelySavedQsos()
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM qsodata WHERE is_temporary = 1";
            command.ExecuteNonQuery();
            transaction.Commit();
        }

        private static void DeleteListenerConfigs(SqliteConnection connection, List<ListenerConfig> configs)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM listeners";
            command.ExecuteNonQuery();
        }

        private static void SaveListenerConfigs(SqliteConnection connection, List<ListenerConfig> configs)
        {
            configs.ForEach(config =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = insertListenerConfigsSql;
                command.Parameters.Add(new SqliteParameter("@Name", config.Name));
                command.Parameters.Add(new SqliteParameter("@QsoPort", config.QsoPort));
                command.Parameters.Add(new SqliteParameter("@ForwardPort", config.ForwardPort));
                command.Parameters.Add(new SqliteParameter("@AcknowledgePort", config.AcknowledgePort));
                command.Parameters.Add(new SqliteParameter("@MessageFormat", config.MessageFormat));
                command.Parameters.Add(new SqliteParameter("@IsActive", config.IsActive));
                command.ExecuteNonQuery();
            });
        }

        public List<ServerQsoAmount> GetServerQsoAmounts()
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = getServerQsoAmountsSql;
            using var reader = command.ExecuteReader();
            return GetData<ServerQsoAmount>(reader);
        }

        public List<QsoExportExpectedAmounts> GetQsoAmountsForExport()
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = getQsoAmountsForExportSql;
            using var reader = command.ExecuteReader();
            return GetData<QsoExportExpectedAmounts>(reader);
        }

        private Dictionary<string, string> GetTableColumns(string tablename)
        {
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
                        progressUpdater.Invoke($"Deleted {deleted} duplicates of {dups.Count}");
                    }
                });
                progressUpdater.Invoke($"Deleted {deleted} duplicates of {dups.Count}");

                ExecuteSavingQsoRecords(connection, dups, importId, progressUpdater: progressUpdater);

                transaction.Commit();
            }
            catch (SqliteException)
            {
                transaction.Rollback();
                throw;
            }
        }

        public List<Dictionary<string, string?>> ImportQsoRecords(List<Dictionary<string, string?>> qsoRecords, string folder, string fileName, Func<string, Task> progressUpdater)
        {
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
                return dups;
            }
            catch (SqliteException)
            {
                transaction.Rollback();
                throw;
            }
        }

        public void SaveQsoRecords(List<Dictionary<string, string?>> qsoRecords, int? importId = null, bool isTemporary = false)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                ExecuteSavingQsoRecords(connection, qsoRecords, importId, isTemporary: isTemporary);
                transaction.Commit();
            }
            catch (SqliteException)
            {
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

                try
                {
                    command.ExecuteNonQuery();
                    succCount++;
                }
                catch (SqliteException ex)
                {
                    if (ex.SqliteExtendedErrorCode != SQLITE_CONSTRAINT_UNIQUE) throw;
                    dups.Add(qsoRecord);
                    dupsCount++;
                }
                if (n % 10 == 0)
                {
                    progressUpdater?.Invoke($"Saved: {succCount} + dups: {dupsCount} = {n} of {qsoRecords.Count}");
                }
            }
            progressUpdater?.Invoke($"Saved: {succCount} + dups: {dupsCount} = {n} of {qsoRecords.Count}");
            return dups;
        }

        public Dictionary<int, QsoMessage> GetTemporaryQsoMessages()
        {
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
                    AdifQsoData = reader.GetString(4)
                };
                qsoMessages.Add(id, qsoMessage);
            }
            return qsoMessages;
        }

        public void DeleteTemporaryQsoRecord(int id)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = deleteTemporaryQsoQsl;
                command.Parameters.Add(new SqliteParameter("@id", id));
                command.ExecuteNonQuery();
                transaction.Commit();
            }
            catch (SqliteException)
            {
                transaction.Rollback();
                throw;
            }
        }

        public Dictionary<int, string> GetAdif(QsoExportFilters exportFilters)
        {
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
                    sb.Append(")");
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
                    sb.Append(" AND q.sourceIp IS NULL");
                }
                else
                {
                    sb.Append(" AND q.sourceIp = @sourceIp");
                    AddSqlParameter(command, "sourceIp", exportFilters.SourceIp);
                }
            }
            sb.Append(" ORDER BY q.qso_time");
            command.CommandText = sb.ToString();
        }

        private static void AddSqlParameter(SqliteCommand command, string paramKey, object paramValue, List<string>? parameterKeys = null)
        {
            command.Parameters.Add(new SqliteParameter($"@{paramKey}", paramValue ?? DBNull.Value));
            if (parameterKeys != null)
            {
                parameterKeys.Add(paramKey);
            }
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
                int endIndex = commandText.IndexOfAny(new char[] { ' ', ',', ')' }, atIndex);
                if (endIndex == -1) endIndex = commandText.Length;
                string parameterKey = commandText.Substring(atIndex, endIndex - atIndex);
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

        private static List<T> GetData<T>(SqliteDataReader reader)
        {
            List<T> results = [];
            while (reader.Read())
            {
                Type type = typeof(T);
                T? item = (T?)Activator.CreateInstance(type);
                if (item == null)
                {
                    continue;
                }
                foreach (PropertyInfo prop in type.GetProperties())
                {
                    object value = reader[prop.Name];
                    if (value is DBNull) continue;
                    var propType = prop.PropertyType;
                    propType = Nullable.GetUnderlyingType(propType) ?? propType;
                    prop.SetValue(item, Convert.ChangeType(value, propType));
                }
                results.Add(item);
            }
            return results;
        }
    }
}