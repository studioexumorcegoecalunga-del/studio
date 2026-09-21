# Estrutural — Fase 40: comparação operacional V1 x V2

Implementado:
- RFCONFIGCOMPARARSOLVER: tolerâncias configuráveis para deslocamento, reação e esforço.
- RFCOMPARARSOLVERS: seleciona uma viga e combinação, lê o resultado legado V1, reconstrói cargas, executa o Solver V2, persiste o resultado V2 versionado e compara os vetores.
- O relatório informa maior diferença absoluta e índice governante para u, R e esforços.
- O resultado V2 fica separado em ROFAMA_COMB_RESULT_V2_<COMBINACAO>, preservando o resultado V1.

Estado:
V2 permanece em validação. O comando permite medir divergências em desenhos reais sem promover automaticamente o novo solver.

Próxima fase:
- comparação em lote de todas as vigas/combinações;
- relatório consolidado aprovado/reprovado;
- investigar diferenças por sinal, liberação e carga pontual;
- somente após isso promover V2.
