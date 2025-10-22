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
