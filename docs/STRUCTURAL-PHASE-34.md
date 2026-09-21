# Estrutural — Fase 34: tabela, associatividade e benchmark

Implementado:
- RFTABELAENVELOPE: tabela AutoCAD com ROFAMA_STRUCT_ID, máximos absolutos de V/M/deslocamento e combinação governante.
- GeneratedGraphicsService: infraestrutura para marcar gráficos gerados com handle da viga fonte e tipo.
- RFLIMPARESTRUTURAL: remove gráficos/anotações associativos previamente marcados para uma viga.
- RFTESTARPOSPROCESSADOR: benchmark de referência para sinais e valores clássicos de V/M em viga simplesmente apoiada com UDL e carga pontual central.

Atenção:
- A infraestrutura associativa está criada, mas os comandos antigos de desenho ainda precisam passar a chamar GeneratedGraphicsService.Tag ao criar cada Polyline/DBText.
- A API AutoCAD Table/CellRange ainda precisa de compilação real no AutoCAD 2026.
- O benchmark deve ser executado após compilação; ele não substitui testes unitários independentes do solver.

Próxima fase:
- ligar tagging/limpeza a todos os diagramas e anotações;
- corrigir qualquer sinal que o benchmark revelar;
- substituir interpolação do envelope por avaliação direta das funções do elemento;
- auditoria estática integrada antes do dimensionamento.
