# Estrutural — Fase 27: anotação, escalas e tabela V7

Implementado:
- RFCONFIGDIAGRAMAS: escalas de cortante, momento, amplificação da deformada e altura de texto.
- RFANOTARDIAGRAMAS: insere no DWG os máximos absolutos de V, M e deslocamento para cada viga analisada.
- RFTABELAV7: cria tabela AutoCAD com reação máxima, momento extremo máximo e deslocamento nodal máximo.

Layer:
- RF-EST-RESULTADOS

Nota de auditoria:
- A API Table/CellRange precisa ser confirmada em compilação real contra AutoCAD 2026.
- As escalas configuráveis ficam persistidas no NOD; o comando de desenho da Fase 26 ainda deve ser ligado a essas configurações em revisão posterior.
- Identificação da tabela ainda é sequencial V01... e deve migrar para ROFAMA_STRUCT_ID.
