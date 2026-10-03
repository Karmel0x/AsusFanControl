# Asus Fan Control

A lightweight fan control utility for ASUS laptops that suffer from a specific hardware or firmware-level signal reading issue, where the fan operates physically but reports **0 RPM** everywhere — Armoury Crate, BIOS, and all third-party fan control apps.

---

## The Problem

On certain ASUS laptops, a motherboard-level or signal-reading issue causes the system to report the fan speed as **0 RPM**, even when the fan is physically spinning. Because all standard fan control software relies on that RPM signal to make decisions, they cannot control the fan correctly — they assume the fan is not running and fail to respond to rising temperatures. This leads to **thermal throttling**, reduced performance, and potential long-term heat damage.

---

## Why This App Works

The **MyASUS** app ships a fan hardware diagnostic test (`Fan Diagnosis`) that includes a low-level library (`AsusWinIO64.dll`, licensed to ASUSTek COMPUTER INC.) capable of **directly commanding the fan** via the `HealthyTable` interface — bypassing the broken RPM signal entirely. This project exposes that capability through a simple GUI, making it possible to control your fans even when every other tool fails.

> **Compatibility note:** ASUS System Control Interface and the `ASUS System Analysis` service must be running. Both are automatically installed with the MyASUS app.
>
> Confirmed working on: VivoBook, ZenBook, TUF Gaming, ROG Strix, ROG Zephyrus, ROG Flow.

---

## Features

### GUI (`AsusFanControlGUI.exe`)
- **Manual fan speed control** — drag a slider to set fan speed (0–100%).
- **Turn on/off fan control** — checkbox to toggle between managed and system-default mode.
- **Live stats display** — shows current fan RPM and CPU temperature with manual refresh buttons.
- **Auto refresh stats** — timer-based automatic refresh so you can monitor in real time.
- **Configurable refresh interval** — set auto-refresh between 1 and 30 seconds (`Advanced → Refresh interval`). Default is **3 seconds**.
- **Auto fan curve** — automatically adjusts fan speed based on CPU temperature using a configurable curve table (see below).
- **Minimize to tray on close** — keeps the app running silently in the background.
- **Turn off fan control on exit** — safely hands fan control back to the system when you close the app.
- **Forbid unsafe settings** — prevents setting fan speed below 40% or above 99% to protect your hardware.

### Command Line (`AsusFanControl.exe`)
```
AsusFanControl.exe <args>
    --get-fan-speeds                              Print all fan RPMs
    --set-fan-speeds=0-100                        Set all fans to a percentage (0 = disable test mode)
    --get-fan-count                               Print number of detected fans
    --get-fan-speed=fanId                         Print RPM for a specific fan (comma-separated)
    --set-fan-speed=fanId:0-100                   Set a specific fan (comma-separated, percent)
    --get-cpu-temp                                Print current CPU temperature
```

---

## Auto Fan Curve — How It Works

The fan curve feature is the key improvement over simple manual control. Here's what makes it work correctly:

### The Approach
When **Fan Curve** mode is enabled, the app:
1. **Reads the CPU temperature** once per refresh tick and displays it in the UI.
2. **Uses that same UI-level reading** to look up the correct fan speed from your configured curve — no extra or redundant system call is made.
3. **Applies the new fan speed** via the same `AsusWinIO64` library path that manual control uses, completely bypassing the broken RPM signal channel.

This is important: an earlier draft of the fan curve feature re-called the temperature read *separately* inside the fan curve logic, which added unnecessary overhead and deviated from the principle of using only what the UI already has. The current implementation reads temperature once, shows it to you, and acts on it — clean and consistent.

### Configuring the Curve
Click **Fan Curve Settings** to open the curve editor. Add temperature/fan speed pairs:

| Temperature (°C) | Fan Speed (%) |
|-----------------|---------------|
| 30              | 30            |
| 50              | 50            |
| 70              | 80            |
| 90              | 100           |

Between points, the speed is **linearly interpolated** — so at 60°C it would be 65% with the example above. The curve is saved automatically.

### Enabling Auto Curve
1. Check **Turn on fan control**
2. Check **Fan Curve**
3. Auto-refresh is enabled automatically. Set your preferred interval under `Advanced → Refresh interval`.

---

## Installation & Usage

### Requirements
- Windows x64
- [MyASUS](https://apps.microsoft.com/store/detail/myasus/9N7R5S6B0ZZH) installed (provides the `ASUS System Analysis` service and the necessary DLL)
- The `ASUS System Analysis` service must be running

### Download
Go to [Releases](../../releases) and download the latest version.

### Run as Administrator
The app requires elevated privileges to communicate with the ASUS hardware interface. Right-click the `.exe` and select **Run as administrator**, or set it to always run as admin in the Properties.

---

## Technical Notes

- **`AsusWinIO64.dll`** is © ASUSTek COMPUTER INC. and is bundled for convenience. If MyASUS is installed, the original can be found at:  
  `C:\Windows\System32\DriverStore\FileRepository\asussci2.inf_amd64_-\ASUSSystemAnalysis\`
- Settings are saved per-user via the standard .NET application settings mechanism.
- Fan curve data is serialized as a simple `temp,speed;temp,speed` string and stored in user settings.

---

## License

See [LICENSE](LICENSE).
