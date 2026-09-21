$ErrorActionPreference="Stop"
$dest=Join-Path $env:APPDATA "Autodesk\ApplicationPlugins\ROFAMACAD.bundle"
if(Test-Path $dest){Remove-Item $dest -Recurse -Force;Write-Host "ROFAMA CAD PRO removido."}else{Write-Host "ROFAMA CAD PRO não estava instalado neste perfil."}
