using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioClipsSO clipsSO;
    [SerializeField] private AudioMixer masterMixer;
    private static Dictionary<GameAudioType, AudioDetails> audioDetails;

    public int audioPoolSize = 4;
    List<AudioSource> audioSources = new();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioDetails = clipsSO.audioDetails;
        BuildAudioSourcePool();
    }

    public static void Play(GameAudioType audioType)
    {
        if (!TryGetAudioDetails(audioType, out var details)) return;
        if (!TryGetClip(details, out var clip)) return;

        // // var source = GetAvailableSource(details.mixerGroup);
        // if (source == null)
        // {
        //     Debug.LogWarning($"[AudioManager] No available AudioSource for group '{details.mixerGroup?.name}'. Consider increasing maxSimultaneous.");
        //     return;
        // }

        // if (details.loop)
        // {
        //     source.clip = clip;
        //     source.loop = true;
        //     source.Play();
        // }
        // else
        // {
        //     source.loop = false;
        //     source.PlayOneShot(clip);
        // }
    }


    public static void TryStop(GameAudioType audioType)
    {

    }

    public static async Awaitable WaitEndOfAudio(GameAudioType audioType)
    {

    }
    #region pool functions
    private void BuildAudioSourcePool()
    {
        for (int i = 0; i < audioPoolSize; i++)
        {
            // var audioSource = new GameObject($"AudioSource_{i}").AddComponent<AudioSource>();
            // audioSource.transform.parent = transform;
            // audioSource.loop = false;
            // audioSource.playOnAwake = false;
            // audioSources.Add(audioSource);
            var audioSource = new AudioSource()
            {
                name = $"AudioSource_{i}",
                playOnAwake = false,
                loop = false
            };
            audioSources.Add(audioSource);
        }
    }
    #endregion
    #region  Helper Functions
    private static bool TryGetAudioDetails(GameAudioType audioType, out AudioDetails audioDetails)
    {
        if (AudioManager.audioDetails.TryGetValue(audioType, out audioDetails)) return true;
        Debug.LogWarning($"[AudioManager] No AudioDetails registered for '{audioType}'.");
        return false;
    }

    private static bool TryGetClip(AudioDetails details, out AudioClip clip)
    {
        clip = details.GetRandomClip();
        if (clip != null) return true;
        Debug.LogWarning("[AudioManager] AudioClip is null check AudioClipsSO.");
        return false;
    }
    #endregion

}