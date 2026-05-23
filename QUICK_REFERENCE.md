# QSOCollector - Quick Reference Card

**Cheat Sheet** | **One-Page Guide** | **Print This!**

---

## ⚡ 60-Second Setup

### Server PC
```
1. Run QSOCollector.exe
2. Click "Server" tab
3. Enter Port: 5000
4. Click "Start Server"
5. Status: 🟢 Running
```

### Client PC (Each Logger)
```
1. Run QSOCollector.exe
2. Click "Client" tab
3. Enter Server IP: 192.168.1.50
4. Enter Server Port: 5000
5. Click "UDP Listeners"
6. Add listener: Port 12060, Format: N1MM
7. Click "Start Client"
8. Status: 🟢 Connected
```

---

## 🔑 Key Commands

| Task | Steps |
|------|-------|
| **Start Server** | Server tab → Port 5000 → "Start Server" |
| **Connect Client** | Client tab → IP/Port → "Start Client" |
| **Configure UDP** | Client tab → "UDP Listeners" → Add → Save |
| **Export QSOs** | "QSO Export" → Filter → Export → Mark as exported |
| **Import QSOs** | "QSO Import" → Select file → Continue |
| **Search QSOs** | "Search QSOs" → Enter callsign → Search |
| **Enable Autostart** | Check "Start automatically" checkbox |

---

## 📍 Important Paths

```
Database:       %AppData%\QSOCollector\qso.db
Logs:           %AppData%\QSOCollector\logs\
Export Default: %UserProfile%\Documents\QSOCollector\export\
Import Default: %UserProfile%\Documents\QSOCollector\import\
```

---

## 🌐 Network Configuration

### Default Ports
- Server TCP: **5000** (configurable)
- Logger UDP: **12060** (from logger settings)
- Forward UDP: **12080** (optional)
- Heartbeat UDP: **12061** (optional)

### Find Your IP
```powershell
ipconfig
# Look for "IPv4 Address" (e.g., 192.168.1.50)
```

### Check Port Open
```powershell
Test-NetConnection -ComputerName 192.168.1.50 -Port 5000
```

---

## 🎯 Daily Workflow

### Morning
```
1. Start Server
2. Verify all Clients connecting
3. Check Server Log for no errors
4. Enable autostart on all PCs
```

### During Operation
```
1. Monitor Server Log occasionally
2. Check "Sent to Server" counter increasing
3. Verify no error messages
4. Database size increasing normally
```

### Evening
```
1. Export new QSOs
2. Check "Mark as exported" checkbox ⭐ IMPORTANT
3. Upload ADIF file to ClubLog
4. Verify in ClubLog
5. Note "Last Exported QSO" timestamp
```

---

## ⚠️ Common Issues & Quick Fixes

| Problem | Solution |
|---------|----------|
| **Client shows "Disconnected"** | Check IP/Port, ping server, check firewall |
| **No QSOs arriving** | Verify UDP Listener port, check format matches logger, mark Active ✓ |
| **Duplicate QSOs in ClubLog** | ALWAYS mark as exported after upload! |
| **Server won't start** | Port in use? Try 5001. Check firewall. |
| **High CPU usage** | Disable "Log details" checkbox |
| **Database error** | Close app, wait 10 sec, reopen |

---

## 📊 Monitoring Checklist

Daily check:
- [ ] Server status: 🟢 Running
- [ ] Clients connected: # ___
- [ ] QSOs in database: # ___
- [ ] No errors in Server Log
- [ ] "Temporary Saved" on clients: 0
- [ ] "Sent to Server" counter: ✓ Increasing

---

## 🔍 Status Indicators

### Client Server Status
```
🟢 Connected    → QSOs sending immediately
🔴 Disconnected → QSOs buffering locally
🟡 Reconnecting → Auto-retry in progress
```

### Checkboxes to Remember
```
✓ "Active"              → Enable UDP listener
✓ "Log details"         → Verbose logging (troubleshooting only)
✓ "Mark as exported"    → Prevent duplicate ClubLog uploads ⭐
✓ "Start automatically" → Auto-launch with Windows
```

---

## 📋 Export Best Practices

```
Daily Export Workflow:
1. Open "QSO Export"
2. Select "New QSOs" ← (Important!)
3. Review "Filtered" count
4. Click "Export"
5. Save with date in filename
6. ✓ CHECK "Mark as exported" ← (CRITICAL!)
7. Upload to ClubLog
8. Verify in ClubLog
9. Next day: Repeat (only NEW QSOs will export)
```

---

## 🎮 Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| Enter | Execute search in QSO Search |
| Ctrl+A | Select all text |
| Ctrl+C | Copy selected text |
| Ctrl+V | Paste text |
| Backspace | Delete character |
| Tab | Move to next field |

