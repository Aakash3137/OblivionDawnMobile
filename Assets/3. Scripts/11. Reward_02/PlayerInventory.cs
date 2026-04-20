// using UnityEngine;
// using System.Collections.Generic;

// public class PlayerInventory : MonoBehaviour
// {
//     public static PlayerInventory Instance;

//     private int gems;
//     private int fragments;
//     private int mapShards;

//     private Dictionary<string, int> units = new Dictionary<string, int>();

//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject); // IMPORTANT
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }

//     public void AddGems(int amount) => gems += amount;
//     public void AddFragments(int amount) => fragments += amount;
//     public void AddMapShards(int amount) => mapShards += amount;

//     public void AddUnit(string unitID, int amount)
//     {
//         if (!units.ContainsKey(unitID))
//             units[unitID] = 0;

//         units[unitID] += amount;
//     }
// }