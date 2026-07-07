# ClassicLogon

Classic XP-style custom logon shell for Windows 10/11. Replaces the modern Windows lock screen with a classic, lightweight login interface.

## Features

- **XP-style UI** — Custom-drawn classic login screen with user avatars, hover animations, and rounded avatar backgrounds
- **User avatars** — Loads user account pictures directly from the registry
- **Password hints** — Reads password hints from SAM registry (click the `?` button)
- **Wrong password feedback** — Inline error bubble with hint guidance
- **Smart auto-login** — Detects auto-login configuration and shows a simplified welcome screen at boot
- **Configurable power button** — Bottom-left power button launches a configurable shortcut (see `power.conf`)
- **Single instance** — Prevents duplicate login windows via system Mutex
- **Transparent login pass-through** — Becomes click-through during FlaUI interaction with the real LogonUI
- **No external dependencies** — No themes, no locale files, no INI configs needed

## How It Works

1. `LauncherService.exe` runs as a Windows Service, watching session events (lock, logoff, logon)
2. On session lock, it injects `LogonShell.exe` into the Winlogon desktop via `CreateProcessAsUser`
3. The custom login UI overlays the real LogonUI and collects credentials
4. Credentials are validated with `LogonUser()` API
5. [FlaUI](https://github.com/FlaUI/FlaUI) automation interacts with the real LogonUI to complete login
6. After login, the welcome screen is shown briefly, then the application exits

## Build Requirements

- Visual Studio 2022 (or Build Tools) with .NET Framework 4.8 targeting pack
- NuGet packages (auto-restored): FlaUI.Core, FlaUI.UIA3, Interop.UIAutomationClient

## Build & Install

```batch
# 1. Build
build.bat

# 2. Install (run as Administrator)
build\install.bat

# 3. Uninstall (run as Administrator)
C:\ClassicLogonShell\uninstall.bat
```

The installer will:
- Copy all files to the target directory
- Create and start the `ClassicLogonShell` Windows Service
- Place `uninstall.bat` in the install directory for later removal

## Configuration

### Power Button

The power button (left side of bottom bar) can launch any program or shortcut.

1. Create `power.conf` in the install directory
2. Write the target path on the first line:

```
C:\Windows\System32\shutdown.exe /s /t 0
```

If `power.conf` is not found, it falls back to `shutdown.lnk` in the same directory.

### User Exclusions

The following accounts are excluded from the user list:
- Administrator, Guest, DefaultAccount, WDAGUtilityAccount
- Any account ending with `$`

## Limitations

- **No multi-language support yet** — All UI text is hardcoded in Chinese. Multi-language support is planned for a future release.
- **Local accounts only** — Domain accounts are not supported
- **Requires SYSTEM privileges** — Must run as a Windows Service on the Winlogon desktop
- **Windows 10/11 only** — Tested on Windows 10 22H2 and Windows 11

## Credits

Based on [Classic Logon Shell](https://github.com/your-link-here) (Release 1.2).

## License

MIT
