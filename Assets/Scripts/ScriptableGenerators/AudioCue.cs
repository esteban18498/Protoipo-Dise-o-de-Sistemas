using UnityEngine;

[CreateAssetMenu(menuName = "Audio/AudioCue")]
public class AudioCue : ScriptableObject
{
    public AudioClip[] clips;
    [Range(-24, 6)] public float baseVolumeDb = 0;
    public bool randomizePitch = false;
    public Vector2 pitchRange = new Vector2(1f, 1f);
    public bool spatial3D = false;
    [Range(0f, 1f)] public float spatialBlend = 0f;

    public AudioClip Pick() => (clips == null || clips.Length == 0) ? null : clips[Random.Range(0, clips.Length)];
    public float PickPitch() => randomizePitch ? Random.Range(pitchRange.x, pitchRange.y) : 1f;
}

