# Estrutural — Fase 24: liberações efetivas e solver V6

Implementado:
- ReleasedBeamSolver: aplica condensação estática local aos graus de liberdade rotacionais liberados.
- RFMATRIZVIGA6: usa as liberações cadastradas por RFLIBERACAOVIGA e grava resultados V6.
- Detecção explícita de matriz singular como mecanismo/instabilidade.
- RFTESTARLIBERACOES: benchmark de viga biapoiada obtida por liberações e teste de mecanismo livre.

Limitação atual:
- V6 considera cargas distribuídas por vão; a integração simultânea das cargas pontuais do V5 com as liberações será consolidada na próxima revisão.
- A condensação implementada deve passar por compilação e benchmark independente no AutoCAD/.NET antes de uso de engenharia.

Próximo:
- unificar V5 + V6;
- cargas pontuais com liberações;
- diagramas M/V e deformada a partir dos resultados;
- ampliar benchmarks Gerber.
