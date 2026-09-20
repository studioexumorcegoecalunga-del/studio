# Estrutural — Fase 18: cargas independentes por vão

Implementado:
- RFCARGASVAOS: permite informar carga distribuída individual para cada vão real gravado.
- RFMATRIZVIGA3: usa diretamente o vetor de comprimentos reais e o vetor de cargas por vão.
- RFAUDITARVAOS: verifica se a quantidade de cargas coincide com a quantidade de vãos antes da análise.

A entrada de carga é explícita e rastreável. Nesta fase ainda não foram implementados balanços, liberações de momento, apoios elásticos ou cargas concentradas.

Próxima fase: condições de contorno por nó e extensão do solver para translações/rotações com barras em balanço e liberações.
