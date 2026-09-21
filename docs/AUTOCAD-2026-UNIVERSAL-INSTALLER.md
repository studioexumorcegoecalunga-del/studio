# Instalador único para a família AutoCAD 2026

A Autodesk dividiu o AutoCAD 2026 em dois runtimes gerenciados:
- AutoCAD 2026 até Update 1.1: .NET 8.
- AutoCAD 2026 Update 1.2 e posteriores: .NET 10.

Por isso, uma única DLL não é a estratégia segura para toda a família 2026. O pacote ROFAMA passa a conter dois binários compilados do mesmo código-fonte:
- Contents/Net8/RofamaCad.dll
- Contents/Net10/RofamaCad.dll

O instalador é único. Durante a instalação, ele seleciona a variante compatível e reescreve apenas o ModuleName do PackageContents.xml da cópia instalada.

Antes de distribuir:
1. Compilar build-all-2026.ps1 em uma máquina Windows com AutoCAD 2026 e SDK/assemblies compatíveis.
2. Validar a identificação da build do acad.exe em instalações 2026.0/1.1 e 2026.1.2+.
3. Executar smoke tests em ambos os runtimes.
4. Só então compilar o instalador Inno Setup e publicar o EXE.

Observação: a detecção por ProductVersion precisa ser confirmada nas máquinas reais. Se a numeração de arquivo não diferenciar 1.1/1.2 de forma confiável, substituir por detecção de runtime/registro antes do release.
