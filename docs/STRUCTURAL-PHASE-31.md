# Estrutural — Fase 31: catálogo e envelopes de combinações

Implementado:
- RFLISTARCOMBINACOES: lista as combinações persistidas e seus fatores.
- RFEXCLUIRCOMBINACAO: remove uma combinação do catálogo.
- RFENVELOPE: percorre resultados de todas as combinações existentes em cada viga e grava máximos/mínimos de deslocamento vertical nodal, reação vertical e momento de extremidade.
- RFRELATORIOENVELOPE: apresenta os envelopes usando ROFAMA_STRUCT_ID quando disponível.

Limitação importante:
O envelope desta fase usa valores nodais e esforços de extremidade já persistidos. O envelope contínuo de V(x), M(x) e v(x) ao longo dos vãos requer reconstruir cada combinação com suas cargas e amostrar posições comuns; isso será a próxima evolução.

Próxima fase:
- envelope contínuo por posição;
- diagramas gráficos superior/inferior;
- identificar qual combinação governa cada máximo/mínimo;
- tabela de envelope.
