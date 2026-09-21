# Estrutural — Fase 20: benchmarks do solver 2-GDL

Implementado:
- RFTESTARFRAME: bateria com viga simplesmente apoiada sob UDL, balanço, viga bi-engastada e viga contínua simétrica de dois vãos.
- RFAUDITARFRAME: verifica no DWG o equilíbrio vertical entre cargas por vão e reações do solver V4.

Os três primeiros casos usam resultados clássicos fechados para reações e momentos de apoio. O caso contínuo verifica equilíbrio e simetria.

Ainda é necessário validar deslocamentos/rotações e convenções de sinais com referência independente, além de adicionar carga concentrada, liberações internas e testes de instabilidade.
