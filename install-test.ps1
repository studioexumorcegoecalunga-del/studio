param([ValidateSet("Net8","Net10")][string]$Target="Net10")
$ErrorActionPreference="Stop"
$root=$PSScriptRoot;$source="$root\ROFAMACAD.bundle";$dll="$source\Contents\$Target\RofamaCad.dll";if(!(Test-Path $dll)){throw "RofamaCad.dll não encontrada para $Target. Execute prepare-test-build.ps1 primeiro."}
$dest=Join-Path $env:APPDATA "Autodesk\ApplicationPlugins\ROFAMACAD.bundle";if(Test-Path $dest){Remove-Item $dest -Recurse -Force};Copy-Item $source $dest -Recurse -Force
$xml=Join-Path $dest "PackageContents.xml";$x=Get-Content $xml -Raw;$x=$x.Replace("./Contents/Net8/RofamaCad.dll","./Contents/$Target/RofamaCad.dll").Replace("(.NET 8)","(.$Target)");Set-Content $xml $x -Encoding UTF8
Write-Host "ROFAMA CAD PRO TESTE instalado em $dest";Write-Host "Abra/reinicie o AutoCAD 2026."
