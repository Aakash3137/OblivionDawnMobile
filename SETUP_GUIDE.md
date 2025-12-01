# Hex Warrior Clone - Setup Guide

## ✅ COMPLETED FIXES

### 1. Tile Perspective Fix
- **Fixed**: `IsTileLocallyOwned()` now correctly identifies local player
- **Result**: Host sees their tiles as green, Client sees their tiles as green
- Each player sees enemy tiles as red

---

## 📋 IMPLEMENTATION ROADMAP

### **PHASE 1: Hex Grid Foundation** ✅ COMPLETE
- [x] Hex tiles with NetworkObject
- [x] Tile ownership system
- [x] Perspective-based rendering
- [x] Tile selection with raycasting
- [x] Client perspective fix

### **PHASE 2: Card & Spawning System** 🔄 IN PROGRESS
**New Scripts Created:**
- `CardData.cs` - ScriptableObject for card definitions
- `NetworkCardManager.cs` - Handles card selection and spawning
- `NetworkBuilding.cs` - Buildings that spawn units
- `NetworkUnit.cs` - Units with movement and combat
- `CardUI.cs` - UI component for cards
- `CardDeckUI.cs` - Displays player's deck

**Setup Steps:**
1. Create CardData assets (Right-click → Create → Game → Card Data)
2. Add NetworkCardManager to scene with NetworkObject
3. Assign cards to NetworkCardManager.availableCards
4. Create UI Canvas with CardDeckUI component
5. Assign card prefabs with NetworkObject + NetworkBuilding/NetworkUnit

### **PHASE 3: Unit Movement & Combat** 🔄 BASIC DONE
**Features Implemented:**
- Units move forward automatically
- Capture tiles as they walk over them
- Detect and attack enemies/buildings
- Health and damage system
- Network synchronized

**TODO:**
- Add pathfinding (A* on hex grid)
- Add animations (walk, attack, die)
- Optimize enemy detection

### **PHASE 4: Resources & Economy** ⏳ PENDING
**TODO:**
- Create resource collection system
- Add resource UI display
- Implement card costs validation
- Add resource tiles

### **PHASE 5: Polish** ⏳ PENDING
**TODO:**
- Animations
- VFX and particles
- Sound effects
- Mobile touch optimization
- Network lag compensation

---

## 🎮 HOW IT WORKS

### Tile System
1. Tiles are pre-placed in scene with NetworkObject
2. Each tile has `initialOwnerSide` set in Inspector (Player/Enemy/None)
3. Host spawns at position[0] with 0° rotation
4. Client spawns at position[1] with 180° rotation
5. Each player sees their tiles as green, enemy as red

### Card System Flow
1. Player selects card from UI (CardUI)
2. Player clicks on tile (NetworkTile.HandleClick)
3. NetworkCardManager receives click
4. RPC sent to server to spawn building/unit
5. Server spawns NetworkObject with correct owner
6. All clients see the spawned object

### Unit Behavior
1. Units spawn from buildings every X seconds
2. Units move forward automatically
3. Capture tiles they walk over
4. Stop and attack when enemy in range
5. Continue moving when enemy destroyed

### Combat System
1. Unit detects enemy in range
2. Stops moving, starts attacking
3. Deals damage every attackCooldown seconds
4. Enemy health decreases
5. Enemy despawns when health <= 0

---

## 🔧 UNITY SETUP CHECKLIST

### Scene Setup
- [ ] Add NetworkHexGridManager to scene
- [ ] Add NetworkSideManager to scene (assign materials)
- [ ] Add NetworkCardManager with NetworkObject
- [ ] Place hex tiles with NetworkTile component
- [ ] Set tile initialOwnerSide in Inspector
- [ ] Assign tiles to "HexTile" layer

### Prefab Setup
**Player Prefab:**
- [ ] NetworkObject component
- [ ] NetworkPlayer component
- [ ] Camera as child (disabled by default)
- [ ] MeshRenderer for visual

**Tile Prefab:**
- [ ] NetworkObject component
- [ ] NetworkTile component
- [ ] BoxCollider (size: 1.5, 0.1, 1.5)
- [ ] Renderer component
- [ ] Layer: HexTile

**Building Prefab:**
- [ ] NetworkObject component
- [ ] NetworkBuilding component
- [ ] Assign unitPrefab to spawn
- [ ] Set spawnInterval

**Unit Prefab:**
- [ ] NetworkObject component
- [ ] NetworkUnit component
- [ ] Animator (optional)
- [ ] Collider for detection

### UI Setup
- [ ] Create Canvas
- [ ] Add CardDeckUI component
- [ ] Create CardUI prefab with Button, Image, TextMeshProUGUI
- [ ] Assign to CardDeckUI.cardUIPrefab

### Materials
- [ ] Create green material (Player tiles)
- [ ] Create red material (Enemy tiles)
- [ ] Create yellow material (Selected tiles)
- [ ] Assign to NetworkSideManager

---

## 🎯 NEXT STEPS

### Immediate (Phase 2 Completion)
1. Create CardData assets for different units
2. Setup UI canvas with card deck
3. Create building and unit prefabs
4. Test spawning system in multiplayer

### Short Term (Phase 3)
1. Implement A* pathfinding on hex grid
2. Add unit animations
3. Improve combat detection range
4. Add unit types (tanks, aircraft)

### Medium Term (Phase 4)
1. Resource system implementation
2. Economy balancing
3. Card cost validation

### Long Term (Phase 5)
1. VFX and polish
2. Mobile optimization
3. Network optimization
4. Matchmaking improvements

---

## 🐛 KNOWN ISSUES & SOLUTIONS

### Issue: Tiles not clickable
**Solution:** Ensure tiles have BoxCollider and are on "HexTile" layer

### Issue: Units not spawning
**Solution:** Check NetworkBuilding.unitPrefab is assigned and has NetworkObject

### Issue: Wrong tile colors
**Solution:** Fixed in NetworkTile.IsTileLocallyOwned()

### Issue: Camera not activating
**Solution:** NetworkPlayer.UpdateCameraForScene() handles this automatically

---

## 📝 CODE ARCHITECTURE

```
NetworkTile (on each hex)
    ↓ (click)
NetworkCardManager (singleton)
    ↓ (RPC spawn)
NetworkBuilding (spawns units)
    ↓ (timer)
NetworkUnit (moves & fights)
    ↓ (captures)
NetworkTile (ownership change)
```

---

## 🚀 TESTING WORKFLOW

1. **Solo Test:** Play in Editor, check tile colors
2. **Host Test:** Build and run as Host, check spawning
3. **Client Test:** Join as Client, verify perspective
4. **Multiplayer Test:** Both players spawn units, verify combat
5. **Network Test:** Check lag, sync issues

---

## 💡 TIPS

- Use Debug.Log extensively during development
- Test with 2 builds (Host + Client) not just Editor
- NetworkObject must be on all spawned prefabs
- Only StateAuthority can modify Networked properties
- Use RPCs for client → server communication
- TickTimer is better than Time.deltaTime for network sync

---

## 📚 RESOURCES

- Photon Fusion Docs: https://doc.photonengine.com/fusion
- Hex Grid Guide: https://www.redblobgames.com/grids/hexagons/
- Unity Networking Best Practices
