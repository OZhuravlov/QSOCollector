# QSO Collector - Copilot Development Instructions

## Project Overview
**QSO Collector** is a Windows Forms desktop application designed for multi-station amateur radio DXpeditions to reliably collect, manage, and export QSO (radio contact) logs.

### Problem Statement
- **Challenge**: Multi-station DXpeditions need reliable QSO record collection from multiple PCs in unstable WiFi networks, plus daily ADIF export filtering
- **Solution**: TCP-based reliable transmission of QSO records from clients to a central server, with duplicate-free ADIF exports

### Key Features
- Dual-mode operation (Server/Client) for centralized QSO collection
- Reliable TCP-based client-to-server QSO transmission
- Local database queuing when server is unavailable
- UDP listener support for multiple logger formats (N1MM, ADIF)
- ADIF import/export with filtering to prevent duplication
- Automated daily export scheduling
- System tray autostart capability
- Comprehensive logging via Serilog

---

## Technology Stack

### Core
- **Framework**: .NET 10 (net10.0-windows)
- **Language**: C# 14.0
- **UI**: Windows Forms (WinForms)
- **Database**: SQLite (Microsoft.Data.Sqlite)
- **Logging**: Serilog with rolling file outputs
- **DI/Configuration**: Microsoft.Extensions.*, IOptions pattern
- **Testing**: xUnit, NUnit

### Key Libraries
- `DbUp` - Database migrations
- `Serilog` - Structured logging
- `SQLitePCL.raw` - SQLite compatibility
- `System.Net.Sockets` - TCP/UDP networking

### Project Structure
```
QSOCollector/
├── Forms/                  # Windows Forms UI components
│   ├── MainForm.cs        # Main application window
│   ├── ServerClientMonitoringForm.cs # Client monitoring modal (New Feature)
│   ├── QsoExportForm.cs   # Export dialog UI
│   ├── QsoImportForm.cs   # Import dialog UI
│   ├── ListenerConfigForm.cs # UDP listener configuration
│   ├── QsoAutoExportForm.cs  # Auto-export scheduling UI
│   └── [*].Designer.cs    # Auto-generated designer files
├── Network/               # Networking components
│   ├── Server/
│   │   └── TcpServer.cs   # Server-side TCP listener + client monitoring
│   └── Client/
│       ├── UdpClientListener.cs    # UDP QSO receiver
│       ├── TcpClientInstance.cs    # TCP client connection
│       └── QsoMessageSender.cs     # QSO transmission logic
├── Data/                  # Data access layer
│   ├── DbRepository.cs    # Primary DB operations
│   ├── IDbRepository.cs   # Repository interface
│   └── Migrations/        # SQL migration scripts
├── Service/               # Business logic services
│   └── AutoExportTaskService.cs   # Scheduled ADIF export
├── Models/                # Data models
│   ├── QsoMessage.cs      # QSO transmission model
│   ├── ListenerConfig.cs  # UDP listener configuration
│   ├── QsoExportFilters.cs # Export filtering options
│   ├── ClientMonitoringInfo.cs # Client tracking model (New Feature)
│   └── [*.cs]             # Other domain models
├── Parsers/               # QSO format parsers
│   ├── N1mmContactInfoToTableFieldsMapper.cs
│   ├── N1mmContactInfoToAdifQsoMessageMapper.cs
│   └── AdifToTableFieldsMapper.cs
├── Helpers/               # Utility classes
│   ├── ServerProgressUpdater.cs
│   ├── ClientProgressUpdater.cs
│   ├── TemporarelySavedQsoHandler.cs
│   └── ButtonStyleHandler.cs
├── Logging/               # Logging utilities
│   └── ShortSourceContextEnricher.cs
├── Root/
│   └── Program.cs         # Application entry point, DI setup
├── Properties/            # App settings, resources
├── Migrations/            # SQL database schema migrations
├── QSOCollector.csproj    # Main project file
├── appsettings.json       # Configuration file
└── README.md              # User documentation

QSOCollector.Tests/
├── MainFormTests.cs
├── Parsers/
│   └── AdifParserTests.cs
├── Network/
│   └── UdpTcpIntegrationTests.cs
└── QSOCollector.Tests.csproj
```

