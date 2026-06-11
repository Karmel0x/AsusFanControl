# Asus Fan Control

### Download
Go to [releases](../../releases)

### Run

<details>
    <summary>Command line: <code>AsusFanControl.exe</code></summary>

    AsusFanControl.exe <args>
        --get-fan-speeds
        --set-fan-speeds=0-100 (percent value, 0 for turning off test mode)
        --get-fan-count
        --get-fan-speed=fanId (comma separated)
        --set-fan-speed=fanId:0-100 (comma separated, percent value, 0 for turning off test mode)
        --get-cpu-temp
</details>

GUI: `AsusFanControlGUI.exe`

![AsusFanControlGUI](https://raw.githubusercontent.com/apurvamarya/AsusFanControl/8337888b851a666f9cffb19087daecaad2f9a5f5/assets/Ui.png)

### Why need it?
My laptop does not support the [Fan Profile](https://github.com/Karmel0x/AsusFanControl/assets/25367564/924d990a-bf20-4b8d-bf9d-56c460174d99) option, but it often overheats. Looked for apps to control fans, but none is working.

### Compatibility
This program should work on any laptop with x64 Windows where [Fan Diagnosis](https://github.com/Karmel0x/AsusFanControl/assets/25367564/7129833b-97af-4da8-9148-b71e49552ea4) in [MyASUS](https://apps.microsoft.com/store/detail/myasus/9N7R5S6B0ZZH) application is working, as it uses the same library.

[ASUS System Control Interface](https://www.asus.com/support/faq/1047338/) is necessary for this software to work — `ASUS System Analysis` service [must be running](../../issues/16). It is automatically installed with the `MyASUS` app.

Included `AsusWinIO64.dll` is licenced to `(c) ASUSTek COMPUTER INC.` which can be found in `C:\Windows\System32\DriverStore\FileRepository\asussci2.inf_amd64_-\ASUSSystemAnalysis\` if you have MyASUS installed.

[Works on](../../issues/13):
- ASUS: VivoBook, ZenBook, TUF Gaming, ROG Strix, ROG Zephyrus, ROG Flow

---

### Fork & Enhancements by [Apurvam Arya](https://github.com/apurvamarya)

This fork migrates the project to **.NET 10** (SDK-style project format) and introduces a redesigned GUI with a **neon/cyberpunk dark theme**:

- Migrated from .NET Framework 4.7.2 to `net10.0-windows` with SDK-style csproj
- Fully redesigned UI with a black & cyan neon color scheme
- Custom `NeonMenuRenderer` for styled `MenuStrip` and context menus — hover highlights, neon checkmarks, and bordered dropdowns
- All controls (trackbar, buttons, labels, checkboxes) re-skinned with `Consolas` font and neon palette
- Tray icon context menu inherits the neon theme
- Codebase modernized with C# nullable reference types enabled