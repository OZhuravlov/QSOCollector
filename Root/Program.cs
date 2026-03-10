using DbUp;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QSOCollector.Data;
using QSOCollector.Logging;
using QSOCollector.Models;
using QSOCollector.Service;
using Serilog;
using Serilog.Filters;
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
        public static readonly string defaultAutoExportFolder = appDataFolder + "\\auto\\QSO_export";
        public static readonly bool isPremiumAutoExportEnabled = false;

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

            Log.Logger = CreateLogger();
            StartupParams startupParams = GetStartupParams();

            Batteries.Init();
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Check and initialize the database
            string dbFileName = "qsoCollector.s3db";
            string connectionString = InitializeDatabase(dbFileName);
            RunMigrations(connectionString);

            try
            {
                // 2. Resolve the form from the DI container to get ILogger<T> injected
                Application.SetCompatibleTextRenderingDefault(false);
                Application.EnableVisualStyles();

                IHost host = ConfigureServices(connectionString);

                InitializeAdditionalFolders(host);

                var mainForm = ActivatorUtilities.CreateInstance<QsoCollectorForm>(host.Services, startupParams);
                Application.Run(mainForm);
            }
            finally
            {
                Log.CloseAndFlush(); // Ensure logs are saved before exit
            }
        }

        private static Serilog.Core.Logger CreateLogger() => new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .Enrich.With(new ShortSourceContextEnricher(30))

            // Branch 1: The Client Log
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(Matching.FromSource("QSOCollector.Network.Client"))
                .WriteTo.File(appDataFolder + "\\logs\\client.log",
                outputTemplate: "{Timestamp:u} [{Level:u3}] {ShortSourceContext,-30}: {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day, // New file every day
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                retainedFileCountLimit: 30))          // Keep 30 days of logs

            // Branch 2: The Server Log
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(Matching.FromSource("QSOCollector.Network.Server"))
                .WriteTo.File(appDataFolder + "\\logs\\server.log",
                outputTemplate: "{Timestamp:u} [{Level:u3}] {ShortSourceContext,-30}: {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                retainedFileCountLimit: 30
                ))

            // 3. General/Catch-all (Optional)
            .WriteTo.File(appDataFolder + "\\logs\\all-events.log",
            outputTemplate: "{Timestamp:u} [{Level:u3}] {ShortSourceContext,-30}: {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30)

            .CreateLogger();

        private static void InitializeAdditionalFolders(IHost host)
        {
            InitializeFolder(importFolder);
            InitializeFolder(exportFolder);
            InitializeFolder(configFolder);
            InitializeFolder(defaultAutoExportFolder);
            string autoExportFolder = host.Services.GetService<IOptions<AutoExportTaskOptions>>().Value.Folder;
            if (!string.IsNullOrEmpty(autoExportFolder))
            {
                InitializeFolder(autoExportFolder);
            }
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

        private static IHost ConfigureServices(string connectionString)
        {
            return Host.CreateDefaultBuilder()
                        // Ensure appsettings.json from the application folder is loaded
                        // explicitly so values come from the JSON file (base) and
                        // environment variables can still override when present.
                        .ConfigureAppConfiguration((hostingContext, config) =>
                        {
                            // Use the application base directory to locate appsettings.json
                            config.SetBasePath(AppContext.BaseDirectory);
                            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                            config.AddEnvironmentVariables();
                        })
                        .ConfigureLogging(logging =>
                        {
                            logging.ClearProviders(); // Removes default Console/Debug loggers
                            logging.AddSerilog();     // Adds Serilog to ILoggingBuilder
                        })
                        .ConfigureServices((context, services) =>
                        {
                            services.AddTransient<QsoCollectorForm>(); // Register your main form for DI
                            // register DbRepository as implementation of IDbRepository so it can be injected by interface
                            services.AddSingleton<IDbRepository>(_ => new DbRepository(connectionString));
                        })
                        .Build();
        }
    }
}
