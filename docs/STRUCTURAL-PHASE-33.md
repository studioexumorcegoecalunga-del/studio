# Estrutural — Fase 33: malha comum e anotação de governantes

Implementado:
- EnvelopeInterpolationService: avalia curvas em uma abscissa comum por interpolação entre amostras, eliminando a estratégia anterior de simplesmente escolher a amostra mais próxima.
- EnvelopeV2Service: gera envelope V/M/deformada em uma malha comum densa (200 divisões + posições especiais existentes).
- RFANOTARENVELOPE: escreve no DWG os extremos governantes de cortante, momento e deslocamento, incluindo o nome da combinação governante.
- Layer RF-EST-ENV-TEXTO para anotações.

Observação técnica:
A interpolação é uma melhoria importante sobre nearest-neighbor, mas ainda não é uma avaliação fechada/exata de V(x), M(x) e v(x). Antes de dimensionamento normativo final, o pós-processador deve evoluir para funções de elemento avaliadas diretamente em qualquer x e o solver/liberações deve passar pelos benchmarks completos e compilação AutoCAD 2026.

Próxima fase:
- ligar EnvelopeV2Service ao comando principal de geração;
- tabela de envelope com ROFAMA_STRUCT_ID;
- limpeza associativa dos gráficos/anotações antigos;
- benchmark automatizado do pós-processamento.
