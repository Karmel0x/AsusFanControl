# Asus Fan Control

### Download
Go to [releases](../../releases)

### Run
Command line turn on: `AsusFanControl.exe <fan speed percent>`  
Command line turn off: `AsusFanControl.exe 0`  
GUI: `AsusFanControlGUI.exe`  

![AsusFanControlGUI](https://github.com/Karmel0x/AsusFanControl/assets/25367564/cccd136c-f1cb-4218-b150-d874d0d5e5af)

### Why need it?
My laptop does not support the [Fan Profile](https://github.com/Karmel0x/AsusFanControl/assets/25367564/924d990a-bf20-4b8d-bf9d-56c460174d99) option, but it often overheats. Looked for apps to control the fans, but none is working.

### Compatibility
This program should work on any laptop with x64 windows where [Fan Diagnosis](https://github.com/Karmel0x/AsusFanControl/assets/25367564/7129833b-97af-4da8-9148-b71e49552ea4) in [MyASUS](https://apps.microsoft.com/store/detail/myasus/9N7R5S6B0ZZH) application is working as it is using same library.

Included `AsusWinIO64.dll` is licenced to `(c) ASUSTek COMPUTER INC.` which can be found in `C:\Windows\System32\DriverStore\FileRepository\asussci2.inf_amd64_-\ASUSSystemAnalysis\` if you have MyASUS installed.

Confirmed compatibility: 
- ASUS VivoBook 15 X512FL
