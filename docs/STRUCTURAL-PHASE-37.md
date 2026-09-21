# Estrutural — Fase 37: integração global das liberações

Implementado:
- UnifiedBeamSolverV2: solver global que usa ReleasedBeamElement com condensação em bloco e recovery das rotações liberadas.
- Mapeamento dos DOFs retidos de cada elemento para o sistema global.
- DisconnectedDofService integrado antes da solução.
- DOFs sem rigidez e sem carga são retirados; DOFs sem rigidez com carga geram mecanismo.
- Cargas pontuais passam pela mesma condensação do elemento.
- Recuperação final de esforços usa K/f completos.
- RFTESTARSOLVERV2: benchmark integrado com viga biapoiada, extremidades liberadas, balanço e detecção de mecanismo.

Atenção:
O tratamento de cargas pontuais na recuperação de esforços ainda merece validação independente de sinais/equivalência; o solver V2 não substitui os comandos de produção até os benchmarks passarem em compilação real.

Próxima fase:
- benchmark de duas vigas/vãos com liberação interna;
- comparação numérica independente;
- adaptar Matrix/CombinationAnalysis para opção Solver V2;
- auditoria de equilíbrio e sinais;
- somente depois promover V2 como solver padrão.
