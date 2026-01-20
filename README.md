# QSOCollector

## Overview
QSOCollector is a software application designed for multi-station amateur radio DXpedition to collect and manage their QSO (contact) record logs efficiently.

## Problem Statement
For multi-station DXpeditions, managing and consolidating QSO records from various loggers and multiple computers can be challenging.
The most of the existing logging software used in DXpeditions are designed for contest operations rather then for DXpedition needs.

What is important for big DXpedition?
1. Reliable collection of QSO records from all PCs (clients) to the central PC (server) in unstable network.
   Usually loggers used in DXpeditions has the networking feature implemented for collecting all QSO records to a single central server using UDP broadcasting.
   However, UDP protocol is not reliable in WiFi network which might cause loss or duplication of QSO records on server PC.
   In most cases WiFi is the only option for DXpedition networking.
   The solution is to use TCP protocol when send QSO logs from client PC to server PC.
2. Daily basis ADIF file generation for uploading to the Clublog with minimum duplication of QSO records.
   The loggers used in DXpeditions can export ADIF records but without filtering options. So each day the only full log could exported from logger.
   The solution is to implement filtering mechanism on server side to mark already exported QSO.

## Features
- Collecting all QSOs broadcasted via UDP by loggers locally avoiding QSO loss
- Sending of collected QSOs from client PCs to server PC via reliable TCP protocol and store them in central database
- Store QSOs on client PC when server unavailable and sending them to server PC once it restored
- Importing any ADIF files on server side if needed
- Exporting ADIF files with different filtering options to avoid duplication of exported QSOs

## Implementation
- Application can run as Server and/or as Client (only 1 PC should be running as Server)
- Client runs on each client PC, listens for QSOs broadcasted via UDP by all loggers on the same PC (no network involved on this step)
  Very important to have client running when logger is running to avoid loss of QSO records. So client could be started automatically with Windows startup.
  Supported formats: 
        - N1MM format which used to send QSO info to ClubLog (N1MM, DxLog, SDC, etc.)
        - ADIF
- Client sends received QSO records to Server via reliable TCP connection.
- Server runs on server PC and collects QSO records from all Clients, stores them in central database.
  Server could be started automatically with Windows startup.
- In case of network failure between Client and Server, Client stores QSO records locally and sends them to Server when connection is restored.
  So no QSO records will be lost even if both Client PC and Server PC are restarted multiple times during DXpedition.
- Client provides such options:
    - configure unlimited number of UDP ports to listen for QSOs broadcasted by loggers on the same PC.
    - configure application acknowledge UDP port and be aware the logger is running even if it's not sending QSOs.
    - configure another UDP port to forward received QSO messages
      because on Windows only 1 application from the same PC could listen on specific UDP port, the other listener (for example clublog livestream application) could be configured to listen on forward port.
