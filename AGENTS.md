# OblivionDawnMobile - Agent Guide

## Project Type
Unity 2023+ mobile strategy game (URP). **Multiplayer/Network system is currently non-functional; ignore Photon Fusion/Network scripts.**

## Scope of Work
Only the following scenes are active and functional:
- `Loading`
- `MainScene`
- `SinglePlayerScene`

## Directory Structure
- `Assets/3. Scripts/` -- Primary game code. Only direct child folders with numbers are active (e.g., `Assets/3. Scripts/4. SinglePlayer/`).
- `Assets/8. ScriptableObjects/Scenario Mode/` -- Unit/Building stats and registries.
- `Assets/_Res/Addressable/` -- Runtime assets (Audio, Effects, Models, Spine).
- **Excluded**: `ExportProject`, `ModularGameUIKit` are NOT used. Exceptions: `TextMesh Pro` and `GoogleMobileAds`.

## Game Architecture (Single Player)
- **State Management**: `GameStateManager.cs` handles flow: `BOOTING` → `LOADING` → `MAIN_MENU` → `PLAYING` → `PAUSED` → `VICTORY/DEFEAT`.
- **Hex Grid**: managed by `HexGridManager.cs`. Tiles are generated via `CubeGridManager` on scene load. (Do NOT use `HexSnap.cs`).
- **Entity Tracking**: `GameplayRegistry.cs` and `EntityRegistry.cs` provide centralized lookup for units and buildings by `Side` and `Type`.
- **Core Loop**: Player places buildings $\rightarrow$ units auto-spawn $\rightarrow$ move $\rightarrow$ combat.

## Asset Loading & Data
- **Addressables**: All runtime assets (audio, effects, models, UI prefabs) MUST be loaded via Addressables. Never use `Resources.Load`.
- **JSON Data**: Global configurations are stored in `Assets/_Res/Addressable/01FirstBundle/JsonData/` (GlobalXxxInfo.json).
- **Spine**: Skeletal animations are loaded from `Assets/_Res/Addressable/Spine/`.

## Important Conventions
- **Side/Owner**: Use `Side` enum and `SideManager` to determine ownership, NOT Unity tags.
- **Custom Editor Tools**: `Assets/3. Scripts/7. Custom EditorTools/` contains property drawers and editor windows (UpgradeEditor, FactionUnitEditor).
- **Sizing**: Hexes use axial coordinates (q, r) internally, converted to world space via `HexGridManager.HexToWorld`.

## ScriptableObject Data
- **Registries**: `AllUnitData` and `AllBuildingData` map SOs by `FactionName` and type.
- **Units**: `UnitProduceStatsSO` (via `AllUnitData.cs`).
- **Buildings**: `BuildingDataSO` base class. Specialized types: `OffenseBuildingDataSO`, `DefenseBuildingDataSO`, `MainBuildingDataSO`, `ResourceBuildingDataSO`.
- **Upgrades**: Logic resides in `StatUpgrade.cs`. `Base Costs.cs` is a test file and should be ignored.
- **Generation**: SOs have `GenerateLevels()` buttons that call `StatUpgrade` to procedurally calculate level-based stats.
