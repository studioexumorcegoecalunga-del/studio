param(
  [ValidateSet("Net8","Net10")][string]$Target = "Net8",
  [string]$AcadDir = "C:\Program Files\Autodesk\AutoCAD 2026"
)
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$proj = Join-Path $root "src\RofamaCad\RofamaCad.csproj"
$out = Join-Path $root "ROFAMACAD.bundle\Contents\Windows"
New-Item -ItemType Directory -Force -Path $out | Out-Null
dotnet build $proj -c Release -p:ACAD_DIR="$AcadDir" -p:RofamaTarget=$Target
$tfm = if ($Target -eq "Net10") { "net10.0-windows" } else { "net8.0-windows" }
Copy-Item (Join-Path $root "src\RofamaCad\bin\Release\$tfm\RofamaCad.dll") $out -Force
Write-Host "Bundle preparado em $out para $Target"
