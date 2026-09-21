# Estrutural — Fase 41: comparação V1 x V2 em lote

Implementado:
- RFCOMPARARSOLVERSEMLOTE: percorre todas as vigas RF-EST-VIGA e todos os resultados de combinação V1 existentes.
- Reconstrói as cargas de cada combinação e executa o Solver V2.
- Persiste cada resultado V2 separadamente.
- Aplica as tolerâncias configuradas por RFCONFIGCOMPARARSOLVER.
- Exibe totais de comparações dentro/fora da tolerância e até 10 maiores divergências.
- SolverDifferenceClassifier: infraestrutura para classificar divergências por dimensão de resultado, liberações, cargas pontuais, deslocamento ou equilíbrio/sinal.

Importante:
O comando precisa ser executado em um DWG real que já possua vigas, combinações e resultados V1. Nesta sessão não há AutoCAD 2026 em execução, portanto a fase implementa a execução dentro do plugin, não afirma que os benchmarks passaram.

Próxima fase:
- persistir relatório consolidado no DWG;
- aplicar classificador em lote;
- comando para listar apenas divergências;
- preparar checklist de promoção do V2.
