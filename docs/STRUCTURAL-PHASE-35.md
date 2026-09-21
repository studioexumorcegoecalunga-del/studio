# Estrutural — Fase 35: avaliação direta e gráficos associativos

Implementado:
- BeamFieldEvaluator: avaliação direta de V(x), M(x) e deslocamento FE interpolado em qualquer abscissa, usando os resultados do elemento e as cargas distribuídas/pontuais.
- RFATUALIZARENVELOPE: reconstrói todas as combinações, cria uma malha comum, avalia diretamente cada combinação, apaga gráficos associativos anteriores da viga e redesenha envelopes V/M/deformada com tagging por handle.
- RFAUDITARESTRUTURAL: auditoria integrada de vigas sem ID, vãos analíticos, apoios, pré-dimensionamento ou resultados de combinação.
- Os novos envelopes usam GeneratedGraphicsService.Tag, permitindo atualização sem acumular duplicatas.

Limitações mantidas:
- deslocamento usa interpolação cúbica de Hermite do elemento de viga; com um elemento por vão, cargas distribuídas não produzem a solução fechada quartica exata;
- liberação de extremidades ainda precisa da revisão de condensação/recovery prevista;
- compilação real AutoCAD 2026 continua obrigatória antes de tratar a ferramenta como validada.

Próxima fase recomendada:
revisar rigorosamente o solver de liberações, recuperação de forças e DOFs desconectados antes de qualquer dimensionamento de concreto armado.
