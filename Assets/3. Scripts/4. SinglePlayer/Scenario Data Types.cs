
using System;

public enum FactionName
{
    Medieval = 0,
    Present = 1,
    Futuristic = 2,
    Galvadore = 3
}
public enum ScenarioResourceType
{
    Food = 0,
    Gold = 1,
    Metal = 2,
    Power = 3
}
public enum ScenarioUnitType
{
    Melee = 0,
    Ranged = 1,
    AOERanged = 2,
    Air = 3
}

public enum ScenarioOffenseType
{
    MeleeBuilding = 0,
    RangedBuilding = 1,
    AOERangedBuilding = 2,
    AirBuilding = 3
}
public enum ScenarioDefenseType
{
    Wall = 0,
    Turret = 1,
    AntiTank = 2,
    AntiAir = 3
}

// Preassign 100 indexes for each faction
public enum GameUnitName
{
    // Default Units in first line 
    // Medieval Faction Units
    Sayuri = 0, Veer, Griffin, Ballista,
    ShieldBearer,
    Crossbowman,
    WarEagle,
    KnightCaptain,
    FireArcher,
    Trebuchet,
    WarElephant,
    PhoenixRider,
    Catapult,
    Paladin,

    // Present Faction Units
    Soldier = 100, Tank, Aircraft, ArtilleryGun,
    Marine,
    Sniper,
    Helicopter,
    RocketLauncher,
    ArmouredAPC,
    AntiTankGun,
    FighterJet,
    MortarTeam,
    SpecialForces,
    MLRS,
    BomberAircraft,

    // Futuristic Faction Units
    Mech = 200, PlasmaTurret, Drone, GuidedMissile,
    CyberWarrior,
    RailGun,
    StealthDrone,
    QuantumCannon,
    NanoSwarm,
    LaserBeam,
    IonBomber,
    EMPGrenadier,
    BattleAndroid,
    HoverBike,

    // Galvadore Faction Units
    Sabretooth = 300, Pugrash, FireDragon, Gemitshu,
    Wraith,
    BoneArcher,
    ShadowBat,
    VoidMage,
    Golem,
    DeathKnight,
    InfernalDrake,
    CurseCannon,
    DarkTroll,
    SpectralArcher
}

public enum GameBuildingName
{
    // main building - Offense - Defense - Resource
    // Medieval Faction Buildings
    MedievalCity = 0,

    MedievalMeleeSpawner, MedievalRangedSpawner, MedievalAOERangedSpawner, MedievalAirSpawner,

    MedievalWall = 20, MedievalTurret, MedievalAntiTank, MedievalAntiAir,

    MedievalFoodFarm = 80, MedievalGoldMine, MedievalMetalMine, MedievalPowerPlant,


    // Present Faction Buildings
    PresentCity = 100, PresentMeleeSpawner, PresentRangedSpawner, PresentAOERangedSpawner, PresentAirSpawner,

    PresentWall = 120, PresentTurret, PresentAntiTank, PresentAntiAir,

    PresentFoodFarm = 180, PresentGoldMine, PresentMetalMine, PresentPowerPlant,


    // Futuristic Faction Buildings
    FutureCity = 200,

    FutureMeleeSpawner, FutureRangedSpawner, FutureAOERangedSpawner, FutureAirSpawner,

    FutureWall = 220, FutureTurret, FutureAntiTank, FutureAntiAir,

    FutureFoodFarm = 280, FutureGoldMine, FutureMetalMine, FuturePowerPlant,


    // Galvadore Faction Buildings
    GalvadoreCity = 300,

    GalvadoreMeleeSpawner, GalvadoreRangedSpawner, GalvadoreAOERangedSpawner, GalvadoreAirSpawner,

    GalvadoreWall = 320, GalvadoreTurret, GalvadoreAntiTank, GalvadoreAntiAir,

    GalvadoreFoodFarm = 380, GalvadoreGoldMine, GalvadoreMetalMine, GalvadorePowerPlant
}

public static class ScenarioDataTypes
{
    public static readonly FactionName[] _factionEnumValues = (FactionName[])Enum.GetValues(typeof(FactionName));
    public static readonly ScenarioResourceType[] _resourceEnumValues = (ScenarioResourceType[])Enum.GetValues(typeof(ScenarioResourceType));
    public static readonly ScenarioUnitType[] _unitEnumValues = (ScenarioUnitType[])Enum.GetValues(typeof(ScenarioUnitType));
    public static readonly ScenarioOffenseType[] _offenseEnumValues = (ScenarioOffenseType[])Enum.GetValues(typeof(ScenarioOffenseType));
    public static readonly ScenarioDefenseType[] _defenseEnumValues = (ScenarioDefenseType[])Enum.GetValues(typeof(ScenarioDefenseType));
    public static readonly GameUnitName[] _gameUnitEnumValues = (GameUnitName[])Enum.GetValues(typeof(GameUnitName));
}