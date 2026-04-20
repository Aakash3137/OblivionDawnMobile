using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RewardBundleGeneratorWindow : EditorWindow
{
    private int numberOfBundles = 7;

    private int baseGems = 10;
    private int baseFragments = 20;
    private int baseMapShards = 5;

    private float growthFactor = 1.2f;

    private string savePath = "Assets/Rewards/Generated/";

    private CurrencyRewardData gemData;
    private CurrencyRewardData fragmentData;
    private CurrencyRewardData mapShardData;

    [MenuItem("Tools/Reward Bundle Generator")]
    public static void ShowWindow()
    {
        GetWindow<RewardBundleGeneratorWindow>("Reward Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Generate Reward Bundles", EditorStyles.boldLabel);

        numberOfBundles = EditorGUILayout.IntField("Number of Bundles", numberOfBundles);

        GUILayout.Space(10);

        GUILayout.Label("Base Values", EditorStyles.boldLabel);
        baseGems = EditorGUILayout.IntField("Base Gems", baseGems);
        baseFragments = EditorGUILayout.IntField("Base Fragments", baseFragments);
        baseMapShards = EditorGUILayout.IntField("Base Map Shards", baseMapShards);

        growthFactor = EditorGUILayout.FloatField("Growth Factor", growthFactor);

        GUILayout.Space(10);

        GUILayout.Label("Reward Data References", EditorStyles.boldLabel);
        gemData = (CurrencyRewardData)EditorGUILayout.ObjectField("Gem Data", gemData, typeof(CurrencyRewardData), false);
        fragmentData = (CurrencyRewardData)EditorGUILayout.ObjectField("Fragment Data", fragmentData, typeof(CurrencyRewardData), false);
        mapShardData = (CurrencyRewardData)EditorGUILayout.ObjectField("MapShard Data", mapShardData, typeof(CurrencyRewardData), false);

        GUILayout.Space(10);

        savePath = EditorGUILayout.TextField("Save Path", savePath);

        if (GUILayout.Button("Generate Bundles"))
        {
            GenerateBundles();
        }
    }

    private void GenerateBundles()
    {
        if (!AssetDatabase.IsValidFolder(savePath))
        {
            Debug.LogError("Invalid folder path!");
            return;
        }

        for (int i = 0; i < numberOfBundles; i++)
        {
            RewardBundle bundle = ScriptableObject.CreateInstance<RewardBundle>();
            bundle.rewards = new List<RewardInstance>();

            int gems = Mathf.RoundToInt(baseGems * Mathf.Pow(growthFactor, i));
            int fragments = Mathf.RoundToInt(baseFragments * Mathf.Pow(growthFactor, i));
            int shards = Mathf.RoundToInt(baseMapShards * Mathf.Pow(growthFactor, i));

            bundle.rewards.Add(new RewardInstance { rewardData = gemData, amount = gems });
            bundle.rewards.Add(new RewardInstance { rewardData = fragmentData, amount = fragments });
            bundle.rewards.Add(new RewardInstance { rewardData = mapShardData, amount = shards });

            string assetName = savePath + "RewardBundle_" + (i + 1) + ".asset";

            AssetDatabase.CreateAsset(bundle, assetName);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Reward Bundles Generated!");
    }
}