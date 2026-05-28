# Game Design Document (GDD)

# Missão Bios

**Aluno:** Andressa Rodrigues Lopes  
**E-mail:** an.lopes@catolicasc.edu.br  

**Status do Projeto:**  
Pesquisa

**Versão do Documento:** v0.2  
**Última atualização:** 24/05/2026

---

# 1. Visão Geral

## Elevated Pitch

Missão Bios é um jogo educativo em 2D, com visual pixel art e perspectiva top-down, onde o jogador assume o papel de um aprendiz em uma estação científica no planeta fictício Vitta.
Durante a missão, a estação é contaminada por micro-organismos desconhecidos, afetando os cientistas e comprometendo os sistemas da base. O jogador deve investigar o problema e restaurar a estação por meio da exploração e da resolução de desafios baseados em conceitos de ciências do ensino fundamental.
O diferencial do jogo está na aprendizagem baseada na experimentação, inspirada no método científico, incentivando o jogador a observar, testar hipóteses, aprender com erros e construir conhecimento de forma ativa.

---

## Gênero

- **Educativo:** O jogo tem como objetivo principal promover a aprendizagem de conceitos científicos de forma interativa, utilizando situações práticas e contextualizadas dentro da narrativa.
- **Puzzle:** A progressão do jogo ocorre por meio da resolução de desafios que exigem raciocínio lógico, interpretação de informações e tomada de decisão baseada em observação e experimentação.
- **Aventura:** O jogador explora diferentes áreas da estação espacial, interage com personagens e descobre informações que contribuem para o avanço da história e resolução dos problemas.

---

## Público-Alvo

- **Crianças entre 7 e 11 anos:** Faixa etária correspondente ao ensino fundamental inicial, onde há introdução a conceitos básicos de ciências.
- **Estudantes do ensino fundamental:** O jogo pode ser utilizado como ferramenta complementar ao ensino escolar, reforçando conteúdos de forma prática e interativa.
- **Jogadores casuais:** O jogo é projetado para ser acessível, com mecânicas simples e intuitivas, permitindo que qualquer jogador compreenda e jogue sem necessidade de experiência prévia.

---

## Plataformas

- **Web:** A escolha da plataforma web visa facilitar o acesso ao jogo, permitindo sua utilização em diferentes dispositivos com navegador, especialmente em ambientes educacionais como escolas, sem necessidade de instalação.

---

# 2. Acesso ao Projeto

