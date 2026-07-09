# Release

This document describes the manual release process.

## 1) Create MSI Installer

In Windows:

- Build `NnPensionTracker.Installer` project using build configuration `Release plus Install`.

Result:

- `NnPensionTracker.Installer/bin/x64/Release plus Install/en-US/NnPensionTracker-[version].msi`

## 2) Crete .deb bundle

In Linux:

- Run the `build-deb.sh` script.

Result:

- `NnPensionTracker.Cli/bin/Release/net10.0/linux-x64/nn-pension-tracker.[version].linux-x64.deb`

## 3) Portable Package

TBD