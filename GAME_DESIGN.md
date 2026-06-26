# BULLET CRASH — Game Design & Systems Reference

> **Single source of truth** para todas as decisões de design e arquitetura.
> Regras de *processo* (como o Claude trabalha) ficam no [CLAUDE.md](CLAUDE.md).

## Project overview
- Game name: BULLET CRASH
- Genre: .io Arena Shooter com árvore de evolução (inspirado em diep.io e archer.io)
- Engine: Unity 2D
- Art style: Pixel art, dark fantasy dungeon
- Developer: Solo dev
- Target platform: **WebGL** (browser-first — otimização é prioridade em toda decisão técnica)
- Session length: Sem tempo definido — o player joga até morrer

---

## Core game loop
1. Player entra na arena e começa do nível 1, sem qualquer upgrade
2. Arena contém **objetos destrutíveis** (caixas, cristais, etc.) e **bots** espalhados pelo mapa
3. Player destrói objetos e mata bots para acumular **XP**
4. Ao acumular XP suficiente, o player sobe de nível e ganha **pontos de evolução**
5. Player aloca pontos na **Árvore de Evolução** (stats ou habilidades de build)
6. Dificuldade escala com o nível do player — bots ficam mais agressivos e numerosos
7. Ao morrer, a sessão termina e o player recomeça do nível 1 (**sem meta-progressão**)

---

## Player
- Movimento: top-down, 8-direcional (WASD / analog stick)
- Disparo: **manual** — botão esquerdo do mouse mantido pressionado
- Mira: o player **aponta com o mouse** — projéteis seguem a direção do cursor
- Stats base: HP, velocidade, dano, cadência de tiro, range XP
- HP bar visível no HUD o tempo todo
- Morte termina a sessão instantaneamente — tela de Game Over, sem respawn

---

## Arena (mapa)
- Mapa **finito e fechado** — paredes sólidas nas bordas, sem scroll infinito
- Tamanho fixo por fase (ex.: 40×40 unidades)
- Objetos destrutíveis distribuídos pelo mapa no spawn (caixas, barricas, cristais)
- Bots spawnados em posições aleatórias fora da visão do player
- Obstáculos estáticos opcionais para criar chokepoints e cover
- **Sem câmera com zoom automático** — câmera segue o player com offset fixo

---

## Objetos destrutíveis
- Existem no mapa desde o início; novos spawnados periodicamente para manter densidade
- Têm HP fixo baixo (1–3 hits)
- Ao destruir: dropam XP orbs, ocasionalmente dropam power-ups temporários
- Não perseguem o player — são alvos passivos
- Variedade visual futura: caixa de madeira, barrica, cristal, urna

---

## Bots (inimigos controlados por IA)
- Inicialmente substituem jogadores reais — IA simples suficiente para WebGL
- AI padrão: detectar player/outros bots no range → mover em direção → atacar
- Bots também sobem de nível e têm builds aleatórias (futuro: configurable por dificuldade)
- Ao morrer: dropam XP orbs proporcionais ao nível deles
- Respawn em posição aleatória após cooldown (manter densidade de bots na arena)
- **Sem pathfinding complexo** — Vector2.MoveTowards com desvio simples de obstáculos

---

## XP e nivelamento
- XP é obtido destruindo objetos e matando bots
- XP orbs ficam no chão e são atraídos ao player quando dentro do range (upgradável)
- Barra de XP visível no HUD (sem labels, barra colorida)
- Ao encher a barra: **pausa o jogo** e abre a tela de evolução
- A quantidade de XP necessária cresce por nível (curva exponencial suave)
- Não há limite de nível por sessão

---

## Sistema de evolução (árvore)

### Estrutura geral
Ao subir de nível o player ganha **1 ponto de evolução** para alocar livremente na árvore.
A árvore tem dois eixos:

#### Eixo 1 — Stats base (sempre disponíveis, empilháveis)
Cada nó pode ser comprado múltiplas vezes até um cap por sessão.

| Stat | Efeito por ponto | Cap por sessão |
|---|---|---|
| Max HP | +10 HP | 10× |
| Velocidade | +8% velocidade | 8× |
| Dano | +10% dano de projétil | 10× |
| Cadência | +8% fire rate | 10× |
| Range XP | +20% raio de atração | 6× |
| Penetração | projétil atravessa +1 inimigo | 3× |

#### Eixo 2 — Build paths (upgrades de arma e habilidade)
Caminhos que mudam o comportamento do projétil ou adicionam habilidades. Cada caminho tem 3 níveis.
Caminhos são **exclusivos entre si por slot** — o player escolhe 1 de N no slot ao desbloquear.

| Caminho | Nível 1 | Nível 2 | Nível 3 |
|---|---|---|---|
| Duplo Cano | 2 projéteis simultâneos | 3 projéteis (spread) | 4 projéteis (leque) |
| Sniper | projétil 2× mais rápido | penetra todos inimigos | ricochete em parede |
| Explosivo | projétil explode ao impacto (AoE) | AoE maior | deixa área de fogo |
| Escudo | escudo frontal absorve 1 hit | regenera o escudo | escudo rebate projéteis |
| Vampiro | kills restauram 2 HP | kills restauram 5 HP | 10% lifesteal do dano |

> Lista inicial — expandir conforme implementação.

### Regras da árvore
- Stats base não têm pré-requisito — sempre disponíveis
- Build paths exigem nível mínimo para desbloquear (ex.: Duplo Cano unlock no nível 3)
- Só 1 Build path ativo por slot (implementar no mínimo 2 slots de build futuramente)
- A tela de evolução mostra a árvore completa com nós disponíveis destacados
- Player não pode desfazer pontos gastos

