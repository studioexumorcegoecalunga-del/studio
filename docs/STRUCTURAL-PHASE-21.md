# Estrutural — Fase 21: cargas concentradas

Implementado:
- PointLoadBeamSolver: extensão do elemento de viga 2-GDL com cargas pontuais em posição arbitrária do vão usando funções de forma cúbicas consistentes.
- RFCARGAPONTUAL: cadastra uma ou mais cargas pontuais por vão, com posição local e intensidade.
- RFMATRIZVIGA5: resolve simultaneamente cargas distribuídas por vão e cargas pontuais.

Próximos controles necessários:
- benchmark de carga pontual no meio do vão e em balanço;
- auditoria global incluindo a soma das cargas pontuais;
- representação gráfica das cargas;
- liberações internas de momento.
