# Estrutural — Fase 16: validação numérica inicial

Implementado:
- RFTESTARSOLVER: executa benchmarks automáticos de 1, 2 e 3 vãos, verificando equilíbrio global e simetria.
- RFAUDITARMATRIZ: audita no desenho a igualdade entre carga total armazenada e soma das reações calculadas pelo solver V2, com tolerância explícita.

Estes testes são necessários, mas não suficientes para validar um software de cálculo estrutural. Ainda faltam benchmarks independentes de momentos de extremidade, rotações, casos assimétricos, vãos desiguais, cargas distintas por trecho, balanços, liberações e comparação com referência externa confiável.

Nenhum resultado deve ser tratado como dimensionamento normativo apenas por passar nesta bateria.
