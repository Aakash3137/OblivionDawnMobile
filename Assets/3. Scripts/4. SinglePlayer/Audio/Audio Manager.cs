using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioClipsSO clipsSO;
    [SerializeField] private AudioMixer audioMixer;
    private static Dictionary<AudioMixerGroup, string> exposedParameters = new();
    private static AudioSource loopAudioSource;
    [SerializeField] private AudioSource threeDimensionalAudioSourcePrefab;

    [Header("Pool Settings")]
    [SerializeField] private int defaultCapacity = 3;
    [SerializeField] private int maxSize = 20;
    public static ObjectPool<AudioSource> oneShotAudioPool;
    public static ObjectPool<AudioSource> threeDimensionalAudioPool;


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

        CreatePool();
        CacheExposedParameters();

        loopAudioSource = new GameObject("Loop Audio Source").AddComponent<AudioSource>();
        loopAudioSource.transform.parent = transform;
        loopAudioSource.playOnAwake = false;
        loopAudioSource.loop = true;
    }

    public static void PlayOneShot(GameAudioType audioType)
    {
        if (!TryGetAudioDetails(audioType, out var details)) return;
        if (!TryGetClip(details, out var clip)) return;

        var source = oneShotAudioPool.Get();
        source.clip = clip;
        source.outputAudioMixerGroup = details.mixerGroup;
        source.Play();

        _ = ReleaseAfterPlay(source);
    }

    public static void PlayInLoop(GameAudioType audioType)
    {
        if (!TryGetAudioDetails(audioType, out var details)) return;
        if (!TryGetClip(details, out var clip)) return;

        _ = FadeInAndOut(loopAudioSource, clip, details.mixerGroup);
    }

    public static AudioSource Play3DSound(AudioDetails details)
    {
        if (!TryGetClip(details, out var clip)) return null;

        var source = threeDimensionalAudioPool.Get();
        _ = FadeInAndOut(source, clip, details.mixerGroup);
        return source;
    }

    private static async Awaitable FadeInAndOut(AudioSource source, AudioClip clip, AudioMixerGroup mixerGroup, float duration = 2f)
    {
        if (source.clip != null)
        {
            float startVolume = source.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                await Awaitable.NextFrameAsync();
            }

            source.volume = 0f;
            source.Stop();
        }

        source.clip = clip;
        source.outputAudioMixerGroup = mixerGroup;
        source.Play();

        float fadeElapsed = 0f;
        while (fadeElapsed < duration)
        {
            fadeElapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, 1f, fadeElapsed / duration);
            await Awaitable.NextFrameAsync();
        }

        source.volume = 1f;
    }

    private static async Awaitable ReleaseAfterPlay(AudioSource source)
    {
        await Awaitable.WaitForSecondsAsync(source.clip.length);
        oneShotAudioPool.Release(source);
    }

    private static float GetVolume(AudioMixerGroup mixerGroup)
    {
        instance.audioMixer.GetFloat(exposedParameters[mixerGroup], out float db);
        return DecibelToLinear(db);
    }

    private static float LinearToDecibel(float linear) =>
         linear > 0.0001f ? Mathf.Log10(linear) * 20f : -80f;

    private static float DecibelToLinear(float dB) =>
        Mathf.Pow(10f, dB / 20f);

    #region pool functions
    private void CreatePool()
    {
        oneShotAudioPool = new(
            InstantiateOneShotAudio,
            OnGet,
            OnRelease,
            OnDestroyPoolObject,
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );

        threeDimensionalAudioPool = new(
            Instantiate3DAudio,
            OnGet,
            OnRelease,
            OnDestroyPoolObject,
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }
    private AudioSource Instantiate3DAudio()
    {
        var source = Instantiate(threeDimensionalAudioSourcePrefab, transform);
        source.name = "3D Audio Source";
        source.gameObject.SetActive(false);
        return source;
    }
    private AudioSource InstantiateOneShotAudio()
    {
        var source = new GameObject("One Shot Audio Source").AddComponent<AudioSource>();
        source.transform.parent = transform;
        source.playOnAwake = false;
        return source;
    }

    private void OnGet(AudioSource source)
    {
        source.gameObject.SetActive(true);
    }

    private void OnRelease(AudioSource source)
    {
        source.transform.position = Vector3.zero;
        source.clip = null;
        source.Stop();
        source.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(AudioSource source)
    {
        Destroy(source.gameObject);
    }
    #endregion

    #region  Helper Functions
    private static bool TryGetAudioDetails(GameAudioType audioType, out AudioDetails audioDetails)
    {
        if (instance.clipsSO.audioDetails.TryGetValue(audioType, out audioDetails)) return true;
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
    private void CacheExposedParameters()
    {
        var mixerGroups = audioMixer.FindMatchingGroups("Master");

        foreach (var group in mixerGroups)
        {
            exposedParameters.Add(group, $"{group.name}Volume");
        }
    }
    #endregion

}