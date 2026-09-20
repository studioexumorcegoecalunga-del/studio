# Estrutural — Fase 04

Implementado:
- RFESTGRAFO: leitura das relações geométricas laje-viga e viga-pilar.
- RFFORMAS: primeira camada de anotação da planta de formas.
- RFESTRELATORIO: resumo de pilares, vigas, lajes, fundações, comprimento de eixos e área geométrica.

Limitações atuais:
- o grafo é geométrico e ainda não calcula esforços;
- a relação laje-viga usa proximidade/interseção de envelopes;
- a planta de formas é preliminar, sem cortes, cotas estruturais, níveis ou seções dimensionadas;
- não há detalhamento de armaduras.

Próxima fase: persistir relações do grafo, distribuir ações das lajes às vigas, acumular reações nos pilares e preparar verificações rastreáveis.
