# Character Select And Workspace Scenes

`Muckhold_Workspace` is the first scene in Build Settings and the editor play-mode start scene.
This scene is the current `MainGame Muckhold` prototype. Use `Back To Lobby` when you need the hub.

## Camera Controls

Every scene has a `Main Camera` with `SceneCameraController`.
Every scene canvas has `UIManager` linked to that camera controller.

- Move: `WASD` or arrow keys
- Pan: right mouse drag or middle mouse drag
- Zoom: mouse wheel
- Reset from UI: connect a button to `UIManager.ResetCameraView`

## Main Lobby

`Main_Lobby` is the team hub and contains these main actions:

- `!! Play Our Game (Prototype) Button`: loads `Muckhold_Workspace`
- `Team ID Cards Button`: loads `SampleScene`
- `Personal Work Rooms Button`: opens the personal character scene list
- `Build Review Room Button`: loads `MainWorkspaceRoom_ForBuild` and opens related editor assets/scripts in the Unity Editor

## Character Select

`SampleScene` contains `Character Select Canvas` and 10 hierarchy-visible team ID card objects under `Character Slots`.
Each character object owns a `CharacterSlot` component.

The character classes are:

- `Archer`
- `Assassin`
- `Bard`
- `Blacksmith`
- `Healer`
- `Mage`
- `Spellblade`
- `Summoner`
- `Tactician`
- `Tanker`

When a character is clicked, `White Detail Panel` opens with:

- `Name:`
- `Description:`
- `Open Personal Workspace Button`

The personal workspace button routes to the selected character scene, such as `Bard_CheonWooyoung`.

## Direct Character Scene Buttons

`Character Scene Panel` contains 10 button objects named exactly like their target scenes:

- `Archer_KwonSeonghyeok`
- `Assassin_KimDonghyuk`
- `Bard_CheonWooyoung`
- `Blacksmith_ChoiJaeho`
- `Healer_KangHeewon`
- `Mage_JeongDongwon`
- `Spellblade_KimMinseok`
- `Summoner_JiJaewook`
- `Tactician_Daniel`
- `Tanker_KimSeonggwang`

Each object owns a `CharacterSceneLoadButton` component and stores its target scene name.

## Shared Workspace

`Muckhold_Workspace` contains the current `MainGame Muckhold` prototype under `WorkspaceRoot_Shared`.
It is intentionally simple and uses placeholder shapes:

- green ground
- green player dot
- base core
- blue mineral boxes
- yellow chest boxes
- red enemy dots

Each placeholder owns a `MuckholdWorkspaceActor` component so the object has its own role data.

## Editor Rebuild

Use this menu item when the scenes need to be rebuilt:

`OODong/Character Select/Rebuild Main Menu And Character Scenes`

Batch validation entry point:

`OODong.CharacterSelect.Editor.CharacterSelectSceneBuilder.RebuildAndValidateRequestedSetup`