---

## Application Architecture

### Initialization Flow (Program.cs)
1. **Single Instance Check**: Mutex prevents multiple application instances
2. **Logging Setup**: Serilog configured with context-aware file outputs:
   - `client.log` - Client-side network operations
   - `server.log` - Server-side operations
   - General logs for other components
3. **Database**: SQLite initialized with DbUp migrations
4. **Dependency Injection**: IHost with services registered:
   - `IDbRepository` (DbRepository)
   - `AutoExportTaskService` (with IOptions<AutoExportTaskOptions>)
   - TCP/UDP network components
5. **Windows Forms**: MainForm loaded with DI container

### Data Folder Structure
```
%USERPROFILE%\Documents\QSOCollector/
├── db/                    # SQLite database files
├── logs/                  # Rolling log files (30-day retention)
├── config/                # User configuration exports
├── import/                # Default ADIF import location
├── export/                # Manual ADIF export location
└── auto/
    └── QSO_export/        # Automated export output
        ├── all/           # All exported QSOs
        └── premium/       # Premium callsign subset (if enabled)
```

---

## Core Components & Patterns

### 1. Server Mode (TcpServer.cs)
- **Purpose**: Central collection point for QSOs from all clients
- **Operation**:
  - Listens on configurable TCP port (e.g., 12345)
  - Accepts connections from multiple clients
  - Receives QSO messages and stores in central SQLite database
  - Tracks connected clients and their status
- **Thread Model**: Async/await with CancellationToken
- **Client Monitoring** (New Feature):
  - Maintains `ConcurrentDictionary<string, ClientMonitoringInfo>` keyed by client IP address
  - Tracks per-client metrics: Connection time, last activity, QSOs received, status (Connected/Disconnected)
  - Public method `GetClientsMonitoring()` provides thread-safe access for UI
  - `AcceptedClient` class updates dictionary in real-time:
    - Sets status to `Connected` on client acceptance
    - Updates `LastActivityTime` on every successful message receive
    - Increments `QsosReceived` after each successful database save
    - Sets status to `Disconnected` before all client disconnections (multiple error paths)
  - Disconnected clients retained indefinitely in dictionary for history viewing

### 2. Client Mode (UdpClientListener + TcpClientInstance)
- **UDP Listeners** (UdpClientListener.cs):
  - Listens on multiple UDP ports (one per logger)
  - Receives QSO broadcasts in N1MM or ADIF format
  - Optionally forwards to secondary UDP port (for ClubLog livestream)
  - Monitors heartbeat port (logger alive indicator)
  - Queues QSO messages for TCP transmission
  
- **TCP Client** (TcpClientInstance.cs):
  - Connects to server on startup
  - Transmits queued QSO messages reliably
  - Stores locally when server unavailable (TemporarelySavedQsoHandler)
  - Retransmits stored QSOs when connection restored

### 3. Database Access (DbRepository.cs)
- **Implements**: `IDbRepository` interface
- **Key Operations**:
  - `SaveQsoRecords()` - Store QSO data (new or temporary)
  - `GetAdif()` - Export with QsoExportFilters (new-only, date range, all)
  - `SetQSOsExported()` - Mark QSOs exported to prevent duplication
  - `ImportQsoRecords()` - Import ADIF files
  - `SaveExportHours()` / `GetExportHours()` - Auto-export scheduling
  - Configuration persistence (settings, listener configs)

### 4. Auto-Export Service (AutoExportTaskService.cs)
- **Purpose**: Scheduled ADIF export (typically daily)
- **Operation**:
  - Loads configuration from `DbRepository.LoadSettings()` on initialization
  - Configured export hours (e.g., 06:00, 18:00 UTC)
  - Checks at :10 minute mark each hour
  - Exports only new (unmarked) QSOs
  - Creates timestamped `.adi` files in `auto/QSO_export/all/`
  - Logs export history to `export_info.log`
  - Marks exported QSOs in database
