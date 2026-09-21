# Estrutural — Fase 23: liberações e estabilidade

Implementado:
- RFLIBERACAOVIGA: cadastro persistente de liberação de momento no início, fim ou ambas as extremidades de cada barra/vão.
- ReleaseModelService: leitura e consolidação das liberações armazenadas.
- RFESTABILIDADE: triagem topológica de condições de apoio/liberação potencialmente instáveis.

Importante:
Nesta fase o cadastro das liberações está pronto, mas o solver V5 ainda não faz condensação estática dos graus de liberdade liberados. Portanto, a liberação não altera os resultados numéricos ainda. Isso é intencional para evitar simular uma capacidade que o núcleo ainda não possui.

Próxima fase:
- implementar condensação estática da rigidez e do vetor de cargas do elemento;
- criar solver V6 que aplique efetivamente as liberações;
- benchmarks de viga biapoiada obtida por liberações, Gerber e mecanismo instável.
