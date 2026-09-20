# Plano de teste — AutoCAD 2026

## Instalação
1. Compilar com build.ps1.
2. Copiar ROFAMACAD.bundle para uma pasta ApplicationPlugins reconhecida pelo AutoCAD.
3. Abrir AutoCAD 2026 e confirmar carregamento.
4. Executar RFSOBRE.

## Testes mínimos
- desenhar paredes ortogonais e inclinadas;
- espessuras 0,15 / 0,20 / 0,25 m;
- inserir portas e janelas em diferentes rotações;
- gerar áreas;
- criar cotas;
- gerar e atualizar 3D;
- salvar, fechar e reabrir DWG;
- verificar persistência dos metadados;
- testar UNDO/REDO;
- testar desenho com UCS alterado.

## Critério de V1.0
Nenhuma exceção não tratada nos comandos principais e validação em projetos reais antes de marcar release estável.
