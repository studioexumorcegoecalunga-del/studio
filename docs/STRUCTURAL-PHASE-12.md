# Estrutural — Fase 12: análise por vãos

Implementado:
- RFANALISARVAOS: divide geometricamente cada eixo de viga nos pontos associados a pilares e calcula, para cada trecho, esforços do modelo biapoiado independente sob a carga linear equivalente existente.
- RFDIAGRAMAVIGA: primeira representação gráfica, anotando o momento máximo armazenado por viga na layer RF-EST-DIAGRAMA.

Limitações:
- ainda não há análise matricial de viga contínua;
- cada trecho é biapoiado independente;
- o comando de diagrama ainda não desenha curvas M/V;
- não há rigidez EI, recalques, balanços ou redistribuição.

Próxima fase: criar nós persistentes e barras analíticas, montar matriz de rigidez 2D para vigas contínuas e obter reações/momentos de extremidade de forma consistente.
