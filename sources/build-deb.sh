#!/usr/bin/env bash
# Builds a framework-dependent .deb package for nn-pension-tracker.
#
# Prerequisites on the build machine:
#   - .NET SDK 10.0+  (dotnet)
#   - dpkg-deb        (apt install dpkg)
#
# Prerequisites on the target machine:
#   - .NET Runtime 10.0  (dotnet-runtime-10.0 or dotnet10)
#
# Usage:
#   chmod +x build-deb.sh
#   ./build-deb.sh [Release|Debug]

set -euo pipefail

CONFIGURATION="${1:-Release}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT="$SCRIPT_DIR/NnPensionTracker.Cli/NnPensionTracker.Cli.csproj"

echo "==> Building .deb package (configuration: $CONFIGURATION)"

dotnet publish "$PROJECT" \
    --configuration "$CONFIGURATION" \
    --runtime linux-x64 \
    --no-self-contained \
    /t:CreateDeb

# Locate the produced .deb file and print its path.
DEB_FILE=$(find "$SCRIPT_DIR/NnPensionTracker.Cli/bin/$CONFIGURATION" \
    -maxdepth 3 -name "*.deb" | head -1)

if [[ -n "$DEB_FILE" ]]; then
    echo ""
    echo "==> Package ready: $DEB_FILE"
    echo "    Install with:  sudo dpkg -i \"$DEB_FILE\""
    echo "    Then run with: nn-pension-tracker"
else
    echo "WARNING: .deb file not found in the expected output directory."
fi

