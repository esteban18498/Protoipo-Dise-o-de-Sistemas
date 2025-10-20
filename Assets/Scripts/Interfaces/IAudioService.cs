public interface IAudioService
{
    // Music
    void PlayMusic(AudioCue cue, bool loop = true, float fade = 0.5f);
    void TransitionMusic(AudioCue cue, bool loop = true, float fade = 0.5f);
    void PauseMusic();
    void ResumeMusic();
    void StopMusic(float fade = 0.25f);

    // SFX
    void PlaySfx(AudioCue cue, UnityEngine.Vector3? worldPos = null);
    void PlaySfxOneShot(UnityEngine.AudioClip clip, float volume = 1f);

    // Buses
    void SetBusVolume(string exposedParam, float linear01);
    void MuteBus(string exposedParam, bool mute);
}
