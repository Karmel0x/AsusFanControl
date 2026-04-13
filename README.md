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

The GUI can now:

- register itself to launch automatically when you sign in to Windows from `Advanced > Launch on Windows sign-in` by creating a scheduled task that runs `run.bat`
- restore the last enabled control mode when it starts, including starting minimized to the tray when Windows launches it
- apply a configurable five-point CPU temperature to fan speed curve with linear interpolation between points
- edit that curve by dragging points on a line graph and restore the saved curve correctly on the next launch

Important: on systems that require ASUS fan control access through the original launcher flow, start the GUI with `run.bat`, not by opening `AsusFanControlGUI.exe` directly. The launcher elevates and uses `PsExec.exe` to start the GUI as `SYSTEM`.
Windows sign-in launch now runs in your account's interactive session and can start minimized to the tray.

![AsusFanControlGUI](https://github.com/Karmel0x/AsusFanControl/assets/25367564/fe197ad0-7079-4d51-ae78-177cb6369e96)

### Why need it?

My laptop does not support the [Fan Profile](https://github.com/Karmel0x/AsusFanControl/assets/25367564/924d990a-bf20-4b8d-bf9d-56c460174d99) option, but it often overheats. Looked for apps to control fans, but none is working.

### Compatibility

This program should work on any laptop with x64 windows where [Fan Diagnosis](https://github.com/Karmel0x/AsusFanControl/assets/25367564/7129833b-97af-4da8-9148-b71e49552ea4) in [MyASUS](https://apps.microsoft.com/store/detail/myasus/9N7R5S6B0ZZH) application is working as it is using same library.

[ASUS System Control Interface](https://www.asus.com/support/faq/1047338/) is necessary for this software to work - `ASUS System Analysis` service [must be running](../../issues/16). It's automatically installed with `MyASUS` app.

Included `AsusWinIO64.dll` is licenced to `(c) ASUSTek COMPUTER INC.` which can be found in `C:\Windows\System32\DriverStore\FileRepository\asussci2.inf_amd64_-\ASUSSystemAnalysis\` if you have MyASUS installed.

[Works on](../../issues/13):

- ASUS: VivoBook, ZenBook, TUF Gaming, ROG Strix, ROG Zephyrus, ROG Flow
