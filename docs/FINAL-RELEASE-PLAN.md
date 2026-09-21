# ROFAMA CAD PRO — Plano de versão final e instalador

## Gate obrigatório
1. Compilar contra as DLLs do AutoCAD 2026 instalado.
2. Escolher runtime correto:
   - AutoCAD 2026 até 2026.1.1: .NET 8.
   - AutoCAD 2026.1.2 ou posterior: .NET 10.
3. Executar benchmarks estruturais V2 e auditorias.
4. Executar comparação V1 x V2 em lote em DWGs de teste.
5. Corrigir divergências antes de promover o V2.
6. Smoke test dos comandos de arquitetura, 3D, documentação e estrutura.
7. Testar instalação/desinstalação em perfil Windows limpo.
8. Gerar ZIP do .bundle e instalador EXE.
9. Congelar versão 1.0.0 e criar release.

## Empacotamento
- package-release.ps1 compila o projeto, atualiza o bundle de saída e cria ZIP.
- installer/ROFAMACAD.iss cria instalador por usuário em %APPDATA%\Autodesk\ApplicationPlugins\ROFAMACAD.bundle.
- O instalador EXE depende do Inno Setup 6 estar instalado na máquina de build.

## Estado
A infraestrutura de release está pronta no código-fonte. A compilação final não pode ser certificada sem AutoCAD 2026/assemblies reais e ambiente Windows de build/teste.
