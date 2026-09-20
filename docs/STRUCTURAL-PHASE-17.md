# Estrutural — Fase 17: benchmarks ampliados e relatório matricial

Implementado:
- RFTESTARSOLVER2: casos adicionais com vãos desiguais e carregamentos diferentes por trecho, além de verificações de equilíbrio e sanidade numérica.
- RFRESULTADOSMATRIZ: consolida máximos absolutos de rotações, reações e momentos de extremidade gravados pelo solver V2.

Limite atual:
Os benchmarks internos verificam propriedades matemáticas básicas, mas não constituem validação independente. A próxima etapa deve comparar casos selecionados com soluções analíticas/publicadas ou com outro solver confiável e documentar as diferenças.

Antes de armaduras, o motor ainda precisa suportar corretamente condições de contorno, balanços/liberações e carga por trecho sem depender de uma carga média da viga inteira.
