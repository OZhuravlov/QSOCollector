using QSOCollector.Data;
using Serilog;

namespace QSOCollector.Tests.Data
{
    [TestClass]
    public class DbRepositoryQsoSearchTests
    {
        private IDbRepository dbRepository = null!;
        private string testConnectionString = null!;

        [TestInitialize]
        public void Setup()
        {
            // Create in-memory test database
            testConnectionString = "Data Source=:memory:";
            InitializeTestDatabase();
            dbRepository = new DbRepository(testConnectionString);
        }

        private void InitializeTestDatabase()
        {
            // Initialize logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            // Create database schema
            using var connection = new Microsoft.Data.Sqlite.SqliteConnection(testConnectionString);
            connection.Open();

            // Create qsodata table
            var createTableCommand = connection.CreateCommand();
            createTableCommand.CommandText = @"
                CREATE TABLE qsodata (
                    id INTEGER PRIMARY KEY,
                    call TEXT NOT NULL,
                    qso_time DATETIME,
                    mode_group TEXT,
                    mode TEXT,
                    band TEXT,
                    freq TEXT,
                    operator TEXT,
                    source_ip_address TEXT,
                    is_temporary BOOLEAN DEFAULT 0
                );

                CREATE INDEX idx_qsodata_call ON qsodata(call);
            ";
            createTableCommand.ExecuteNonQuery();

            // Insert test data
            InsertTestData(connection);
        }

