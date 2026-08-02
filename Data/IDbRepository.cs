using QSOCollector.Models;

namespace QSOCollector.Data
{
    public interface IDbRepository
    {
        const int SQLITE_CONSTRAINT_UNIQUE = 2067;
        const int SQLITE_CONSTRAINT_FOREIGN_KEY = 787;

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
        void DeleteQsoRecord(int id);
        Dictionary<int, string> GetAdif(QsoExportFilters exportFilters);
        void SetQSOsExported(List<int> keys, string folder, string fileName, QsoExportFilters filter, bool isConfirmed);
        List<string> GetExportHours();
        void SaveExportHours(List<string> hours);
        DateTime GetLatestExportTaskTime();
        List<Dictionary<string, object?>> SearchQsosByCall(string callPattern, string? modeGroup = null, string? band = null, int maxResults = 200);
        List<string> GetDistinctModeGroups();
        List<string> GetDistinctBands();
        string? GetListenerConcatRuleNames(int id);
        Band GetBand(int id);
        List<Band> GetBands(bool isActiveOnly = false);
        List<SatRule> GetSatRules();
        SatRule GetSatRule(int id);
        List<string> GetSatModes();
        void SaveSatRule(SatRule rule);
        void DeleteSatRule(int value);
        void DeactivateSatRule(int value);
        void ActivateSatRule(int value);
        void RemoveRulesFromListener(int listenerId, List<int> selectedRuleIds);
        void AssignRuleToListener(int listenerId, int ruleId);
        List<SatRule> GetListenerSatRules(int listenerId);
    }
}
