#define MyAppName "ROFAMA CAD PRO"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "ROFAMA"
[Setup]
AppId={{B8B61E36-48D9-4F45-923A-8D2D84E29D02}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={localappdata}\ROFAMA\CADPRO\1.0.0
DisableDirPage=yes
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
OutputDir=..\dist
OutputBaseFilename=ROFAMA-CAD-PRO-Setup-1.0.0
Compression=lzma2
SolidCompression=yes
ArchitecturesAllowed=x64compatible
[Files]
Source: "..\ROFAMACAD.bundle\*"; DestDir: "{app}\ROFAMACAD.bundle"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "DetectAndInstall.ps1"; DestDir: "{app}"; Flags: ignoreversion
[Run]
Filename: "powershell.exe"; Parameters: "-NoProfile -ExecutionPolicy Bypass -File ""{app}\DetectAndInstall.ps1"" -SourceBundle ""{app}\ROFAMACAD.bundle"""; Flags: runhidden waituntilterminated
[UninstallRun]
Filename: "powershell.exe"; Parameters: "-NoProfile -ExecutionPolicy Bypass -Command ""$p=Join-Path $env:APPDATA 'Autodesk\ApplicationPlugins\ROFAMACAD.bundle'; if(Test-Path $p){{Remove-Item $p -Recurse -Force}}"""; Flags: runhidden
