# BULLET CRASH — Game Design & Systems Reference

> **Single source of truth** para todas as decisões de design e arquitetura.
> Regras de *processo* (como o Claude trabalha) ficam no [CLAUDE.md](CLAUDE.md).

## Project overview
- Game name: BULLET CRASH
- Genre: Bullet Heaven / Auto-battler (inspired by Vampire Survivors and MegaBonk)
- Engine: Unity 2D
- Art style: Pixel art, dark fantasy dungeon
- Developer: Solo dev
- Target platform: PC (WebGL export optional)
- Session length: 15–30 minutes per run

---

## Core game loop
1. Player selects a character
2. Player is dropped into a map with a run timer (15–30 min)
3. Enemies spawn automatically and move toward the player
4. Player only moves — all attacks are fully automatic
5. Killing enemies drops XP gems
6. Collecting XP gems fills the XP bar
7. When XP bar fills, game pauses and shows Level Up screen (pick 1 of 3 passive upgrades)
8. Bosses spawn at fixed time checkpoints (e.g. minute 5, 10, 20)
9. Run ends on player death (Game Over) or surviving the full timer (Victory)
10. Coins collected during run are saved permanently for meta-progression

---

## Player
- Movement: top-down, 8-directional (WASD / left analog stick)
- No manual attack input — attacks are fully automatic
- No weapon selection during gameplay
- Stats: HP, movement speed, XP magnet range
- HP bar visible in HUD at all times
- Death triggers Game Over screen

---

## Weapons system
- All weapons fire automatically on their own cooldown timer
- Player never manually selects or activates weapons during a run
- Weapons can evolve (combine weapon + passive item) at certain upgrade thresholds
- Weapon slots are NOT shown in the HUD (clean UI philosophy)

---

## Enemies
- All enemies use simple AI: move toward player position
- Spawn outside camera view via a Spawner system
- Spawn rate and enemy HP/count scale with run timer
- On death: drop XP gems, occasionally drop coins
- Boss enemies spawn at fixed time checkpoints, have phase patterns

---

## XP and level up
- XP gems are dropped by enemies on death
- Gems are attracted to player when within magnet range (upgradeable)
- XP bar fills silently at the bottom of the screen (thin purple strip, no labels)
- On level up: game pauses, Level Up overlay appears with 3 random passive upgrade cards
- Player clicks one card to select — no confirm button needed
- Cards are passive upgrades only: stat boosts, utility, special effects
- Card rarities: Common (1 dot), Uncommon (2 dots), Rare (3 dots / golden)

---

## Passive upgrades (card pool)
- IRON SKIN — increases max HP by 25
- SWIFT BOOTS — increases movement speed by 15%
- SOUL MAGNET — increases XP gem attraction range
- (expand this list as new upgrades are implemented)

---

## Meta-progression
- Coins persist between runs
- Coins are spent in the Upgrades screen (main menu)
- Upgrades unlock permanent passive buffs that apply to all future runs
- Characters are unlocked via meta-progression or achievement triggers

---

## UI screens
| Screen | Status | Figma file |
|---|---|---|
| Main Menu | Prompt ready | — |
| HUD (in-game) | Prompt ready | — |
| Level Up | Prompt ready | — |
| Game Over | Prompt ready | — |
| Character Select | Pending | — |
| Stage Select | Pending | — |
| Victory | Pending | — |
| Upgrades / Meta | Pending | — |
| Achievements | Pending | — |
| Options | Pending | — |
| Loading Screen | Pending | — |

---

## HUD layout (in-game)
- Single top bar, full width, 56px tall, #0A0A12 90% opacity
- Left zone: timer, kill count, coin count
- Center zone: HP bar (most prominent element)
- Right zone: current level
- Bottom: XP bar, 8px tall, full width, no labels
- Center of screen: completely empty — gameplay only
- No weapon slots, no passive item icons visible during gameplay

---

## UI color palette
- Background dark: #0A0A12
- Panel: #1E1230
- Border default: #4A2E7A
- Border selected / primary: #F0C040
- Border danger: #6E1E1E
- HP bar fill: #CC2222
- XP bar fill: #8866CC
- Text primary: #E8E8F0
- Text golden: #F0C040
- Text muted: #6A5A8A
- Overlay (level up): #0A0A12 80% opacity
- Overlay (game over): #0A0A12 90% opacity

---

## UI typography
- Font: Press Start 2P (pixel font) for all game UI
- No rounded corners anywhere — pixel art uses sharp 90° edges
- All pixel borders: 2px default, 4px for primary panels and buttons
- Buttons: no border-radius, sharp edges

---

## Architecture notes
- **Player movement (implemented):** 3 camadas desacopladas em `Assets/Scripts/Player/` —
  `PlayerInputReader : IMovementInput` (lê WASD + analog stick), `PlayerStats` (stats vivos
  do run) e `PlayerMovement` (só aplica `direction * MoveSpeed.Value` no Rigidbody2D em
  FixedUpdate). Movimento não conhece input nem upgrade. Movimento NÃO é Strategy de propósito
  (só há um modo de andar) — extrair só se surgir dash/teleporte.
- **Stats system (implemented):** `Assets/Scripts/Stats/` — `Stat` (base + lista de
  `StatModifier`, valor final cacheado por dirty-flag) e `StatModifier` (Flat / PercentAdd /
  PercentMult, com `Source` para remoção por fonte). É o backbone dos upgrades: SWIFT BOOTS =
  `PercentAdd 0.15`; IRON SKIN = `Flat 25`. Reutilizável em qualquer projeto. Fórmula:
  `(base + ΣFlat) * (1 + ΣPercentAdd) * Π(1 + PercentMult)`.
- **Stats base por personagem:** `CharacterData` (ScriptableObject, menu `Bullet Crash/Character
  Data`). `PlayerStats` lê o SO no Awake e envolve cada valor num `Stat` mutável — o asset nunca
  é alterado em runtime.
- Object Pool: required for projectiles and enemies from day one — no runtime Instantiate/Destroy in hot paths
- Spawner: time-based, reads a wave config ScriptableObject
- Enemy AI: simple Vector2.MoveTowards, no pathfinding needed initially
- Stats system: ScriptableObject per character defining base stats
- Upgrade system: ScriptableObject per upgrade card (icon, title, description, rarity, effect type, value)
- Save system: PlayerPrefs for coins and meta-progression; JSON for run records
- All bars (HP, XP, cooldown): RectTransform.sizeDelta width control — never use Image.fillAmount for pixel-accurate bars

---

## Instructions for Claude Code
- Always read this file at the start of every task
- After implementing any new system, update the relevant section in this file
- When a new UI screen is created in Figma, update the Status and Figma file columns in the UI screens table
- When a new passive upgrade is implemented, add it to the passive upgrades list
- When a new enemy type is added, document it under the Enemies section
- Never contradict decisions already recorded here — if a change is needed, flag it explicitly before modifying
- Keep this file as the single source of truth for all design and architecture decisions
