using DbUp;
using Microsoft.Data.Sqlite;
using System.Reflection;
using SQLitePCL;

namespace QSOCollector
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Batteries.Init();
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            // Check and initialize the database
            string dbPath = ".\\db";
            string dbFileName = "qsoCollector.s3db";
            string connectionString = InitializeDatabase(dbPath, dbFileName);
            RunMigrations(connectionString);
            Application.Run(new qsoCollectorForm(connectionString));
        }

        static string InitializeDatabase(string dbPath, string dbFileName)
        {
            try
            {
                if (!Directory.Exists(dbPath))
                {
                    Directory.CreateDirectory(dbPath);
                }
                string dbFullPath = Path.Combine(dbPath, dbFileName);
                string connectionString = $"Data Source={dbFullPath}";
                if (!File.Exists(dbFullPath))
                {
                    using var connection = new SqliteConnection(connectionString);
                    connection.Open();
                    connection.Close();
                }
                return connectionString;
            }
            catch (IOException ioEx)
            {
                throw new Exception("Error accessing the database file.", ioEx);
            }
            catch (UnauthorizedAccessException uaEx)
            {
                throw new Exception("Insufficient permissions to access the database file.", uaEx);
            }
            catch (Exception)
            {
                throw;
            }
        }

        static void RunMigrations(string connectionString)
        {
            var upgrader = DeployChanges.To
                .SqliteDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw new Exception("Database migration failed", result.Error);
            }
        }
    }
}
