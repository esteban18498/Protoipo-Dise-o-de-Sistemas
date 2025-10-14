using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
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


    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPrompt = null;


    [Header("Levels to Load")]

    public string _newGameLevel;
    private string levelToLoad;
    [SerializeField] private GameObject noSavedGameDialog = null;

    private const string MasterVolKey = "masterVolume";
    private const string SensitivityKey = "masterSensitivity";
    private const string InvertYKey = "masterInvertY";


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
        //if (invertYToggle.isOn)
        //{
        //    PlayerPrefs.SetInt("masterInvertY", 1);
        //    // Invert Y
        //}
        //else
        //{
        //    PlayerPrefs.SetInt("masterInvertY", 0);
        //    // not invert
        //}

        // Save invert
        if (invertYToggle) PlayerPrefs.SetInt(InvertYKey, invertYToggle.isOn ? 1 : 0);

        // Save sensitivity (as int)
        PlayerPrefs.SetInt(SensitivityKey, mainControllerSen);

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
    }

    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2f);
        confirmationPrompt.SetActive(false);
    }


}
