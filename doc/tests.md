# Install Tests

## I) Portable

### 1) `portable` marker

- Verify the unzipped directory contains the `portable` file.

### 2) Database directory created in local directory

- Load data
  - `nn-pension-tracker account import contributions.pdf`
- Verify `Data` directory is created in the application's root and it contains the `contributions.json` file with imported data.

### 3) Configuration loaded from local directory

- Change culture info to `en-US` in `appsettings.json`
- Display data and check it is displayed in `en-US` format
  - `nn-pension-tracker account`

### 4) Configuration NOT loaded from system directory

- Create `appsettings.json` file in `C:\ApplicationData\NN Pension Tracker`
- 

## II) MSI Install

TBD

## III) deb Install

TBD