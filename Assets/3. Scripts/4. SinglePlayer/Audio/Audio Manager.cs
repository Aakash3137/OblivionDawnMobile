using System.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioClipsSO clipsSO;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float fadeDuration = 3f;
    private const float MIN_DB = -80f;
    private const float MAX_DB = 0f;
    GameAudioType previousAudioType;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public static void PlayAudioOnce(GameAudioType audioType)
    {
        AudioList audioList = instance.clipsSO.audioClips[(int)audioType];
        AudioClip[] clips = audioList.clips;
        AudioClip randomClip = clips[Random.Range(0, clips.Length)];

        if (randomClip == null)
            GameDebug.Log("<color=#ffd700> [AudioManager] Audio clip is null</color>");

        var prvMixerGroup = instance.audioSource.outputAudioMixerGroup;
        instance.audioSource.outputAudioMixerGroup = audioList.mixerGroup;
        instance.audioSource.PlayOneShot(randomClip);

        // Debug.Log($"<color=#ffd700> [AudioManager] Playing audio clip: {randomClip.name} </color>");

        _ = instance.ResetMixerGroup(prvMixerGroup, randomClip.length);
    }

    private async Awaitable ResetMixerGroup(AudioMixerGroup mixerGroup, float duration)
    {
        await Awaitable.WaitForSecondsAsync(duration);
        instance.audioSource.outputAudioMixerGroup = mixerGroup;
    }

    public static void TransitionAudio(GameAudioType audioType)
    {
        if (instance.previousAudioType == audioType) return;

        instance.previousAudioType = audioType;
        AudioList audioList = instance.clipsSO.audioClips[(int)audioType];
        AudioClip[] clips = audioList.clips;
        AudioClip randomClip = clips[Random.Range(0, clips.Length)];

        instance.audioSource.outputAudioMixerGroup = audioList.mixerGroup;
        instance.audioSource.loop = true;
        instance.VolumeTransition(audioList.mixerGroup, randomClip);
    }

    private void VolumeTransition(AudioMixerGroup mixerGroup, AudioClip audioClip)
    {
        if (!mixerGroup.audioMixer.GetFloat("MusicVolume", out float currentVolume))
            currentVolume = MAX_DB;

        LSequence.Create()
                .Append(LMotion.Create(currentVolume, MIN_DB, fadeDuration * 0.25f)
                                .WithEase(Ease.OutExpo)
                                .WithOnComplete(() =>
                                {
                                    audioSource.clip = audioClip;
                                    audioSource.Play();
                                    // Debug.Log($"<color=#ffd700> [AudioManager] Playing audio clip: {audioClip.name} </color>");
                                })
                                .BindToAudioMixerFloat(mixerGroup.audioMixer, "MusicVolume")
                                .AddTo(this))
                .AppendInterval(0.1f)
                .Append(LMotion.Create(MIN_DB, MAX_DB, fadeDuration * 0.75f)
                        .WithEase(Ease.OutExpo)
                        .BindToAudioMixerFloat(mixerGroup.audioMixer, "MusicVolume")
                        .AddTo(this))
                .Run();
    }
}

