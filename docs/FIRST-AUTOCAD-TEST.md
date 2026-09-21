# Primeiro teste real no AutoCAD 2026

## Objetivo
A partir deste ponto, a validação deve ocorrer na máquina Windows que possui o AutoCAD 2026.

## Procedimento
1. Fechar o AutoCAD.
2. Abrir PowerShell na raiz do projeto.
3. Executar prepare-test-build.ps1.
4. Instalar a variante correspondente com install-test.ps1.
5. Abrir o AutoCAD.
6. Confirmar o Ribbon ROFAMA.
7. Executar RFSOBRE.
8. Executar os benchmarks estruturais:
   - RFTESTARSOLVER
   - RFTESTARSOLVER2
   - RFTESTARFRAME
   - RFTESTARCARGAPONTUAL
   - RFTESTARLIBERACOES2
   - RFTESTARSOLVERV2
   - RFTESTARSOLVERV2AVANCADO
   - RFAUDITARSOLVERV2
9. Em DWG de teste, executar RFCOMPARARSOLVERSEMLOTE e RFSTATUSSOLVERV2.
10. Não usar resultados estruturais como projeto executivo enquanto o gate V2 e os testes reais não forem concluídos.

## Compatibilidade
Use Net8 para AutoCAD 2026 até 2026.1.1 e Net10 para 2026.1.2+.

## Release
Após os testes, package-release.ps1 + Inno Setup geram o instalador final.