- Server provides such options for managing collected QSO records:
    - Import ADIF records if needed (for example for some reason Client hasn't been started and ADIF logs extracted directly from logger could be imported to QSO Collector)
    - Export ADIF with multiple filtering options (the most useful is export only new QSOs not exported before)
    - Mark exported QSOs to avoid duplication in future exports

## How to Use
### Server
Application could run as Server to collect QSOs from multiple Clients. Only 1 PC should run as Server. 
To use QSOCollector in Server mode you need to Select "Server" tab and specify a local TCP port to listen for incoming connections from Clients.

#### Collecting QSOs
Use "Start Server" and "Stop Server" buttons to start/stop Server.
In the "Server Log" section you could see the log of Server activity. "Show Log details" checkbox could be used to enable/disable detailed logging.
Above the log there is an information about QSO amounts in database.

#### Exporting QSOs
Use this feature to export QSOs from database to ADIF file with different filtering options.
Press "QSO Export" button to open "Export to ADIF" dialog.

Main filter:
 - "New QSOs" to export only new QSOs which haven't not exported before
 - "By Date"  to export QSOs for specific date range
 - "All QSOs" to export all QSOs from database

Secondary filters allows to narrow down QSOs after applying main filter.
After applying any filter the number of QSOs to export will be shown at the bottom in "QSO Amount" -> "Filtered".
The total number of QSOs in database is shown in "QSO Amount" -> "Total".

After configuring filters press "Export" button to specify the output ADIF file name and export QSOs.
The defaut prompted folder for export is MyDocuments\QSOCollector\export. The default prompted file name contains current date and time.
After successful export a message box will show ADIF file path and name and prompt to set QSOs as exported in database (Recommended) to avoid exporting them next time.
The explorer window will be opened with exported ADIF file selected.
After closing export dialog the "Last Exported QSO" statistic will be updated.

#### Importing QSOs
Use this feature to import QSOs from ADIF file to database.
Press "QSO Import" button to open "Import from ADIF" dialog.
Choose ADIF file to import. The defaut prompted folder for export is MyDocuments\QSOCollector\import
After selecting ADIF file the part of content will be shown in File preview textbox.
Press "Continue import" button to start importing QSOs from selected ADIF file to database.
The progress bar will show the progress of importing and the number of new saved and duplicted QSOs will be shown as well.
Duplicated QSOs are those which already exist in database and won't be saved again.
After closing import dialog the amounts in Server QSO statistics will be updated.

### Client
Application could run as Client on each client PC to collect QSOs broadcasted by different loggers UDP protocol on the same PC and send them to Server via reliable TCP connection.
To use QSOCollector in Client mode you need to Select "Client" tab and configure Server IP address (or name) and Server port.
To continue you need to configure UDP listeners.

#### UDP Listeners
UDP listeners are used to collect QSOs broadcasted by loggers on the same PC. To configure UDP listener press "UDP Listeners" on "Client" tab.
In the "UDP Listeners" dialog you could add/remove/configure UDP listeners and save this config. You can also export config to the file in order to import on the other Client.
Each UDP listener could be configured with the following options (* marked as mandatory):
- *Listener Name   - any name to identify the listener, i.e. Logger name
- *QSO Port        - the port to listen for QSOs. This port should match the port configured in logger for broadcasting QSOs.
- Forward Port     - the port to forward received QSO messages. Because on Windows only 1 application from the same PC could listen on specific UDP port,
                     this port could be used by other application on the same PC to listen for QSOs (for example Clublog Livestream application).
                     If no other application needs to listen for QSOs then leave this field empty.
- Heart Beat Port  - the port to listen for heart beat message from logger (if supported by logger).
                     Could be used to make sure the application is alive even if it doesn't send QSO data. 
                     If logger doesn't support such messages then leave this field empty.
- *Format          - the format of QSOs broadcasted by logger. Supported formats are N1MM (the one used for ClubLog livestream) and ADIF.
- Active           - checkbox to enable/disable this listener without removing it from the list.

#### Collecting QSOs
When UDP listeners are configured then Client could be started/stopped with "Start Client" and "Stop Client" buttons.
Client will try to connect to Server. If connection is successful then Client starts UDP listeners to listen for QSOs broadcasted by loggers. Once QSO is received it will be sent to Server.
In the "Client Log" section you could see the log of Client activity. The "Log details" checkbox above "Client Log" right top corner could be used to enable/disable detailed logging.
The "Server Status" section shows the current status of connection to Server.
If Server is Unavailable then Client will continue collecting QSOs and store them into local Database. Client will send QSOs once Server is available again.
"QSO processing" section shows the statistics about amount of received, sent-to-server, temporary-saved and rejected QSOs and last known timestamp for each category.
When Client is started it will listen for QSOs broadcasted by loggers on configured UDP ports.
When QSO is received by Client it will be sent to Server via reliable TCP connection.

### Autostart Application
It's very important to have Application always running to avoid loss of QSO records. This is especially important for Client mode which should be running on each client PC where logger is running.
To make it easier Application could be started automatically with Windows startup by enabling "Start automatically" option at the right top corner.
Application will be started with the current configuration and minimized to system tray and could be restored by double clicking on system tray icon.

