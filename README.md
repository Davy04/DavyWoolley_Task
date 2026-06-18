# DavyWoolley_Task

A 2D top-down action game made in Unity (URP). The player explores the map,
fights enemies with a staff that shoots projectiles, collects loot and manages
an inventory.

## Controls

| Action | Input |
|--------|-------|
| Move | `W` `A` `S` `D` |
| Aim | Mouse |
| Attack / use equipped item | Left Mouse Button |
| Select hotbar slot | `1` – `5` |
| Open / close bag | `Tab` |
| Move items | Drag & drop |
| Drop item (bag open) | Right Mouse Button on the item |
| Pause | Pause button (UI) |

> The equipped hotbar item decides the action: a staff shoots a projectile, a
> potion heals. Just left-click.

## Main Systems

- **Inventory** — hotbar + bag with drag & drop, stacking, and JSON save
  (one file per slot, in the user's persistent data folder).
- **Combat** — weapons are ScriptableObjects (Strategy pattern), so the player
  is weapon-agnostic. Projectiles deal damage, knockback and explode on hit.
- **Health** — pure HP component that fires damage/heal/death events; UI bars,
  audio, hit effects and loot react to those events.
- **Enemies** — movement AI, attack with knockback, loot drops, and a spawner
  that pursues the player.
- **Loot & pickup** — world items are thrown out and pulled in by a magnet;
  coins use object pooling, capped at 20 in the scene (oldest recycled).
- **Audio** — central manager for one-shot SFX and looping background music.
- **Flow & display** — Main Menu, Pause and Game Over screens, with the view
  locked to a 16:9 aspect ratio.

## How to Run

1. Download the build zip and extract the **whole folder**.
2. Run `DavyWoolley_Task.exe`.

To open in Unity: clone the repo and open the project folder with the Unity
Editor, then load the menu scene and press Play.
