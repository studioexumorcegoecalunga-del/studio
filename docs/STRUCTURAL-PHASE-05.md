# Estrutural — Fase 05: caminho de cargas

## Implementado
- RFDISTRIBUIRCARGAS
  - lê gk e qk cadastrados nas lajes;
  - calcula a carga característica total por área;
  - identifica vigas geometricamente adjacentes;
  - reparte a carga igualmente entre essas vigas;
  - transfere a carga de cada viga igualmente aos pilares geometricamente associados;
  - grava ROFAMA_BEAM_LOAD e ROFAMA_COLUMN_REACTION.
- RFCARGASRELATORIO
  - audita as somas armazenadas em vigas e pilares.

## Importante
Este algoritmo é deliberadamente conservador quanto à sua classificação: é um caminho de cargas geométrico simplificado, não uma análise estrutural. Ele ainda não considera área de influência real, direção de trabalho da laje, continuidade, rigidez, peso próprio automático, paredes, combinações ELU/ELS, vento ou efeitos de segunda ordem.

A próxima fase deve substituir o rateio uniforme por áreas/faixas de influência e adicionar auditoria de equilíbrio antes de qualquer pré-dimensionamento.
