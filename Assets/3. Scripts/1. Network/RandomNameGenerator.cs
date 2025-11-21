using UnityEngine;

public static class RandomNameGenerator
{
    private static readonly string[] NamePool =
    {
        "Falcon","Panther","Raven","Viper","Rhino","Hydra","Wolf",
        "Cobra","Titan","Spartan","Raptor","Sabre","Thunder","Eagle",
        "Dragon","Phantom","Ghost","Knight"
    };

    public static string GetRandomName()
    {
        string baseName = NamePool[Random.Range(0, NamePool.Length)];
        int number = Random.Range(100, 999);
        return $"{baseName}-{number}";
    }
}