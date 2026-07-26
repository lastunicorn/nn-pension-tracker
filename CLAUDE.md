# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A .NET 10 console application (`nn-pension-tracker`) that tracks the Romanian Mandatory Private Pension (Pillar II, NN fund): importing contribution statements (PDF), importing/showing fund NAV history (from NN's web API or CSV), and exporting data for use in PortfolioPerformance.

## Commands

All commands run from `sources/`, against the solution file `NnPensionTracker.slnx`.

```bash
# restore / build (CI uses these two)
dotnet restore ./NnPensionTracker.slnx --configfile ../nuget.config
dotnet build ./NnPensionTracker.slnx -c Release --no-restore

# run all tests
dotnet test ./NnPensionTracker.Cli.Tests/NnPensionTracker.Cli.Tests.csproj

# run a single test (xunit, by fully-qualified name or filter)
dotnet test ./NnPensionTracker.Cli.Tests/NnPensionTracker.Cli.Tests.csproj --filter "FullyQualifiedName~ConstructorTests"

# run the CLI locally
dotnet run --project ./NnPensionTracker.Cli/NnPensionTracker.Cli.csproj -- account show

# build the Linux .deb package (framework-dependent, requires dpkg-deb)
./build-deb.sh [Release|Debug]

# build the Windows MSI installer
# build the NnPensionTracker.Installer project with configuration "Release plus Install" (Windows only, WiX-based)
```

There is no lint step; code style is enforced by convention (see below) rather than an analyzer.

## Architecture

Layered/ports-and-adapters, mirroring the conventions in `.github/copilot-instructions.md` (read that file for the full rules — this is the condensed, project-specific view):

- **`NnPensionTracker.Domain`** — plain domain types (`FundNav`, `DataLabel`, `Contribution` comes from the external `DustInTheWind.NN.Toolkit` package). No dependencies on anything else in the solution.
- **`NnPensionTracker.Ports.DataAccess`** — the persistence port. `Database` is an in-memory store loaded from/saved to disk on open/`SaveAllAsync` (via `ContributionPersister`, `FundRecordPersister`, `DataLabelParser`). `IUnitOfWork`/`UnitOfWork` exposes lazily-created repositories (`IContributionRepository`, `IFundNavRepository`, `IDataLabelRepository`) backed by that `Database`. There is no real database engine — "the database" is a JSON-backed file store opened once per process at the path resolved by `DeploymentEnvironment`.
- **`NnPensionTracker.Ports.FileSystemAccess`** — thin wrapper (`IFileSystemService`/`FileSystemService`) around file I/O, used by import/export use cases instead of touching `System.IO` directly.
- **`NnPensionTracker.Cli.Presentation`** — the application/use-case layer *and* presentation layer combined (no separate `Application` project in this solution). Each operation is a RequestR use case class implementing `IUseCase<TRequest>` (`Task Execute(TRequest, CancellationToken)`), living in its own `UseCases/<Name>/` subdirectory alongside its request/diagnostics types (e.g. `UseCases/ImportFundFromWeb/{ImportFundFromWebUseCase,ImportFundFromWebRequest,ImportDiagnostics,UnixDateInterval}.cs`). Use cases take dependencies via constructor injection (null-checked, stored in `readonly` fields), never depend on each other, and receive their options through the request object sent on the `RequestBus`. Console rendering helpers (`XConsole`, `ConsoleTools` tables) live under `ConsoleUtils/`.
- **`NnPensionTracker.Cli`** — composition root. `Setup.ConfigureServices` wires the DI container (all use cases `AddTransient`, `IUnitOfWork` `AddScoped`, `Database`/`DeploymentEnvironment`/`IConfigurationRoot` singletons). `DeploymentEnvironment` resolves OS- and packaging-specific paths for config (`appsettings.json`) and the data directory — it distinguishes three deployment modes by checking for a `portable` marker file next to the executable: **portable** (paths relative to the exe), **Windows** (`ProgramData`/`AppData\Roaming`, app dir `"Nn Pension Tracker"`), **Linux** (`/etc`, `~/.config`, app dir `"nn-pension-tracker"`) — see `doc/install-locations.md` for the resolved paths per deployment method. CLI parsing/dispatch uses `ConsoleTools.Commando`: `Program.Main` builds a Commando `Application` (`ApplicationBuilder` + `Setup.ConfigureServices`) and runs it. Each CLI command is a thin `IConsoleCommand` class in `Commands/` (`[NamedCommand("account-import")]`, `[NamedParameter]`/`[AnonymousParameter]` properties) that maps its parameters to a request and sends it through the RequestR `RequestBus`. Because Commando matches a single-token command name, the noun/verb grammar (`account import`) is preserved by `NounVerbCommandParser`, a custom `ICommandParser` that folds the leading noun+verb pair into `noun-verb` when such a command is registered; bare `account`/`fund` are alias command classes deriving from the `*ShowCommand`s. `help` is Commando's built-in command (generated from command metadata) — there is no HelpUseCase.
- **`NnPensionTracker.Installer`** — WiX MSI packaging project (Windows only; build with configuration `Release plus Install`).
- **`NnPensionTracker.Cli.Tests`** — xunit + FluentAssertions, currently covering `UnixDateInterval` (in `UseCases/ImportFundFromWeb/`) and `NounVerbCommandParser`. Test classes are grouped in a `<ClassName>Tests/` directory with one file per tested method (e.g. `UnixDateIntervalTests/ConstructorTests.cs`, `ContainsTests.cs`).

External dependencies worth knowing: `DustInTheWind.NN.Toolkit` provides `INnApiClient` (NN's fund NAV web API) and pension-domain types; `Tabula` + PDF parsing is used for contribution statement import; `CsvHelper` for CSV import/export; `ConsoleTools.Commando` (+ `.Parsing`, `.Setup.Microsoft`) for CLI command parsing/dispatch and `ConsoleTools`/`ConsoleTools.Controls.Tables` for console output.

## Code conventions (from `.github/copilot-instructions.md`)

- No `var` — use explicit types.
- LINQ lambda parameter name: `x`.
- Use `new()` (target-typed) when instantiating objects.
- Object initializers with more than one property: one property per line.
- No braces for single-line `if`/`for`/`using` bodies.
- No underscore-prefixed fields.
- No XML doc comments for internal-only types; only for types published as a NuGet package.
- Tests: `Assert.Throws`/`.Should().Throw()` lambdas use a block body; one test file per tested public method (including the constructor), grouped under a `<ClassName>Tests/` directory; test method names follow `[Having<setup>_]When<action>_Then<expected>` (the `Having` part is only used when relevant setup needs to be called out).
- Use cases must not reference each other or UI types; shared logic goes in a domain service or helper instead.
