# Estrutural — Fase 38: validação ampliada do Solver V2

Implementado:
- RFTESTARSOLVERV2AVANCADO: testes de dois vãos, simetria, equilíbrio vertical, carga pontual central e rótula interna.
- SolverV2EquilibriumService: balanço independente de forças verticais e momentos em torno do início da viga.
- RFAUDITARSOLVERV2: caso misto de dois vãos com cargas distribuídas diferentes e duas cargas pontuais, exibindo resíduos de força e momento.

Critérios atuais:
- equilíbrio vertical: soma das reações deve reproduzir a carga aplicada;
- equilíbrio de momentos: reações verticais + reações de momento devem equilibrar os momentos das cargas;
- rótulas: momentos recuperados nas extremidades liberadas devem tender a zero;
- simetria: casos simétricos devem produzir reações extremas simétricas.

Importante:
Esses benchmarks estão implementados no código-fonte, mas os resultados reais dependem da compilação/execução no AutoCAD 2026. O Solver V2 ainda não foi promovido como solver padrão.

Próxima fase:
- criar adaptador de produção para executar casos/combinações pelo V2;
- manter opção de comparação V1 x V2;
- armazenar versão do solver nos resultados;
- só promover V2 após testes reais passarem.
