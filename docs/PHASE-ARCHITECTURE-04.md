# Fase Arquitetura 04

Implementado:
- serviço central para consultar pavimento ativo;
- metadado ROFAMA_STOREY com ID, nome, elevação e pé-direito;
- comando RFVINCULARPAVIMENTO para associar objetos existentes;
- RFESCADA com cálculo geométrico preliminar de espelhos/pisos;
- RFPLATIBANDA com altura e espessura registradas.

Importante:
A escada é uma ferramenta de pré-dimensionamento geométrico. A conformidade final depende do uso da edificação, acessibilidade, incêndio, código local e normas aplicáveis.

Próximo:
- fazer novos elementos herdarem automaticamente o pavimento ativo;
- usar elevação/pé-direito na geração 3D;
- gerar platibanda 3D;
- telhado quatro águas;
- preparar solução/build e pacote de teste.
