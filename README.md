# 🚀 Missão Bios

Missão Bios é um jogo educativo em 2D (pixel art, visão top-down) onde o jogador assume o papel de um aprendiz em uma estação científica no planeta fictício Vitta.

Durante uma missão de pesquisa, amostras coletadas do planeta contaminam a estação com micro-organismos desconhecidos, afetando os cientistas e comprometendo o funcionamento dos sistemas. O jogador deve investigar o problema e restaurar a base por meio da exploração e da resolução de desafios científicos.

---

## 🎯 Objetivo do Projeto

O objetivo é desenvolver uma experiência interativa que estimule o aprendizado de ciências por meio da experimentação, incentivando o pensamento científico em crianças.

O jogo busca ensinar conceitos do ensino fundamental de forma prática e contextualizada, evitando a memorização e promovendo a aprendizagem ativa.

---

## 🧠 Proposta Educacional

O jogo é baseado em uma abordagem inspirada no método científico, onde o jogador:

- observa situações e problemas
- formula hipóteses
- testa soluções
- aprende com os erros

O aprendizado acontece durante a jogabilidade, com feedbacks claros e explicações simples fornecidas pelos personagens do jogo.

---

## 🎮 Gameplay (Resumo)

O jogador explora uma estação espacial e resolve três desafios principais:

- 🔬 **Laboratório:** análise de micro-organismos presentes em amostras do planeta  
- 🏥 **Enfermaria:** identificação de problemas em pacientes e criação de soluções  
- 💧 **Sistema de Água:** purificação de água contaminada utilizando métodos científicos  

As decisões do jogador influenciam o progresso da estação, representado por uma barra de restauração dos sistemas.

---

## ⭐ Diferencial

- Aprendizagem baseada em experimentação (não em perguntas e respostas)
- Uso de erros como parte do processo de aprendizado
- Integração entre narrativa, gameplay e conteúdo educativo
- Desafios que variam a cada tentativa, incentivando compreensão em vez de memorização

---

## 👥 Público-Alvo

- Crianças entre 7 e 11 anos  
- Estudantes do ensino fundamental  
- Jogadores casuais 

---

## 💻 Plataforma

- Web (navegador)


---

## 🛠️ Como rodar localmente

### Pré-requisitos

- [Godot 4.6 **.NET**](https://godotengine.org/download) (a versão com suporte a C#, não a padrão)
- [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0)

### Passo a passo

1. Clone o repositório e abra a pasta no Godot (`Importar` → selecione o `project.godot`).
2. Clique em **Build** (ícone de martelo, canto superior direito) pra compilar os scripts C#.
3. Aperte **F5** pra jogar. A cena inicial é o menu principal.

### Controles

| Ação | Teclas |
|---|---|
| Mover | Setas ou `W` `A` `S` `D` |
| Interagir | `E` |
| Pausar / voltar | `Esc` |

---

## 📁 Estrutura do projeto

```
missao-bios/
├── scenes/      # cenas do Godot (menu, estação, jogador, NPCs e interfaces)
├── scripts/     # código C# (Managers, Player, NPC, Interaction, Puzzle, UI, Audio, Save)
├── resources/   # recursos do Godot (animações dos personagens)
├── data/        # dados do jogo (ex.: microrganismosDB.json)
└── docs/        # GDD e validação de demanda
```

Os gerenciadores em `scripts/Managers` são autoloads (singletons) registrados no `project.godot`.

---

## 🔀 Fluxo de trabalho

- `main`: versão estável
- `dev`: integração do que está em desenvolvimento
- `feature/*`: uma branch por tarefa, aberta a partir da `dev`
- As Pull Requests entram na `dev`
