# Estrutural — Fase 29: cargas independentes por caso

Implementado:
- LoadCaseDataService: persiste cargas distribuídas e pontuais separadamente para cada caso ativo.
- RFCARGASVAOSCASO: grava q por vão dentro do caso selecionado por RFCASOCARGA.
- RFCARGAPONTUALCASO: adiciona cargas P por vão/posição dentro do caso ativo.
- RFMATRIZVIGA8: executa o UnifiedBeamSolver usando exclusivamente as cargas do caso ativo e grava resultado independente em ROFAMA_MATRIX_CASE_<CASO>.

Consequência:
Casos como PERMANENTE, SOBRECARGA, VENTO_X etc. podem coexistir sem sobrescrever os dados uns dos outros.

Ainda não há coeficientes normativos automáticos. A próxima fase deve criar combinações como composição matemática configurável de casos, mantendo fatores informados pelo usuário/projeto até a camada normativa ser validada.
