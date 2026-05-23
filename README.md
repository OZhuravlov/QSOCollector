# QSOCollector - DXpedition QSO Management System

> **Multi-Station Amateur Radio DXpedition QSO Collection & Management**
>
> ![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?logo=.net)
> ![Platform](https://img.shields.io/badge/Platform-Windows-0078D4)
> ![Status](https://img.shields.io/badge/Status-Active-green)

## 📋 Quick Navigation

- **[System Requirements](#system-requirements)** - Hardware & software prerequisites
- **[Installation](#installation)** - Setup & configuration
- **[Quick Start Guide](#quick-start-guide)** - Get running in 5 minutes
- **[Features](#features)** - Detailed feature list
- **[How to Use](#how-to-use)** - Server & Client operation
- **[QSO Search](#qso-search-feature)** - Search & filter QSOs
- **[Troubleshooting](#troubleshooting)** - Common issues & solutions
- **[FAQ](#frequently-asked-questions)** - Questions & answers
- **[Full User Manual](./UserManual/index.html)** - Interactive HTML manual (recommended)

---

## 🎯 Overview

**QSOCollector** is a professional-grade Windows application designed specifically for **multi-station DXpeditions** to collect, manage, and export QSO (radio contact) records with reliability and precision.

Unlike traditional contest logging software, QSOCollector is purpose-built for DXpedition operations where:
- Multiple logging stations operate simultaneously
- Network connectivity is unstable (typically WiFi)
- Duplicate QSO elimination is critical for daily uploads
- Centralized QSO database management is essential

---

## ⚠️ Problem Statement

**The Challenge:**
DXpeditions face critical issues when managing QSO records from multiple loggers across multiple PCs:
1. **Network unreliability** - UDP broadcasting fails in WiFi environments, causing QSO loss/duplication
2. **Export duplication** - Daily ADIF exports include previously exported QSOs
3. **Decentralized logs** - Each logger maintains separate logs; no unified database exists
4. **Manual consolidation** - Merging logs from multiple sources is error-prone and time-consuming

**The Solution - QSOCollector:**
| Problem | Solution |
|---------|----------|
| UDP unreliability | Reliable TCP protocol with local queuing |
| Export duplication | Intelligent filtering to mark/skip exported QSOs |
| Decentralized logs | Central SQLite database on server PC |
| Multiple formats | Support for N1MM and ADIF formats |
| Network failures | Automatic client-side caching & sync |


## 🔧 System Requirements

### Minimum Requirements
| Component | Requirement |
|-----------|-------------|
| **OS** | Windows 10/11 (22H2 or later) |
| **Framework** | .NET 10 Desktop Runtime |
| **.NET Runtime** | [Download .NET 10](https://dotnet.microsoft.com/download/dotnet/10.0) |
| **RAM** | 2 GB minimum (4 GB recommended) |
| **Disk Space** | 200 MB for application + database |
| **Network** | Ethernet or WiFi 802.11ac minimum |

### Optional Components
- **Admin Rights**: Required for Windows startup autostart feature
- **Firewall**: TCP port configuration needed (see [Network Configuration](#network-configuration))

### Supported Logger Software
- **N1MM** (N1MM, DXLog, SDC, etc.)
- **ADIF** (any ADIF-compatible logger)

---

## 📥 Installation

### Step 1: Download & Extract
1. Download **QSOCollector.zip** from GitHub releases
2. Extract to a permanent location (e.g., `C:\Program Files\QSOCollector` or `C:\Users\[username]\QSOCollector`)
3. Do NOT run from Downloads or temporary folders

### Step 2: Install .NET 10 Runtime (if needed)
```powershell
# Check if .NET 10 is installed
dotnet --version

# If not installed, download from:
# https://dotnet.microsoft.com/download/dotnet/10.0
```

### Step 3: Run Application
- **First Launch**: Double-click `QSOCollector.exe`
- **Database**: SQLite database will be created automatically in `%AppData%\QSOCollector\`
- **Logs**: Application logs stored in same directory

### Step 4: Configure Mode
- Select **Server** tab to run as Server (one PC only)
- Select **Client** tab to run as Client (multiple PCs)

---

## ⚡ Quick Start Guide

### 5-Minute Server Setup

```
1. Launch QSOCollector.exe

2. Click "Server" tab

3. Enter TCP Port (e.g., 5000) where Clients will connect

4. Click "Start Server"

5. Monitor "Server Log" for activity

6. Confirm "Server Status" shows "Running"
```

### 5-Minute Client Setup

```
1. Launch QSOCollector.exe on each logger PC

2. Click "Client" tab

3. Enter Server IP Address (e.g., 192.168.1.50)

4. Enter Server TCP Port (must match Server)

5. Click "UDP Listeners" and configure:
   - Listener Name: "N1MM Logger"
   - QSO Port: 12060 (from your logger)
   - Format: N1MM or ADIF

6. Click "Start Client"

7. Monitor "Server Status" for connection

8. Enable "Start automatically" for Windows startup
```

---

## ⭐ Features
- Collecting all QSOs broadcasted via UDP by loggers locally avoiding QSO loss
- Sending of collected QSOs from client PCs to server PC via reliable TCP protocol and store them in central database
- Store QSOs on client PC when server unavailable and sending them to server PC once it restored
- Importing any ADIF files on server side if needed
- Exporting ADIF files with different filtering options to avoid duplication of exported QSOs

### Core Capabilities
- ✅ **Reliable TCP Transport** - Collect QSOs from multiple loggers without loss
- ✅ **Centralized Database** - Single SQLite database for all QSO records
- ✅ **Network Resilience** - Automatic local caching when server is unavailable
- ✅ **Duplicate Prevention** - Intelligent ADIF export filtering
- ✅ **Multi-Format Support** - N1MM and ADIF input formats
- ✅ **Flexible Filtering** - Export by date range, new QSOs only, or all QSOs
- ✅ **QSO Search** - Search & filter records by callsign, band, mode
- ✅ **Auto-startup** - Optional Windows startup integration
- ✅ **Activity Logging** - Detailed logs for debugging & troubleshooting
- ✅ **Import/Export** - ADIF file import and export with formatting options

---

## 🏗️ Architecture

### Server Mode
- Runs on **one central PC** only
- **Listens** for TCP connections from Clients on specified port
- **Stores** all QSO records in central SQLite database
- **Provides** import/export functionality
- **Tracks** exported QSOs to prevent duplication

### Client Mode
- Runs on **each logger PC** (multiple instances allowed)
- **Listens** for QSO broadcasts from local loggers via UDP
- **Buffers** QSOs locally if server unavailable
- **Transmits** QSOs to Server via reliable TCP protocol
- **Auto-retries** on connection failures

### Communication Flow
```
┌─────────────────────────────────────────────────────────┐
│                    DXpedition Setup                      │
├─────────────────────────────────────────────────────────┤
│                                                           │
│  Logger1 ──UDP──┐                      ┌─ Server PC ─┐   │
│                 │                      │             │    │
│  Logger2 ──UDP──┼─ Client PC #1 ──TCP──┤  Central    │    │
│                 │                      │  Database   │    │
│  Logger3 ──UDP──┘                      │             │    │
│                                        └─────────────┘    │
│                                                            │
│  Logger4 ──UDP──┐                                          │
│                 │                                         │
│  Logger5 ──UDP──┼─ Client PC #2                           │
│                 │     (automatic buffering on disconnect) │
│  Logger6 ──UDP──┘                                          │
│                                                            │
└─────────────────────────────────────────────────────────┘
```

---

## 📖 How to Use

### Server Operations

#### Starting the Server
1. Click **"Server" tab**
2. Enter **TCP Port** (e.g., 5000) - must match Client configuration
3. Click **"Start Server"** button
4. Monitor **"Server Log"** for incoming client connections
5. Confirm status bar shows **"Running"**

#### Collecting QSOs
- All Clients will automatically send QSOs to this Server
- View activity in **Server Log** section
- Enable **"Show Log details"** checkbox for verbose logging
- QSO statistics displayed above log (Total, Received, Sent)

#### Exporting QSOs
1. Click **"QSO Export"** button to open Export dialog
2. Select main filter:
   - **"New QSOs"** - Only QSOs not previously exported (recommended)
   - **"By Date"** - Specific date range
   - **"All QSOs"** - Export all QSOs in database
3. Apply secondary filters if needed (Mode Group, Band, Operator)
4. View filtered count at bottom (**"QSO Amount" → "Filtered"**)
5. Click **"Export"** and choose output filename
6. File saved to `%UserProfile%\Documents\QSOCollector\export\`
7. **Recommended**: Mark as exported to prevent duplication in next export
8. Stats updated with "Last Exported QSO" timestamp

#### Importing QSOs
1. Click **"QSO Import"** button
2. Browse to ADIF file (default folder: `%UserProfile%\Documents\QSOCollector\import\`)
3. Preview file content in dialog
4. Click **"Continue import"**
5. Monitor progress bar
6. Results show: new QSOs saved + duplicates skipped
7. Database stats update automatically

### Client Operations

#### Configuring UDP Listeners

UDP Listeners collect QSO broadcasts from local loggers on the same PC.

1. Click **"Client" tab**
2. Click **"UDP Listeners"** button
3. Click **"Add"** to create new listener
4. Configure:
   - **Listener Name** ⭐ (required): Name for identification (e.g., "N1MM Logger", "DXLog")
   - **QSO Port** ⭐ (required): Port from your logger settings (e.g., 12060 for N1MM)
   - **Forward Port**: Optional - forward to another application listening for QSOs
   - **Heart Beat Port**: Optional - port for logger keep-alive messages
   - **Format** ⭐ (required): Choose "N1MM" or "ADIF" (must match logger output)
   - **Active**: Check to enable this listener
5. Click **"Save"** to persist configuration
6. Click **"Export Config"** to save settings to file (for use on other client PCs)

#### Connecting to Server

1. Click **"Client" tab**
2. Enter **Server IP Address** (e.g., 192.168.1.50 or hostname)
3. Enter **Server TCP Port** (must match server configuration)
4. Click **"Start Client"**
5. Monitor **"Server Status"** section:
   - 🟢 **Connected** - QSOs will send immediately
   - 🔴 **Disconnected** - QSOs stored locally, retry when available

#### Monitoring Client Activity

- **Client Log**: View all client events and errors
- **"Log details"** checkbox: Enable verbose logging for troubleshooting
- **Server Status**: Connection status indicator
- **QSO Processing**: Statistics on received, sent, cached, rejected QSOs
- **Last Timestamp**: Track when each category last had activity

#### Network Disconnection Handling

When server becomes unavailable:
1. Client automatically switches to **local buffering mode**
2. All received QSOs stored in local SQLite database
3. "Server Status" changes to **Disconnected** 🔴
4. When server comes back online, Client automatically:
   - Reconnects
   - Sends all buffered QSOs
   - Resumes real-time transmission
5. No QSOs are lost during downtime

### QSO Search Feature

The QSO Search feature allows you to quickly find specific QSO records in the database.

#### Opening QSO Search

1. Click **"Server" tab**
2. Click **"Search QSOs"** button
3. Modal dialog opens with search interface

#### Performing a Search

1. Enter **callsign pattern** in search box (e.g., "N0CALL")
   - Minimum 3 alphanumeric characters required
   - Text auto-converts to UPPERCASE
   - Invalid characters rejected automatically

2. (Optional) Select filters:
   - **Mode Group**: CW, SSB, DATA, etc.
   - **Band**: 80m, 40m, 20m, etc.
   - Filters auto-populate from search results

3. Press **Enter** or click **"Search"** button

4. Results display in table (max 200 rows):
   - **Callsign**: QSO partner call
   - **QSO Time**: Date and time of QSO
   - **Mode Group**: CW/SSB/DATA
   - **Mode**: Specific mode (CW, USB, etc.)
   - **Band**: Frequency band
   - **Frequency**: Exact frequency
   - **Operator**: Operator name/call
   - **Source IP**: Which client sent QSO

#### Search Patterns

| Example | Result |
|---------|--------|
| `N0CALL` | Contains "N0CALL" |
| `N0%` | Starts with "N0" |
| `%CALL` | Ends with "CALL" |
| `N_CALL` | One character wildcard |

#### Sorting Results

- Click any column header to sort ascending
- Click again to sort descending
- Sort indicator shows in header

### Autostart Configuration

**Important**: Enable autostart to ensure QSOs are not lost during DXpedition.

1. Check **"Start automatically"** checkbox (top-right of window)
2. Application will:
   - Minimize to system tray on startup
   - Run with previously saved configuration
   - Restore from tray by double-clicking tray icon
3. Can be toggled on/off at any time

---

## 🔌 Network Configuration

### Finding Your IP Address

```powershell
# Open Command Prompt and run:
ipconfig

# Look for "IPv4 Address" (e.g., 192.168.1.50)
```

### Recommended Port Numbers

| Service | Port | Notes |
|---------|------|-------|
| Server TCP | 5000-5010 | Any available port |
| Logger UDP | 12060+ | From logger configuration |
| Forward UDP | 12080+ | For alternate listeners |

### Firewall Configuration

**Windows Firewall - Allow QSOCollector:**

1. Open **Windows Defender Firewall** → Advanced Settings
2. Click **"Inbound Rules"** → **"New Rule"**
3. Choose **"Program"**, browse to `QSOCollector.exe`
4. Select **"Allow the connection"**
5. Check: Domain, Private, Public
6. Finish

**Router Configuration (for remote access):**
- Port forward TCP port to server PC
- Not required for local DXpedition network

---

## ❓ Frequently Asked Questions

### General

**Q: Can I run multiple servers?**  
A: No. Only ONE PC should run in Server mode. Others must be Clients.

**Q: Can I move QSOCollector to a different PC?**  
A: Yes. Copy entire folder and database location. Database stored in `%AppData%\QSOCollector\`.

**Q: What if I need to reset the database?**  
A: Delete database file in `%AppData%\QSOCollector\`. New database created on next launch (WARNING: QSOs lost).

### Client Issues

**Q: Client shows "Server Unavailable" but server is running**  
A: Check firewall, ensure IP address and port are correct, verify server is actually running.

**Q: QSOs arriving but not showing in server database**  
A: Check "Server Log" for errors. Ensure clients are connected (green indicator). Check "QSO Processing" stats.

**Q: UDP listeners show but no QSOs received**  
A: Verify logger is broadcasting on correct port. Check listener "Active" checkbox. Check Format matches logger output.

### Export/Import

**Q: Getting duplicate QSOs in daily exports?**  
A: Use "New QSOs" filter, and mark QSOs as exported after each export. Check client for duplicate sends.

**Q: Import failing with "Invalid ADIF file"**  
A: Ensure ADIF file is valid and not corrupted. Try opening in text editor to verify format.

**Q: Can I export to other formats?**  
A: Only ADIF format is supported for export. Use ADIF converters for other formats.

### Performance

**Q: Large number of QSOs slowing down the application?**  
A: Search is limited to 200 results. Archive old QSOs or split into multiple databases if needed.

**Q: Why is database growing large?**  
A: Each QSO record takes ~200 bytes. 10,000 QSOs ≈ 2 MB. Database files can be archived/backed up.

---

## 🔧 Troubleshooting

### Server Won't Start

**Symptom**: "Start Server" button does nothing or error appears

**Solution**:
1. Check if port is already in use: `netstat -ano | findstr :5000`
2. Try different port number
3. Check Windows Firewall allows QSOCollector
4. Review Server Log for detailed error message

### Clients Not Connecting

**Symptom**: "Server Unavailable" persists even when server is running

**Solution**:
1. Verify server IP and port are correct
2. Test connectivity: `ping [server-ip]`
3. Test port open: `Test-NetConnection -ComputerName [server-ip] -Port 5000`
4. Check Windows Firewall on both PCs
5. Restart both Client and Server

### QSOs Not Received

**Symptom**: QSOs from loggers not appearing in server database

**Solution**:
1. Verify UDP listeners are configured with correct ports
2. Check listener "Active" checkbox
3. Verify logger is broadcasting (check logger configuration)
4. Enable "Log details" on client and monitor for UDP reception
5. Test logger directly: `netstat -ano | findstr UDP`

### High CPU or Memory Usage

**Symptom**: Application sluggish or using excessive resources

**Solution**:
1. Disable "Log details" - reduces verbose logging
2. Check database size (`%AppData%\QSOCollector\qso.db`)
3. Reduce search results limit or archive old QSOs
4. Restart application to clear memory

### Database Errors

**Symptom**: "Database locked" or similar error

**Solution**:
1. Ensure only one instance of Server is running
2. Close and reopen application
3. Check disk space availability
4. Backup `%AppData%\QSOCollector\qso.db` and try repair tool

---

## 📚 Additional Resources

- **[Full Interactive Manual](./UserManual/index.html)** - Comprehensive documentation
- **[QSO Search Feature Guide](./QSO_SEARCH_QUICK_REFERENCE.md)** - Search detailed guide
- **[GitHub Repository](https://github.com/OZhuravlov/QSOCollector)** - Source code
- **[Issues & Feedback](https://github.com/OZhuravlov/QSOCollector/issues)** - Report problems
- **[Copilot Developer Guide](./.copilot/instructions.md)** - For developers

---

## 📝 License & Support

QSOCollector is open-source software.

- **Source**: [GitHub - OZhuravlov/QSOCollector](https://github.com/OZhuravlov/QSOCollector)
- **Issues**: Report bugs on GitHub Issues
- **Contributing**: Pull requests welcome for improvements

---

## 🚀 Advanced Topics (See Full Manual)

- Network performance tuning
- Database maintenance & optimization
- Bulk ADIF import/export
- Client monitoring dashboard
- Logging configuration & analysis
- Development & API reference

For detailed information on any topic, see the **[Full Interactive User Manual](./UserManual/index.html)**.

---

*Last Updated: February 2026*  
*.NET 10 • Windows 10/11 • Apache 2.0 License*

