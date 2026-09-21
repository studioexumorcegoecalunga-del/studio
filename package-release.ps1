param([ValidateSet("Net8","Net10")][string]$Target="Net10",[string]$AcadDir="C:\Program Files\Autodesk\AutoCAD 2026",[string]$Version="1.0.0")
$ErrorActionPreference="Stop"
$root=$PSScriptRoot
& "$root\build.ps1" -Target $Target -AcadDir $AcadDir
$dist=Join-Path $root "dist";New-Item -ItemType Directory -Force $dist|Out-Null
$zip=Join-Path $dist "ROFAMA-CAD-PRO-$Version-$Target.zip";if(Test-Path $zip){Remove-Item $zip -Force}
Compress-Archive -Path "$root\ROFAMACAD.bundle" -DestinationPath $zip -CompressionLevel Optimal
$iss=Join-Path $root "installer\ROFAMACAD.iss"
$iscc=(Get-Command ISCC.exe -ErrorAction SilentlyContinue).Source
if(-not $iscc){$candidates=@("$env:ProgramFiles(x86)\Inno Setup 6\ISCC.exe","$env:ProgramFiles\Inno Setup 6\ISCC.exe");$iscc=$candidates|Where-Object{Test-Path $_}|Select-Object -First 1}
if($iscc){& $iscc $iss}else{Write-Warning "Inno Setup 6 não encontrado. ZIP criado; instalador EXE não foi compilado."}
Write-Host "Pacote: $zip"
