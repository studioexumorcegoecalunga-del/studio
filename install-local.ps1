param([string]$Bundle="$PSScriptRoot\ROFAMACAD.bundle")
$ErrorActionPreference="Stop"
$dest=Join-Path $env:APPDATA "Autodesk\ApplicationPlugins\ROFAMACAD.bundle"
if(Test-Path $dest){Remove-Item $dest -Recurse -Force}
Copy-Item $Bundle $dest -Recurse -Force
Write-Host "ROFAMA CAD PRO instalado em $dest"
Write-Host "Reinicie o AutoCAD 2026 para testar."
