param([ValidateSet("Net8","Net10","Both")][string]$Target="Both",[string]$AcadDir="C:\Program Files\Autodesk\AutoCAD 2026")
$ErrorActionPreference="Stop"
$root=$PSScriptRoot
function Need($name,$hint){if(-not(Get-Command $name -ErrorAction SilentlyContinue)){throw "$name não encontrado. $hint"}}
Need "dotnet" "Instale o SDK .NET 8 e, para AutoCAD 2026.1.2+, também o SDK .NET 10."
$required=@("AcCoreMgd.dll","AcDbMgd.dll","AcMgd.dll","AdWindows.dll")
foreach($f in $required){if(!(Test-Path (Join-Path $AcadDir $f))){throw "Biblioteca não encontrada: $AcadDir\$f"}}
$targets=if($Target-eq"Both"){@("Net8","Net10")}else{@($Target)}
foreach($t in $targets){Write-Host "=== Compilando $t ===";dotnet build "$root\src\RofamaCad\RofamaCad.csproj" -c Release -p:ACAD_DIR="$AcadDir" -p:RofamaTarget=$t;if($LASTEXITCODE-ne 0){throw "Falha no build $t"};$tfm=if($t-eq"Net10"){"net10.0-windows"}else{"net8.0-windows"};$out="$root\ROFAMACAD.bundle\Contents\$t";New-Item -ItemType Directory -Force $out|Out-Null;Copy-Item "$root\src\RofamaCad\bin\Release\$tfm\RofamaCad.dll" $out -Force}
Write-Host "Build concluído. Execute .\install-test.ps1 para instalar o pacote de teste."
