# Asus Fan Control

### Download
Go to [releases](../../releases)

### Run

<details>
    <summary>Command line: `AsusFanControl.exe`</summary>
    
    AsusFanControl.exe <args>
        --get-fan-speeds
        --set-fan-speeds=0-100 (percent value, 0 for turning off test mode)
        --get-fan-count
        --get-fan-speed=fanId (comma separated)
        --set-fan-speed=fanId:0-100 (comma separated, percent value, 0 for turning off test mode)
        --get-cpu-temp
</details>

GUI: `AsusFanControlGUI.exe`  

After building (e.g. Release | x64), the output folder `bin\x64\Release\` contains the exes and `run.bat`. Double‑click `run.bat` to launch the GUI with admin rights (it uses PsExec if present). To have PsExec copied automatically, place `PsExec.exe` in the `AsusFanControlGUI` project folder once; then every build will copy it to the output folder.

![AsusFanControlGUI](https://github.com/Karmel0x/AsusFanControl/assets/25367564/fe197ad0-7079-4d51-ae78-177cb6369e96)

### Why need it?
My laptop does not support the [Fan Profile](https://github.com/Karmel0x/AsusFanControl/assets/25367564/924d990a-bf20-4b8d-bf9d-56c460174d99) option, but it often overheats. Looked for apps to control fans, but none is working.

### Compatibility
This program should work on any laptop with x64 windows where [Fan Diagnosis](https://github.com/Karmel0x/AsusFanControl/assets/25367564/7129833b-97af-4da8-9148-b71e49552ea4) in [MyASUS](https://apps.microsoft.com/store/detail/myasus/9N7R5S6B0ZZH) application is working as it is using same library.

[ASUS System Control Interface](https://www.asus.com/support/faq/1047338/) is necessary for this software to work - `ASUS System Analysis` service [must be running](../../issues/16). It's automatically installed with `MyASUS` app.

**If fan control has no effect:** Run the app **as Administrator** (right‑click → Run as administrator). The GUI is built to request admin elevation. Also ensure the ASUS System Analysis service is running (Services → "ASUS System Analysis" or "AsusSystemAnalysis") and that your model supports [Fan Diagnosis](https://github.com/Karmel0x/AsusFanControl/assets/25367564/7129833b-97af-4da8-9148-b71e49552ea4) in MyASUS. You can test from command line (as admin): `AsusFanControl.exe --get-fan-count` then `AsusFanControl.exe --set-fan-speeds=80`.

Included `AsusWinIO64.dll` is licenced to `(c) ASUSTek COMPUTER INC.` which can be found in `C:\Windows\System32\DriverStore\FileRepository\asussci2.inf_amd64_-\ASUSSystemAnalysis\` if you have MyASUS installed.

[Works on](../../issues/13): 
- ASUS: VivoBook, ZenBook, TUF Gaming, ROG Strix, ROG Zephyrus, ROG Flow

---

### Modifications

This fork adds temperature-based fan control and UI improvements on top of the original Asus Fan Control:

- **Manual vs Auto mode**  
  - **Manual:** Fan speed is set by the trackbar (unchanged).  
  - **Auto:** Fan speed follows a temperature curve. The window shows CPU temperature, GPU temperature, and the fan percent that curve produces.

- **Fan curve setpoints (Auto mode)**  
  Choose **2 or 3 setpoints**. Each setpoint is a temperature (°C) and a fan percent. Below the first setpoint the fans stay off; at each setpoint that percent turns on and holds until the next one. The default 3-point curve matches the previous fixed ranges:  
  - &lt; 35°C → 0% (fans off)  
  - 35°C → 45%  
  - 55°C → 80%  
  - 75°C → 100%  
  **Edit** switches between a CPU curve and a GPU curve. **Use GPU curve** applies both, and the fans run at whichever curve requests the higher speed. GPU temperature comes from the same `AsusWinIO64.dll` the app already uses (`Thermal_Read_GpuTS1L_Temperature`, or `Thermal_Read_GpuTS1R_Temperature` when the left sensor has no reading). Laptops without that sensor show GPU as N/A and keep using the CPU curve. Every fan still receives the same percent; this library path does not expose separate CPU and GPU fan channels.  
  "Forbid unsafe settings" (Advanced menu) still applies a 40–99% clamp when enabled.

- **5-second debounce**  
  In Auto mode, when the curve result moves to a new fan percent, that percent is applied only after it has stayed there for 5 seconds, to avoid reacting to short spikes. Changing setpoints, mode, or the refresh button still applies immediately.

- **Start with Windows**  
  Advanced → **Start with Windows** registers a logon task named `AsusFanControlGUI` that launches the app with highest privileges. If Task Scheduler cannot create the task, the current user's Run key is used instead (that path can show a UAC prompt at logon). Turning the option off removes both. A second copy does not start if the app is already running.

- **Fans off in sleep**  
  Advanced → **Turn fans off on sleep** (on by default). Fan control holds ASUS test mode, which is what leaves the fans spinning through sleep. On suspend or hibernate the app clears test mode so the laptop can stop the fans, then applies Manual or Auto again on wake. Uncheck the option to leave test mode set while the machine sleeps.

- **Performance**  
  Hardware reads (CPU temp, fan RPM) run on a background thread and the refresh timer runs every 5 seconds to reduce system load and keep the UI responsive.

- **UI**  
  - Auto mode edits 2 or 3 setpoints in the same window. Manual mode uses a shorter window.  
  - CPU and GPU temperature are shown under the fan RPM.  
  - Fixed single-window layout; form does not maximize.