        private void InsertTestData(Microsoft.Data.Sqlite.SqliteConnection connection)
        {
            var testData = new[]
            {
                ("N0CALL", "2026-02-13 20:28:30", "CW", "CW", "80m", "3.573", "W5XYZ", "192.168.1.1"),
                ("W5XYZ", "2026-02-13 20:29:00", "SSB", "SSB", "40m", "7.079133", "N0CALL", "192.168.1.2"),
                ("K0ABC", "2026-02-13 20:30:15", "DATA", "AFSK45", "20m", "14.070", "VE3TEST", "192.168.1.3"),
                ("N0CALL/0", "2026-02-13 20:31:45", "CW", "CW", "40m", "7.035", "W5XYZ", "192.168.1.1"),
                ("VE3TEST", "2026-02-13 20:32:00", "SSB", "USB", "80m", "3.860", "N0CALL", "192.168.1.4"),
            };

            foreach (var (call, qsoTime, modeGroup, mode, band, freq, op, ip) in testData)
            {
                var insertCommand = connection.CreateCommand();
                insertCommand.CommandText = @"
                    INSERT INTO qsodata (call, qso_time, mode_group, mode, band, freq, operator, source_ip_address, is_temporary)
                    VALUES (@call, @qsoTime, @modeGroup, @mode, @band, @freq, @operator, @sourceIp, 0)
                ";
                insertCommand.Parameters.Add(new Microsoft.Data.Sqlite.SqliteParameter("@call", call));
                insertCommand.Parameters.Add(new Microsoft.Data.Sqlite.SqliteParameter("@qsoTime", qsoTime));
                insertCommand.Parameters.Add(new Microsoft.Data.Sqlite.SqliteParameter("@modeGroup", modeGroup));
                insertCommand.Parameters.Add(new Microsoft.Data.Sqlite.SqliteParameter("@mode", mode));
                insertCommand.Parameters.Add(new Microsoft.Data.Sqlite.SqliteParameter("@band", band));
                insertCommand.Parameters.Add(new Microsoft.Data.Sqlite.SqliteParameter("@freq", freq));
                insertCommand.Parameters.Add(new Microsoft.Data.Sqlite.SqliteParameter("@operator", op));
                insertCommand.Parameters.Add(new Microsoft.Data.Sqlite.SqliteParameter("@sourceIp", ip));
                insertCommand.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void SearchQsosByCall_WithExactMatch_ReturnsMatchingResults()
        {
            // Arrange
            string callPattern = "%N0CALL%";

            // Act
            var results = dbRepository.SearchQsosByCall(callPattern);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.HasCount(2, results, "Should find 2 QSOs with N0CALL");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(results.All(r => r["call"].ToString().Contains("N0CALL")));
        }

        [TestMethod]
        public void SearchQsosByCall_WithWildcard_ReturnsAllMatches()
        {
            // Arrange
            string callPattern = "%N0%";

            // Act
            var results = dbRepository.SearchQsosByCall(callPattern);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.HasCount(2, results, "Should find 2 QSOs starting with N0");
        }

        [TestMethod]
        public void SearchQsosByCall_WithModeGroupFilter_ReturnsFilteredResults()
        {
            // Arrange
            string callPattern = "%";
            string modeGroup = "CW";

            // Act
            var results = dbRepository.SearchQsosByCall(callPattern, modeGroup);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.HasCount(2, results, "Should find 2 CW QSOs");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(results.All(r => r["mode_group"].ToString() == "CW"));
        }

        [TestMethod]
        public void SearchQsosByCall_WithBandFilter_ReturnsFilteredResults()
        {
            // Arrange
            string callPattern = "%";
            string band = "40m";

            // Act
            var results = dbRepository.SearchQsosByCall(callPattern, band: band);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.HasCount(2, results, "Should find 2 QSOs on 40m band");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(results.All(r => r["band"].ToString() == "40m"));
        }

        [TestMethod]
        public void SearchQsosByCall_WithCombinedFilters_ReturnsFilteredResults()
        {
            // Arrange
            string callPattern = "%";
            string modeGroup = "CW";
            string band = "40m";

            // Act
            var results = dbRepository.SearchQsosByCall(callPattern, modeGroup, band);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.HasCount(1, results, "Should find 1 QSO matching all filters");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("N0CALL/0", results[0]["call"].ToString());
        }

        [TestMethod]
        public void SearchQsosByCall_WithNoMatches_ReturnsEmptyList()
        {
            // Arrange
            string callPattern = "%NONEXISTENT%";

            // Act
            var results = dbRepository.SearchQsosByCall(callPattern);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsEmpty(results, "Should return empty list for non-matching pattern");
        }

        [TestMethod]
        public void SearchQsosByCall_ReturnsCorrectColumns()
        {
            // Arrange
            string callPattern = "%N0CALL%";

            // Act
            var results = dbRepository.SearchQsosByCall(callPattern);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotEmpty(results);
            var firstResult = results[0];
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(firstResult.ContainsKey("call"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(firstResult.ContainsKey("qso_time"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(firstResult.ContainsKey("mode_group"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(firstResult.ContainsKey("mode"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(firstResult.ContainsKey("band"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(firstResult.ContainsKey("freq"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(firstResult.ContainsKey("operator"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(firstResult.ContainsKey("source_ip_address"));
        }

        [TestMethod]
        public void GetDistinctModeGroups_ReturnsAllUniqueGroups()
        {
            // Act
            var modeGroups = dbRepository.GetDistinctModeGroups();

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.HasCount(3, modeGroups, "Should return 3 distinct mode groups");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Contains("CW", modeGroups);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Contains("SSB", modeGroups);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Contains("DATA", modeGroups);
        }

        [TestMethod]
        public void GetDistinctBands_ReturnsAllUniqueBands()
        {
            // Act
            var bands = dbRepository.GetDistinctBands();

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.HasCount(4, bands, "Should return 4 distinct bands");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Contains("80m", bands);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Contains("40m", bands);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Contains("20m", bands);
        }

        [TestMethod]
        public void SearchQsosByCall_ExcludesTemporaryQsos()
        {
            // Arrange - Add a temporary QSO
            using (var connection = new Microsoft.Data.Sqlite.SqliteConnection(testConnectionString))
            {
                connection.Open();
                var insertCommand = connection.CreateCommand();
                insertCommand.CommandText = @"
                    INSERT INTO qsodata (call, qso_time, mode_group, mode, band, freq, operator, source_ip_address, is_temporary)
                    VALUES ('N0CALL', '2026-02-13 21:00:00', 'CW', 'CW', '80m', '3.573', 'W5XYZ', '192.168.1.5', 1)
                ";
                insertCommand.ExecuteNonQuery();
            }

            string callPattern = "%N0CALL%";

            // Act
            var results = dbRepository.SearchQsosByCall(callPattern);

            // Assert - Should still be 2 (temporary QSO excluded)
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.HasCount(2, results, "Should exclude temporary QSOs");
        }

        [TestMethod]
        public void SearchQsosByCall_RespectMaxResults()
        {
            // Arrange - Add many QSOs
            using (var connection = new Microsoft.Data.Sqlite.SqliteConnection(testConnectionString))
            {
                connection.Open();
                for (int i = 0; i < 250; i++)
                {
                    var insertCommand = connection.CreateCommand();
                    insertCommand.CommandText = @"
                        INSERT INTO qsodata (call, qso_time, mode_group, mode, band, freq, operator, source_ip_address, is_temporary)
                        VALUES ('TEST' + @index, '2026-02-13 20:00:00', 'CW', 'CW', '80m', '3.573', 'OP', '192.168.1.1', 0)
                    ";
                    insertCommand.Parameters.Add(new Microsoft.Data.Sqlite.SqliteParameter("@index", i));
                    insertCommand.ExecuteNonQuery();
                }
            }

            string callPattern = "%";

            // Act
            var results = dbRepository.SearchQsosByCall(callPattern, maxResults: 200);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsLessThanOrEqualTo(200, results.Count, "Should respect max results limit");
        }
    }
}
