using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public sealed class MenuController : MonoBehaviour
{
    // ---------- Services ----------
    private SettingsService settings = new SettingsService(new PlayerPrefsSettingsStore());

    // ---------- Audio (UI) ----------
    [Header("Audio")]
    [SerializeField] private Slider volumeSlider = null;        // Master 0..1
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField, Range(0f, 1f)] private float defaultVolume = 0.2f;

    // (Optional) If you add dedicated sliders later, just wire these and they’ll work:
    [SerializeField] private Slider musicSlider = null;         // 0..1 (optional)
    [SerializeField] private TMP_Text musicTextValue = null;
    [SerializeField] private Slider sfxSlider = null;           // 0..1 (optional)
    [SerializeField] private TMP_Text sfxTextValue = null;

    // ---------- Gameplay ----------
    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text ControllerSenTextvalue = null;
    [SerializeField] private Slider controllerSenSlider = null;
    [SerializeField] private int defaultSensitivity = 4;
    public int mainControllerSen = 4;

    [Header("Toggle Settings")]
    [SerializeField] private Toggle invertYToggle = null;

    // ---------- Graphics ----------
    [Header("Graphics Settings")]
    [SerializeField] private TMP_Text brightnessTextValue = null;
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private float defaultBrightness = 1.5f;

    [Space(10)]
    [SerializeField] private TMP_Dropdown qualityDropdown = null;
    [SerializeField] private Toggle fullscreenToggle = null;

    private int _qualityLevel;
    private bool _isFullScreen;
    private float _brightnessLevel;

    [Header("Resolution Dropdown")]
    public TMP_Dropdown resolutionDropdown;
    private bool _ignoreResChange = false;
    private Resolution[] _allModes;     // OS modes (w,h,refresh)
    private Vector2Int[] _uniqueRes;    // Unique sizes

    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPrompt = null;

    [Header("Levels to Load")]
    public string _newGameLevel;
    private string levelToLoad;
    [SerializeField] private GameObject noSavedGameDialog = null;

    // ---------- Keys still handled locally (not in SettingsService) ----------
    private const string SensitivityKey = "masterSensitivity";
    private const string InvertYKey = "masterInvertY";
    private const string ResolutionIndexKey = "masterResIndex";

    // =======================
    // Lifecycle
    // =======================
    private void Awake()
    {

        // ----- GAMEPLAY prefs -----
        int savedSensitivity = PlayerPrefs.GetInt(SensitivityKey, defaultSensitivity);
        if (controllerSenSlider)
        {
            controllerSenSlider.wholeNumbers = true;
            controllerSenSlider.value = savedSensitivity;
            controllerSenSlider.onValueChanged.RemoveAllListeners();
            controllerSenSlider.onValueChanged.AddListener(SetControllerSen);
        }
        SetControllerSen(savedSensitivity);

        bool invertY = PlayerPrefs.GetInt(InvertYKey, 0) == 1;
        if (invertYToggle) invertYToggle.isOn = invertY;

        // ----- GRAPHICS (brightness/quality/fullscreen) -----
        float savedBrightness = Mathf.Clamp(settings.Brightness, 0f, 2f);
        _brightnessLevel = savedBrightness;
        if (brightnessSlider)
        {
            brightnessSlider.minValue = 0f;
            brightnessSlider.maxValue = 2f;
            brightnessSlider.value = savedBrightness;
            brightnessSlider.onValueChanged.RemoveAllListeners();
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
        }
        SetBrightness(savedBrightness);

        int savedQuality = Mathf.Clamp(settings.Quality, 0, QualitySettings.names.Length - 1);
        _qualityLevel = savedQuality;
        QualitySettings.SetQualityLevel(_qualityLevel);
        if (qualityDropdown)
        {
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
            qualityDropdown.value = _qualityLevel;
            qualityDropdown.onValueChanged.RemoveAllListeners();
            qualityDropdown.onValueChanged.AddListener(SetQuality);
            qualityDropdown.RefreshShownValue();
        }

        bool savedFullscreen = settings.Fullscreen;
        _isFullScreen = savedFullscreen;
        Screen.fullScreen = savedFullscreen;
        if (fullscreenToggle)
        {
            fullscreenToggle.isOn = savedFullscreen;
            fullscreenToggle.onValueChanged.RemoveAllListeners();
            fullscreenToggle.onValueChanged.AddListener(SetFullScreen);
        }
    }

    private void Start()
    {
        // ----- AUDIO: load saved values and apply to Mixer -----
        float savedMaster = settings.MasterVolume; // 0..1
        if (volumeSlider)
        {
            volumeSlider.value = savedMaster;
            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        }
        OnMasterVolumeChanged(savedMaster); // live apply & text

        if (musicSlider)
        {
            float savedMusic = settings.MusicVolume;
            musicSlider.value = savedMusic;
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            OnMusicVolumeChanged(savedMusic);
        }
        if (sfxSlider)
        {
            float savedSfx = settings.SfxVolume;
            sfxSlider.value = savedSfx;
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            OnSfxVolumeChanged(savedSfx);
        }

        // ----- RESOLUTIONS -----
        _allModes = Screen.resolutions;
        if (_allModes == null || _allModes.Length == 0)
            _allModes = new[] { Screen.currentResolution };

        _uniqueRes = _allModes
            .Select(r => new Vector2Int(r.width, r.height))
            .Distinct()
            .ToArray();

        if (resolutionDropdown)
        {
            resolutionDropdown.ClearOptions();
            var opts = _uniqueRes.Select(v => $"{v.x} x {v.y}").ToList();
            resolutionDropdown.AddOptions(opts);
        }

        int targetIndex;
        if (PlayerPrefs.HasKey(ResolutionIndexKey))
        {
            targetIndex = Mathf.Clamp(PlayerPrefs.GetInt(ResolutionIndexKey), 0, _uniqueRes.Length - 1);
        }
        else
        {
            var (dw, dh) = GetDesktopResolution();
            targetIndex = FindBestResolutionIndex(dw, dh);
            if (targetIndex < 0) targetIndex = 0;
        }

        _ignoreResChange = true;
        ApplyResolutionIndex(targetIndex, keepCurrentMode: true);
        if (resolutionDropdown)
        {
            resolutionDropdown.SetValueWithoutNotify(targetIndex);
            resolutionDropdown.RefreshShownValue();
            resolutionDropdown.onValueChanged.RemoveAllListeners();
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }
        _ignoreResChange = false;
    }

    // =======================
    // Scene Buttons
    // =======================
    public void NewGameDialogYes() => SceneManager.LoadScene(_newGameLevel);

    public void LoadGameDialogYes()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            if (noSavedGameDialog) noSavedGameDialog.SetActive(true);
        }
    }

    public void ExitButton()
    {
#if !UNITY_EDITOR
        Application.Quit();
#else
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // =======================
    // Audio (Master/Music/SFX)
    // =======================
    public void OnMasterVolumeChanged(float v01)
    {
        AudioManager.Instance.SetBusVolume("MasterVolume", Mathf.Clamp01(v01));
        if (volumeTextValue) volumeTextValue.text = v01.ToString("0.0");
    }

    public void OnMasterVolumeApply()
    {
        float v = volumeSlider ? volumeSlider.value : defaultVolume;
        settings.MasterVolume = Mathf.Clamp01(v);
        if (confirmationPrompt) StartCoroutine(ConfirmationBox());
    }

    public void OnMusicVolumeChanged(float v01)
    {
        AudioManager.Instance.SetBusVolume("MusicVolume", Mathf.Clamp01(v01));
        if (musicTextValue) musicTextValue.text = v01.ToString("0.0");
    }

    public void OnMusicVolumeApply()
    {
        if (!musicSlider) return;
        settings.MusicVolume = Mathf.Clamp01(musicSlider.value);
        if (confirmationPrompt) StartCoroutine(ConfirmationBox());
    }

    public void OnSfxVolumeChanged(float v01)
    {
        AudioManager.Instance.SetBusVolume("SfxVolume", Mathf.Clamp01(v01));
        if (sfxTextValue) sfxTextValue.text = v01.ToString("0.0");
    }

    public void OnSfxVolumeApply()
    {
        if (!sfxSlider) return;
        settings.SfxVolume = Mathf.Clamp01(sfxSlider.value);
        if (confirmationPrompt) StartCoroutine(ConfirmationBox());
    }

    // =======================
    // Gameplay
    // =======================
    public void SetControllerSen(float sensitivity)
    {
        mainControllerSen = Mathf.RoundToInt(sensitivity);
        if (ControllerSenTextvalue) ControllerSenTextvalue.text = sensitivity.ToString("0");
    }

    public void GameplayApply()
    {
        if (invertYToggle) PlayerPrefs.SetInt(InvertYKey, invertYToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt(SensitivityKey, mainControllerSen);
        PlayerPrefs.Save();
        if (confirmationPrompt) StartCoroutine(ConfirmationBox());
    }

    // =======================
    // Graphics
    // =======================
    public void SetBrightness(float brightness)
    {
        _brightnessLevel = brightness;
        if (brightnessTextValue) brightnessTextValue.text = brightness.ToString("0.0");
    }

    public void SetFullScreen(bool isFullScreen)
    {
        _isFullScreen = isFullScreen;
    }

    public void SetQuality(int qualityIndex)
    {
        _qualityLevel = qualityIndex;
    }

    public void SetResolution(int uniqueIndex)
    {
        if (_ignoreResChange) return;
        ApplyResolutionIndex(uniqueIndex, keepCurrentMode: true);
        if (resolutionDropdown) PlayerPrefs.SetInt(ResolutionIndexKey, resolutionDropdown.value);
    }

    private void ApplyResolutionIndex(int uniqueIndex, bool keepCurrentMode = true)
    {
        if (_uniqueRes == null || _uniqueRes.Length == 0) return;
        if (uniqueIndex < 0 || uniqueIndex >= _uniqueRes.Length) return;

        var size = _uniqueRes[uniqueIndex];

        var best = _allModes
            .Where(m => m.width == size.x && m.height == size.y)
            .OrderByDescending(m => m.refreshRate)
            .FirstOrDefault();

        if (best.width == 0 || best.height == 0)
            best = Screen.currentResolution;

        var mode = keepCurrentMode
            ? Screen.fullScreenMode
            : (_isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);

        Screen.SetResolution(best.width, best.height, mode, best.refreshRate);

        if (resolutionDropdown)
        {
            resolutionDropdown.SetValueWithoutNotify(uniqueIndex);
            resolutionDropdown.RefreshShownValue();
        }
    }

    public void GraphicsApply()
    {
        settings.Brightness = _brightnessLevel;
        settings.Quality = _qualityLevel;
        settings.Fullscreen = _isFullScreen;

        QualitySettings.SetQualityLevel(_qualityLevel);
        Screen.fullScreen = _isFullScreen;

        if (resolutionDropdown) PlayerPrefs.SetInt(ResolutionIndexKey, resolutionDropdown.value);
        PlayerPrefs.Save();

        if (confirmationPrompt) StartCoroutine(ConfirmationBox());
    }

    // =======================
    // Reset sections
    // =======================
    public void ResetButton(string menuType)
    {
        if (menuType == "Audio")
        {
            float v = Mathf.Clamp01(defaultVolume);

            if (volumeSlider) volumeSlider.value = v;
            OnMasterVolumeChanged(v);
            settings.MasterVolume = v;

            if (musicSlider)
            {
                if (musicTextValue) musicTextValue.text = v.ToString("0.0");
                musicSlider.value = v;
                OnMusicVolumeChanged(v);
                settings.MusicVolume = v;
            }
            if (sfxSlider)
            {
                if (sfxTextValue) sfxTextValue.text = v.ToString("0.0");
                sfxSlider.value = v;
                OnSfxVolumeChanged(v);
                settings.SfxVolume = v;
            }

            if (confirmationPrompt) StartCoroutine(ConfirmationBox());
        }
        else if (menuType == "Gameplay")
        {
            if (ControllerSenTextvalue) ControllerSenTextvalue.text = defaultSensitivity.ToString("0");
            if (controllerSenSlider) controllerSenSlider.value = defaultSensitivity;
            mainControllerSen = defaultSensitivity;
            if (invertYToggle) invertYToggle.isOn = false;
            GameplayApply();
        }
        else if (menuType == "Graphics")
        {
            _brightnessLevel = defaultBrightness;
            if (brightnessSlider) brightnessSlider.value = defaultBrightness;
            if (brightnessTextValue) brightnessTextValue.text = defaultBrightness.ToString("0.0");

            _qualityLevel = 2;
            if (qualityDropdown) qualityDropdown.SetValueWithoutNotify(2);
            QualitySettings.SetQualityLevel(_qualityLevel);

            _isFullScreen = true;
            if (fullscreenToggle) fullscreenToggle.SetIsOnWithoutNotify(true);
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

            var (dw, dh) = GetDesktopResolution();
            if (dw == 0 || dh == 0)
            {
                int idx1080 = FindBestResolutionIndex(1920, 1080);
                if (idx1080 >= 0) { dw = 1920; dh = 1080; }
            }

            int idx = FindBestResolutionIndex(dw, dh);
            if (idx < 0) idx = 0;

            _ignoreResChange = true;
            ApplyResolutionIndex(idx, keepCurrentMode: false);
            if (resolutionDropdown)
            {
                resolutionDropdown.SetValueWithoutNotify(idx);
                resolutionDropdown.RefreshShownValue();
            }
            _ignoreResChange = false;

            settings.Brightness = _brightnessLevel;
            settings.Quality = _qualityLevel;
            settings.Fullscreen = _isFullScreen;
            if (resolutionDropdown) PlayerPrefs.SetInt(ResolutionIndexKey, idx);
            PlayerPrefs.Save();

            if (confirmationPrompt) StartCoroutine(ConfirmationBox());
        }
    }

    // =======================
    // Helpers
    // =======================
    public IEnumerator ConfirmationBox()
    {
        if (!confirmationPrompt) yield break;
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2f);
        confirmationPrompt.SetActive(false);
    }

    private int FindBestResolutionIndex(int targetW, int targetH)
    {
        if (_uniqueRes == null || _uniqueRes.Length == 0) return -1;

        // Exact match
        for (int i = 0; i < _uniqueRes.Length; i++)
            if (_uniqueRes[i].x == targetW && _uniqueRes[i].y == targetH)
                return i;

        // Closest by area delta
        long bestDiff = long.MaxValue;
        int bestIdx = -1;
        for (int i = 0; i < _uniqueRes.Length; i++)
        {
            var v = _uniqueRes[i];
            long diff = (long)Mathf.Abs(v.x - targetW) * (long)Mathf.Abs(v.y - targetH);
            if (diff < bestDiff) { bestDiff = diff; bestIdx = i; }
        }
        return bestIdx;
    }

    private (int w, int h) GetDesktopResolution()
    {
        int sysW = Display.main != null ? Display.main.systemWidth : 0;
        int sysH = Display.main != null ? Display.main.systemHeight : 0;
        if (sysW > 0 && sysH > 0) return (sysW, sysH);

        var cr = Screen.currentResolution;
        return (cr.width, cr.height);
    }
}
