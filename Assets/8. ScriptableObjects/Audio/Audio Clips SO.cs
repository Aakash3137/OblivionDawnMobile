using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioClipsSO", menuName = "Scriptable Objects/AudioClipsSO")]
public class AudioClipsSO : ScriptableObject
{
    public AudioList[] audioClips;

    // void OnValidate()
    // {
    //     var enumValues = System.Enum.GetValues(typeof(GameAudioType));
    //     if (audioClips.Length != enumValues.Length)
    //     {
    //         audioClips = new AudioList[enumValues.Length];
    //     }

    //     for (int i = 0; i < audioClips.Length; i++)
    //     {
    //         audioClips[i].audioType = (GameAudioType)enumValues.GetValue(i);
    //         audioClips[i].name = audioClips[i].audioType.ToString();
    //     }
    // }
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
