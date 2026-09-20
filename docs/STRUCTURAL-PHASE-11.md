# Estrutural — Fase 11: preparação do motor de análise

Implementado:
- RFCOMBINACOES: estrutura os componentes G/Q e cria registros separados de serviço/projeto, deliberadamente sem coeficientes normativos nesta fase.
- RFVIGAANALISE: análise estática preliminar de vigas como biapoiadas sob carga uniformemente distribuída, armazenando w, Mmax e Vmax.
- RFANALISERELATORIO: resumo dos máximos esforços obtidos.

A edição ABNT NBR 6118:2026 foi confirmada no catálogo oficial da ABNT antes desta fase. Nenhum coeficiente ou regra protegida da norma foi copiado para o código.

Antes de transformar RFCOMBINACOES em ELU/ELS normativo, os coeficientes, categorias de ações, simultaneidade e combinações devem ser parametrizados e validados a partir das normas aplicáveis e da documentação licenciada do projeto.
