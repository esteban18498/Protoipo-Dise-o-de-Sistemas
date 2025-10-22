using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [Header("Sources")]
    [SerializeField] private GameObject actorRoot;          // El “Character 1” o “Character 2”
    [Header("Bars")]
    [SerializeField] private ResourceBarUI hpBar;
    [SerializeField] private ResourceBarUI staminaBar;

    private IHealth health;
    private ISpendable stamina;

    private void Awake()
    {
        if (!actorRoot)
        {
            Debug.LogError($"{name}: actorRoot no asignado.");
            return;
        }

        health = actorRoot.GetComponentInChildren<IHealth>();
        stamina = actorRoot.GetComponentInChildren<ISpendable>();

        if (hpBar && health != null) hpBar.Bind((Component)health);
        if (staminaBar && stamina != null) staminaBar.Bind((Component)stamina);
    }
}
