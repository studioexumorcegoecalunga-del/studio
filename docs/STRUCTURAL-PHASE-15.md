# Estrutural — Fase 15: propriedades e vãos reais

Implementado:
- RFMATERIAL: grava fck e módulo E informados pelo usuário no Named Objects Dictionary do DWG.
- RFGRAVARVAOS: persiste os comprimentos geométricos reais de cada vão de viga.
- RFMATRIZVIGA2: usa os vãos reais e o E cadastrado, removendo o E fixo e a uniformização dos comprimentos da versão anterior.

O módulo E é entrada explícita do projeto nesta fase; o software não o deduz automaticamente de uma norma. Isso mantém a origem do dado rastreável.

Próxima etapa: benchmarks automatizados do solver, equilíbrio de reações, casos de 1/2/3 vãos e comparação com soluções analíticas conhecidas antes de qualquer dimensionamento.