- **Configuration**: Settings stored in database with key `"AutoExportFolder"` (defaults to `Program.defaultAutoExportFolder`)
- **Execution**: Background task started with `Start()`, stopped with `Stop()`
- **Dependency**: `IDbRepository` for settings and data access

### 5. Format Parsers
- **N1MM Format** (N1mmContactInfoToTableFieldsMapper, N1mmContactInfoToAdifQsoMessageMapper):
  - Parses N1MM/DxLog UDP broadcasts
  - Maps to standardized QSO model, then to ADIF
  
- **ADIF Format** (AdifToTableFieldsMapper):
  - Imports/exports standard ADIF QSO records

### 6. UI Forms
- **MainForm.cs**: Tabbed interface (Server/Client tabs), real-time status display
- **QsoExportForm.cs**: Export dialog with filtering (New/By Date/All)
- **QsoImportForm.cs**: Import dialog with preview and progress
- **ListenerConfigForm.cs**: UDP listener configuration CRUD
- **QsoAutoExportForm.cs**: Auto-export schedule editor
- **ServerClientMonitoringForm.cs**: Modal form displaying real-time connected clients (New Feature)
  - Displays: IP address, status, connection time, last activity, QSO count
  - Auto-refreshes every 10 seconds from `ConcurrentDictionary<string, ClientMonitoringInfo>`
  - Shows connected clients first, then disconnected (for history)
  - Sorted by most recent activity
  - Thread-safe reads from server's monitoring dictionary
- **Cleanup Forms**: Data management/archival

---

## Data Models

### QsoMessage
```csharp
public class QsoMessage
{
    public string? Source { get; set; }           // Callsign
    public string OriginalFormat { get; set; }    // N1MM or ADIF
    public string OriginalQsoData { get; set; }   // Raw format
    public string AdifQsoData { get; set; }       // Normalized ADIF
    public bool Replace { get; set; }             // Duplicate handling flag
    public bool IsHeartBeat { get; }              // True if Source == "TEST"
}
```

### ListenerConfig
```csharp
public class ListenerConfig
{
    public int Id { get; set; }
    public string Name { get; set; }              // Logger identifier (N1MM, DxLog, etc.)
    public int QsoPort { get; set; }              // UDP port listening for QSOs (required)
    public int? ForwardPort { get; set; }         // Forward to other listeners (optional)
    public int? AcknowledgePort { get; set; }     // Heartbeat listener (optional)
    public string MessageFormat { get; set; }     // N1MM or ADIF
    public bool IsActive { get; set; }            // Enable/disable without deletion
}
```

### QsoExportFilters
```csharp
public class QsoExportFilters
{
    public bool IsNewOnly { get; set; }           // Export only new (unmarked) QSOs
    public DateTime DateFrom { get; set; }        // Start date filter
    public DateTime DateTo { get; set; }          // End date filter
}
```

### ClientMonitoringInfo (New Feature)
```csharp
public class ClientMonitoringInfo
{
    public required string IpAddress { get; set; }      // Client IP address (dictionary key)
    public ClientStatus Status { get; set; }            // Connected or Disconnected
    public DateTime ConnectionTime { get; set; }        // UTC time when client accepted
    public DateTime LastActivityTime { get; set; }      // UTC time of last message/heartbeat
    public int QsosReceived { get; set; }               // Count of QSOs successfully saved to DB
}

public enum ClientStatus
{
    Unknown,        // Initial state (rarely used)
    Connected,      // Client actively sending messages
    Disconnected    // Client disconnected (kept for history)
}
```
- **Purpose**: Track real-time client connections and activity for monitoring form
- **Thread-Safety**: Shared via `ConcurrentDictionary<string, ClientMonitoringInfo>` in TcpServer
- **Lifecycle**: Added on client acceptance, status updated to Disconnected on disconnection, never removed

