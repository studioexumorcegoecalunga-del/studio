param([string]$AcadDir="C:\Program Files\Autodesk\AutoCAD 2026")
$ErrorActionPreference="Stop"
$root=$PSScriptRoot
foreach($t in @("Net8","Net10")){
 dotnet build "$root\src\RofamaCad\RofamaCad.csproj" -c Release -p:ACAD_DIR="$AcadDir" -p:RofamaTarget=$t
 $tfm=if($t-eq"Net10"){"net10.0-windows"}else{"net8.0-windows"}
 $out=Join-Path $root "ROFAMACAD.bundle\Contents\$t";New-Item -ItemType Directory -Force $out|Out-Null
 Copy-Item "$root\src\RofamaCad\bin\Release\$tfm\RofamaCad.dll" $out -Force
}
Write-Host "Builds AutoCAD 2026 Net8 e Net10 preparados."
