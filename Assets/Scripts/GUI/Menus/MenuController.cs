using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;


public sealed class MenuController : MonoBehaviour
{
    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 1.0f;

    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text ControllerSenTextvalue = null;
    [SerializeField] private Slider controllerSenSlider = null;
    [SerializeField] private int defaultSensitivity = 4;
    public int mainControllerSen = 4;


    [Header("Toggle Settings")]
    [SerializeField] private Toggle invertYToggle = null;


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

    [Header("Resolution Dropdowns")]
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;
    private bool _ignoreResChange = false;
    private Resolution[] _allModes; // All modes reported by the OS (width, height, refresh)
    private Vector2Int[] _uniqueRes; // Unique width×height entries for the dropdown (no refresh)


    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPrompt = null;


    [Header("Levels to Load")]

    public string _newGameLevel;
    private string levelToLoad;
    [SerializeField] private GameObject noSavedGameDialog = null;

    private const string MasterVolKey = "masterVolume";
    private const string SensitivityKey = "masterSensitivity";
    private const string InvertYKey = "masterInvertY";
    private const string BrightnessKey = "masterBrightness";
    private const string QualityKey = "masterQuality";
    private const string FullscreenKey = "masterFullscreen";
    private const string ResolutionIndexKey = "masterResIndex";


    private void Awake()
    {
        // --- Volume ---
        float savedVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(MasterVolKey, defaultVolume));
        if (volumeSlider) volumeSlider.value = savedVolume;
        SetMasterVolume(savedVolume);
        if (volumeSlider)
        {
            volumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
            volumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        // --- Sensitivity (int, whole numbers) ---
        int savedSensitivity = PlayerPrefs.GetInt(SensitivityKey, defaultSensitivity);
        if (controllerSenSlider)
        {
            controllerSenSlider.wholeNumbers = true; // keep ints
            controllerSenSlider.value = savedSensitivity;
            controllerSenSlider.onValueChanged.RemoveAllListeners();
            controllerSenSlider.onValueChanged.AddListener(SetControllerSen);
        }
        SetControllerSen(savedSensitivity);

        // --- Invert Y ---
        bool invertY = PlayerPrefs.GetInt(InvertYKey, 0) == 1;
        if (invertYToggle) invertYToggle.isOn = invertY;

        // ---------- GRAPHICS ----------
        // Brightness
        float savedBrightness = PlayerPrefs.GetFloat(BrightnessKey, defaultBrightness);
        savedBrightness = Mathf.Clamp(savedBrightness, 0f, 2f);     // rango típico
        _brightnessLevel = savedBrightness;                         // SINCRONIZAR backing field
        if (brightnessSlider)
        {
            brightnessSlider.minValue = 0f;
            brightnessSlider.maxValue = 2f;
            brightnessSlider.value = savedBrightness;
            brightnessSlider.onValueChanged.RemoveAllListeners();
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
        }
        SetBrightness(savedBrightness); // actualiza el TMP_Text

        // Quality
        int savedQuality = PlayerPrefs.GetInt(QualityKey, QualitySettings.GetQualityLevel());
        _qualityLevel = Mathf.Clamp(savedQuality, 0, QualitySettings.names.Length - 1);
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

        // Fullscreen
        bool savedFullscreen = PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;
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
        // 1) Read all modes
        _allModes = Screen.resolutions;
        if (_allModes == null || _allModes.Length == 0)
            _allModes = new[] { Screen.currentResolution }; // fallback

        // 2) Unique width×height (drop refresh); keep natural order by first appearance
        _uniqueRes = _allModes
            .Select(r => new Vector2Int(r.width, r.height))
            .Distinct() // requires using System.Linq
            .ToArray();

        // 3) Dropdown options from unique sizes
        if (resolutionDropdown)
        {
            resolutionDropdown.ClearOptions();
            var opts = _uniqueRes.Select(v => $"{v.x} x {v.y}").ToList();
            resolutionDropdown.AddOptions(opts);
        }

        // 4) Pick initial index: pref → desktop → 0
        int targetIndex;
        if (PlayerPrefs.HasKey(ResolutionIndexKey))
        {
            targetIndex = Mathf.Clamp(PlayerPrefs.GetInt(ResolutionIndexKey), 0, _uniqueRes.Length - 1);
        }
        else
        {
            var (dw, dh) = GetDesktopResolution();
            targetIndex = FindBestResolutionIndex(dw, dh); // now searches in _uniqueRes
            if (targetIndex < 0) targetIndex = 0;
        }

        // 5) Apply once (highest refresh for that size), wire listener
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



    #region Scene Buttons
    public void NewGameDialogYes() => SceneManager.LoadScene(_newGameLevel);

    public void LoadGameDialogYes()
    {
        if (PlayerPrefs.HasKey("SavedLevel")) // Do we have a file SavedLevel?
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            noSavedGameDialog.SetActive(true);
        }
    }

    public void ExitButton()
    {
        Application.Quit();
    }
    #endregion