---

## Armas
- Arma base: projétil único, cadência média, dano médio
- Sem seleção manual de arma — a build path modifica o comportamento da arma base
- **Disparo manual:** botão esquerdo do mouse mantido pressionado dispara continuamente respeitando o Reload
- **Mira pelo mouse:** projétil parte do player em direção ao cursor no momento do disparo
- **Object Pool obrigatório** para projéteis — nunca Instantiate/Destroy em hot path

---

## Escalamento de dificuldade
- A cada N níveis do player, bots ganham +X% HP e dano
- Novos tipos de bots com comportamentos diferentes desbloqueiam em thresholds de nível
- Taxa de respawn de bots aumenta gradualmente
- Objetos destrutíveis continuam spawnando na mesma taxa (manter fonte passiva de XP)

---

## UI screens
| Tela | Status |
|---|---|
| Main Menu | Pendente |
| Arena HUD | Pendente |
| Tela de Evolução (árvore) | Pendente |
| Game Over | Pendente |
| Options | Pendente |
| Loading Screen | Pendente |

---

## HUD layout (in-game)
- **Topo:** barra fina com HP (esquerda), nível atual (centro), kill count (direita)
- **Fundo:** barra de XP full-width, 8px, sem labels
- **Centro da tela:** limpo — só gameplay
- Sem ícones de arma, sem item slots visíveis
- Paleta e tipografia mantidas da versão anterior (ver seções abaixo)

---

## UI color palette
- Background dark: `#0A0A12`
- Panel: `#1E1230`
- Border default: `#4A2E7A`
- Border selected / primary: `#F0C040`
- Border danger: `#6E1E1E`
- HP bar fill: `#CC2222`
- XP bar fill: `#8866CC`
- Text primary: `#E8E8F0`
- Text golden: `#F0C040`
- Text muted: `#6A5A8A`
- Overlay (evolução): `#0A0A12` 80% opacity
- Overlay (game over): `#0A0A12` 90% opacity

---

## UI typography
- Font: Press Start 2P (pixel font) para todo UI do jogo
- Sem border-radius — pixel art usa bordas 90°
- Bordas pixel: 2px default, 4px para painéis e botões primários

---

## Otimização WebGL — regras obrigatórias
> WebGL é a plataforma alvo. Toda decisão técnica deve considerar o impacto no bundle size e na performance no browser.

- **Object Pool obrigatório** para qualquer objeto que spawna em volume: projéteis, XP orbs, bots, VFX
- **Sem pathfinding pesado** — IA de bots usa Vector2.MoveTowards + steering behavior simples
- **Sem física complexa** — Rigidbody2D Kinematic para projéteis; colisão por trigger
- **Sem Instantiate/Destroy em hot paths** — sempre pool
- **Draw calls:** sprites em atlas (SpriteAtlas), materiais compartilhados, câmera com culling ativo
- **Audio:** AudioMixer com grupos; clips curtos em memória, músicas em streaming
- **GC:** evitar allocations em Update (sem LINQ, sem `new` em hot path, strings → StringBuilder se necessário)
- **Build:** IL2CPP, strip engine code, compressão Brotli para WebGL
- **ScriptableObjects** para dados estáticos — nunca duplicar dados em runtime

---

## Architecture notes
- **Player movement (implementado):** `PlayerInputReader` (WASD/analog) → `PlayerStats` → `PlayerMovement` (FixedUpdate, Rigidbody2D). Desacoplado por interface `IMovementInput`.
- **Stats system (implementado):** `Stat` + `StatModifier` (Flat/PercentAdd/PercentMult). Backbone dos upgrades da árvore. Fórmula: `(base + ΣFlat) × (1 + ΣPercentAdd) × Π(1 + PercentMult)`. Reutilizável.
- **CharacterData:** ScriptableObject com stats base — nunca alterado em runtime.
- **EvolutionTree:** ScriptableObject por nó da árvore (tipo, custo em pontos, efeito, pré-requisitos). `EvolutionManager` gerencia pontos disponíveis e aplica `StatModifier` ou ativa `BuildPath`.
- **BuildPath:** Strategy pattern via ScriptableObject (herda `WeaponBehavior`) — cada caminho de build é uma implementação de `WeaponBehavior`. Compatível com a base existente.
- **Bot AI:** `BotController` com estado simples (Idle → Chase → Attack). Sem NavMesh — posição calculada por `Vector2.MoveTowards` com separação de steering entre bots.
- **XP orbs:** Pool de GameObjects, atraídos via Lerp no Update quando dentro do range.
- **Spawner:** `ArenaSpawner` lê `ArenaConfig` (ScriptableObject) com densidade de bots e objetos por nível. Spawna fora do campo de visão do player.
- **Sem meta-progressão por design** — nenhum dado persiste entre sessões. PlayerPrefs apenas para configurações (volume, resolução).
- HP/XP bars: `RectTransform.sizeDelta` — nunca `Image.fillAmount` para barras pixel-accurate.

---

## Instructions for Claude Code
- Sempre ler este arquivo no início de toda task
- Após implementar qualquer sistema novo, atualizar a seção relevante aqui
- Quando uma tela nova for criada, atualizar a tabela de UI screens
- Quando um novo nó da árvore for implementado, adicioná-lo à tabela de evolução
- Quando um novo tipo de bot for adicionado, documentá-lo na seção Bots
- Nunca contradizer decisões registradas aqui — sinalizar explicitamente antes de modificar
- **Toda decisão técnica deve considerar impacto em WebGL** — checar seção de otimização
