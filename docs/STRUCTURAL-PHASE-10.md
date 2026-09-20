# Estrutural — Fase 10: pré-dimensionamento experimental

Implementado:
- RFPREDIMENSIONAR: grava sugestões geométricas preliminares para vigas, pilares e lajes.
- RFPREDIMRELATORIO: lista as sugestões armazenadas e o método heurístico usado.

Critérios atuais são deliberadamente marcados como NÃO NORMATIVOS:
- vigas: h aproximado por L/10, com mínimo interno de 0,30 m;
- pilares: seção quadrada por faixas da reação tributária armazenada;
- lajes: espessura aproximada pelo menor vão/40, com mínimo interno de 0,10 m.

Essas relações são apenas ponto de partida geométrico para edição e testes do fluxo. Não representam atendimento à ABNT NBR 6118, não calculam armaduras e não substituem ELU/ELS, estabilidade, detalhamento ou revisão profissional.

A edição vigente da norma de projeto de estruturas de concreto deve ser conferida no catálogo oficial da ABNT antes de implementar verificações normativas.