    #region Volume
    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat(MasterVolKey, AudioListener.volume);
        PlayerPrefs.Save();
        if (confirmationPrompt != null) StartCoroutine(ConfirmationBox());

    }


    #endregion

    #region Gameplay
    public void SetControllerSen(float sensitivity)
    {
        mainControllerSen = Mathf.RoundToInt(sensitivity);
        ControllerSenTextvalue.text = sensitivity.ToString("0");
    }

    public void GameplayApply()
    {
        // Save invert
        if (invertYToggle) PlayerPrefs.SetInt(InvertYKey, invertYToggle.isOn ? 1 : 0);

        // Save sensitivity (as int)
        PlayerPrefs.SetInt(SensitivityKey, mainControllerSen);

        StartCoroutine(ConfirmationBox());
    }

    #endregion

    #region Graphics

    public void SetBrightness(float brightness)
    {
        _brightnessLevel = brightness;
        brightnessTextValue.text = brightness.ToString("0.0");
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

        // Find all modes for this size and pick the highest refresh
        var best = _allModes
            .Where(m => m.width == size.x && m.height == size.y)
            .OrderByDescending(m => m.refreshRate)
            .FirstOrDefault();

        // Fallback if not found (shouldn't happen)
        if (best.width == 0 || best.height == 0)
            best = Screen.currentResolution;

        var mode = keepCurrentMode
            ? Screen.fullScreenMode
            : (_isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);

        Screen.SetResolution(best.width, best.height, mode, best.refreshRate);

        // Do NOT trigger onValueChanged
        if (resolutionDropdown)
        {
            resolutionDropdown.SetValueWithoutNotify(uniqueIndex);
            resolutionDropdown.RefreshShownValue();
        }
    }



    public void GraphicsApply()
    {
        PlayerPrefs.SetFloat("masterBrightness", _brightnessLevel);
        // Change brightness with post processing or whatever we change it with.

        PlayerPrefs.SetInt("masterQuality",_qualityLevel);
        QualitySettings.SetQualityLevel(_qualityLevel);

        PlayerPrefs.SetInt("masterFullscreen", (_isFullScreen ? 1 : 0));
        Screen.fullScreen = _isFullScreen;

        PlayerPrefs.SetInt(ResolutionIndexKey, resolutionDropdown.value);

        PlayerPrefs.Save();

        StartCoroutine(ConfirmationBox());

    }

    #endregion

    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }

        else if (MenuType == "Gameplay")
        {
            ControllerSenTextvalue.text = defaultSensitivity.ToString("0");
            controllerSenSlider.value = defaultSensitivity;
            mainControllerSen = defaultSensitivity;
            invertYToggle.isOn = false;
            GameplayApply();
        }

        else if (MenuType == "Graphics")
        {
            // 1) Estado interno primero
            _brightnessLevel = defaultBrightness;
            if (brightnessSlider) brightnessSlider.value = defaultBrightness;
            if (brightnessTextValue) brightnessTextValue.text = defaultBrightness.ToString("0.0");

            _qualityLevel = 2;
            if (qualityDropdown) qualityDropdown.SetValueWithoutNotify(2);
            QualitySettings.SetQualityLevel(_qualityLevel);

            _isFullScreen = true;
            if (fullscreenToggle) fullscreenToggle.SetIsOnWithoutNotify(true);
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

            // 2) Resolver a la resolución de escritorio (única por width×height)
            var (dw, dh) = GetDesktopResolution();

            // Fallback duro si Display.main no reporta nada
            if (dw == 0 || dh == 0)
            {
                int idx1080 = FindBestResolutionIndex(1920, 1080);
                if (idx1080 >= 0) { dw = 1920; dh = 1080; }
            }

            int idx = FindBestResolutionIndex(dw, dh);
            // Fallback seguro al primer tamaño disponible de la lista única
            // (si usás _uniqueRes, esto evita depender del listado por modos)
            if (idx < 0) idx = 0;

            // 3) Aplicar sin disparar el listener
            _ignoreResChange = true;
            ApplyResolutionIndex(idx, keepCurrentMode: false);
            if (resolutionDropdown)
            {
                resolutionDropdown.SetValueWithoutNotify(idx);
                resolutionDropdown.RefreshShownValue();
            }
            _ignoreResChange = false;

            // 4) Guardar prefs (incluye el índice actual del dropdown)
            PlayerPrefs.SetFloat(BrightnessKey, _brightnessLevel);
            PlayerPrefs.SetInt(QualityKey, _qualityLevel);
            PlayerPrefs.SetInt(FullscreenKey, (_isFullScreen ? 1 : 0));
            if (resolutionDropdown) PlayerPrefs.SetInt(ResolutionIndexKey, idx);
            PlayerPrefs.Save();

            if (confirmationPrompt != null) StartCoroutine(ConfirmationBox());
        }

    }

    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2f);
        confirmationPrompt.SetActive(false);
    }

    #region HelperFunctions
    private int FindBestResolutionIndex(int targetW, int targetH)
    {
        if (_uniqueRes == null || _uniqueRes.Length == 0) return -1;

        // exact width×height
        for (int i = 0; i < _uniqueRes.Length; i++)
            if (_uniqueRes[i].x == targetW && _uniqueRes[i].y == targetH)
                return i;

        // closest by area delta
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
        // Prefer system (desktop) size when available
        int sysW = Display.main != null ? Display.main.systemWidth : 0;
        int sysH = Display.main != null ? Display.main.systemHeight : 0;

        if (sysW > 0 && sysH > 0) return (sysW, sysH);

        // Fallback to current display mode if system size isn’t available
        var cr = Screen.currentResolution;
        return (cr.width, cr.height);
    }

    #endregion
}
