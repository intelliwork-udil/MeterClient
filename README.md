# MeterClient: High-Concurrency DLMS/COSEM Smart Meter Simulator

[![Platform](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)
[![Protocol](https://img.shields.io/badge/Protocol-DLMS%2FCOSEM-green.svg)](https://www.dlms.com/)

## 🏗️ Overview

The **MeterClient** is a high-performance, asynchronous simulator designed to emulate a massive fleet of smart meters (supporting 100,000+ concurrent connections). Built on **.NET 10**, it serves as a critical infrastructure component for stress-testing and validating **Meter Data Collection (MDC)** systems. 

Unlike simple packet-replay tools, MeterClient implements a state-aware communication layer that handles the DLMS/COSEM handshake, authentication cycles, and dynamic data generation for billing, instantaneous readings, and event logs.

---

## 🚀 Key Technical Features

### 1. High-Concurrency Engine
*   **Asynchronous I/O**: Leverages `Task-based Asynchronous Pattern (TAP)` and `System.Net.Sockets` to manage thousands of active TCP streams with minimal memory overhead.
*   **Resource Throttling**: Intelligent connection management to simulate realistic meter behavior and network latency.

### 2. Protocol & Security
*   **DLMS/COSEM Support**: Implements the AARQ/AARE handshake, heartbeats, and OBIS-code-based command processing.
*   **Security Mechanisms**: Supports High-Level Security (HLS) and password-based authentication (Pass 3 association).
*   **Command Classifier**: A robust internal engine to decode and route incoming MDC requests (Billing, Instantaneous, LPRO, Events, etc.).

### 3. Data Simulation & Persistence
*   **Dynamic Sampling**: Simulates realistic energy consumption profiles, including Load Profile (LPRO), Billing (Monthly/Daily), and Event Logs.
*   **Excel-Driven Configuration**: Easily import meter credentials (MSN, Passwords) and network parameters via `.xlsx` or `.csv` files.
*   **Reactive Logging**: Utilizes `System.Reactive` for high-throughput, thread-safe logging of communication traces.

---

## 🛠️ Technology Stack

*   **Runtime**: .NET 10.0 (Core)
*   **Communication**: TCP/IP (Sockets)
*   **Data Parsing**: ExcelDataReader, CsvHelper, Newtonsoft.Json
*   **Reactive Extensions**: System.Reactive (for event-driven logging)
*   **Architecture**: Domain-Driven Design (Business Logic isolation for Meter Objects)

---

## 📂 Project Structure

```text
MeterClient/
├── BL/                       # Business Logic: Meter Objects (DMDT, MDSM, IPPO, etc.)
├── Helper/                   # Utilities: Progress bars, Logging, CSV handlers
├── MeterClientsGenerator.cs  # High-volume client orchestration and Excel parsing
├── MeterConfigurationUI.cs   # Core simulation engine and command loop
├── Program.cs                # Entry point and configuration bootstrapping
└── appsettings.json          # Global connection settings (IP, Port, Intervals)
```

---

## 🚦 Getting Started

### Prerequisites
*   [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
*   An active MDC Server to receive connections.

### Configuration
Update `appsettings.json` with your MDC server details:
```json
{
  "MDC_Connections": {
    "ipAddress": "127.0.0.1",
    "port": 5000,
    "communicationInterval": 5,
    "filePath": "meter100k.xlsx",
    "NeedsConnecting": 1
  }
}
```

### Building & Running
1.  **Clone the repository**:
    ```bash
    git clone <repo-url>
    ```
2.  **Restore and Build**:
    ```bash
    dotnet restore
    dotnet build -c Release
    ```
3.  **Run the Simulator**:
    ```bash
    dotnet run --project MeterClient/MeterClient.csproj
    ```

---

## 📊 Operational Guide

1.  **Load Meters**: On startup, the system parses the configured Excel file.
2.  **Select Range**: You can specify a subset of meters to run (e.g., Meter 001 to 500).
3.  **Monitor Logs**:
    *   **Console**: Real-time communication traces (Green: Receive, Blue: Send).
    *   **FileSystem**: Check `MeterSamplingData/` and `MeterConfigs/` for generated state and sampling history.

---

## 🛡️ Security & Performance Notes

*   **Stateless Logging**: The system uses a `CsvThreadLoggerUtility` to ensure high-concurrency logging doesn't become a bottleneck.
*   **Handshake Latency**: Simulated meters include randomized delays in authentication responses to mimic real-world hardware constraints.
*   **Task Management**: Each meter runs its own lifecycle, ensuring that a failure in one connection doesn't disrupt the entire fleet.

---

## 👨‍💻 Principal's Design Philosophy

> "The goal of this simulator is not just to send packets, but to behave like a physical asset. By isolating the Business Logic of the meter (BL folder) from the communication transport (UI/Generator), we've created a platform where new DLMS objects can be added with zero impact on the concurrency engine."

---

## 📝 License
Proprietary - Developed for MDC Validation and Stress Testing.
