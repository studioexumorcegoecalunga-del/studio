# Estrutural — Fase 25: solver unificado V7

Implementado:
- UnifiedBeamSolver reúne em um único núcleo:
  - cargas distribuídas independentes por vão;
  - cargas pontuais em posição arbitrária;
  - apoios simples, engastados e extremidades livres;
  - liberações rotacionais de extremidade;
  - detecção de matriz singular/mecanismo.
- RFMATRIZVIGA7 grava ROFAMA_MATRIX_ANALYSIS_V7.
- RFTESTARV7 valida combinações UDL + carga pontual em viga simples, balanço e viga com liberações.

O V7 passa a ser o núcleo preferencial para evolução. V4/V5/V6 permanecem temporariamente para rastreabilidade e comparação.

Próxima fase: pós-processamento do V7 para diagramas V/M e deformada, com amostragem por vão e descontinuidades em cargas pontuais.
