# BulletCrash

Projeto Unity (2D) em transformação: começou como teste técnico (top-down com inventário,
clique-pra-atacar) e está virando um **bullet heaven / auto-battler** no estilo
*Vampire Survivors* e *MegaBonk*. O player **só se movimenta e escolhe upgrades**;
o jogo **mira e atira sozinho**.

> **Fonte de verdade do design:** [GAME_DESIGN.md](GAME_DESIGN.md) (GDD completo — loop,
> sistemas, paleta de UI, telas, notas de arquitetura). **Ler no início de toda task** e
> **atualizar** a seção relevante ao implementar um sistema novo. Nunca contradizer uma
> decisão registrada lá sem sinalizar explicitamente antes.

## Como o Claude deve trabalhar neste projeto

Aja como um **desenvolvedor sênior** guiando as decisões. Em ordem de prioridade:

1. **POO + Clean Code sempre.** Nomes claros, funções curtas, responsabilidade única.
   Sem código morto, sem comentário óbvio. Comentário só explica *porquê*, não *o quê*.
2. **Tudo modular e escalável para reúso em outros projetos.** Sistemas devem ser
   data-driven (ScriptableObject), desacoplados por interface/evento, e não depender de
   detalhes de *este* jogo. Pergunta-guia: "eu conseguiria arrancar este sistema e colar
   em outro projeto?"
3. **SOLID como régua, não como religião.** Nível de abstração alvo: **pragmático
   escalável** — SOLID + ScriptableObjects + eventos + DI manual leve (injeção por
   `[SerializeField]`/construtor/Init). **Não** introduzir DI container (Zenject/VContainer),
   nem camadas formais, a menos que o usuário peça.
4. **Padrão fora do comum = avisar e explicar ANTES de implementar.** Sempre que for usar
   um padrão arquitetural não-trivial (MVC, MVVM, State Machine, Command, Observer formal,
   Service Locator, ECS, etc.), **explicar em PT-BR**: (a) o que é, (b) por que é a melhor
   escolha aqui, (c) como será implementado, (d) o trade-off. Só então codar.
   Strategy via `WeaponBehavior` e Object Pooling já são padrões aceitos da base.
5. **Explicar decisões em português (PT-BR).** O usuário pensa nas escolhas junto.
   Recomendar uma opção, não despejar um menu de alternativas.

Este comportamento é **padrão do projeto** — vale em toda sessão, não precisa ser repedido.

## Arquitetura atual (base herdada)

- **Strategy de armas:** `WeaponBehavior : ScriptableObject` com `Perform(PlayerAttack)`
  abstrato. Base correta para os tipos de tiro/arma — estender daqui, não com `if/else`.
- **Object Pooling:** `GameObjectPool`, `ProjectilePool`, `CoinPool`. Obrigatório para
  projéteis e inimigos no bullet hell — nunca `Instantiate/Destroy` em loop de gameplay.
- **Data-driven:** `Item`/`ItemDatabase` (ScriptableObject). Caminho dos upgrades roguelike.
- **Eventos:** ex. `Health.OnDeath`. Base de Observer para XP/loot/morte.
- **Singletons:** `InventoryManager.Instance`, `AudioManager.Instance`. Tolerados, mas
  preferir injeção/eventos em sistemas novos; não criar singleton novo sem justificar.

## Direção do redesign (survivor roguelike)

Mudanças estruturais em relação à base:
- **Auto-fire + auto-targeting** substitui o clique do mouse de `PlayerAttack`.
- **Sistema de progressão**: XP, level-up, escolha de 3 upgrades (roguelike run).
- **Spawn por waves/tempo** com dificuldade escalando, substituindo o `EnemySpawner` fixo.
- **Meta-progressão** (roguelite) entre runs — definir depois.

## Estrutura de pastas (`Assets/Scripts/`)

```
Core/        ← infraestrutura PORTÁVEL (copiável p/ outro projeto). NÃO depende de game code.
  Stats/     → Stat, StatModifier            (namespace BulletCrash.Core.Stats)
  Pooling/   → GameObjectPool (genérico)      (namespace BulletCrash.Core.Pooling)
Player/ Enemy/ Combat/ Items/ Audio/ UI/ VFX/ Camera/   ← game code (namespace default)
Pooling/     → ProjectilePool, CoinPool       ← pools TIPADOS do jogo (usam Core/Pooling)
Inventory/   → ⚠️ legado, marcado p/ REMOÇÃO (GDD: jogo não tem inventário/seleção manual).
              Não estender; remover quando o auto-fire substituir a cola do clique.
```

Pastas por domínio. Ao criar um sistema novo do survivor (Progression/XP, Upgrades,
Waves, Save/Meta), criar a pasta do domínio.

## Convenções de código

- C# / Unity 6.3. Um tipo por arquivo, nome do arquivo = nome do tipo.
- **Namespaces:** só módulos reutilizáveis em `Core/` levam namespace (`BulletCrash.Core.*`);
  são bibliotecas e **não podem** referenciar game code (dependência flui app→lib).
  Game code fica no **namespace default** (assembly único, sem asmdef). Manter essa regra:
  não namespacear game code novo a menos que se introduza assembly definitions.
- `[SerializeField] private` + propriedade de leitura, em vez de campo público.
- Evitar `GetComponent`/`Find` em `Update`; cachear em `Awake`/`Start`.
- Mover scripts: mover sempre o `.cs` **e** o `.cs.meta` juntos (preserva o GUID e as
  referências em cenas/prefabs).
- Não tocar em `Assets/TextMesh Pro/`, `Assets/Epic Toon FX/` (pacotes de terceiros).
