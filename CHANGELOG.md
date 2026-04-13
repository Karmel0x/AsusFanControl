<!-- markdownlint-configure-file {"MD024": {"siblings_only": true}} -->

# Changelog

All notable changes to this project will be documented in this file.

## [1.2.0] - 2026-04-13

### Added

- Added a draggable line-graph editor for the five-point CPU temperature to fan speed curve in the GUI.

### Fixed

- Fixed temperature-curve settings being overwritten during startup before the saved curve was restored.

## [1.1.0] - 2026-04-12

### Added

- Added a GUI option to register Asus Fan Control to launch automatically when the current user signs in to Windows.
- Added minimized startup handling so the GUI can start from Windows sign-in without opening the main window.
- Added persisted fan control state so the GUI restores the last enabled control mode on startup.
- Added a configurable five-point CPU temperature to fan speed curve with interpolated automatic fan control.

### Changed

- Startup registration now uses a scheduled task that launches `run.bat` instead of starting `AsusFanControlGUI.exe` directly.
- The build now includes `run.bat`, and the GUI warns when it was started without the launcher context required by some ASUS laptops.
- Windows sign-in launch now runs in the signed-in user's interactive session instead of an inaccessible SYSTEM desktop, while still allowing minimized tray startup.
