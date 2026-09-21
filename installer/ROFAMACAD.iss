#define MyAppName "ROFAMA CAD PRO"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "ROFAMA"
#define MyAppExeName "RofamaCad.dll"
[Setup]
AppId={{B8B61E36-48D9-4F45-923A-8D2D84E29D02}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={userappdata}\Autodesk\ApplicationPlugins\ROFAMACAD.bundle
DisableDirPage=yes
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
OutputDir=..\dist
OutputBaseFilename=ROFAMA-CAD-PRO-Setup-1.0.0
Compression=lzma2
SolidCompression=yes
ArchitecturesAllowed=x64compatible
UninstallDisplayName={#MyAppName}
[Files]
Source: "..\ROFAMACAD.bundle\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
[Run]
Filename: "{cmd}"; Parameters: "/c echo ROFAMA CAD PRO instalado. Reinicie o AutoCAD 2026."; Flags: postinstall skipifsilent runhidden