---

## Logging Strategy

### Serilog Configuration
Located in `Program.cs`, configured with **context-aware file outputs**:

1. **Client Log** (`client.log`):
   - Source filter: `QSOCollector.Network.Client`
   - Captures UDP/TCP client-side operations
   - Rolling daily, 30-day retention

2. **Server Log** (`server.log`):
   - Source filter: `QSOCollector.Network.Server`
   - Captures server acceptor/handler operations
   - Rolling daily, 30-day retention

3. **General Logs**:
   - Auto-export service, DB operations, UI events
   - Example: `Log.ForContext<AutoExportTaskService>()`

### Structured Logging Best Practices
- Use named parameters: `log.Information("Processing {qsoCount} QSOs", adifEntries.Count)`
- Include context: `Log.ForContext<ClassName>()`
- Use appropriate levels: Debug (detailed), Information (events), Error (exceptions)
- Output format: `{Timestamp:u} [{Level:u3}] {ShortSourceContext,-30}: {Message:lj}{NewLine}{Exception}`

---

## Key Algorithms & Business Logic

### Auto-Export Next Execution Time (GetNextExecutionTime)
1. Load configured export hours from database (e.g., [6, 18] for 06:00, 18:00 UTC)
2. Find next hour >= current hour from the list
3. If none found today, move to next day
4. Execute at :10 minute mark (e.g., 06:10, 18:10)
5. Prevent re-execution within same hour window
6. Only export new QSOs marked `IsNewOnly = true`

### QSO Deduplication
- **Export Marking**: When QSOs are exported, `SetQSOsExported()` marks them
- **Import Handling**: Imported QSOs are checked against existing records
- **Replace Flag**: QsoMessage.Replace indicates duplicate/correction handling

### Client Offline Resilience (TemporarelySavedQsoHandler)
- When TCP server unavailable, QSOs saved locally
- On reconnection, stored QSOs transmitted with retry logic
- Prevents data loss during network outages or server downtime

---

## Configuration

### Settings Persistence
- **Pattern**: Key-value store in SQLite `settings` table
- **Methods**: 
  - `DbRepository.LoadSettings()` - Returns `Dictionary<string, string?>`
  - `DbRepository.SaveSetting(key, value)` - Persist individual settings
- **Common Keys**:
  - `"ServerEnabled"` - Server mode enabled
  - `"ServerPort"` - Server TCP port
  - `"ClientEnabled"` - Client mode enabled
  - `"ClientServerNameIp"` - Server address/hostname
  - `"ClientServerPort"` - Server port from client perspective
  - `"AutoStart"` - Auto-start with Windows
  - `"AutoExportFolder"` - Custom auto-export folder path
- **Default Values**: Specified in code if key not found
- **UI Integration**: `MainForm.RestoreSettingsFromDb()` / `SaveSettingsToDB()`

---

## Coding Conventions & Patterns

### Naming
- **Classes**: PascalCase (TcpServer, UdpClientListener)
- **Methods**: PascalCase (GetNextExecutionTime, ExportData)
- **Properties**: PascalCase (IsActive, MessageFormat)
- **Fields**: camelCase with underscore prefix for private fields (_cts, _dbRepository)
- **Constants**: UPPER_CASE (executionMinute = 10; stored as named constant in logic)

### Async/Await Patterns
- Use `async Task` for background operations (TcpServer, AutoExportTaskService)
- Use `CancellationToken` for graceful shutdown
- Use `CancellationTokenSource` for cancellation signaling (cts = new CancellationTokenSource())

### Dependency Injection
- Constructor injection for dependencies: `public AutoExportTaskService(IDbRepository dbRepository, IOptions<AutoExportTaskOptions> options)`
- Interface abstractions: `IDbRepository` for testability
- IOptions<T> pattern for configuration

### Error Handling
- Log exceptions with context: `log.Error(ex, "Export task unexpected exception")`
- Graceful degradation (e.g., Client stores QSOs locally when server unavailable)
- No silent failures; always log at Error level for issues

