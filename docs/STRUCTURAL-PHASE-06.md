# Estrutural — Fase 06

Implementado:
- RFEQUILIBRIO: auditoria entre carga de entrada das lajes, carga transferida às vigas e reações armazenadas nos pilares.
- RFCARGAPAREDE: cadastro explícito de carga linear característica de parede sobre uma viga.
- RFPESOPROPRIO: calcula peso próprio linear preliminar de vigas de concreto a partir das dimensões cadastradas, usando 25 kN/m³.

Os componentes de carga permanecem separados em metadados para rastreabilidade. Nesta fase eles ainda não são combinados automaticamente em ELU/ELS.

Próximo passo: incorporar esses componentes ao caminho de cargas, substituir o rateio uniforme de lajes por faixas/áreas de influência e criar verificações de elementos sem apoio.
