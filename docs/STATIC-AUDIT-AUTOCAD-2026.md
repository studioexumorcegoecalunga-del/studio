# Auditoria estática — AutoCAD 2026

Data: 2026-09-20

## Compatibilidade de framework confirmada na documentação Autodesk
- AutoCAD 2026 até 2026.1.1: alvo .NET 8.
- AutoCAD 2026.1.2 e posteriores: alvo .NET 10.
- O projeto mantém os dois alvos por meio de RofamaTarget.

## Correção aplicada
- RFNIVEL deixou de depender de propriedades de valor padrão em PromptStringOptions; o valor "TÉRREO" passa a ser tratado pela própria lógica do comando.

## Pontos ainda dependentes de compilação real
- Autodesk.AutoCAD.Windows / Ribbon via AdWindows.
- API de Table e CellRange no quadro de esquadrias.
- operações Region/Solid3d e booleanas.
- geração Face para coberturas.
- carregamento do PackageContents.xml na revisão exata instalada.

## Critério
Nenhum item acima deve ser considerado validado apenas por inspeção estática. A RC só deve ser publicada após build com os assemblies da instalação e smoke test no AutoCAD 2026.
