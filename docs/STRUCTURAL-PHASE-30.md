# Estrutural — Fase 30: combinações configuráveis de carregamento

Implementado:
- LoadCombinationService: persiste combinações no NOD do DWG.
- RFCOMBINACAOCARGA: cria uma combinação com N casos e fatores definidos pelo usuário.
- RFANALISARCOMBINACAO: soma vetorialmente cargas distribuídas e pontuais de cada caso, aplica os fatores informados e executa o UnifiedBeamSolver.
- Resultados são armazenados por combinação em ROFAMA_COMB_RESULT_<NOME>.

Exemplo conceitual:
COMB1 = 1.0*PERMANENTE + 1.0*SOBRECARGA

Importante:
Os fatores são configuráveis e não são apresentados como coeficientes normativos automáticos. A futura camada normativa deverá ser baseada em critérios/edições de normas previamente verificados.

Próxima fase:
- listar/editar/excluir combinações;
- envelopes máximos/mínimos entre combinações;
- diagramas por combinação e envelope;
- usar ROFAMA_STRUCT_ID em todas as tabelas.
