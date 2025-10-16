using UnityEngine;

[CreateAssetMenu(menuName = "Audio/UI Audio Settings")]
public class UIAudioSettings : ScriptableObject
{
    public AudioCue hoverDefault;
    public AudioCue selectDefault; // for keyboard/gamepad focus
    public AudioCue clickDefault;  // clicks / submit
    public AudioCue cancelDefault; // back/escape
    public AudioCue changeDefault; // slider/toggle value change
}
