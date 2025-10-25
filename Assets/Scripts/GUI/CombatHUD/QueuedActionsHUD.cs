using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QueuedActionsHUD : MonoBehaviour
{
    public NovaCharacterController character;
    public CombatActionQueue actionQueue;

    [SerializeField] private CombatActionHUD combatActionHUDPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (character == null)
        {
            Debug.LogError("QueuedActionsHUD: character reference is not set.");
            this.enabled = false;
            return;
        }

        actionQueue = character.actionQueue;
        actionQueue.OnQueueUpdated += () =>
        {
            List<ICombatAction> list = actionQueue.GetQueuedActions();
            UpdateHUD(list);
        };
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateHUD(List<ICombatAction> actions)
    {
        //clear childs
        foreach (Transform child in this.transform)
        {
            if (child.name == "ActionPrefabHUD") continue;
            Destroy(child.gameObject);
        }

        for (int i = 0; i < actions.Count; i++)
        {
            CombatActionHUD child = Instantiate(combatActionHUDPrefab, this.transform);
            child.Action = actions[i];
            child.gameObject.SetActive(true);
            child.name = $"Action_{i}";
        }

    }
    

    public string ActionToString(ICombatAction action)
    {
        string text = "|";

        List<Combat_Action_mod> mods = action.Mods.ToList();

        foreach (Combat_Action_mod mod in mods)
        {
            text += mod.ToString() + " ";
        }
        
        text += $" {action.ActionType.ToString() }-> {action.GetType().ToString()} |";
        Debug.Log(text);


        return text;
    }
}


