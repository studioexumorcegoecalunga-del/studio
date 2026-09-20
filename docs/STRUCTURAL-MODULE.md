# Módulo Estrutural — ROFAMA CAD PRO

## Objetivo
Transformar a geometria arquitetônica em um lançamento estrutural preliminar e editável.

## Primeira implementação
- RFESTRUTURA: analisa paredes selecionadas, estima seus eixos, detecta extremidades/interseções e cria candidatos a pilares e eixos de vigas.
- RFLAJEEST: cadastra contorno estrutural de laje.
- Layers RF-EST-PILAR, RF-EST-VIGA e RF-EST-LAJE.
- Metadado ROFAMA_STRUCT identifica elementos como PRELIMINAR_NAO_DIMENSIONADO.
- Integração com pavimento ativo.

## Regras de segurança de engenharia
O lançamento automático não equivale a projeto estrutural dimensionado. Seções, armaduras, cargas, fundações, estabilidade global, flechas, punção, cisalhamento e combinações devem ser calculados e verificados antes da emissão.

## Base normativa a acompanhar
- ABNT NBR 6118:2026 — projeto de estruturas de concreto.
- ABNT NBR 6120 — ações para o cálculo de estruturas de edificações.
- ABNT NBR 8681 — ações e segurança nas estruturas.
- ABNT NBR 6122 — projeto e execução de fundações.
- Demais normas conforme material, uso, vento, incêndio, fundações e sistema construtivo.

Não são incorporados textos protegidos das normas ao repositório. Os critérios numéricos serão implementados apenas quando devidamente verificados e parametrizados.
