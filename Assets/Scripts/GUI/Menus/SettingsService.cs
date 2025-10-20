using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SettingsService
{
    private readonly ISettingsStore store;
    public SettingsService(ISettingsStore store) { this.store = store; }

    // Audio (0..1)
    public float MasterVolume
    {
        get => store.Load01(SettingsKeys.MasterVolume, 0.2f);
        set => store.Save01(SettingsKeys.MasterVolume, value);
    }

    public float MusicVolume
    {
        get => store.Load01(SettingsKeys.MusicVolume, 0.2f);
        set => store.Save01(SettingsKeys.MusicVolume, value);
    }

    public float SfxVolume
    {
        get => store.Load01(SettingsKeys.SfxVolume, 0.2f);
        set => store.Save01(SettingsKeys.SfxVolume, value);
    }

    // Graphics
    public float Brightness
    {
        get => store.LoadFloat(SettingsKeys.BrightnessKey, 1.5f);
        set => store.SaveFloat(SettingsKeys.BrightnessKey, value);
    }

    public int Quality
    {
        get => store.LoadInt(SettingsKeys.QualityKey, 2);
        set => store.SaveInt(SettingsKeys.QualityKey, value);
    }

    public bool Fullscreen
    {
        get => store.LoadBool(SettingsKeys.FullscreenKey, true);
        set => store.SaveBool(SettingsKeys.FullscreenKey, value);
    }
}
