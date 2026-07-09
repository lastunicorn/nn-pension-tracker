# NN Pension Tracker

A console application for tracking the Romanian Mandatory Private Pension (Pillar II), specifically the **NN fund**.

It keeps a local record of:

- Your **contributions** (imported from the monthly/annual PDF statements NN sends you), and
- The **fund's Net Asset Value (NAV)** history (imported either from NN's public web API or from a CSV file, e.g. the historical values file downloadable from NN's website),

so you can see how your account evolved over time, and export the data for further analysis — for example into [PortfolioPerformance](https://www.portfolio-performance.info/).

The application does not connect to your personal NN account and does not perform any transactions — it only reads the contribution PDF you give it and the publicly available fund NAV data, and stores everything locally.

## Installation

Prebuilt packages are produced for Windows, Linux and as a portable, no-install archive. Pick the one that matches your platform.

### Windows (MSI installer)

1. Download the `NnPensionTracker-[version].msi` installer.
2. Install it:
   ```cmd
   msiexec /i NnPensionTracker-[version].msi
   ```
   (or just double-click the file)
3. The `nn-pension-tracker` command is added to the machine `PATH`, so it can be run from any Command Prompt / PowerShell window.

To uninstall:
```cmd
msiexec /x NnPensionTracker-[version].msi
```

### Linux (.deb package)

Requires the .NET 10 runtime (`dotnet-runtime-10.0` or equivalent) to be installed already, since the package is framework-dependent.

```bash
sudo dpkg -i nn-pension-tracker.[version].linux-x64.deb
```

Then run it with:
```bash
nn-pension-tracker
```

### Portable (zip archive)

No installation needed:

1. Download the portable `.zip` archive for your platform.
2. Unzip it anywhere.
3. Run `nn-pension-tracker` (or `NnPensionTracker.Cli.exe` on Windows) directly from that directory.

In portable mode, configuration and data files are kept next to the executable (identified by the presence of a `portable` marker file), so the whole thing stays self-contained and can be moved or deleted freely.

### Where things are stored

| | Portable (zip) | Windows (msi) | Linux (deb) |
| --- | --- | --- | --- |
| Binaries | Unzip directory | `C:\Program Files\NN Pension Tracker` | `/usr/share/nn-pension-tracker` |
| Configuration (`appsettings.json`) | Unzip directory | `C:\ProgramData\NN Pension Tracker` and `%AppData%\NN Pension Tracker` | `/etc/nn-pension-tracker` |
| Database (your data) | Unzip directory | `C:\ProgramData\NN Pension Tracker` | `~/nn-pension-tracker` |

Configuration currently supports a single setting, the display culture, e.g.:

```json
{
  "CultureInfo": "ro-RO"
}
```

See `doc/install-locations.md` for more details.

## Usage

The application is a CLI tool with two nouns

- `account` (your contributions)
- `fund` (the NAV history)

Each noun supports `show`, `import`, `export` and `clear` verbs.

Run without arguments, or `help`, to print the full command reference:

```bash
nn-pension-tracker help
```

### Account (your contributions)

Import contribution records from an NN contribution statement PDF:

```bash
nn-pension-tracker account import --file statement.pdf
```

Show recorded contributions, optionally filtered by year or by a month interval:

```bash
nn-pension-tracker account
nn-pension-tracker account show --year 2025
nn-pension-tracker account show --from 01/2024 --to 06/2025
```

Export contribution records to CSV for PortfolioPerformance (the only supported format, `pp`, is also the default):

```bash
nn-pension-tracker account export
nn-pension-tracker account export --format pp --year 2025
```

Clear all contribution records from the database:

```bash
nn-pension-tracker account clear
```

### Fund (NAV history)

Show fund values already stored in the database, optionally filtered by year or date interval:

```bash
nn-pension-tracker fund
nn-pension-tracker fund show --year 2025
nn-pension-tracker fund show --from 2025-01-01 --to 2025-06-30
```

Preview fund values straight from NN's website, without importing them:

```bash
nn-pension-tracker fund show --source web
nn-pension-tracker fund show --source web --year 2024
```

Import fund values from NN's web API, for a year or a date range (the `web` source is inferred automatically when `--year`/`--from`/`--to` are given):

```bash
nn-pension-tracker fund import --year 2025
nn-pension-tracker fund import --from 2025-01-01 --to 2025-06-30
```

Import fund values from a CSV file (same format as the historical values file downloadable from NN's website; the `file` source is inferred automatically when `--file` is given):

```bash
nn-pension-tracker fund import --file historical_2008.csv
```

Export fund values from the database to a CSV file:

```bash
nn-pension-tracker fund export --file fund-values.csv
nn-pension-tracker fund export --file fund-values-2025.csv --year 2025
```

Clear all fund values from the database:

```bash
nn-pension-tracker fund clear
```

## Building from source

Requires the .NET 10 SDK. All commands run from the `sources/` directory.

```bash
dotnet restore ./NnPensionTracker.slnx --configfile ../nuget.config
dotnet build ./NnPensionTracker.slnx -c Release --no-restore
dotnet run --project ./NnPensionTracker.Cli/NnPensionTracker.Cli.csproj -- account show
```

Run the tests:

```bash
dotnet test ./NnPensionTracker.Cli.Tests/NnPensionTracker.Cli.Tests.csproj
```

Build the Linux `.deb` package:

```bash
./build-deb.sh [Release|Debug]
```

Build the Windows MSI installer by building the `NnPensionTracker.Installer` project with the `Release plus Install` configuration (Windows only, requires WiX).

See `doc/release.md` for the full release process.

## License

Licensed under the GNU General Public License v3.0 — see [LICENSE](LICENSE) for the full text.
