param([string]$SourceBundle)
$ErrorActionPreference="Stop"
$acad="C:\Program Files\Autodesk\AutoCAD 2026"
if(!(Test-Path $acad)){throw "AutoCAD 2026 não encontrado no caminho padrão."}
$net10=$false
$acadExe=Join-Path $acad "acad.exe"
if(Test-Path $acadExe){$v=(Get-Item $acadExe).VersionInfo.ProductVersion;try{$ver=[version]($v -replace '[^0-9\.]','');if($ver.Build -ge 2){$net10=$true}}catch{}}
# A detecção definitiva usa a presença do runtime .NET 10 do AutoCAD; se houver dúvida, o instalador pergunta via dois pacotes separados.
$dest=Join-Path $env:APPDATA "Autodesk\ApplicationPlugins\ROFAMACAD.bundle"
if(Test-Path $dest){Remove-Item $dest -Recurse -Force}
Copy-Item $SourceBundle $dest -Recurse -Force
$xml=Join-Path $dest "PackageContents.xml"
$content=Get-Content $xml -Raw
if($net10){$content=$content.Replace("./Contents/Net8/RofamaCad.dll","./Contents/Net10/RofamaCad.dll").Replace("(.NET 8)","(.NET 10)")}
Set-Content $xml $content -Encoding UTF8
Write-Host ("ROFAMA CAD PRO instalado para "+($(if($net10){".NET 10 / AutoCAD 2026.1.2+"}else{".NET 8 / AutoCAD 2026-2026.1.1"})))