---

## 📱 Device Specifications

### Minimum Server
- Windows 10/11
- .NET 10 runtime
- 2 GB RAM
- 200 MB disk
- Network connection

### Recommended Server
- Windows 10/11
- .NET 10 runtime
- 4-8 GB RAM
- SSD with 1 GB free
- Gigabit Ethernet (wired)

### Client (Any PC with Logger)
- Windows 10/11
- .NET 10 runtime
- 2 GB RAM
- 200 MB disk
- WiFi or Ethernet OK

---

## 💡 Pro Tips

1. **Backup Often**: `%AppData%\QSOCollector\qso.db` contains all QSOs
2. **Mark Exports**: Always "Mark as exported" to prevent duplicates
3. **Test First**: Try with one client before adding more
4. **Static IP**: Set server to static IP (won't disconnect if rebooted)
5. **Autostart**: Enable on all PCs to survive reboots
6. **Verbose Logs**: Only enable "Log details" when troubleshooting
7. **Export Daily**: Don't batch exports - upload to ClubLog daily
8. **Monitor Clients**: Watch for "Temporary Saved" increasing (offline)
9. **Network Stable**: Use Ethernet if possible (more reliable than WiFi)
10. **Firewall Check**: Ensure QSOCollector allowed in Windows Firewall

---

## 🔧 Troubleshooting Commands

```powershell
# Check .NET 10 installed
dotnet --version

# Find your IP address
ipconfig

# Check if port is open
netstat -ano | findstr :5000

# Test connection to server
Test-NetConnection -ComputerName 192.168.1.50 -Port 5000

# Ping server
ping 192.168.1.50

# Check UDP listeners
netstat -ano | findstr UDP
```

---

## 📞 Get Help

### Self-Service
- Open `UserManual/index.html` in browser
- Search for your issue in troubleshooting section
- Check FAQ for common questions
- Review glossary for technical terms

### Online
- GitHub Issues: https://github.com/OZhuravlov/QSOCollector/issues
- Check existing issues first
- Include: error message, logs, steps to reproduce

---

## ✅ Pre-DXpedition Checklist

**Week Before**
- [ ] Install QSOCollector on all PCs
- [ ] Install .NET 10 runtime if needed
- [ ] Test server with one client
- [ ] Verify UDP listener setup
- [ ] Test QSO reception
- [ ] Verify export to ADIF works
- [ ] Set up ClubLog upload process
- [ ] Back up database

**Day Before**
- [ ] Verify all client PCs can find server
- [ ] Test connection from all logger PCs
- [ ] Enable autostart on all PCs
- [ ] Final backup of test database
- [ ] Clear test QSOs from database

**Day Of**
- [ ] Start server BEFORE loggers
- [ ] Verify all clients connecting
- [ ] Monitor first hour closely
- [ ] Check for errors in logs
- [ ] Plan daily export/upload time

---

## 🎯 Operating During DXpedition

### Each Shift
1. Verify server running ✓
2. Check all clients connected ✓
3. Monitor activity occasionally ✓
4. Watch for errors in logs ✓
5. Note QSO count periodically ✓

### Daily (Evening)
1. Export new QSOs
2. Mark as exported (✓ CRITICAL!)
3. Upload to ClubLog
4. Verify arrived correctly
5. Back up database

### Weekly (Optional)
1. Review database size
2. Check logs for patterns
3. Verify performance acceptable
4. Update documentation

### End of DXpedition
1. Final export of all QSOs
2. Archive database to external drive
3. Generate QSO statistics
4. Clean up test data
5. Document setup for next time

---

## 📄 File Locations Quick Ref

| File | Location |
|------|----------|
| Database | `%AppData%\QSOCollector\qso.db` |
| Logs | `%AppData%\QSOCollector\logs\` |
| Config | `%AppData%\QSOCollector\` |
| Exports | `Documents\QSOCollector\export\` |
| Imports | `Documents\QSOCollector\import\` |
| AppData | `C:\Users\[YourName]\AppData\Roaming\` |
| Documents | `C:\Users\[YourName]\Documents\` |

---

## 🌟 Remember

- ⭐ **START SERVER FIRST** before clients
- ⭐ **MARK AS EXPORTED** after every upload
- ⭐ **BACK UP DATABASE** regularly
- ⭐ **ENABLE AUTOSTART** on all PCs
- ⭐ **USE STATIC IP** for server PC
- ⭐ **TEST EVERYTHING** before DXpedition
- ⭐ **MONITOR LOGS** during operation
- ⭐ **VERIFY QSOs** arrive on server

---

**Print This Page!** 📄  
Keep as quick reference during operations.

---

*QSOCollector v1.0 | .NET 10 | Windows 10/11 | February 2026*
