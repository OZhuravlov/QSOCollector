using QSOCollector.Data;
using QSOCollector.Models;
using QSOCollector.Root;
using Serilog;
using System.Text;

namespace QSOCollector.Service
{
    public class AutoExportTaskService
    {
        private readonly ILogger log = Log.ForContext<AutoExportTaskService>();
        private readonly IDbRepository dbRepository;
        private string mainFolder;
        private string allFolder;
        private string premiumFolder;
        private readonly List<double> hours = [];
        private DateTime lastExecutionTime;
        private bool running = false;
        private CancellationTokenSource cts;

        internal AutoExportTaskService(IDbRepository dbRepository)
        {
            this.dbRepository = dbRepository ?? throw new ArgumentNullException(nameof(dbRepository));
        }

        internal void Init()
        {
            Dictionary<string, string?> settings = dbRepository.LoadSettings();
            mainFolder = settings.TryGetValue("AutoExportFolder", out string? value) ? value : Program.defaultAutoExportFolder;
            this.allFolder = Path.Combine(mainFolder, "all");
            this.premiumFolder = Path.Combine(mainFolder, "premium");
            EnsureDirectoriesExist();
            lastExecutionTime = dbRepository.GetLatestExportTaskTime();
            log.Information("Initializing AutoExportTaskService. Output folder {mainFolder}. Last execution time: {lastExecution}", mainFolder, lastExecutionTime.ToString("yyyy-MM-dd HH:mm"));
            LoadHours();
        }

        internal void LoadHours() {
            hours.Clear();
            hours.AddRange(dbRepository.GetExportHours().Select(double.Parse));
            if (hours.Count == 0)
            {
                log.Information("No export hours configured");
            }
            else
            {
                log.Information("Export hours: {hours}", string.Join(", ", hours));
            }
        }

        internal async Task Start()
        {
            log.Information("Starting AutoExportTaskService with export hours: {hours}. Last execution time: {lastExecution}", string.Join(", ", hours), lastExecutionTime.ToString("yyyy-MM-dd HH:mm"));

            running = true;
            while (running)
            {
                cts = new CancellationTokenSource();
                try
                {
                    DateTime nextExecutionTime = GetNextExecutionTime();
                    log.Information("Check next export execution time: {nextExecutionTime}", nextExecutionTime.ToString("yyyy-MM-dd HH:mm"));
                    DateTime now = DateTime.UtcNow;
                    if (now >= nextExecutionTime)
                    {
                        ExportData();
                        lastExecutionTime = now;
                    }
                    await Task.Delay(TimeSpan.FromMinutes(1), cts.Token);
                }
                catch (OperationCanceledException)
                {
                    log.Information("Stopping Auto Export Task");
                    running = false;
                    break;
                }
                catch (Exception ex)
                {
                    log.Error(ex, "Export task unexpected exception");
                    running = false;
                    break;
                }
            }
        }

        internal void Stop()
        {
            cts.Cancel();
            running = false;
        }

        private void ExportData()
        {
            QsoExportFilters filters = new()
            {
                IsNewOnly = true,
                DateTo = ResetMinuteTo(DateTime.UtcNow, 0)
            };

            Dictionary<int, string> adifEntries = dbRepository.GetAdif(filters);

            string infoFilePath = Path.Combine(mainFolder, "export_info.log");
            DateTime now = DateTime.UtcNow;
            if (adifEntries.Count == 0)
            {
                string noDataMessage = $"No new QSOs until {filters.DateTo:yyyy-MM-dd HH:mm}";
                log.Information(noDataMessage);
                File.AppendAllText(infoFilePath, $"{now:yyyy-MM-dd HH:mm}: " + noDataMessage + Environment.NewLine);
                return;
            }
            log.Information("Exporting {qsoAmount} QSOs until: {dateTo}", adifEntries.Count, filters.DateTo);

            var sb = new StringBuilder($"# UR8UQ DXpedition QSO Collector v.{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}\r\n#   Created:  {now:yyyy-MM-dd HH:mm:ss}\r\n#\r\n<ADIF_VER:5>3.1.6\r\n<EOH>\r\n");
            foreach (var item in adifEntries.Values)
            {
                sb.AppendLine(item);
            }
            var fileContent = sb.ToString();

            string fileName = $"export_{now:yyyyMMdd_HHmmss}.adi";
            string exportAllFilePath = Path.Combine(allFolder, fileName);
            log.Information("Exporting {qsoAmount} QSO(s) to file {FilePath}", adifEntries.Count, allFolder);

            File.WriteAllText(exportAllFilePath, fileContent);
            log.Information("Exported QSOs to file: {exportAllFilePath}", exportAllFilePath);
            File.AppendAllText(infoFilePath, $"[{now:yyyy-MM-dd HH:mm}]: {fileName}, All - {adifEntries.Count} QSOs" + Environment.NewLine);
            log.Information("Set QSOs exported to folder {exportAllFilePath} with filename {fileName}. QSO amount {amount}", exportAllFilePath, fileName, adifEntries.Keys.Count);
            dbRepository.SetQSOsExported([.. adifEntries.Keys], exportAllFilePath, fileName, filters, true);
        }

        private void EnsureDirectoriesExist()
        {
            if (!Directory.Exists(mainFolder))
            {
                Directory.CreateDirectory(mainFolder);
            }
            if (!Directory.Exists(allFolder))
            {
                Directory.CreateDirectory(allFolder);
            }
            if (Program.isPremiumAutoExportEnabled && !Directory.Exists(premiumFolder))
            {
                Directory.CreateDirectory(premiumFolder);
            }
        }

        private DateTime GetNextExecutionTime()
        {
            if (hours.Count == 0)
            {
                return DateTime.MaxValue;
            }

            // execute only on 10th minute of hour, i.e. 00:10, 05:10 ...
            int executionMinute = 10;

            DateTime now = DateTime.UtcNow;
            DateTime nextRunTime = now.AddSeconds(-now.Second).AddMicroseconds(now.Microsecond);

            // if executionMinute aleady passed then consider next hour
            if (nextRunTime.Minute > executionMinute) {
                nextRunTime = nextRunTime.AddHours(1);
            }

            // set execution on 10th minute
            nextRunTime = ResetMinuteTo(nextRunTime, executionMinute);

            // if already executed in current hour then consider next hour
            if (nextRunTime.Subtract(lastExecutionTime).TotalMinutes < 60 - executionMinute) {
                nextRunTime = nextRunTime.AddHours(1);
            }

            return GetNextRunTime(nextRunTime, hours);
        }

        private static DateTime GetNextRunTime(DateTime nextRunTime, List<double> hours) {
            // Trying to find next hour in current date
            double nextHour = hours.Where(h => h >= nextRunTime.Hour).DefaultIfEmpty(0).First();
            // if not then swith to the beginning of the next date
            if (nextHour == 0)
            {
                nextRunTime = nextRunTime.AddDays(1).AddHours(-nextRunTime.Hour);
                nextHour = hours.Where(h => h >= nextRunTime.Hour).First();
            }

            // set next run hour
            return ResetHourTo(nextRunTime, nextHour);
        }

        private static DateTime ResetMinuteTo(DateTime datetime, double newMinute) { 
            return datetime.AddMinutes(-datetime.Minute).AddMinutes(newMinute);
        }

        private static DateTime ResetHourTo(DateTime datetime, double newHour)
        {
            return datetime.AddHours(-datetime.Hour).AddHours(newHour);
        }
    }
}