### Testing
- Unit tests in `QSOCollector.Tests/`
- Integration tests for UDP/TCP communication (UdpTcpIntegrationTests.cs)
- Parser tests for format conversion (AdifParserTests.cs)
- Test fixtures for database scenarios

---

## Common Development Tasks

### Adding a New QSO Field
1. Add to ADIF specification in parser (`AdifToTableFieldsMapper.cs` or `N1mmContactInfoToAdifQsoMessageMapper.cs`)
2. Update database schema via migration (SQL file in `Migrations/` folder)
3. Update `QsoMessage` or relevant model if needed
4. Update export filters if field should be filterable
5. Test with sample ADIF files

### Creating a New Export Format
1. Implement parser interface (model-to-format mapper)
2. Add format selection to UI (QsoExportForm.cs)
3. Update `DbRepository.GetAdif()` to support new format
4. Add unit tests in `QSOCollector.Tests/Parsers/`

### Debugging Network Issues
- Check `client.log` and `server.log` in `%USERPROFILE%\Documents\QSOCollector\logs\`
- Enable "Show Log Details" in UI for debug-level logs
- Monitor UDP port binding in Command Prompt: `netstat -ano | findstr :PORT_NUMBER`
- Test UDP listener independently before TCP integration

### Database Schema Changes
1. Create migration SQL file in `Migrations/` (numbered sequentially, e.g., `015_my_change.sql`)
2. Register in `Program.cs` with DbUp
3. Test migration on fresh database and upgrade scenarios
4. Update repository interface/implementation as needed

---

## Premium Features
- **Premium Auto-Export**: Controlled by `Program.isPremiumAutoExportEnabled` flag
- **Premium Callsign Filtering**: Subset export to `premium/` folder via `PremiumCallsign` model
- Currently disabled but infrastructure in place for future enabling

---

## Deployment
- **Setup Project**: `Setup-QSOCollector` (included in solution)
- **Build Output**: Produces installer package
- **Auto-Start**: Registry entry for Windows startup via `appGuid`
- **System Tray**: Application minimizes to tray, restores on icon double-click

---

## Development Environment
- **IDE**: Microsoft Visual Studio Community 2026 (18.4.3)
- **Repository**: GitHub (https://github.com/OZhuravlov/QSOCollector)
- **Shell**: PowerShell
- **Build Target**: net10.0-windows

---

## Key Files Reference
| File | Purpose |
|------|---------|
| `Program.cs` | Entry point, DI setup, logging config |
| `AutoExportTaskService.cs` | Scheduled ADIF export logic (loads settings from DbRepository) |
| `TcpServer.cs` | Server-mode TCP listener |
| `UdpClientListener.cs` | Client-mode UDP receiver |
| `DbRepository.cs` | All database operations including settings persistence |
| `MainForm.cs` | Primary UI form (Server/Client tabs) |
| `QsoExportForm.cs` | Export filtering UI |
| `QsoImportForm.cs` | Import preview UI |
| `ListenerConfigForm.cs` | UDP listener configuration CRUD |
| `appsettings.json` | Optional runtime configuration (logging, etc.) |

---

## Tips for Contributors
1. **Always use Serilog**: `Log.ForContext<ClassName>()` for context-aware logging
2. **Respect Network Resilience**: Design for WiFi failures; implement local fallbacks
3. **Test Deduplication**: Export/import cycles should maintain integrity
4. **Check Log Levels**: Ensure debug info available for troubleshooting
5. **Database Migrations**: Every schema change needs a migration; test upgrades
6. **UI Thread Safety**: Use Invoke/BeginInvoke for cross-thread form updates
7. **Async Cancellation**: Always pass CancellationToken to long-running tasks
8. **Settings Management**: Use `DbRepository.LoadSettings()` / `SaveSetting()` for persistence; avoid hardcoded values
9. **Service Initialization**: Services load their dependencies (e.g., settings) during `Init()`, not constructor

