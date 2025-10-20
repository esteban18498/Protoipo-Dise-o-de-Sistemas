using UnityEngine;

public interface ISettingsStore
{
    // 0..1 normalized floats (e.g., volumes)
    float Load01(string key, float def = 1f);
    void Save01(string key, float v01);

    // General floats (e.g., brightness)
    float LoadFloat(string key, float def = 0f);
    void SaveFloat(string key, float value);

    // Ints (e.g., quality level)
    int LoadInt(string key, int def = 0);
    void SaveInt(string key, int value);

    // Bools (e.g., fullscreen)
    bool LoadBool(string key, bool def = false);
    void SaveBool(string key, bool value);
}

public sealed class PlayerPrefsSettingsStore : ISettingsStore
{
    public float Load01(string key, float def = 1f)
        => Mathf.Clamp01(PlayerPrefs.GetFloat(key, def));

    public void Save01(string key, float v01)
    {
        PlayerPrefs.SetFloat(key, Mathf.Clamp01(v01));
        PlayerPrefs.Save();
    }

    public float LoadFloat(string key, float def = 0f)
        => PlayerPrefs.GetFloat(key, def);

    public void SaveFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    public int LoadInt(string key, int def = 0)
        => PlayerPrefs.GetInt(key, def);

    public void SaveInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    public bool LoadBool(string key, bool def = false)
        => PlayerPrefs.GetInt(key, def ? 1 : 0) == 1;

    public void SaveBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
    }
}
