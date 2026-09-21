# Estrutural — Fase 36: núcleo rigoroso de liberações

Implementado:
- ReleasedBeamElement: condensação estática em bloco para uma ou duas liberações rotacionais simultâneas.
- Preserva K/f completos do elemento.
- Recover reconstrói as rotações internas liberadas por uh = Khh^-1 (fh - Khr ur).
- EndForce recupera esforços com a matriz completa: q = K_full u_full - f_full.
- DisconnectedDofService: detecta DOFs globais com linha de rigidez nula; DOF sem rigidez e sem carga pode ser removido, enquanto DOF sem rigidez com carga é sinalizado como mecanismo.
- RFTESTARLIBERACOES2: benchmarks locais para momentos liberados, reações de viga biapoiada e fixed-end moments.

Importante:
Esta fase cria o núcleo matemático correto, mas ainda não substitui automaticamente o UnifiedBeamSolver. A integração global deve ser feita na próxima fase, porque exige mapear os DOFs retidos de cada elemento para o sistema global e aplicar a redução de DOFs desconectados sem alterar os apoios.

Antes de dimensionamento:
- integrar o novo elemento ao solver global;
- benchmark de viga contínua com liberação interna;
- testar mecanismos;
- comparar com soluções independentes;
- compilar/testar no AutoCAD 2026.
