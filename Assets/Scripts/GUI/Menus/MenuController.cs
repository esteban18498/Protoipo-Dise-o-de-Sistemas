using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

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
        // Inicializa resoluciones
        resolutions = Screen.resolutions;
        if (resolutions == null || resolutions.Length == 0)
            resolutions = new[] { Screen.currentResolution }; // fallback

        // Construye opciones del dropdown
        if (resolutionDropdown)
        {
            resolutionDropdown.ClearOptions();
            var opts = new List<string>(resolutions.Length);
            for (int i = 0; i < resolutions.Length; i++)
                opts.Add($"{resolutions[i].width} x {resolutions[i].height}");
            resolutionDropdown.AddOptions(opts);
        }

        // Elegí la resolución “preferida”: si hay pref, úsalo; si no, la del escritorio
        int targetIndex;
        if (PlayerPrefs.HasKey(ResolutionIndexKey))
            targetIndex = Mathf.Clamp(PlayerPrefs.GetInt(ResolutionIndexKey), 0, resolutions.Length - 1);
        else
        {
            var (dw, dh) = GetDesktopResolution();
            targetIndex = FindBestResolutionIndex(dw, dh);
            if (targetIndex < 0) targetIndex = 0;
        }

        // Aplicar sin disparar onValueChanged
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

    public void SetResolution(int resolutionIndex)
    {
        if (_ignoreResChange) return;               
        ApplyResolutionIndex(resolutionIndex, keepCurrentMode: true);
    }

    private void ApplyResolutionIndex(int index, bool keepCurrentMode = true)
    {
        if (resolutions == null || resolutions.Length == 0) return;
        if (index < 0 || index >= resolutions.Length) return;

        var r = resolutions[index];
        var mode = keepCurrentMode
            ? Screen.fullScreenMode
            : (_isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);

        Screen.SetResolution(r.width, r.height, mode, r.refreshRate);

        if (resolutionDropdown)
        {
            resolutionDropdown.SetValueWithoutNotify(index);
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

        if (MenuType == "Gameplay")
        {
            ControllerSenTextvalue.text = defaultSensitivity.ToString("0");
            controllerSenSlider.value = defaultSensitivity;
            mainControllerSen = defaultSensitivity;
            invertYToggle.isOn = false;
            GameplayApply();
        }

        if (MenuType == "Graphics")
        {
            // Estado interno primero
            _brightnessLevel = defaultBrightness;
            if (brightnessSlider) brightnessSlider.value = defaultBrightness;
            if (brightnessTextValue) brightnessTextValue.text = defaultBrightness.ToString("0.0");

            _qualityLevel = 2;
            if (qualityDropdown) qualityDropdown.SetValueWithoutNotify(2);
            QualitySettings.SetQualityLevel(_qualityLevel);

            _isFullScreen = true;
            if (fullscreenToggle) fullscreenToggle.SetIsOnWithoutNotify(true);
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

            // Resolver desktop a lo que tenga el SO
            var (dw, dh) = GetDesktopResolution();

            // Si Display.main no devuelve nada, intentá hardcode 1920x1080 si existe
            if (dw == 0 || dh == 0)
            {
                int idx1080 = FindBestResolutionIndex(1920, 1080);
                if (idx1080 >= 0) { dw = 1920; dh = 1080; }
            }

            int idx = FindBestResolutionIndex(dw, dh);
            if (idx < 0) idx = resolutions.Length - 1; // fallback: la más alta

            // Aplicar sin disparar el listener
            _ignoreResChange = true;
            ApplyResolutionIndex(idx, keepCurrentMode: false);
            if (resolutionDropdown)
            {
                resolutionDropdown.SetValueWithoutNotify(idx);
                resolutionDropdown.RefreshShownValue();
            }
            _ignoreResChange = false;

            // Guardar prefs (incluye el índice actual del dropdown)
            PlayerPrefs.SetFloat(BrightnessKey, _brightnessLevel);
            PlayerPrefs.SetInt(QualityKey, _qualityLevel);
            PlayerPrefs.SetInt(FullscreenKey, (_isFullScreen ? 1 : 0));
            if (resolutionDropdown) PlayerPrefs.SetInt(ResolutionIndexKey, resolutionDropdown.value);
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
        if (resolutions == null || resolutions.Length == 0) return -1;

        int bestIndex = -1, bestRefresh = -1;
        for (int i = 0; i < resolutions.Length; i++)
        {
            var r = resolutions[i];
            if (r.width == targetW && r.height == targetH && r.refreshRate > bestRefresh)
            {
                bestRefresh = r.refreshRate;
                bestIndex = i;
            }
        }
        if (bestIndex >= 0) return bestIndex;

        long bestDiff = long.MaxValue;
        for (int i = 0; i < resolutions.Length; i++)
        {
            var r = resolutions[i];
            long diff = (long)Mathf.Abs(r.width - targetW) * (long)Mathf.Abs(r.height - targetH);
            if (diff < bestDiff)
            {
                bestDiff = diff;
                bestIndex = i;
            }
        }
        return bestIndex;
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
