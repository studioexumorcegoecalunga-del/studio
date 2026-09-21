# Estrutural — Fase 28: integração de escalas, IDs e caso de carga

Implementado:
- StructuralIdService: leitura centralizada do ROFAMA_STRUCT_ID para relatórios/tabelas.
- RFCASOCARGA: define o caso de carga ativo no projeto (identificação/rastreabilidade).
- RFDIAGRAMASV8: usa diretamente as escalas persistidas por RFCONFIGDIAGRAMAS e registra o caso ativo que originou a última geração dos diagramas.

Importante:
- O caso de carga nesta fase é uma identificação de contexto. As cargas ainda usam os registros atuais da viga; não existe ainda banco independente de cargas por caso.
- Próxima etapa deve versionar ROFAMA_SPAN_LOADS e ROFAMA_POINT_LOADS por caso, e depois criar combinações entre casos.
- StructuralIdService está pronto para substituir IDs sequenciais em tabelas e relatórios.
