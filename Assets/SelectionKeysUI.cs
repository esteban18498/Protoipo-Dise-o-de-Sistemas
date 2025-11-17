using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionKeysUI : MonoBehaviour
{
    public GameObject keyboardUI;
    public GameObject gamepadUI;

    public void ShowKeyboardUI()
    {
        keyboardUI.SetActive(true);
        gamepadUI.SetActive(false);
    }

    public void ShowGamepadUI()
    {
        keyboardUI.SetActive(false);
        gamepadUI.SetActive(true);
    }
}
