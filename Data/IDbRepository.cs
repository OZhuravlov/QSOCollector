using QSOCollector.Models;

namespace QSOCollector.Data
{
    public interface IDbRepository
    {
        string GetConnectionString();
        Dictionary<string, string?> LoadSettings();
        void SaveSetting(string key, string? value);
        List<ListenerConfig> GetListenerConfigs();
        void ReplaceListenerConfigs(List<ListenerConfig> configs);
        void CleanClientQsos();
        void CleanupServerQsoData();
        void CleanRawQsoData();
        List<ServerQsoAmount> GetServerQsoAmounts();
        List<QsoExportExpectedAmounts> GetQsoAmountsForExport();
        void ForceImportQsoRecords(List<Dictionary<string, string?>> dups, string folder, string fileName, Func<string, Task> progressUpdater);
        List<Dictionary<string, string?>> ImportQsoRecords(List<Dictionary<string, string?>> qsoRecords, string folder, string fileName, Func<string, Task> progressUpdater);
        void SaveQsoRecords(List<Dictionary<string, string?>> qsoRecords, int? importId = null, bool isTemporary = false);
        void SaveRawQso(QsoMessage qsoMessage);
        Dictionary<int, QsoMessage> GetTemporaryQsoMessages();
        void DeleteTemporaryQsoRecord(int id);
        Dictionary<int, string> GetAdif(QsoExportFilters exportFilters);
        void SetQSOsExported(List<int> keys, string folder, string fileName, QsoExportFilters filter, bool isConfirmed);
        List<string> GetExportHours();
        void SaveExportHours(List<string> hours);
        DateTime GetLatestExportTaskTime();
    }
}
