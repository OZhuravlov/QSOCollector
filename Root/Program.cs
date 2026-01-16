using DbUp;
using Microsoft.Data.Sqlite;
using QSOCollector.Models;
using SQLitePCL;
using System.Reflection;

namespace QSOCollector.Root
{
    public static class Program
    {
        public static readonly string appGuid = "dce43cbd-39a4-49df-9e4e-4e3e85adfd83";
        public static readonly string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\"
                + Assembly.GetExecutingAssembly().GetName().Name;
        private static readonly string dbFolder = appDataFolder + "\\db";
        public static readonly string importFolder = appDataFolder + "\\import";
        public static readonly string exportFolder = appDataFolder + "\\export";
        public static readonly string configFolder = appDataFolder + "\\config";

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using Mutex mutex = new(false, "Global\\" + appGuid);
            if (!mutex.WaitOne(0, false))
            {
                MessageBox.Show("The QSO Collector is already running.\nPlease check it in System tray or from Task Manager");
                return;
            }

            StartupParams startupParams = GetStartupParams();

            Batteries.Init();
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Check and initialize the database
            string dbFileName = "qsoCollector.s3db";
            string connectionString = InitializeDatabase(dbFileName);
            RunMigrations(connectionString);
            InitializeAdditionalFolders();
            Application.Run(new QsoCollectorForm(connectionString, startupParams));
        }

        private static void InitializeAdditionalFolders()
        {
            InitializeFolder(importFolder);
            InitializeFolder(exportFolder);
            InitializeFolder(configFolder);
        }

        private static string InitializeDatabase(string dbFileName)
        {
            try
            {
                InitializeFolder(dbFolder);
                string dbFullPath = Path.Combine(dbFolder, dbFileName);
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
        private static void InitializeFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        private static StartupParams GetStartupParams()
        {
            StartupParams startupParams = new();
            string[] args = Environment.GetCommandLineArgs();
            foreach (var param in args)
            {
                switch (param)
                {
                    case "--quiet":
                    case "-q":
                    case "/q":
                        startupParams.IsQuiet = true;
                        break;
                    case "--start-server":
                    case "-s":
                    case "/s":
                        startupParams.StartServer = true;
                        break;
                    case "--start-client":
                    case "-c":
                    case "/c":
                        startupParams.StartClient = true;
                        break;
                    case "--debug":
                    case "-d":
                    case "/d":
                        startupParams.Debug = true;
                        break;
                }
            }
            return startupParams;
        }
    }
}
