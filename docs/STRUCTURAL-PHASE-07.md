# Estrutural — Fase 07

Implementado:
- RFCARGASINTEGRADAS: integra, por viga, a parcela recebida das lajes, a carga linear de paredes e o peso próprio linear da viga; grava a carga característica total e a transfere aos pilares associados.
- RFVERIFICARAPOIOS: identifica vigas sem pilares geometricamente associados e vigas com apenas um apoio detectado.

Observação:
A parcela de laje ainda vem do algoritmo simplificado da fase anterior. As cargas integradas são valores característicos preliminares, sem combinações ELU/ELS e sem análise de esforços.

Próxima etapa: algoritmo de áreas de influência/direção de trabalho das lajes, persistência das relações de apoio e relatório gráfico de inconsistências.
