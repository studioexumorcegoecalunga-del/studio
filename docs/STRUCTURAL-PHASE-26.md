# Estrutural — Fase 26: diagramas V/M e deformada

Implementado:
- BeamPostProcessor: amostragem por vão, incluindo pontos imediatamente antes/depois de cargas concentradas.
- RFDIAGRAMASV7: gera polilinhas de esforço cortante, momento fletor e deformada em layers separados.
- RFRELATORIOV7: resumo de reação vertical máxima, momento de extremidade máximo e deslocamento vertical nodal máximo.

Layers:
- RF-EST-DIAGRAMA-V
- RF-EST-DIAGRAMA-M
- RF-EST-DEFORMADA

Observações:
- escalas gráficas atuais são de visualização e não constituem escala automática de prancha;
- a deformada usa interpolação cúbica pelos deslocamentos/rotações nodais;
- os diagramas internos são reconstruídos a partir dos esforços de extremidade, UDL e cargas pontuais, devendo ser benchmarkados antes de uso de engenharia.

Próxima fase:
- anotações de máximos/mínimos e valores nos apoios;
- escala gráfica configurável;
- diagrama por combinação/caso de carga;
- exportação de tabela de resultados.
