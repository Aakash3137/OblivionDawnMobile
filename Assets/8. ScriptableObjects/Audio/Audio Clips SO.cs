using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioClipsSO", menuName = "Scriptable Objects/AudioClipsSO")]
public class AudioClipsSO : SerializedScriptableObject
{
    public Dictionary<GameAudioType, AudioDetails> audioDetails = new();

    private void OnValidate()
    {
        var enumValues = System.Enum.GetValues(typeof(GameAudioType));

        foreach (GameAudioType audioType in enumValues)
        {
            if (!audioDetails.ContainsKey(audioType))
            {
                audioDetails.Add(audioType, new()
                {
                    audioClips = new()
                });
            }
        }
    }
}

[System.Serializable]
public struct AudioDetails
{
    public AudioMixerGroup audioGroup;
    public List<AudioClip> audioClips;
    public bool loop;

    public AudioClip GetRandomClip()
    {
        return audioClips[Random.Range(0, audioClips.Count)];
    }
}

public enum GameAudioType
{
    None = 0,
    // UI
    ButtonClick,
    PlayButton,
    BackButton,

    // Music
    MenuMusic = 21,
    GameMusic,

    // SFX
    ResourceTick = 41,
    BattleSFX,
    ResourceSFX,
    MainSFX,
}