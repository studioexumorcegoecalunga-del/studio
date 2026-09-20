# Estrutural — Fase 14: primeiro solver matricial

Implementado:
- BeamStiffnessSolver: núcleo numérico independente do AutoCAD para vigas contínuas, com montagem da matriz rotacional, cargas nodais equivalentes, solução por eliminação de Gauss e recuperação de momentos de extremidade e reações.
- RFMATRIZVIGA: conecta o solver ao modelo analítico persistido no DWG.

Estado experimental:
- deslocamentos verticais dos apoios são assumidos nulos;
- os graus de liberdade resolvidos são rotações nodais;
- E é provisoriamente fixado no código;
- os vãos de uma linha são temporariamente uniformizados;
- a carga é tratada como uniformemente distribuída;
- ainda faltam balanços, liberações, recalques e testes de benchmark.

Próximo passo: persistir coordenadas reais dos nós/vãos, propriedades de material e seção; remover E provisório; criar testes numéricos com soluções conhecidas antes de usar o solver em qualquer verificação normativa.
