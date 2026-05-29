# NN Pension Tracker - Installation Info

## Install

To install the application run the command:

```cmd
msiexec /i NnPensionTracker.Installer.msi
```

## Uninstall

To uninstall:

```cmd
msiexec /x NnPensionTracker.Installer.msi
```

## Runtime

The application has a CLI interface. Run it in the console:

```cmd
nn-pension-tracker --help
```

### Configuration

After installation, the application is reading configuration from the following locations:
- Common settings file
  - `C:\ProgramData\NN Pension Tracker\appsettings.json`
- User specific settings file
  - `C:\Users\[user]\AppData\Roaming\NN Pension Tracker\appsettings.json`

### User Data (database)

The application is storing the user data in the common location (accessible by all users:
	- `C:\ProgramData\NN Pension Tracker\Data`