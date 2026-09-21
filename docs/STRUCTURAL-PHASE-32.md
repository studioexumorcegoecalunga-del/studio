# Estrutural — Fase 32: envelope contínuo e combinação governante

Implementado:
- ContinuousEnvelopeService: alinha/amostra resultados de várias combinações e calcula envelopes inferior/superior por posição.
- RFENVELOPECONTINUO: reconstrói V(x), M(x) e deformada de cada combinação, cria curvas superior/inferior e persiste o envelope completo.
- RFGOVERNANTEENVELOPE: identifica o maior valor absoluto de V, M e deslocamento, sua posição e a combinação governante.

Layers:
- RF-EST-ENV-V
- RF-EST-ENV-M
- RF-EST-ENV-D

Observação:
A reconstrução usa os resultados do solver e as cargas persistidas por combinação. Os pontos comuns são obtidos pela união das amostras; valores de uma curva em uma abscissa ausente usam a amostra mais próxima. Isso é adequado como primeira implementação gráfica, mas deve evoluir para interpolação/avaliação analítica em abscissas comuns antes de dimensionamento final.

Próxima fase:
- avaliação analítica comum para todos os casos;
- tabela gráfica de envelope;
- marcação no DWG das combinações governantes;
- limpeza/atualização associativa dos diagramas antigos.
