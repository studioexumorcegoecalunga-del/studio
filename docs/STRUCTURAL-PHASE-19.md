# Estrutural — Fase 19: condições de contorno e 2 GDL por nó

Implementado:
- FrameBeamSolver: elemento de viga Euler-Bernoulli com deslocamento vertical e rotação em cada nó, matriz 4x4 por barra, montagem global, cargas nodais equivalentes e recuperação de forças de extremidade.
- RFAPOIOSVIGA: define cada nó como Simples, Engaste ou Livre.
- RFMATRIZVIGA4: executa o novo solver usando vãos reais, cargas por vão, E do projeto e condições de contorno.

Esta fase passa a permitir modelar extremidades livres e, portanto, configurações de balanço quando a topologia e os apoios forem definidos adequadamente.

Ainda faltam liberações internas de momento, cargas concentradas, recalques, molas, Timoshenko/cisalhamento e benchmarks independentes específicos do solver 2-GDL.
