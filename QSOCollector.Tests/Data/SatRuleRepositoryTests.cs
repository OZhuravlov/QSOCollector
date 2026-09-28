using Microsoft.Data.Sqlite;
using QSOCollector.Data;
using QSOCollector.Models;
using Assert = Xunit.Assert;

namespace QSOCollector.Tests.Data
{
    public class SatRuleRepositoryTests
    {
        [Fact]
        public void SaveSatRule_PersistsImportFlagAndReturnsIt()
        {
            using var database = new SatRuleDatabase();
            var rule = CreateRule(isApplyForImport: true);

            database.Repository.SaveSatRule(rule);

            var savedRule = Assert.Single(database.Repository.GetSatRules());
            Assert.Equal(rule.Name, savedRule.Name);
            Assert.True(savedRule.IsApplyForImport);
        }

        [Fact]
        public void UpdateSatRuleApplyForImport_PersistsValueReadByGetSatRule()
        {
            using var database = new SatRuleDatabase();
            database.Repository.SaveSatRule(CreateRule(isApplyForImport: false));
            var savedRule = Assert.Single(database.Repository.GetSatRules());

            database.Repository.UpdateSatRuleApplyForImport(savedRule.Id!.Value, true);

            Assert.True(database.Repository.GetSatRule(savedRule.Id.Value).IsApplyForImport);

            database.Repository.UpdateSatRuleApplyForImport(savedRule.Id.Value, false);

            Assert.False(database.Repository.GetSatRule(savedRule.Id.Value).IsApplyForImport);
        }

        private static SatRule CreateRule(bool isApplyForImport) => new()
        {
            Name = "Test satellite",
            SourceFreqFrom = 145.8,
            SourceFreqTo = 146.0,
            PropagationMode = "SAT",
            SatName = "AO-91",
            SatMode = "FM",
            IsActive = true,
            IsApplyForImport = isApplyForImport
        };

        private sealed class SatRuleDatabase : IDisposable
        {
            private readonly string databasePath = Path.Combine(Path.GetTempPath(), $"qso-collector-tests-{Guid.NewGuid():N}.db");

            public SatRuleDatabase()
            {
                var connectionString = $"Data Source={databasePath}";
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = """
                    CREATE TABLE bands (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        name TEXT NOT NULL,
                        alt_name TEXT NOT NULL,
                        n1mm_name TEXT NOT NULL,
                        freq_mhz_from REAL NOT NULL,
                        freq_mhz_to REAL NOT NULL,
                        designator TEXT,
                        is_active BOOLEAN NOT NULL DEFAULT TRUE
                    );
                    CREATE TABLE sat_rules (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        name TEXT NOT NULL UNIQUE,
                        orig_freq_mhz_from REAL NOT NULL,
                        orig_freq_mhz_to REAL NOT NULL,
                        propagation_mode TEXT NOT NULL,
                        sat_name TEXT NOT NULL,
                        sat_mode TEXT,
                        band_id INTEGER,
                        band_rx_id INTEGER,
                        freq_mhz REAL,
                        freq_mhz_rx REAL,
                        is_active BOOLEAN NOT NULL DEFAULT TRUE,
                        is_apply_for_import BOOLEAN NOT NULL DEFAULT FALSE
                    );
                    """;
                command.ExecuteNonQuery();
                Repository = new DbRepository(connectionString);
            }

            public DbRepository Repository { get; }

            public void Dispose()
            {
                SqliteConnection.ClearAllPools();
                File.Delete(databasePath);
            }
        }
    }
}
