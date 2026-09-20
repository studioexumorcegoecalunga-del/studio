# Instalação de desenvolvimento — AutoCAD 2026

## Pré-requisitos
- Windows 64-bit.
- AutoCAD 2026 instalado.
- .NET SDK compatível com a revisão do AutoCAD.
- Assemblies do AutoCAD disponíveis na pasta de instalação.

## Compilação
Para a linha inicial do AutoCAD 2026:
`powershell -ExecutionPolicy Bypass -File .\build-net8.ps1`

Para a revisão que usa .NET 10:
`powershell -ExecutionPolicy Bypass -File .\build-net10.ps1`

## Instalação local
Após compilar:
`powershell -ExecutionPolicy Bypass -File .\install-local.ps1`

Reinicie o AutoCAD e execute `RFSOBRE`.

## Estado
Este pacote ainda é candidato de desenvolvimento. A compilação e o carregamento precisam ser validados em uma máquina com AutoCAD 2026 antes de publicar uma release.
