# Cinderkeep

Cinderkeep is the team's main Unity game project.

## Current Scene Flow

- `Assets/Scenes/Main_Lobby.unity`
  - Main menu scene.
  - Use this only for simple entry UI such as game start and quit.

- `Assets/Scenes/MainGame/Cinderkeep_Game.unity`
  - Main game scene.
  - Put actual gameplay objects, player work, map work, combat, resources, building, and QA targets here.

## Removed Legacy Flow

The old lobby convenience flow is no longer part of the active project.
Legacy files are kept under `Assets/_Disabled` only for reference.

## Team Folder Rule

- Scripts: `Assets/Scripts`
- Scenes: `Assets/Scenes`
- Prefabs: `Assets/Prefabs`
- Data: `Assets/Data`
- Tests: `Assets/Tests`
- Experiments: `Assets/_Sandbox`
- Disabled files: `Assets/_Disabled`

Do not add new gameplay work to disabled or legacy folders.
