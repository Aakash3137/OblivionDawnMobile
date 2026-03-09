using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioClipsSO", menuName = "Scriptable Objects/AudioClipsSO")]
public class AudioClipsSO : ScriptableObject
{
    public AudioList[] audioClips;

    void OnValidate()
    {
        var enumValues = System.Enum.GetValues(typeof(GameAudioType));

        if (audioClips == null || audioClips.Length != enumValues.Length)
        {
            var resized = new AudioList[enumValues.Length];

            // Preserve existing data when resizing
            if (audioClips != null)
            {
                for (int i = 0; i < Mathf.Min(audioClips.Length, resized.Length); i++)
                {
                    resized[i] = audioClips[i];
                }
            }

            audioClips = resized;
        }

        for (int i = 0; i < enumValues.Length; i++)
        {
            audioClips[i].audioType = (GameAudioType)enumValues.GetValue(i);
            audioClips[i].name = audioClips[i].audioType.ToString();
        }
    }

    public AudioClip GetClip(GameAudioType type)
    {
        foreach (var list in audioClips)
        {
            if (list.audioType == type && list.clips != null && list.clips.Length > 0)
                return list.clips[Random.Range(0, list.clips.Length)];
        }

        Debug.LogWarning($"[AudioClipsSO] No clips found for type: {type}");
        return null;
    }

    public bool TryGetAudioList(GameAudioType type, out AudioList result)
    {
        foreach (var list in audioClips)
        {
            if (list.audioType == type)
            {
                result = list;
                return true;
            }
        }

        result = default;
        return false;
    }
}

[System.Serializable]
public struct AudioList
{
    [HideInInspector] public string name;
    public GameAudioType audioType;
    public AudioMixerGroup mixerGroup;
    public AudioClip[] clips;
}

public enum GameAudioType
{
    NONE,
    ButtonClick,
    PlayButton,
    BackButton,
    MenuMusic,
    GameMusic,
    BattleMusic
}