using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CombatActionHUD : MonoBehaviour
{
    [Header("Debug Options")]
    public bool DebugText = false;

    [Header("Combat Action - Must be assigned")]
    public ICombatAction Action;


    [Header("internal References")]

    [SerializeField] private TextMeshProUGUI actionNameText;

    [SerializeField] private actionTypeHud actionTypeHudElement;
    [SerializeField] private GameObject arrowModsContainer;
    [SerializeField] private actionModHud arrowModprefab;


    // Start is called before the first frame update
    void Start()
    {
        if (Action == null)
        {
            this.enabled = false;
            Debug.LogError("CombatActionHUD: action reference is not set.");
            return;
        }
        Init();
    }

    public void Init()
    {
        if (DebugText && actionNameText != null)
        {
            actionNameText.text = Action.GetType().ToString();
        }

        if (actionTypeHudElement != null)
        {
            actionTypeHudElement.type = Action.ActionType;
            actionTypeHudElement.ConfigImage();
            
        }

        if (arrowModsContainer != null && arrowModprefab != null)
        {
            foreach (Transform child in arrowModsContainer.transform)
            {
                if (child == arrowModprefab.transform) continue;
                Destroy(child.gameObject);
            }

            foreach (Combat_Action_mod mod in Action.Mods)
            {
                actionModHud modHudInstance = Instantiate(arrowModprefab, arrowModsContainer.transform);
                modHudInstance.Mod = mod;
                modHudInstance.ConfigImage();
                modHudInstance.gameObject.SetActive(true);

            }
        }
        //TODO init HUD elements according to action data

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
