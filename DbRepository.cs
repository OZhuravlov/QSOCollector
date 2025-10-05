using Microsoft.Data.Sqlite;
using System.Reflection;
using System.Text;

namespace QSOCollector
{
    public class DbRepository
    {
        private const string getTableColumnsSql = "SELECT UPPER(p.name) name, UPPER(p.type) type FROM sqlite_master m JOIN pragma_table_info(m.name) p WHERE lower(m.name) = lower(@tablename) and not p.pk";
        private const string selectSettingsSql = "SELECT key, value FROM settings";
        private const string insertSettingsSql = "INSERT OR REPLACE INTO settings (key, value) VALUES (@key, @value)";
        private const string getListenerConfigsSql = "SELECT id as Id, protocol as Protocol, qso_port as QsoPort, acknowledge_port as AcknowledgePort, message_format as MessageFormat, is_active IsActive, description as Description FROM listeners WHERE is_active = true";
        private const string getServerQsoAmountsSql = "SELECT mode QsoAmountMode, COUNT(CASE WHEN qso_time >= current_date THEN 1 END) TodayQsoAmount, count(*) TotalQsoAmount, COUNT(q.exported_time) ExportedQsoAmount, MAX(qso_time) LastQsoTime, MAX(q.exported_time) LastExportedQsoTime FROM qsodata q WHERE q.is_temporary = false GROUP BY mode UNION ALL SELECT 'Total', COUNT(CASE WHEN qso_time >= current_date THEN 1 END), COUNT(*), COUNT(q.exported_time), MAX(qso_time), MAX(q.exported_time) FROM qsodata q WHERE q.is_temporary = false";
        private const string getQsoAmountsForExportSql = "SELECT COALESCE(q.programid, '<UNKNOWN>') ProgramId, q.exported_time IS NOT NULL IsExported, DATE(q.qso_time) QsoDate, CASE WHEN q.mode NOT IN ('SSB', 'CW') THEN 'DATA' ELSE q.mode END ModeGroup, q.mode Mode, q.band Band, COALESCE(q.operator, '<UNKNOWN>') Operator, COALESCE(q.source_ip_address, '<UNKNOWN>') SourceIp, count(*) Count FROM qsodata q WHERE q.is_temporary = false GROUP BY COALESCE(q.programid, '<UNKNOWN>'), q.exported_time IS NOT NULL, DATE(q.qso_time), CASE WHEN q.mode NOT IN ('SSB', 'CW') THEN 'DATA' ELSE q.mode END, q.mode, q.band, COALESCE(q.operator, '<UNKNOWN>'), COALESCE(q.source_ip_address, '<UNKNOWN>')";
        private const string insertQsoSql = "INSERT INTO qsodata (is_temporary, source_ip_address, is_imported, qso_time, programid, station_callsign, qso_date, qso_date_off, call, time_on, time_off, band, freq, freq_rx, mode, contest_id, rst_sent, rst_rcvd, exch_sent, exch_rcvd, operator, my_gridsquare, gridsquare, distance, comment, pfx, dxcc_pref, cqz, ituz, cont, qslmsg, dxcc, orig_format, orig_qsodata, adif_qsodata)" +
                    " VALUES (@is_temporary, @source_ip_address, @is_imported, @qso_time, @programid, @station_callsign, @qso_date, @qso_date_off, @call, @time_on, @time_off, @band, @freq, @freq_rx, @mode, @contest_id, @rst_sent, @rst_rcvd, @exch_sent, @exch_rcvd, @operator, @my_gridsquare, @gridsquare, @distance, @comment, @pfx, @dxcc_pref, @cqz, @ituz, @cont, @qslmsg, @dxcc, @orig_format, @orig_qsodata, @adif_qsodata)";
        private const string getTemporaryQsoSql = "SELECT id, programid, orig_format, orig_qsodata, adif_qsodata " +
            "  FROM qsodata " +
            " WHERE is_temporary = 1 AND orig_format IS NOT NULL AND orig_qsodata IS NOT NULL " +
            " ORDER BY id " +
            " LIMIT 100";
        private const string deleteTemporaryQsoQsl = "DELETE FROM qsodata WHERE is_temporary = 1 and id = @id";
        private const int SQLITE_CONSTRAINT_UNIQUE = 2067;

        private readonly string connectionString;
        private readonly Dictionary<string, string> qsodataColumns;

        public DbRepository(string dbConnectionString)
        {
            connectionString = dbConnectionString;
            qsodataColumns = GetTableColumns("qsodata");
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
            using var command = connection.CreateCommand();
            command.CommandText = insertSettingsSql;
            command.Parameters.Add(new SqliteParameter("@key", key));
            command.Parameters.Add(new SqliteParameter("@value", value));
            command.ExecuteNonQuery();
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

        public void SaveQsoRecords(List<Dictionary<string, string?>> qsoRecords, bool isImported = false, bool isTemporary = false)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            foreach (var qsoRecord in qsoRecords)
            {
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

                AddSqlParameter(command, "is_imported", isImported, addedParamKeys);
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

                try { 
                    command.ExecuteNonQuery(); 
                } 
                catch (SqliteException ex)
                {
                    if (ex.SqliteExtendedErrorCode != SQLITE_CONSTRAINT_UNIQUE) throw;
                }

            }
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
            using var command = connection.CreateCommand();
            command.CommandText = deleteTemporaryQsoQsl;
            command.Parameters.Add(new SqliteParameter("@id", id));
            command.ExecuteNonQuery();
        }

        private static void AddSqlParameter(SqliteCommand command, string paramKey, object paramValue, List<string> parameterKeys)
        {
            command.Parameters.Add(new SqliteParameter($"@{paramKey}", paramValue));
            parameterKeys.Add(paramKey);
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
                    "INTEGER" => Int32.Parse(value),
                    "REAL" => Double.Parse(value, System.Globalization.CultureInfo.InvariantCulture),
                    "DOUBLE" => Double.Parse(value, System.Globalization.CultureInfo.InvariantCulture),
                    "BOOLEAN" => Boolean.Parse(value),
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
                    prop.SetValue(item, Convert.ChangeType(value, propType));
                }
                results.Add(item);
            }
            return results;
        }
    }
}