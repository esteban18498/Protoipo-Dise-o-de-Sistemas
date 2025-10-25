using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CombatActionHUD : MonoBehaviour
{
    [Header("Combat Action - Must be assigned")]
    public ICombatAction action;


    [Header("internal References")]
    [SerializeField] private GameObject actionTypeIMG;
    [SerializeField] private GameObject arrowModsContainer;
    [SerializeField] private GameObject arrowModprefab;


    // Start is called before the first frame update
    void Start()
    {
        if (action == null)
        {
            this.enabled = false;
            Debug.LogError("CombatActionHUD: action reference is not set.");
            return;
        }
        Init();
    }

    public void Init()
    {
        //TODO init HUD elements according to action data

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
