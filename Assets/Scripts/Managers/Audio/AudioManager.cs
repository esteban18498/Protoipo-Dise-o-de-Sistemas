using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public sealed class AudioManager : MonoBehaviour, IAudioService
{
    public static IAudioService Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer; // assign your GameAudio mixer

    [Header("Music Sources (A/B)")]
    [SerializeField] private AudioSource musicA;
    [SerializeField] private AudioSource musicB;

    [Header("SFX Pool")]
    [SerializeField] private int sfxPoolSize = 12;
    [SerializeField] private AudioSource sfxPrefab; // Output: SFX
    [SerializeField] private Transform sfxPoolParent;

    private AudioSource[] sfxPool;
    private int sfxIndex;
    private AudioSource current, next;
    private bool usingA = true;

    private void Awake()
    {
        // Comment this if you set up the audio bootstrapper as initializer
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initializers
        current = musicA;
        next = musicB;

        sfxPool = new AudioSource[sfxPoolSize];
        for (int i = 0; i < sfxPoolSize; i++)
        {
            sfxPool[i] = Instantiate(sfxPrefab, sfxPoolParent);
            sfxPool[i].playOnAwake = false;
        }
    }

    // -------- Music --------
    public void PlayMusic(AudioCue cue, bool loop = true, float fade = 0.5f)
    {
        InternalPlay(current, cue, loop);
        FadeTo(current, 0f, fade);
        FadeTo(next, -80f, fade * 0.5f);
    }

    public void TransitionMusic(AudioCue cue, bool loop = true, float fade = 0.5f)
    {
        var target = usingA ? next : current;
        InternalPlay(target, cue, loop);
        FadeTo(target, 0f, fade);
        FadeTo(usingA ? current : next, -80f, fade);
        usingA = !usingA;
        current = usingA ? musicA : musicB;
        next = usingA ? musicB : musicA;
    }

    public void PauseMusic() { if (current.isPlaying) current.Pause(); if (next.isPlaying) next.Pause(); }
    public void ResumeMusic() { if (current.clip) current.UnPause(); if (next.clip) next.UnPause(); }

    public void StopMusic(float fade = 0.25f)
    {
        FadeTo(current, -80f, fade, () => current.Stop());
        FadeTo(next, -80f, fade, () => next.Stop());
    }

    private void InternalPlay(AudioSource src, AudioCue cue, bool loop)
    {
        if (!cue) return;
        var clip = cue.Pick();
        if (!clip) return;
        src.clip = clip;
        src.loop = loop;
        src.pitch = cue.PickPitch();
        src.spatialBlend = cue.spatial3D ? Mathf.Clamp01(cue.spatialBlend) : 0f;
        src.volume = DbToLinear(cue.baseVolumeDb);
        src.Play();
    }

    // -------- SFX --------
    public void PlaySfx(AudioCue cue, Vector3? worldPos = null)
    {
        if (!cue) return;
        var clip = cue.Pick();
        if (!clip) return;
        var src = NextSfxSource();
        if (worldPos.HasValue) src.transform.position = worldPos.Value;
        src.spatialBlend = cue.spatial3D ? Mathf.Clamp01(cue.spatialBlend) : 0f;
        src.pitch = cue.PickPitch();
        src.PlayOneShot(clip, DbToLinear(cue.baseVolumeDb));
    }

    public void PlaySfxOneShot(AudioClip clip, float volume = 1f)
    {
        if (!clip) return;
        var src = NextSfxSource();
        src.spatialBlend = 0f;
        src.pitch = 1f;
        src.PlayOneShot(clip, Mathf.Clamp01(volume));
    }

    private AudioSource NextSfxSource()
    {
        var src = sfxPool[sfxIndex];
        sfxIndex = (sfxIndex + 1) % sfxPool.Length;
        return src;
    }

    // -------- Buses --------
    public void SetBusVolume(string exposedParam, float linear01) =>
        mixer.SetFloat(exposedParam, LinearToDb(Mathf.Clamp01(linear01)));

    public void MuteBus(string exposedParam, bool mute) =>
        mixer.SetFloat(exposedParam, mute ? -80f : 0f);

    // -------- Helpers --------
    private static float LinearToDb(float linear) => linear <= 0.0001f ? -80f : Mathf.Log10(linear) * 20f;
    private static float DbToLinear(float db) => Mathf.Pow(10f, db / 20f);

    private void FadeTo(AudioSource src, float targetDb, float time, System.Action onDone = null)
    {
        if (!gameObject.activeInHierarchy) { src.volume = DbToLinear(targetDb); onDone?.Invoke(); return; }
        StartCoroutine(FadeCoroutine(src, targetDb, time, onDone));
    }

    private IEnumerator FadeCoroutine(AudioSource src, float targetDb, float time, System.Action onDone)
    {
        float startDb = LinearToDb(Mathf.Max(0.0001f, src.volume));
        float t = 0f;
        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            float db = Mathf.Lerp(startDb, targetDb, t / time);
            src.volume = DbToLinear(db);
            yield return null;
        }
        src.volume = DbToLinear(targetDb);
        onDone?.Invoke();
    }
}
