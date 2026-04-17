using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GamePlay3DAudioManager : MonoBehaviour
{
    private float maxDistance = 9f;
    private CancellationTokenSource cts;
    private Dictionary<ScenarioUnitType, int> maxSFXperUnitType = new();
    private Dictionary<ScenarioUnitType, List<AudioSource>> audioSourcesPerUnitType = new();

    private void OnEnable()
    {
        cts?.Dispose();
        cts = new CancellationTokenSource();
    }
    private void Start()
    {
        foreach (var type in ScenarioDataTypes._unitEnumValues)
        {
            maxSFXperUnitType.Add(type, 5);
            audioSourcesPerUnitType.Add(type, new());
        }
        _ = InitDynamicSFX();
    }

    private async Awaitable InitDynamicSFX()
    {
        while (!cts.IsCancellationRequested)
        {
            await Awaitable.WaitForSecondsAsync(0.2f, cts.Token);
            PlayDynamicSFX();
        }
    }
    private void PlayDynamicSFX()
    {
        foreach (var type in ScenarioDataTypes._unitEnumValues)
        {
            var allUnitsOfType = GameplayRegistry.GetUnits(type);
            var currentSources = audioSourcesPerUnitType[type];
            int cap = Mathf.Min(maxSFXperUnitType[type], allUnitsOfType.Count);

            if (currentSources.Count < cap)
            {
                var unit = allUnitsOfType[Random.Range(0, allUnitsOfType.Count)];
                var source = AudioManager.Play3DSound(unit.audioDetails);

                if (source != null)
                {
                    source.GetComponent<FollowTarget3dAudio>().target = unit.transform;
                    currentSources.Add(source);
                }
            }
            else if (currentSources.Count > cap)
            {
                var sourceToRemove = currentSources[0];
                sourceToRemove.GetComponent<FollowTarget3dAudio>().target = null;
                AudioManager.threeDimensionalAudioPool.Release(sourceToRemove);
                currentSources.Remove(sourceToRemove);
            }
        }
    }

    private void OnDisable()
    {
        if (cts != null)
        {
            cts.Cancel();
            cts.Dispose();
        }
    }
}
