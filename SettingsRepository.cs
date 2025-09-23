using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.Common;

namespace QSOCollector
{
    public class SettingsRepository
    {
        private readonly string connectionString;

        public SettingsRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Dictionary<string, string?> LoadSettings()
        {
            var settings = new Dictionary<string, string?>();
            using (DbConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT key, value FROM settings";
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string key = reader.GetString(0);
                            string? value = reader.GetString(1);
                            settings[key] = value;
                        }
                    }
                }
            }
            return settings;
        }

        public void SaveSetting(string key, string value)
        {
            using (DbConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT OR REPLACE INTO settings (key, value) VALUES (@key, @value)";
                    command.Parameters.Add(new SqliteParameter("@key", key));
                    command.Parameters.Add(new SqliteParameter("@value", value));
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<ListenerConfig>? GetListenerConfigs()
        {
            List<ListenerConfig>? listeners = null;
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT id as Id, protocol as Protocol, qso_port as QsoPort, acknowledge_port as AcknowledgePort, message_format as MessageFormat, is_active IsActive, description as Description FROM listeners WHERE is_active = true";
                    using (var reader = command.ExecuteReader())
                    {
                        listeners = GetData<ListenerConfig>(reader);
                    }
                }
            }
            return listeners;
        }

        private List<T> GetData<T>(IDataReader reader)
        {
            var results = new List<T>();
            while (reader.Read())
            {
                var type = typeof(T);
                T? item = (T?)Activator.CreateInstance(type);
                if (item == null)
                {
                    continue;
                }
                foreach (var prop in type.GetProperties())
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