| Item | Link |
|-----|-----|
| Build jogável | Link do Itch.io a ser gerado na fase de produção (Próximo Semestre). |
| Repositório | [GitHub](https://github.com/AndressaLp/missao-bios) |
| Instruções de execução | Execução direta via Navegador Web através do Itch.io. Sem necessidade de instalação. |

---

# 3. Pesquisa e Referências

## Jogos de Referência

### Among Us:
<img width="100%" height="auto" alt="referência do jogo Among Us" src="./assets/amongus.png" />

- Descrição:

Among Us é um jogo multiplayer em visão top-down onde jogadores realizam tarefas dentro de uma nave espacial enquanto tentam identificar impostores.

-	Aspectos que inspiraram o projeto:
    -	Perspectiva top-down simples e clara
    -	Interação com objetos do cenário (tarefas)
    -	Interface minimalista e intuitiva
    -	Organização do mapa em salas com funções específicas

### Stardew Valley:
<img width="100%" height="auto" alt="referência do jogo Stardew Valley" src="./assets/StardewValley.png"/>

- Descrição:

Stardew Valley é um jogo de simulação e exploração em pixel art, onde o jogador interage com personagens, gerencia recursos e realiza diversas atividades.

- Aspectos que inspiraram o projeto:
    - Estilo visual em pixel art
    - Sistema de interação com NPCs
    - Exploração livre do ambiente
    - Interface simples com inventário acessível

### Undertale:
<img width="100%" height="auto" alt="referência do jogo Undertale" src="./assets/undertale.png"/>

- Descrição:

Undertale é um RPG independente focado em narrativa e interação com personagens, com forte uso de diálogos e escolhas.

- Aspectos que inspiraram o projeto:
    - Sistema de diálogos com personagens (NPCs)
    - Uso de texto para guiar o jogador
    - Conexão entre narrativa e mecânicas
    - Feedback ao jogador durante as interações

---

## Análise das Referências

Os jogos analisados apresentam características importantes que influenciam diretamente o desenvolvimento de Missão Bios.

Todos eles possuem interfaces simples e acessíveis, o que facilita a compreensão do jogador, especialmente importante para o público infantil. Além disso, utilizam interações diretas com o ambiente, permitindo que o jogador aprenda fazendo, em vez de apenas recebendo informações.

Outro ponto em comum é a organização do jogo em espaços bem definidos, como salas ou áreas específicas, o que ajuda na navegação e estruturação dos objetivos, algo que será aplicado na divisão da estação em laboratório, enfermaria e outras salas.

As principais ideias que inspiraram o projeto foram:
- Uso de perspectiva top-down para facilitar a visualização
- Exploração guiada, mas com certa liberdade para o jogador
- Interação com NPCs para ensinar e orientar
- Interface simples e intuitiva, adequada para crianças
- Integração entre narrativa e gameplay, tornando o aprendizado mais natural

Essas referências contribuíram para a construção de uma experiência que combina exploração, narrativa e resolução de problemas, alinhada com a proposta educativa do jogo.

---

# 4. Hipóteses de Design

| Hipótese | Como será testada |
|-----|-----|
| Jogadores aprendem melhor ao experimentar em vez de responder perguntas diretas | Playtests observando se o jogador consegue resolver puzzles sem instruções explícitas |
| Feedback com dicas (em vez de “erro”) reduz frustração | Testar respostas dos jogadores ao errar e analisar se continuam tentando |
| Interface simples facilita o entendimento para crianças | Testes com usuários verificando se conseguem jogar sem ajuda |
| Divisão do jogo em salas (laboratório, enfermaria, etc.) melhora a organização | Observar se o jogador entende facilmente para onde deve ir |
| Pequenas variações nos puzzles aumentam rejogabilidade | Testar múltiplas partidas e verificar se o jogador continua engajado |

## Pilares do jogo

- Aprender Jogando: O jogador aprende conceitos científicos por meio da experimentação e resolução de problemas.
- Simplicidade e Clareza: Interface, controles e objetivos fáceis de entender, especialmente para crianças.
- Exploração Guiada: O jogador tem liberdade para explorar, mas sempre com objetivos claros.
- Feedback Positivo: Erros não punem diretamente, mas ensinam e incentivam novas tentativas.

---

# 5. Gameplay

## Core Loop

Explorar a estação → Interagir com NPCs → Resolver desafios científicos → Receber feedback → Restaurar sistemas → Avançar para o próximo desafio

<div align="center">
  <img width="60%" heigth="auto" alt="Core Loop principal" src="./assets/coreloop.png"/>
</div>

## Loops Secundários

- Conversar com NPC → receber explicação → aplicar no puzzle
- Testar solução → errar → receber dica → tentar novamente
- Coletar item → usar no puzzle → entender função do item

<div align="center">
  <img width="60%" heigth="auto" alt="Loops secundários" src="./assets/loopsecundario.png"/>
</div>

---

## Mecânicas Principais

| Mecânica | Descrição |
|-----|-----|
| Movimento | O jogador se move utilizando teclado (WASD ou setas) em um ambiente 2D top-down |
| Interação | Interage com NPCs e objetos usando tecla ou mouse |
| Diálogo | Sistema de caixas de texto que guiam o jogador |
| Coleta de itens | Coleta e utiliza itens simples no inventário |
| Resolução de puzzles | Identificação, associação e experimentação para resolver desafios |

## Camera

- Tipo: Top-down (visão de cima)
- Comportamento: Fixa acompanhando o jogador
- Objetivo: Facilitar visualização e navegação

---

## Sistemas

**Vitória**

O jogador vence ao:
- Resolver os 3 desafios principais
- Restaurar completamente os sistemas da estação
- Finalizar a missão com sucesso

**Derrota**

Não há derrota tradicional.

O jogador:
- Erra → recebe explicação
- Após várias tentativas → puzzle reinicia
- A barra de progresso diminui

**Progressão**

A progressão é baseada em:
- Conclusão de desafios
- Restauração gradual da estação (barra de progresso)
- Avanço entre salas (laboratório → enfermaria → água)

Cada puzzle concluído:
- Aumenta a barra de progresso
- Desbloqueia o próximo desafio
- Avança na história

---

# 6. Escopo do Projeto

## O jogo inclui

- 1 mapa principal (estação espacial dividida em salas)
- 3 desafios principais (laboratório, enfermaria e purificação da água)
- 3 NPCs principais com diálogos e orientações
- Sistema de movimentação do personagem (top-down)
- Sistema de interação com objetos e NPCs
- Sistema de diálogo com feedback educativo
- Sistema de objetivos (missões)
- Sistema de inventário simples (até 3 itens)
- Sistema de progressão por barra (restauração da estação)
- Variação simples nos puzzles para rejogabilidade
- Cutscene inicial e final (simples, com imagens/texto)

## O jogo não inclui

- Multiplayer
- Sistema de combate
- Sistema complexo de crafting
- Mundo aberto amplo
- IA avançada
- Sistema de níveis/upgrades do personagem
- Geração procedural complexa
- Backend online (ranking/login online)

---

# 7. Prototipagem

Planejamento dos protótipos que serão desenvolvidos:

| Protótipo | Objetivo | Resultado esperado |
|-----|-----|-----|
| Movimento do personagem | Validar controles (WASD/setas) | Jogador se move com facilidade |
| Interação com objetos | Testar interação com tecla/mouse | Interações claras e intuitivas |
| Sistema de diálogo | Validar leitura e avanço de texto | Jogador entende instruções |
| Puzzle simples | Testar lógica de tentativa e erro | Jogador aprende com feedback |
| Sistema de feedback | Testar dicas após erro | Redução de frustração |

---

# 8. Interface (UI/UX)

## HUD

- Nome do jogador
- Objetivos atuais (missões)
- Barra de inventário (até 3 itens)
- Botão/menu de pausa
- Botão de ajuda (“como jogar”)

<div style="display:flex;flex-direction:column;" align="center">
  <img width="60%" heigth="auto" alt="Protótipo de baixa fidelidade do HUD" src="./assets/hud.png"/>
  <p>Protótipo de baixa fidelidade da interface de usuário (HUD).</p>
</div>

---

## Menus

- menu principal
    - Novo jogo
    - Continuar jogo
    - Selecionar jogador
    - Como jogar
    - Opção de som (ligar/desligar)

<div style="display:flex;flex-direction:column;" align="center">
  <img width="60%" heigth="auto" alt="Protótipo de baixa fidelidade do menu principal" src="./assets/menuprincipal.png"/>
  <p>Protótipo de baixa fidelidade da interface de usuário (menu principal).</p>
</div>

- Menu de pausa
    - Continuar
    - Voltar ao menu
    - Como jogar

<div style="display:flex;flex-direction:column;" align="center">
  <img width="60%" heigth="auto" alt="Protótipo de baixa fidelidade do menu de pausa" src="./assets/menupausa.png"/>
  <p>Protótipo de baixa fidelidade da interface de usuário (menu de pausa).</p>
</div>

## Flow de menus

<div align="center">
  <img width="60%" heigth="auto" alt="Flow dos menus" src="./assets/flowmenu.png"/>
</div>

## Controles

<div style="display:flex;flex-direction:column;" align="center">
  <img width="60%" heigth="auto" alt="Protótipo de baixa fidelidade dos controles do jogo" src="./assets/controles.png"/>
  <p>Protótipo de baixa fidelidade dos controles do jogo.</p>
</div>
